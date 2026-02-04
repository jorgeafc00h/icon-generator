// Static Web App Module

@description('Static Web App name')
param name string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

@description('SKU name')
@allowed([
  'Free'
  'Standard'
])
param sku string = 'Free'

// Static Web App
resource staticWebApp 'Microsoft.Web/staticSites@2023-01-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: sku
    tier: sku
  }
  properties: {
    buildProperties: {
      skipGithubActionWorkflowGeneration: true
    }
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
  }
}

// Outputs
output id string = staticWebApp.id
output name string = staticWebApp.name
output defaultHostname string = 'https://${staticWebApp.properties.defaultHostname}'
// apiKey removed from outputs for security
