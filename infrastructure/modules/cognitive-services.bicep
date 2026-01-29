// Azure OpenAI / Cognitive Services Module

@description('Cognitive Services account name')
param name string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

@description('SKU name')
@allowed([
  'S0'
  'S1'
])
param sku string = 'S0'

// Cognitive Services Account (OpenAI)
resource cognitiveService 'Microsoft.CognitiveServices/accounts@2023-10-01-preview' = {
  name: name
  location: location
  tags: tags
  kind: 'OpenAI'
  sku: {
    name: sku
  }
  properties: {
    customSubDomainName: name
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
}

// Outputs
output id string = cognitiveService.id
output name string = cognitiveService.name
output endpoint string = cognitiveService.properties.endpoint
output apiKey string = cognitiveService.listKeys().key1
output domain string = cognitiveService.properties.endpoint
