// App Service Plan Module

@description('App Service Plan name')
param name string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

@description('SKU configuration')
param sku object

@description('Kind of plan')
@allowed([
  'linux'
  'windows'
])
param kind string = 'linux'

@description('Reserved for Linux')
param reserved bool = true

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: name
  location: location
  tags: tags
  kind: kind
  sku: sku
  properties: {
    reserved: reserved
  }
}

// Outputs
output planId string = appServicePlan.id
output planName string = appServicePlan.name
