// Azure SQL Database Module

@description('SQL Server name')
param serverName string

@description('Database name')
param databaseName string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

@description('SQL Administrator login')
param administratorLogin string = 'sqladmin'

@description('SQL Administrator password')
@secure()
param administratorPassword string

@description('Database SKU')
@allowed([
  'Basic'
  'S0'
  'S1'
  'S2'
  'P1'
  'P2'
])
param sku string = 'Basic'

@description('Enable public network access')
param enablePublicAccess bool = true

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: enablePublicAccess ? 'Enabled' : 'Disabled'
  }
}

// Firewall rule - Allow Azure services
resource firewallRuleAzure 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = if (enablePublicAccess) {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// SQL Database
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: tags
  sku: {
    name: sku
    tier: sku == 'Basic' ? 'Basic' : (sku == 'S0' || sku == 'S1' || sku == 'S2' ? 'Standard' : 'Premium')
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: sku == 'Basic' ? 2147483648 : 268435456000 // 2GB for Basic, 250GB for others
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
  }
}

// Outputs
output serverId string = sqlServer.id
output serverName string = sqlServer.name
output databaseId string = sqlDatabase.id
output databaseName string = sqlDatabase.name
output fullyQualifiedDomainName string = sqlServer.properties.fullyQualifiedDomainName
output connectionString string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${databaseName};Persist Security Info=False;User ID=${administratorLogin};Password=${administratorPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
