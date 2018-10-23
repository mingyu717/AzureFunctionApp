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
$hash['invitationExpiredDays'] = $OctopusParameters["invitationExpiredDays"]
$hash['CDKAutolineServiceUrl'] = $OctopusParameters["CDKAutolineServiceUrl"]
$hash['CDKAutolineServiceKey'] = $OctopusParameters["CDKAutolineServiceKey"]
$hash['DealerConfigurationServiceKey'] = $OctopusParameters["DealerConfigurationServiceKey"]
$hash['DealerConfigurationServiceUrl'] = $OctopusParameters["DealerConfigurationServiceUrl"]
$hash['invitationFromPhoneNumber'] = $OctopusParameters["invitationFromPhoneNumber"]
$hash['pilvoAuthId'] = $OctopusParameters["pilvoAuthId"]
$hash['pilvoAuthToken'] = $OctopusParameters["pilvoAuthToken"]
$hash['serviceBookingAppUrl'] = $OctopusParameters["serviceBookingAppUrl"]
$hash['sendGridApiKey'] = $OctopusParameters["sendGridApiKey"]
$hash['fromEmailAddress'] = $OctopusParameters["fromEmailAddress"]
$hash['serviceBookingUrlPlaceHolder'] = $OctopusParameters["serviceBookingUrlPlaceHolder"]
$hash['serviceBookingEmail'] = $OctopusParameters["serviceBookingEmail"]

UpdateAppSettings -AppSettings $hash `
    -ResourceGroup $OctopusParameters["AzureResourceGroupName"] `
    -FunctionAppName $OctopusParameters["AzureFunctionName"] 