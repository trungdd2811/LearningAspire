targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for all deployed resources')
param location string

@description('Id of the user or app to assign application roles')
param principalId string = ''

@secure()
param sql_password string = 'Trung123456'

var tags = {
  'azd-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
    location: location
    tags: tags
    principalId: principalId
  }
}

module learning_aspire_application 'learning-aspire-application/learning-aspire-application.module.bicep' = {
  name: 'learning-aspire-application'
  scope: rg
  params: {
    location: location
    logAnalyticsWorkspaceId: resources.outputs.AZURE_LOG_ANALYTICS_WORKSPACE_ID
  }
}
module learning_aspire_sqlserver 'learning-aspire-sqlserver/learning-aspire-sqlserver.module.bicep' = {
  name: 'learning-aspire-sqlserver'
  scope: rg
  params: {
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalName: resources.outputs.MANAGED_IDENTITY_NAME
  }
}
module redis_cache 'redis-cache/redis-cache.module.bicep' = {
  name: 'redis-cache'
  scope: rg
  params: {
    keyVaultName: resources.outputs.SERVICE_BINDING_KV9DC776CF_NAME
    location: location
  }
}
output MANAGED_IDENTITY_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output MANAGED_IDENTITY_NAME string = resources.outputs.MANAGED_IDENTITY_NAME
output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = resources.outputs.AZURE_LOG_ANALYTICS_WORKSPACE_NAME
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = resources.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID
output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_ID
output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN
output SERVICE_BINDING_KV9DC776CF_ENDPOINT string = resources.outputs.SERVICE_BINDING_KV9DC776CF_ENDPOINT

output LEARNING_ASPIRE_APPLICATION_APPINSIGHTSCONNECTIONSTRING string = learning_aspire_application.outputs.appInsightsConnectionString
output LEARNING_ASPIRE_SQLSERVER_SQLSERVERFQDN string = learning_aspire_sqlserver.outputs.sqlServerFqdn