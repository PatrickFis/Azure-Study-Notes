# Turn on the managed identity on the specified VM
Connect-AzAccount

$ResourceGroupName = "az204keyvault"
$VmName = "az204tempvm"

$Vm = Get-AzVM -ResourceGroupName $ResourceGroupName -Name $VmName

Update-AzVM -ResourceGroupName $ResourceGroupName -VM $Vm -IdentityType SystemAssigned