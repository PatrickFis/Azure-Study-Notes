# Get the first access key from a storage account and store it in a key vault
Connect-AzAccount

$StorageAccountName = "az204patrickstorage"
$ResourceGroupName = "fischerpl18_rg_1227"

# Get the first key from the storage account
$StorageAccountKey = (Get-AzStorageAccountKey  -ResourceGroupName $ResourceGroupName `
        -AccountName $StorageAccountName) | Where-Object { $_.KeyName -eq "key1" }

$StorageAccountKeyValue = $StorageAccountKey.Value

$KeyVaultName = "az204appservicekeyvault"
$SecretValue = ConvertTo-SecureString $StorageAccountKeyValue -AsPlainText -Force

Set-AzKeyVaultSecret -VaultName $KeyVaultName -Name $StorageAccountName `
    -SecretValue $SecretValue