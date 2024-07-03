targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param principalId string

@description('')
param principalName string


resource sqlServer_Ke7emd7ch 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: toLower(take('learning-aspire-sqlserver${uniqueString(resourceGroup().id)}', 24))
  location: location
  tags: {
    'aspire-resource-name': 'learning-aspire-sqlserver'
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
