targetScope = 'resourceGroup'
@minLength(1)
@maxLength(10)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@description('')
param location string = resourceGroup().location

@description('')
param keyVaultName string

var resourceName = toLower(take('redis-${environmentName}-${uniqueString(resourceGroup().id)}', 24))

resource keyVault_IeF8jZvXV 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource redisCache_B6HmHCOQ5 'Microsoft.Cache/Redis@2020-06-01' = {
  name: resourceName
  location: location
  tags: {
    'aspire-resource-name': resourceName
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
