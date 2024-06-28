# Include other files
Import-Module ".\infra\deployed-scripts\common-functions.psm1"

$azContext = GetAzCurrentContext
$currentUser = Get-AzADUser -UserPrincipalName $azContext.Account.Id

# Define parameters
Write-Host "Please enter the following parameters to deploy the infrastructure."
$environmentName = Read-Host "Please enter the environment name and application name (e.g. dev-aspire, test-aspire, prod-aspire), 10 characters max"
$deploymentRegion = SelectDeploymentAzureRegion
$principalId = $currentUser.Id
$sqlPassword = Read-Host "Please enter the password of SQL Server"
$parameters = @{
    environmentName = $environmentName
    location = $deploymentRegion
    principalId = $principalId
    sql_password = $sqlPassword
}
Write-Host "Selected environment name is $environmentName"
Write-Host "Selected region is $deployementRegion"
Write-Host "Selected principalId is $principalId"
Write-Host "Selected sql password is $sqlPassword"
Write-Host "================================================="
# Path to the Bicep file
$mainBicepFilePath = ".\infra\main.bicep"
Write-Host "Deploying $mainBicepFilePath"

# # Spinner characters
# $spinner = @('|', '/', '-', '\')

# # Start the long-running task in the background
# $job = Start-Job -ScriptBlock {
#    # Deploy main.bicep at the subscription scope
#     $deploymentName = "mainDeployment-" + (Get-Date -Format FileDateTimeUniversal)
#     try {
#     #  $deploymentOutput = az deployment sub create `
#     #    --name $deploymentName `
#     #    --location $location `
#     #    --template-file $mainBicepFilePath `
#     #    --parameters environmentName=$environmentName location=$location principalId=$principalId sql_password=$sqlPassword `
#     #    --output json | ConvertFrom-Json
#     $deploymentOutput = New-AzSubscriptionDeployment -Name $deploymentName -Location $deploymentRegion -TemplateFile $mainBicepFilePath -TemplateParameterObject $parameters 
#     # Check if the deployment succeeded
#     if ($deploymentOutput.properties.provisioningState -eq 'Succeeded') {
#         Write-Host "Deployment named $deploymentName succeeded."
#     } else {
#         Write-Host "Deployment named $deploymentName did not succeed. Status: $($deploymentOutput.properties.provisioningState)"
#     }
#     } catch {
#     Write-Error "An error occurred during deployment: $_"
#     # Optionally, add more detailed error handling here
#     exit 1
#     }
# }

# # Spinner animation loop
# while ($job.State -eq 'Running') {
#     foreach ($char in $spinner) {
#         # Display the spinner character
#         Write-Host "`r$char" -NoNewline
#         Start-Sleep -Milliseconds 100
#     }
# }

# # Cleanup
# Remove-Job -Job $job

# # Clear the spinner and display a completion message
# Write-Host "`rDeployment completed."



$deploymentName = "mainDeployment-" + (Get-Date -Format FileDateTimeUniversal)
try {
#  $deploymentOutput = az deployment sub create `
#    --name $deploymentName `
#    --location $location `
#    --template-file $mainBicepFilePath `
#    --parameters environmentName=$environmentName location=$location principalId=$principalId sql_password=$sqlPassword `
#    --output json | ConvertFrom-Json
$deploymentOutput = New-AzSubscriptionDeployment -Name $deploymentName -Location $deploymentRegion -TemplateFile $mainBicepFilePath -TemplateParameterObject $parameters 
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