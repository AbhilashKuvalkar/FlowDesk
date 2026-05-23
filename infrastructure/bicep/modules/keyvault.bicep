param location string
param environmentName string
param redisHostName string
param redisPort int
param appInsightsInstrumentationKey string
param appInsightsConnectionString string

@secure()
param sqlConnectionString string
@secure()
param redisPrimaryKey string

var keyVaultName = 'flowdesk-kv-${environmentName}'

resource keyVault 'Microsoft.KeyVault/vaults@2025-05-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
  }
}

resource sqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: keyVault
  name: 'SqlConnectionString'
  properties: {
    value: sqlConnectionString
  }
}

resource redisConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: keyVault
  name: 'RedisConnectionString'
  properties: {
    value: '${redisHostName}:${redisPort},password=${redisPrimaryKey},ssl=True'
  }
}

resource appInsightsConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: keyVault
  name: 'AppInsightConnectionString'
  properties: {
    value: appInsightsConnectionString
  }
}

resource appInsightsInstrumentationKeySecret 'Microsoft.KeyVault/vaults/secrets@2025-05-01' = {
  parent: keyVault
  name: 'AppInsightInstrumentationKey'
  properties: {
    value: appInsightsInstrumentationKey
  }
}

output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri
