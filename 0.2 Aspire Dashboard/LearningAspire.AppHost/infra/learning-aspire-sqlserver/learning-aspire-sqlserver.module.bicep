targetScope = 'resourceGroup'
@minLength(1)
@maxLength(10)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@description('')
param location string = resourceGroup().location

@description('')
param principalId string

@description('')
param principalName string

var resourceName = toLower(take('sql-${environmentName}-${uniqueString(resourceGroup().id)}', 24))

resource sqlServer_Ke7emd7ch 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: resourceName
  location: location
  tags: {
    'aspire-resource-name': resourceName
  }
  properties: {
    version: '12.0'
    publicNetworkAccess: 'Enabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      login: principalName
      sid: principalId
      tenantId: subscription().tenantId
      azureADOnlyAuthentication: true
    }
  }
}

resource sqlFirewallRule_TfP8HMWC1 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  parent: sqlServer_Ke7emd7ch
  name: 'AllowAllAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource sqlDatabase_eKC421Q92 'Microsoft.Sql/servers/databases@2020-11-01-preview' = {
  parent: sqlServer_Ke7emd7ch
  name: 'employees-sqldb'
  location: location
  properties: {
  }
}

output sqlServerFqdn string = sqlServer_Ke7emd7ch.properties.fullyQualifiedDomainName
