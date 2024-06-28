function GetAzureRegions {
    $locationsJson = az account list-locations --output json | ConvertFrom-Json
    return $locationsJson
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
            Write-Host "$index. $($region.displayName). ($($region.name))"
            $index++
        }

        $selection = Read-Host "Enter the number of your selected region"
        if ($selection -le $regions.Length -and $selection -gt 0) {
            return $regions[$selection - 1].name
        } else {
            Write-Host "Invalid selection. Using the detected region $suggestedRegion."
            return $suggestedRegion
        }
    }

}

function GetAzCurrentAccount {
    $account = az account show --output json | ConvertFrom-Json
    if ($account) {
        Write-Host "You are already logged in to Azure."
    } else {
        Write-Host "You are not logged in to Azure. Please login"
        az login
        $account = az account show --output json | ConvertFrom-Json

    }
    return $account
}

function SelectDeploymentAzureRegion {
    Write-Host "Please select azure region."
    $region = GetUserSelectedRegion "westeurope" 
    return $region
}


function GetAzCurrentLogInAccount {
    $account = az ad signed-in-user show --output json | ConvertFrom-Json
    if ($account) {
        Write-Host "You are already logged in to Azure."
    } else {
        Write-Host "You are not logged in to Azure. Please login"
        az login
        $account = az ad signed-in-user show --output json | ConvertFrom-Json

    }
    return $account
}
