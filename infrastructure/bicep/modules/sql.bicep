param location string
param environmentName string
param sqlAdminLogin string

@secure()
param sqlAdminPassword string

var sqlServerName = 'flowdesk-sql-${environmentName}'
var databaseName = 'FlowDesk'

resource sqlServer 'Microsoft.Sql/servers@2025-01-01' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    minimalTlsVersion: '1.2'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2025-01-01' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  properties:{
    collation: 'SQL_Latin1_General_CP1_CI_AS'
  }
}

resource sqlFirewallRule 'Microsoft.Sql/servers/firewallRules@2025-01-01' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties:{
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output databaseName string = sqlDatabase.name

@secure()
output connectionString string = 'Server=${sqlServer.properties.fullyQualifiedDomainName};Database=${databaseName};User Id=${sqlAdminLogin};Password=${sqlAdminPassword};TrustServerCertificate=True;Encrypt=True;'
