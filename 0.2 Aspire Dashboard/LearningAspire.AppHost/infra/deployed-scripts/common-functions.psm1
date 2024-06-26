function GetAzureRegions {
    $regions = Get-AzLocation | Select-Object -ExpandProperty Location
    return $regions -split "`n"
}

function GetUserSelectedRegion {
    param (
        [string]$suggestedRegion
    )

    $userConfirmation = Read-Host "The default region is $suggestedRegion. Do you want to deploy to this region? (Y/N)"
    if ($userConfirmation -eq 'Y') {
        return $suggestedRegion
    } else {
        $regions = GetAzureRegions
        Write-Host "Please select a region from the following list:"
        $index = 1
        foreach ($region in $regions) {
            Write-Host "$index. $region"
            $index++
        }

        $selection = Read-Host "Enter the number of your selected region"
        if ($selection -le $regions.Length -and $selection -gt 0) {
            return $regions[$selection - 1]
        } else {
            Write-Host "Invalid selection. Using the detected region $suggestedRegion."
            return $suggestedRegion
        }
    }

}

function GetAzCurrentContext {
    $context = Get-AzContext

    if ($context) {
        Write-Host "You are already logged in to Azure."
    } else {
        Write-Host "You are not logged in to Azure. Please login"
        Connect-AzAccount
    }
    return $context
}

function SelectDeploymentAzureRegion {
    Write-Host "Please select azure region."
    $region = GetUserSelectedRegion "westeurope" 
    return $region
}

Export-ModuleMember -Function SelectDeploymentAzureRegion, GetAzCurrentContext



