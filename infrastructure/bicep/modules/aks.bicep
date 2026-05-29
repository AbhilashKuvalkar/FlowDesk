param location string
param environmentName string
param keyVaultName string

var aksName = 'flowdesk-aks-${environmentName}'
var nodeCount = environmentName == 'prod' ? 3 : 1

resource aks 'Microsoft.ContainerService/managedClusters@2026-02-01' = {
  name: aksName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    dnsPrefix: 'flowdesk-${environmentName}'
    agentPoolProfiles: [
      {
        name: 'system'
        count: nodeCount
        vmSize: 'Standard_D2s_v3'
        osType: 'Linux'
        mode: 'System'
      }
    ]
    networkProfile:{
      networkPlugin: 'azure'
      loadBalancerSku: 'standard'
    }
  }
}

// Grant AKS identity access to read key vault secrets
var keyVaultSecretsUserRoleId = '4633458b-17de-408a-b874-0445c86b69e6'

resource keyVaultRef 'Microsoft.KeyVault/vaults@2025-05-01' existing = {
   name: keyVaultName
}

resource aksKeyVaultRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVaultRef
  name: guid(aks.id, keyVaultRef.id, keyVaultSecretsUserRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsUserRoleId)
    principalId: aks.properties.identityProfile.kubeletidentity.objectId
    principalType: 'ServicePrincipal'
  }
}

output aksName string = aks.name
output aksFqdn string = aks.properties.fqdn
output kubeletIdentityObjectId string = aks.properties.identityProfile.kubeletidentity.objectId
