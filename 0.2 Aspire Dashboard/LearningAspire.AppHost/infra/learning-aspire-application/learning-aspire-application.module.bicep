targetScope = 'resourceGroup'
@minLength(1)
@maxLength(10)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@description('')
param location string = resourceGroup().location

@description('')
param applicationType string = 'web'

@description('')
param kind string = 'web'

@description('')
param logAnalyticsWorkspaceId string

var resourceName = toLower(take('ai-${environmentName}-${uniqueString(resourceGroup().id)}', 24))

resource applicationInsightsComponent_OnKLUiKBL 'Microsoft.Insights/components@2020-02-02' = {
  name: resourceName
  location: location
  tags: {
    'aspire-resource-name': resourceName
  }
  kind: kind
  properties: {
    Application_Type: applicationType
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

output appInsightsConnectionString string = applicationInsightsComponent_OnKLUiKBL.properties.ConnectionString
