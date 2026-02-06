// Azure Web App Module

@description('Web App name')
param name string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

@description('App Service Plan ID')
param appServicePlanId string

@description('App Insights Instrumentation Key')
@secure()
param appInsightsInstrumentationKey string

@description('App Insights Connection String')
@secure()
param appInsightsConnectionString string

@description('JWT Secret Key for token signing')
@secure()
param jwtSecretKey string

@description('JWT Issuer')
param jwtIssuer string = 'icon-generator-api'

@description('JWT Audience')
param jwtAudience string = 'icon-generator-client'

@description('JWT Expiration in minutes')
param jwtExpirationMinutes int = 10080

@description('Additional app settings')
param appSettings array = []

@description('Allowed CORS origins')
param allowedOrigins array = [
  'http://localhost:5173'
  'http://localhost:3000'
  'https://mango-bay-068c07f0f.6.azurestaticapps.net'
]

// Web App
resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: name
  location: location
  tags: tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    reserved: true
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10'
      alwaysOn: false // Required for Free tier (F1)
      http20Enabled: true
      appSettings: concat([
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsightsInstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'Jwt__SecretKey'
          value: jwtSecretKey
        }
        {
          name: 'Jwt__Issuer'
          value: jwtIssuer
        }
        {
          name: 'Jwt__Audience'
          value: jwtAudience
        }
        {
          name: 'Jwt__ExpirationMinutes'
          value: string(jwtExpirationMinutes)
        }
      ], appSettings)
      cors: {
        allowedOrigins: allowedOrigins
        supportCredentials: true
      }
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      healthCheckPath: '/health'
    }
  }
}

// Outputs
output webAppId string = webApp.id
output webAppName string = webApp.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output principalId string = webApp.identity.principalId
