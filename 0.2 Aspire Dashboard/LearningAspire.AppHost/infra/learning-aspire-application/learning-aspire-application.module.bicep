targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param applicationType string = 'web'

@description('')
param kind string = 'web'

@description('')
param logAnalyticsWorkspaceId string


resource applicationInsightsComponent_OnKLUiKBL 'Microsoft.Insights/components@2020-02-02' = {
  name: toLower(take('learning-aspire-application${uniqueString(resourceGroup().id)}', 24))
  location: location
  tags: {
    'aspire-resource-name': 'learning-aspire-application'
  }
  kind: kind
  properties: {
    Application_Type: applicationType
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

output appInsightsConnectionString string = applicationInsightsComponent_OnKLUiKBL.properties.ConnectionString
