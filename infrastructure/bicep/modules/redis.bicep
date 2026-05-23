param location string
param environmentName string

var redisName = 'flowdesk-redis-${environmentName}'

resource redisCache 'Microsoft.Cache/redis@2024-11-01' = {
  name: redisName
  location: location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
    redisConfiguration: {
      'maxmemory-policy': 'allkeys-lru'
    }
  }
}

output redisHostName string = redisCache.properties.hostName
output redisPort int = redisCache.properties.port

@secure()
output redisPrimaryKey string = redisCache.listKeys().primaryKey
