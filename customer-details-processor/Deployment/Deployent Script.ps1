function UpdateAppSettings {
 param( [string]$ResourceGroup, [string]$FunctionAppName, [hashtable]$AppSettings )

    Write-Host "Loading Existing AppSettings"
    $webApp = Get-AzureRmWebApp  -ResourceGroupName  $ResourceGroup -Name $FunctionAppName

    Write-Host "Applying New AppSettings"
    $hash = @{}
    ForEach ($kvp in $webApp.SiteConfig.AppSettings) {
        $hash[$kvp.Name] = $kvp.Value
    }

    ForEach ($key in $AppSettings.Keys) {
        $hash[$key] = $AppSettings[$key]
    }

    Write-Host "Saving AppSettings"
    Set-AzureRmWebApp -ResourceGroupName $ResourceGroup -Name $FunctionAppName -AppSettings $hash | Out-Null
    Write-Host "AppSettings Updated"
}

$hash = @{}
$hash['azureBlobConnectionString'] = $OctopusParameters["azureBlobConnectionString"]
$hash['azureBlobContainer'] = $OctopusParameters["azureBlobContainer"]
$hash['communityIdColumn'] = $OctopusParameters["communityIdColumn"]
$hash['csvSource'] = $OctopusParameters["csvSource"]
$hash['customerEmailColumn'] = $OctopusParameters["customerEmailColumn"]
$hash['customerNoColumn'] = $OctopusParameters["customerNoColumn"]
$hash['customerVehicleAppUrl'] = $OctopusParameters["customerVehicleAppUrl"]
$hash['customerVehicleAppKey'] = $OctopusParameters["customerVehicleAppKey"]
$hash['dealerConfigurationServiceKey'] = $OctopusParameters["dealerConfigurationServiceKey"]
$hash['dealerConfigurationServiceUrl'] = $OctopusParameters["dealerConfigurationServiceUrl"]
$hash['exchangePassword'] = $OctopusParameters["exchangePassword"]
$hash['exchangeUrl'] = $OctopusParameters["exchangeUrl"]
$hash['exchangeUserName'] = $OctopusParameters["exchangeUserName"]
$hash['exchangeVersion'] = $OctopusParameters["exchangeVersion"]
$hash['firstNameColumn'] = $OctopusParameters["firstNameColumn"]
$hash['lastKnownMileageColumn'] = $OctopusParameters["lastKnownMileageColumn"]
$hash['lastServiceDateColumn'] = $OctopusParameters["lastServiceDateColumn"]
$hash['makeCodeColumn'] = $OctopusParameters["makeCodeColumn"]
$hash['modelCodeColumn'] = $OctopusParameters["modelCodeColumn"]
$hash['modelDescriptionColumn'] = $OctopusParameters["modelDescriptionColumn"]
$hash['modelYearColumn'] = $OctopusParameters["modelYearColumn"]
$hash['nextServiceMileageColumn'] = $OctopusParameters["nextServiceMileageColumn"]
$hash['nextServiceDateColumn'] = $OctopusParameters["nextServiceDateColumn"]
$hash['phoneNumberColumn'] = $OctopusParameters["phoneNumberColumn"]
$hash['registrationNoColumn'] = $OctopusParameters["registrationNoColumn"]
$hash['roofTopIdColumn'] = $OctopusParameters["roofTopIdColumn"]
$hash['scheduleInHours'] = $OctopusParameters["scheduleInHours"]
$hash['scheduleInMinutes'] = $OctopusParameters["scheduleInMinutes"]
$hash['scheduleInSeconds'] = $OctopusParameters["scheduleInSeconds"]
$hash['surNameColumn'] = $OctopusParameters["surNameColumn"]
$hash['vehicleNoColumn'] = $OctopusParameters["vehicleNoColumn"]
$hash['vinNumberColumn'] = $OctopusParameters["vinNumberColumn"]
$hash['variantCodeColumn'] = $OctopusParameters["variantCodeColumn"]

UpdateAppSettings -AppSettings $hash `
    -ResourceGroup $OctopusParameters["AzureResourceGroupName"] `
    -FunctionAppName $OctopusParameters["AzureFunctionName"] 