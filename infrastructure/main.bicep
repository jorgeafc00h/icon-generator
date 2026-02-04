// ==============================================
// Icon Generator - Main Infrastructure Template
// ==============================================

targetScope = 'resourceGroup'

@description('Environment name (dev, staging, prod)')
@allowed([
  'dev'
  'staging'
  'prod'
])
param environment string = 'dev'

@description('Azure region for all resources')
param location string = resourceGroup().location

@description('Base name for all resources')
param baseName string = 'icongen'

@description('Database type to use')
@allowed([
  'cosmosdb'
  'sql'
])
param databaseType string = 'cosmosdb'

@description('SQL Admin password (required if using Azure SQL)')
@secure()
param sqlAdminPassword string = ''

@description('Stripe Secret Key (required for payments)')
@secure()
param stripeSecretKey string = ''

@description('Stripe Webhook Secret (required for payment webhooks)')
@secure()
param stripeWebhookSecret string = ''

@description('Google OAuth Client ID (required for authentication)')
param googleClientId string = ''

@description('Frontend URL for Stripe redirects')
param frontendUrl string = ''

@description('Tags to apply to all resources')
param tags object = {
  Environment: environment
  Project: 'IconGenerator'
  ManagedBy: 'Bicep'
}

// ==============================================
// Variables
// ==============================================

var resourceSuffix = '${baseName}-${environment}-${uniqueString(resourceGroup().id)}'
var storageAccountName = replace('st${resourceSuffix}', '-', '')
var functionAppName = 'func-${resourceSuffix}'
var appServicePlanName = 'asp-${resourceSuffix}'
var staticWebAppName = 'swa-${resourceSuffix}'
var cosmosAccountName = 'cosmos-${resourceSuffix}'
var sqlServerName = 'sql-${resourceSuffix}'
var sqlDatabaseName = 'IconGeneratorDB'
var openAIName = 'openai-${resourceSuffix}'
var appInsightsName = 'ai-${resourceSuffix}'
var logAnalyticsName = 'log-${resourceSuffix}'
var keyVaultName = 'kv-${take(resourceSuffix, 20)}'

// ==============================================
// Module: Log Analytics Workspace
// ==============================================

module logAnalytics './modules/log-analytics.bicep' = {
  name: 'log-analytics-deployment'
  params: {
    name: logAnalyticsName
    location: location
    tags: tags
    retentionInDays: environment == 'prod' ? 90 : 30
  }
}

// ==============================================
// Module: Application Insights
// ==============================================

module appInsights './modules/app-insights.bicep' = {
  name: 'app-insights-deployment'
  params: {
    name: appInsightsName
    location: location
    tags: tags
    workspaceResourceId: logAnalytics.outputs.workspaceId
  }
}

// ==============================================
// Module: Storage Account
// ==============================================

module storage './modules/storage-account.bicep' = {
  name: 'storage-deployment'
  params: {
    name: storageAccountName
    location: location
    tags: tags
    sku: environment == 'prod' ? 'Standard_GRS' : 'Standard_LRS'
    containers: [
      {
        name: 'generated-icons'
        publicAccess: 'Blob'
      }
      {
        name: 'app-resources'
        publicAccess: 'Blob'
      }
    ]
  }
}

// ==============================================
// Module: Cosmos DB (Conditional)
// ==============================================

module cosmosDb './modules/cosmos-db.bicep' = if (databaseType == 'cosmosdb') {
  name: 'cosmos-deployment'
  params: {
    name: cosmosAccountName
    location: location
    tags: tags
    // Free tier: 1000 RU/s + 25GB storage free forever (one per subscription)
    enableFreeTier: true
    // Serverless: pay per request (good for dev/staging)
    enableServerless: environment != 'prod'
  }
}

// ==============================================
// Module: Azure SQL (Conditional)
// ==============================================

module azureSQL './modules/azure-sql.bicep' = if (databaseType == 'sql') {
  name: 'sql-deployment'
  params: {
    serverName: sqlServerName
    databaseName: sqlDatabaseName
    location: location
    tags: tags
    administratorLogin: 'sqladmin'
    administratorPassword: sqlAdminPassword
    // Basic: $5/month (2GB storage)
    // S0: $15/month (250GB storage, 10 DTUs)
    sku: environment == 'prod' ? 'S1' : 'Basic'
    enablePublicAccess: true
  }
}

// ==============================================
// Module: Azure OpenAI / Cognitive Services
// ==============================================

module openAI './modules/cognitive-services.bicep' = {
  name: 'openai-deployment'
  params: {
    name: openAIName
    location: location
    tags: tags
    sku: 'S0'
  }
}

// ==============================================
// Module: Key Vault
// ==============================================

module keyVault './modules/key-vault.bicep' = {
  name: 'key-vault-deployment'
  params: {
    name: keyVaultName
    location: location
    tags: tags
    enabledForDeployment: true
    enableRbacAuthorization: true
  }
}

// ==============================================
// Module: App Service Plan (for Functions)
// ==============================================

module appServicePlan './modules/app-service-plan.bicep' = {
  name: 'app-service-plan-deployment'
  params: {
    name: appServicePlanName
    location: location
    tags: tags
    sku: {
      name: environment == 'prod' ? 'P1v3' : 'Y1'
      tier: environment == 'prod' ? 'PremiumV3' : 'Dynamic'
    }
    kind: 'linux'
    reserved: true
  }
}

// ==============================================
// Module: Function App
// ==============================================

module functionApp './modules/function-app.bicep' = {
  name: 'function-app-deployment'
  params: {
    name: functionAppName
    location: location
    tags: tags
    appServicePlanId: appServicePlan.outputs.planId
    storageAccountName: storage.outputs.storageAccountName
    storageAccountKey: storage.outputs.storageAccountKey
    appInsightsInstrumentationKey: appInsights.outputs.instrumentationKey
    appInsightsConnectionString: appInsights.outputs.connectionString
    runtime: 'dotnet-isolated'
    runtimeVersion: '10.0'
    appSettings: concat([
      {
        name: 'Database__Type'
        value: databaseType
      }
      {
        name: 'AzureOpenAI__Endpoint'
        value: openAI.outputs.endpoint
      }
      {
        name: 'AzureOpenAI__ApiKey'
        value: openAI.outputs.apiKey
      }
      {
        name: 'AzureOpenAI__DallE3Deployment'
        value: 'dall-e-3'
      }
      {
        name: 'AzureOpenAI__Gpt4oMiniDeployment'
        value: 'gpt-4o-mini'
      }
      {
        name: 'Storage__ConnectionString'
        value: storage.outputs.connectionString
      }
      {
        name: 'Storage__ContainerName'
        value: 'generated-icons'
      }
      {
        name: 'Google__ClientId'
        value: googleClientId
      }
      {
        name: 'AllowedOrigins'
        value: '*'
      }
      {
        name: 'Stripe__SecretKey'
        value: stripeSecretKey
      }
      {
        name: 'Stripe__WebhookSecret'
        value: stripeWebhookSecret
      }
      {
        name: 'Stripe__FrontendUrl'
        value: frontendUrl
      }
    ], databaseType == 'cosmosdb' ? [
      {
        name: 'Database__CosmosEndpoint'
        value: cosmosDb.outputs.endpoint
      }
      {
        name: 'Database__CosmosKey'
        value: cosmosDb.outputs.primaryKey
      }
      {
        name: 'Database__CosmosDatabase'
        value: 'IconGeneratorDB'
      }
    ] : [
      {
        name: 'Database__SqlConnectionString'
        value: azureSQL.outputs.connectionString
      }
      {
        name: 'Database__SqlServer'
        value: azureSQL.outputs.fullyQualifiedDomainName
      }
      {
        name: 'Database__SqlDatabase'
        value: sqlDatabaseName
      }
    ])
  }
}

// ==============================================
// Module: Static Web App
// ==============================================

module staticWebApp './modules/static-web-app.bicep' = {
  name: 'static-web-app-deployment'
  params: {
    name: staticWebAppName
    location: location == 'eastus' ? 'eastus2' : location
    tags: tags
    sku: environment == 'prod' ? 'Standard' : 'Free'
  }
}

// ==============================================
// Outputs
// ==============================================

output resourceGroupName string = resourceGroup().name
output location string = location
output environment string = environment

// Storage outputs
output storageAccountName string = storage.outputs.storageAccountName
output storageBlobEndpoint string = storage.outputs.blobEndpoint

// Database outputs (conditional)
output databaseType string = databaseType
output cosmosAccountName string = databaseType == 'cosmosdb' ? cosmosDb.outputs.accountName : ''
output cosmosEndpoint string = databaseType == 'cosmosdb' ? cosmosDb.outputs.endpoint : ''
output sqlServerName string = databaseType == 'sql' ? azureSQL.outputs.serverName : ''
output sqlDatabaseName string = databaseType == 'sql' ? azureSQL.outputs.databaseName : ''

// OpenAI outputs
output openAIName string = openAI.outputs.name
output openAIEndpoint string = openAI.outputs.endpoint
output openAIKey string = openAI.outputs.apiKey

// Function App outputs
output functionAppName string = functionApp.outputs.functionAppName
output functionAppUrl string = functionApp.outputs.functionAppUrl
output functionAppPrincipalId string = functionApp.outputs.principalId

// Static Web App outputs
output staticWebAppName string = staticWebApp.outputs.name
output staticWebAppUrl string = staticWebApp.outputs.defaultHostname
// static web app token removed from outputs for security

// Monitoring outputs
output appInsightsName string = appInsights.outputs.name
output appInsightsInstrumentationKey string = appInsights.outputs.instrumentationKey
output logAnalyticsWorkspaceId string = logAnalytics.outputs.workspaceId

// Key Vault outputs
output keyVaultName string = keyVault.outputs.name
output keyVaultUri string = keyVault.outputs.vaultUri
