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
$hash['CDKAutolineServiceKey'] = $OctopusParameters["CDKAutolineServiceKey"]
$hash['CDKAutolineServiceUrl'] = $OctopusParameters["CDKAutolineServiceUrl"]
$hash['DealerConfigurationServiceKey'] = $OctopusParameters["DealerConfigurationServiceKey"]
$hash['DealerConfigurationServiceUrl'] = $OctopusParameters["DealerConfigurationServiceUrl"]

UpdateAppSettings -AppSettings $hash `
    -ResourceGroup $OctopusParameters["AzureResourceGroupName"] `
    -FunctionAppName $OctopusParameters["AzureFunctionName"] 