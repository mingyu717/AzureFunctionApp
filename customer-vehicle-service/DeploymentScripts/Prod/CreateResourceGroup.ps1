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

if($IAMGroup)
{
	$Group = Get-AzureRmADGroup -SearchString $IAMGroup
}
else
{
	"Please check Octopus Variable IAMGroup is set"
}

$RoleAssigmentExists = Get-AzureRmRoleAssignment -ResourceGroupName $AzureResourceGroupName -ObjectId $Group.ID
if(!($RoleAssigmentExists))
{

  try
  {
      "Assinging IAM permissions on $AzureResourceGroupName"
      New-AzureRmRoleAssignment -ObjectId $Group.ID -RoleDefinitionName $Role -ResourceGroupName $AzureResourceGroupName
      "Added {0}  as $Role to $AzureResourceGroupName Resource Group" -f $Group.DisplayName

  }
  catch
  {
  "Unable to assign IAM permissions, please check that the $Group.DisplayName group exists in Azure Active Directory"
  }
  Finally
  {
	"Resource Group Provisioning and Role Assigmnet Completed!"
  }
}
    
    