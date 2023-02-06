- [Azure Powershell](#azure-powershell)
  - [Connect to an Azure account (skip if using Cloud Shell)](#connect-to-an-azure-account-skip-if-using-cloud-shell)
  - [Resource Groups](#resource-groups)
    - [Create a Resource Group](#create-a-resource-group)
    - [Delete a Resource Group](#delete-a-resource-group)
  - [Virtual Machines](#virtual-machines)
    - [Creating a VM (Walkthrough)](#creating-a-vm-walkthrough)
  - [Storage Accounts](#storage-accounts)
    - [Create a storage account and transfer a file to it (Walkthrough for creation) (Walkthrough for uploading a file)](#create-a-storage-account-and-transfer-a-file-to-it-walkthrough-for-creation-walkthrough-for-uploading-a-file)
  - [Key Vault](#key-vault)
    - [Create an Azure Key Vault (Walkthrough)](#create-an-azure-key-vault-walkthrough)
    - [Storing an access key from a storage account in a key vault](#storing-an-access-key-from-a-storage-account-in-a-key-vault)
  - [SQL Server](#sql-server)
    - [Create a SQL Server and a database](#create-a-sql-server-and-a-database)
  - [ARM](#arm)
    - [Deploying ARM templates](#deploying-arm-templates)
  - [API Management](#api-management)
    - [Create an API Management Service Instance](#create-an-api-management-service-instance)
    - [Create an API](#create-an-api)
  - [Managed Identities](#managed-identities)
    - [Create an User Assigned Identity](#create-an-user-assigned-identity)
    - [Create a System Assigned Managed Identity](#create-a-system-assigned-managed-identity)
  - [CDN](#cdn)
    - [Create a new CDN Profile](#create-a-new-cdn-profile)
  - [Azure Service Bus](#azure-service-bus)
    - [Create a Service Bus Namespace and a Queue](#create-a-service-bus-namespace-and-a-queue)

# Azure Powershell
[MS Documentation](https://learn.microsoft.com/en-us/powershell/azure/get-started-azureps?view=azps-9.2.0) is a good place to reference Powershell commands.

[Powershell Examples](https://github.com/Azure/azure-docs-powershell-samples)

## Connect to an Azure account (skip if using Cloud Shell)
``` powershell
Connect-AzAccount
```

## Resource Groups
### Create a Resource Group
``` powershell
New-AzResourceGroup -Name <resource group name> `
-Location <location> `
```

### Delete a Resource Group
```powershell
Remove-AzResourceGroup -Name <resource group name> `
-AsJob (optional, runs the command in the background) `
-Force (optional, forces the command to run without asking the user for confirmation)
```

## Virtual Machines
### Creating a VM ([Walkthrough](https://learn.microsoft.com/en-us/powershell/azure/azureps-vm-tutorial?view=azps-9.2.0))
``` powershell
# Create a resource group
New-AzResourceGroup -Name "temptutorial" -Location "EastUS"

# Get credentials for the VM and store them in a variable
$cred = Get-Credential -Message "Enter a username and password for the virtual machine."

# Create the VM using parameters (see https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_splatting?view=powershell-7.3 for information on the vmParams variable)
$vmParams = @{
  ResourceGroupName = 'temptutorial'
  Name = 'TutorialVM1'
  Location = 'eastus'
  ImageName = 'Win2016Datacenter'
  PublicIpAddressName = 'tutorialPublicIp'
  Credential = $cred
  OpenPorts = 3389
  Size = 'Standard_B1s'
}
$newVM1 = New-AzVM @vmParams

# Inspect the VM in the portal or by looking at the newVM1 variable
$newVM1

# Retrieve the name of the VM and username of the admin (note that other properties could be specified of the entire object could be retrieved by not piping the output to Select-Object)
$newVM1.OSProfile | Select-Object -Property ComputerName,AdminUserName

# Retrieve information about the network configuration of the VM
$newVM1 | Get-AzNetworkInterface |
  Select-Object -ExpandProperty IpConfigurations |
    Select-Object -Property Name, PrivateIpAddress

# Retrieve the public IP address, store it, and grab the FQDN from it
$publicIp = Get-AzPublicIpAddress -Name tutorialPublicIp -ResourceGroupName temptutorial
$publicIp | Select-Object -Property Name, IpAddress, @{label='FQDN';expression={$_.DnsSettings.Fqdn}}

# After you're done with the VM remove the resource group
$job = Remove-AzResourceGroup -Name temptutorial -Force -AsJob
```

## Storage Accounts
### Create a storage account and transfer a file to it ([Walkthrough for creation](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-create?tabs=azure-powershell)) ([Walkthrough for uploading a file](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-powershell))
``` powershell
# Create a new resource group for the storage account
$rg = New-AzResourceGroup -Name tempstoragetutorial -Location eastus

# Create a storage account
$storageaccount = New-AzStorageAccount -ResourceGroup $rg.ResourceGroupName `
-Name tempstoragetutorial `
-Location $rg.Location `
-SkuName Standard_LRS `
-Kind StorageV2

# Create a container
$containername = "blobstesting"

New-AzStorageContainer -Name $containername `
-Context $storageaccount.Context `
-Permission Blob

# Upload blobs to the container
$Blob1 = @{
    File = "/home/patrick/htmlapp/html-docs-hello-world/img/azure-portal.png"
    Container = $containername
    Blob = "azure-portal.png"
    Context = $storageaccount.Context
    StandardBlobTier = "Hot"
}
Set-AzStorageBlobContent @Blob1

$Blob2 = @{
    File = "/home/patrick/htmlapp/html-docs-hello-world/img/cdn.png"
    Container = $containername
    Blob = "cdn.png"
    Context = $storageaccount.Context
    StandardBlobTier = "Cool"
}
Set-AzStorageBlobContent @Blob2

$Blob3 = @{
    File = "/home/patrick/htmlapp/html-docs-hello-world/img/cdn.png"
    Container = $containername
    Blob = "Archive/cdn.png"
    Context = $storageaccount.Context
    StandardBlobTier = "Archive"
}
Set-AzStorageBlobContent @Blob3

# List the blobs in a container
Get-AzStorageBlob -Container $containername -Context $storageaccount.Context | Select-Object -Property Name

# Download blobs from a container
$DownloadBlob1 = @{
    Blob = $Blob1.Blob
    Container = $containername
    Destination = "/home/patrick/tmp.png"
    Context = $storageaccount.Context
}
Get-AzStorageBlobContent @DownloadBlob1

$DownloadBlob2 = @{
    Blob = $Blob2.Blob
    Container = $containername
    Destination = "/home/patrick/tmp2.png"
    Context = $storageaccount.Context
}
Get-AzStorageBlobContent @DownloadBlob2

# Remove the storage account when you're done with it (just remove the resource group, but this is just to note the command for removing the storage account)
Remove-AzStorageAccount -Name $storageaccount.StorageAccountName `
-ResourceGroup $storageaccount.ResourceGroupName `
-Force

# Remove the resource group
Remove-AzResourceGroup -name $rg.ResourceGroupName -Force -AsJob
```

## Key Vault
### Create an Azure Key Vault ([Walkthrough](https://learn.microsoft.com/en-us/azure/key-vault/secrets/quick-create-powershell))
``` powershell
# Create a resource group
$rg = New-AzResourceGroup -Name tempkeyvaultgroup -Location eastus

# Create a key vault, feel free to inspect the variable
$kv = New-AzKeyVault -Name tempkeyvaultaz204 `
-ResourceGroupName $rg.ResourceGroupName `
-Location $rg.Location

# If you need to reopen Powershell you can set up $kv again with the following command
$kv = Get-AzKeyVault -Name tempkeyvaultaz204

# Grant yourself access to the key vault. Your user principal name is available in Azure Active Directory.
Set-AzKeyVaultAccessPolicy -VaultName $kv.VaultName `
-UserPrincipalName <see AAD> `
-PermissionsToSecrets get,set,delete,list `
-PermissionsToKeys get,create,delete,list `
-PermissionsToCertificates get,create,delete,list

# Create a secret value
$secretvalue = ConvertTo-SecureString "asdf1234" -AsPlainText -Force

# Store the secret in the vault
$secret = Set-AzKeyVaultSecret -VaultName $kv.VaultName `
-Name "ExampleSecret" `
-SecretValue $secretvalue

# Retrieve the secret from the vault
$secret = Get-AzKeyVaultSecret -VaultName $kv.VaultName `
-Name "ExampleSecret" `
-AsPlainText

# Remove the key vault (just remove the resource group, this is shown to demonstrate the command for removing a key vault)
Remove-AzKeyVault -Name $kv.VaultName -ResourceGroupName $rg.ResourceGroupName -Force -AsJob

# Remove the resource group
Remove-AzResourceGroup -Name $rg.ResourceGroupName -Force -AsJob
```

### Storing an access key from a storage account in a key vault
``` powershell
# Store the storage account access key in a variable
$StorageAccountName = "az204patrickstorage"
$ResourceGroupName = "fischerpl18_rg_1227"

# Get the first key from the account
$StorageAccountKey = (Get-AzStorageAccountKey -ResourceGroupName $ResourceGroupName `
-AccountName $StorageAccountName) | Where-Object {$_.KeyName -eq "key1"}

$StorageAccountKeyValue = $StorageAccountKey.Value

# Convert the key to a secure string so that it can be stored in a key vault
$KeyVaultName = "az204appservicekeyvault"
$SecretValue = ConvertTo-SecureString $StorageAccountKeyValue -AsPlainText -Force

# Store the key in a new secret in the key vault
Set-AzKeyVaultSecret -VaultName $KeyVaultName `
-Name $StorageAccountName `
-SecretValue $SecretValue
```

## SQL Server
### Create a SQL Server and a database
``` powershell
# Create a resource group
$rg = New-AzResourceGroup -Name tempsql -Location eastus

# Create credentials for the SQL Server
$cred = Get-Credential -Message "Enter a username and password for the SQL Server."

# Create the SQL Server
$server = New-AzSqlServer -ResourceGroupName $rg.ResourceGroupName `
-ServerName tempsqlaz204patrick `
-Location $rg.Location `
-SqlAdministratorCredentials $cred

# Grant access to the DB
$serverfirewallrule = New-AzSqlServerFirewallRule -ResourceGroupName $rg.ResourceGroupName `
-ServerName $server.ServerName `
-FirewallRuleName "Allowed IPs" `
-StartIpAddress "0.0.0.0" `
-EndIpAddress "0.0.0.0"

# Create a database
$database = New-AzSqlDatabase -ResourceGroupName $rg.ResourceGroupName `
-ServerName $server.ServerName `
-DatabaseName "tempsqldb" `
-RequestedServiceObjectiveName "S0" `
-SampleName "AdventureWorksLT"

# Delete the resource group after you're done with the database
Remove-AzResourceGroup -Name $rg.ResourceGroupName -Force -AsJob
```

## ARM
### Deploying ARM templates
``` powershell
# Deploying a local template
# Create a resource group for the deployment
New-AzResourceGroup -Name ExampleGroup -Location "East US"

# Deploy a template from a local file
New-AzResourceGroupDeployment `
-Name ExampleDeployment `
-ResourceGroupName ExampleGroup `
-TemplateFile <path-to-template>

# Deploy a template from a remote location
New-AzResourceGroupDeployment `
-Name remoteTemplateDeployment `
-ResourceGroupName ExampleGroup `
-TemplateUri https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/quickstarts/microsoft.storage/storage-account-create/azuredeploy.json
```

## API Management
### Create an API Management Service Instance
``` powershell
New-AzApiManagement -ResourceGroupName <resource group name> `
-Name <APIM name> `
-Location <location> `
-Organization <Organization name> `
-AdminEmail <email> `
-Sku <Developer, Basic, Standard, Premium, Consumption>
```

### Create an API
``` powershell
$ApiMgmtContext = New-AzApiManagementContext -ResourceGroupName <resource group name> `
-ServiceName <service name>

New-AzApiManagementApi -Context $ApiMgmtContext `
-Name <API name> `
-ServiceUrl <API URL> `
-Protocols @("http", "https") `
-Path <path to API>
```

## Managed Identities
### Create an User Assigned Identity
``` powershell
New-AzUserAssignedIdentity -ResourceGroupName <resource group name> `
-Name <identity name> `
-Location <location>
```

### Create a System Assigned Managed Identity
``` powershell
# This example is for a VM
$ResourceGroupName = "az204reviewgroup"
$VmName = "tempvmidentityaz204"

$Vm = Get-AzVM -ResourceGroupName $ResourceGroupName `
-Name $VmName

Update-AzVM -ResourceGroupName $ResourceGroupName `
-VM $Vm `
-IdentityType SystemAssigned
```

## CDN
### Create a new CDN Profile
``` powershell
New-AzCdnProfile -Name <CDN name> `
-ResourceGroupName <resource group name> `
-Location <location, like Global> `
-SkuName <Standard_Verizon, Standard_Microsoft, Standard_Akamai, Premium_Verizon, default = Standard_Akamai>
```

## Azure Service Bus
### Create a Service Bus Namespace and a Queue
``` powershell
# Create a resource group to contain the namespace
New-AzResourceGroup -Name <resource group name> -Location <location>

# Create the namespace
New-AzServiceBusNamespace -ResourceGroupName <resource group name> `
-Name <namespace name> `
-Location <location>

# Create the queue
New-AzServiceBusQueue -ResourceGroupName <resource group name> `
-NamespaceName <namespace name> `
-Name <queue name>

# Get the primary connection string for the namespace to connect to the queue to send and receive messages
Get-AzServiceBusKey -ResourceGroupName <resource group name> `
-Namespace <namespace name> `
-Name RootManageSharedAccessKey
```