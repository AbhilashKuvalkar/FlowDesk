param location string = resourceGroup().location
param environmentName string

@secure()
param sqlAdminLogin string

@secure()
param sqlAdminPassword string

// module SQL Server
module sql 'modules/sql.bicep' = {
  name: 'sql-deployment'
  params: {
    location: location
    environmentName: environmentName
    sqlAdminLogin: sqlAdminLogin
    sqlAdminPassword: sqlAdminPassword
  }
}

// module Redis
module redis 'modules/redis.bicep' = {
  name: 'redis-deployment'
  params: {
    location: location
    environmentName: environmentName
  }
}

// module Container Registry
module acr 'modules/containerregistry.bicep' = {
  name: 'acr-deployment'
  params: {
    location: location
    environmentName: environmentName
  }
}

// module 
module logAnalytics 'modules/monitoring.bicep' = {
  name: 'monitoring-deployment'
  params: {
    environmentName: environmentName
    location: location
  }
}

// module Container Registry
module keyvault 'modules/keyvault.bicep' = {
  name: 'keyvault-deployment'
  params: {
    location: location
    environmentName: environmentName
    redisHostName: redis.outputs.redisHostName
    redisPort: redis.outputs.redisPort
    redisPrimaryKey: redis.outputs.redisPrimaryKey
    sqlConnectionString: sql.outputs.connectionString
    appInsightsConnectionString: logAnalytics.outputs.appInsightsConnectionString
    appInsightsInstrumentationKey: logAnalytics.outputs.appInsightsInstrumentationKey
  }
}

// AKS depends on KeyVault
module aks 'modules/aks.bicep' = {
  name: 'aks-deployment'
  params: {
    location: location
    environmentName: environmentName
    keyVaultName: keyvault.outputs.keyVaultName
  }
}

output aksName string = aks.outputs.aksName
output acrLoginServer string = acr.outputs.acrLoginServer
output keyVaultUri string = keyvault.outputs.keyVaultUri
