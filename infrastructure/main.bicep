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
param environment string = 'prod'

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

@description('JWT Secret Key for token signing (generate with: openssl rand -base64 32)')
@secure()
param jwtSecretKey string = ''

@description('Enable Free Tier for Cosmos DB (opt-in; only one free-tier account per subscription)')
param enableCosmosFreeTier bool = true

@description('Use an existing Cosmos DB account instead of creating a new one')
param useExistingCosmos bool = true

@description('Tags to apply to all resources')
param tags object = {
  Environment: environment
  Project: 'IconGenerator'
  ManagedBy: 'Bicep'
}

// ==============================================
// Variables
// ==============================================

// Use static resource names to match existing resources and enable updates
var storageAccountName = 'sticongen' // Existing storage account with data
var webAppName = 'webapi-icon-generator-${environment}' // Web app name for ASP.NET Core API
var appServicePlanName = 'asp-icon-generator' // Static app service plan name
var staticWebAppName = 'icon-generator-pro' // Existing Static Web App
var cosmosAccountName = 'cosmos-icon-generator' // Existing Cosmos DB with free tier
var sqlServerName = 'sql-icon-generator-${environment}'
var sqlDatabaseName = 'IconGeneratorDB'
var openAIName = 'openai-icon-generator' // Existing OpenAI
var appInsightsName = 'ai-icon-generator-${environment}'
var logAnalyticsName = 'log-icon-generator-${environment}'
var keyVaultName = 'kv-icon-gen-${environment}'

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

module cosmosDb './modules/cosmos-db.bicep' = if (databaseType == 'cosmosdb' && !useExistingCosmos) {
  name: 'cosmos-deployment'
  params: {
    name: cosmosAccountName
    location: location
    tags: tags
    // Free tier: 1000 RU/s + 25GB storage free forever (one per subscription)
    // Controlled via `enableCosmosFreeTier` param to avoid deployment failures
    enableFreeTier: enableCosmosFreeTier
    // Serverless: pay per request (good for dev/staging)
    enableServerless: environment != 'prod'
  }
}

// When using an existing Cosmos DB account we will not attempt to modify its capabilities
// Use runtime functions to read keys/endpoints for wiring into app settings
var cosmosEndpointEffective = databaseType == 'cosmosdb' ? (useExistingCosmos ? reference(resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName), '2023-11-15').documentEndpoint : cosmosDb.outputs.endpoint) : ''
var cosmosPrimaryKeyEffective = databaseType == 'cosmosdb' ? (useExistingCosmos ? listKeys(resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName), '2023-11-15').primaryMasterKey : cosmosDb.outputs.primaryKey) : ''

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
// Module: App Service Plan (for Web App)
// ==============================================

module appServicePlan './modules/app-service-plan.bicep' = {
  name: 'app-service-plan-deployment'
  params: {
    name: appServicePlanName
    location: location
    tags: tags
    sku: {
      name: 'F1' // Free tier
      tier: 'Free'
    }
    kind: 'linux'
    reserved: true
  }
}

// ==============================================
// Module: Web App (ASP.NET Core API)
// ==============================================

module webApp './modules/web-app.bicep' = {
  name: 'web-app-deployment'
  params: {
    name: webAppName
    location: location
    tags: tags
    appServicePlanId: appServicePlan.outputs.planId
    appInsightsInstrumentationKey: appInsights.outputs.instrumentationKey
    appInsightsConnectionString: appInsights.outputs.connectionString
    jwtSecretKey: jwtSecretKey
    jwtIssuer: 'icon-generator-api'
    jwtAudience: 'icon-generator-client'
    jwtExpirationMinutes: 10080 // 7 days
    allowedOrigins: [
      'http://localhost:5173'
      'http://localhost:3000'
      'https://mango-bay-068c07f0f.6.azurestaticapps.net'
    ]
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
      {
        name: 'AppSettings__UnlimitedUsers__0'
        value: 'jorgeafc00h@gmail.com'
      }
    ] , databaseType == 'cosmosdb' ? [
      {
        name: 'Database__CosmosEndpoint'
        value: cosmosEndpointEffective
      }
      {
        name: 'Database__CosmosKey'
        value: cosmosPrimaryKeyEffective
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
output cosmosAccountName string = databaseType == 'cosmosdb' ? cosmosAccountName : ''
output cosmosEndpoint string = databaseType == 'cosmosdb' ? cosmosEndpointEffective : ''
output sqlServerName string = databaseType == 'sql' ? azureSQL.outputs.serverName : ''
output sqlDatabaseName string = databaseType == 'sql' ? azureSQL.outputs.databaseName : ''

// OpenAI outputs
output openAIName string = openAI.outputs.name
output openAIEndpoint string = openAI.outputs.endpoint
output openAIKey string = openAI.outputs.apiKey

// Web App outputs
output webAppName string = webApp.outputs.webAppName
output webAppUrl string = webApp.outputs.webAppUrl
output webAppPrincipalId string = webApp.outputs.principalId

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
