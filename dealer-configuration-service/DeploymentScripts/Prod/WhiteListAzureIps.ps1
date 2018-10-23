$AzureIPListUri ="https://experiecoextensions.blob.core.windows.net/deploymentobjects/AzurePublicIPs_20181008.xml"
$ExperiecoIP ="103.240.133.0/24"

[xml]$Content = curl $AzureIPListUri
if($Content -eq $null)
{
    Throw "Could not fetch Azure IP List, please check the resource exists and the uri is correct"
}


$Resource = Get-AzureRmResource -ResourceGroupName $AzureResourceGroupName -ResourceType Microsoft.Web/sites/config -ResourceName "$AzureFunctionName/web" -ApiVersion 2018-02-01

$p = $Resource.Properties
$p.ipSecurityRestrictions = @()

if($p.ipSecurityRestrictions.Count -le 0)
{
    echo "No IP restrictions Configured, Adding Rules"
    $count=0

    foreach($IPRANGE in $Content.AzurePublicIpAddresses.Region.IPRange.Subnet)
    {
        $restriction = @{}
        $restriction.Add("ipAddress","$IPRANGE")
        $restriction.Add("Name","AzureIPRange-$Count")
        $p.ipSecurityRestrictions+= $restriction
        $Count++

        echo "Adding Ipaddress : $IPRANGE and Count is $Count"
    }

    echo "Adding Experieco On Premises IP Address: $ExperiecoIP"
    $restriction = @{}
    $restriction.Add("ipAddress","$ExperiecoIP")
    $restriction.Add("Name","Experieco-OnPrem")
    $p.ipSecurityRestrictions+= $restriction

    Set-AzureRmResource -ResourceGroupName $AzureResourceGroupName -ResourceType Microsoft.Web/sites/config -ResourceName "$AzureFunctionName/web" -ApiVersion 2018-02-01 -PropertyObject $p -Force 
}













