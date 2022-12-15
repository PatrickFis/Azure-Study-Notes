$ResourceGroup = "powershell-grp"
$Location = "East US 2"
$AppServicePlanName = "az204udemyplan"
$WebAppName = "az204udemywebapp"

Connect-AzAccount

New-AzResourceGroup -Name $ResourceGroup -Location $Location

New-AzAppServicePlan -ResourceGroupName $ResourceGroup `
    -Location $Location -Tier "B1" -NumberofWorkers 1 -Name $AppServicePlanName

New-AzWebApp -ResourceGroupName $ResourceGroup -Name $WebAppName `
    -Location $Location -AppServicePlan $AppServicePlanName