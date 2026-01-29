// Log Analytics Workspace Module

@description('Log Analytics workspace name')
param name string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

@description('Data retention in days')
@minValue(30)
@maxValue(730)
param retentionInDays int = 30

// Log Analytics Workspace
resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: retentionInDays
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// Outputs
output workspaceId string = workspace.id
output workspaceName string = workspace.name
output customerId string = workspace.properties.customerId
