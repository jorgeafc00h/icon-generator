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
// Module: Cosmos DB
// ==============================================

module cosmosDb './modules/cosmos-db.bicep' = {
  name: 'cosmos-deployment'
  params: {
    name: cosmosAccountName
    location: location
    tags: tags
    enableFreeTier: environment == 'dev'
    enableServerless: environment != 'prod'
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
    runtime: 'node'
    runtimeVersion: '18'
    appSettings: [
      {
        name: 'COSMOS_ENDPOINT'
        value: cosmosDb.outputs.endpoint
      }
      {
        name: 'AZURE_OPENAI_ENDPOINT'
        value: openAI.outputs.endpoint
      }
      {
        name: 'STORAGE_CONTAINER_NAME'
        value: 'generated-icons'
      }
    ]
  }
}

// ==============================================
// Module: Static Web App
// ==============================================

module staticWebApp './modules/static-web-app.bicep' = {
  name: 'static-web-app-deployment'
  params: {
    name: staticWebAppName
    location: location
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
output storageConnectionString string = storage.outputs.connectionString
output storageBlobEndpoint string = storage.outputs.blobEndpoint

// Cosmos DB outputs
output cosmosAccountName string = cosmosDb.outputs.accountName
output cosmosEndpoint string = cosmosDb.outputs.endpoint
output cosmosKey string = cosmosDb.outputs.primaryKey

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
output staticWebAppToken string = staticWebApp.outputs.apiKey

// Monitoring outputs
output appInsightsName string = appInsights.outputs.name
output appInsightsInstrumentationKey string = appInsights.outputs.instrumentationKey
output logAnalyticsWorkspaceId string = logAnalytics.outputs.workspaceId

// Key Vault outputs
output keyVaultName string = keyVault.outputs.name
output keyVaultUri string = keyVault.outputs.vaultUri
