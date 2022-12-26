# Not working Powershell script to create an alert usign Log Analytics

Connect-AzAccount

$LogQuery = "Heartbeat | where TimeGenerated > ago(30m)"
$DataSourceId = "/subscriptions/0be49a2e-8088-4281-945c-1236598c6dd0/resourcegroups/fischerpl18_rg_1227/providers/microsoft.operationalinsights/workspaces/loganalytics"

$RuleSource = New-AzScheduledQueryRuleSource -Query $LogQuery `
    -DataSourceId $DataSourceId `
    -QueryType "ResultCount"

$RuleSchedule = New-AzScheduledQueryRuleSchedule -FrequencyInMinutes 5 -TimeWindowInMinutes 5

$TriggerCondition = New-AzScheduledQueryRuleTriggerCondition -ThresholdOperator "GreaterThan" -Threshold 3

$ActionGroupId = "/subscriptions/0BE49A2E-8088-4281-945C-1236598C6DD0/resourceGroups/fischerpl18_rg_1227/providers/microsoft.insights/actionGroups/ARMGroup"

$ActionGroup = New-AzScheduledQueryRuleAznsActionGroup -ActionGroup `
@($ActionGroupId) -EmailSubject "Log Alert"

$AlertAction = New-AzScheduledQueryRuleAlertingAction -AznsAction $ActionGroup -Severity "1" `
    -Trigger $TriggerCondition

$ResourceGroupName = "fischerpl18_rg_1227"

New-AzScheduledQueryRule -ResourceGroupName $ResourceGroupName -Location "East US" `
    -Action $AlertAction -Enable $true -Description "This is an alert based on Log Analytics" `
    -Schedule $RuleSchedule -Source $RuleSource -name "Log Analytics alert"