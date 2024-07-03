targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param keyVaultName string


resource keyVault_IeF8jZvXV 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource redisCache_B6HmHCOQ5 'Microsoft.Cache/Redis@2020-06-01' = {
  name: toLower(take('redis-cache${uniqueString(resourceGroup().id)}', 24))
  location: location
  tags: {
    'aspire-resource-name': 'redis-cache'
  }
  properties: {
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 1
    }
  }
}

resource keyVaultSecret_Ddsc3HjrA 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault_IeF8jZvXV
  name: 'connectionString'
  location: location
  properties: {
    value: '${redisCache_B6HmHCOQ5.properties.hostName},ssl=true,password=${redisCache_B6HmHCOQ5.listKeys(redisCache_B6HmHCOQ5.apiVersion).primaryKey}'
  }
}
