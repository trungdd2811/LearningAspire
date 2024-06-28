# Include other files
. .\infra\deployed-scripts\azure-cli\common-functions-azure-cli.ps1

$azAccount = GetAzCurrentLogInAccount

# Define parameters
Write-Host "Please enter the following parameters to deploy the infrastructure."
$environmentName = Read-Host "Please enter the environment name and application name (e.g. dev-aspire, test-aspire, prod-aspire), 10 characters max"
$deploymentRegion = SelectDeploymentAzureRegion
$principalId = $azAccount.id
$sqlPassword = Read-Host "Please enter the password of SQL Server"

Write-Host "Selected environment name is $environmentName"
Write-Host "Selected region is $deploymentRegion"
Write-Host "Selected principalId is $principalId"
Write-Host "Selected sql password is $sqlPassword"
Write-Host "================================================="
# Path to the Bicep file
$mainBicepFilePath = ".\infra\main.bicep"

Write-Host "Deploying $mainBicepFilePath"

$deploymentName = "mainDeployment-" + (Get-Date -Format FileDateTimeUniversal)
try {
 $deploymentOutput = az deployment sub create `
   --name $environmentName `
   --location $deploymentRegion `
   --template-file $mainBicepFilePath `
   --parameters environmentName=$environmentName location=$deploymentRegion principalId=$principalId sql_password=$sqlPassword `
   --output json | ConvertFrom-Json

#$deploymentOutput = New-AzSubscriptionDeployment -Name $deploymentName -Location $deploymentRegion -TemplateFile $mainBicepFilePath -TemplateParameterObject $parameters 

# Check if the deployment succeeded
if ($deploymentOutput.properties.provisioningState -eq 'Succeeded') {
    Write-Host "Deployment named $deploymentName succeeded."
} else {
    Write-Host "Deployment named $deploymentName did not succeed. Status: $($deploymentOutput.properties.provisioningState)"
}
} catch {
Write-Error "An error occurred during deployment: $_"
# Optionally, add more detailed error handling here
exit 1
}