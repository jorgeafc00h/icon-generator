// Key Vault Module

@description('Key Vault name')
param name string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

@description('Enable for deployment')
param enabledForDeployment bool = true

@description('Enable RBAC authorization')
param enableRbacAuthorization bool = true

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enabledForDeployment: enabledForDeployment
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
    enableRbacAuthorization: enableRbacAuthorization
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
  }
}

// Outputs
output id string = keyVault.id
output name string = keyVault.name
output vaultUri string = keyVault.properties.vaultUri
