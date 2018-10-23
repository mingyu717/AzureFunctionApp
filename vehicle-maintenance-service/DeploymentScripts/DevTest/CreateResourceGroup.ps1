echo "Checking for RG: $AzureResourceGroupName"

try
{
Get-AzureRmResourceGroup -Name $AzureResourceGroupName
}
catch
{
    echo "No Recourse Group found with name : $AzureResourceGroupName"
    echo "Creating New ResourceGroup "
    new-azurermresourcegroup -Name $AzureResourceGroupName -Location $AzureResourceGroupLocation
}
    
    