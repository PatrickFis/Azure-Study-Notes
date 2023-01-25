# ARM
Azure Resource Manager is the deployment and management system for Azure. It is a management layer allowing for creation, updates, and deletion of Azure resources in your subscription.
- ARM has a declarative syntax that allows for any Azure resource to be deployed
- Results are repeatable using templates
- ARM orchestrates ordering operations for you

## Templates [MS Documentation on Template Functions](https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions)
- Template files can be written to extend JSON and use functions provided by ARM. Templates have the following sections
  - Parameters - Allows templates to be used in different environments
  - Variables - Can be constructed from parameters
  - User-defined functions
  - Resources
  - Outputs from the deployed resources
- Templates are converted by ARM to REST API operations
- Templates can be deployed using the following
  - Azure portal
  - Azure CLI
  - PowerShell
  - REST API
  - Button in GitHub repository
  - Azure Cloud Shell
- Templates can be divided into reusable templates so that other templates can just link them together
- Template specs enable you to store a template as a resource type so that they can be shared. You can then use RBAC to manage access to the template spec. Users with read access can then deploy it but not change it.
- Resources can be deployed conditionally using the condition element with a value that resolves to true or false. Note that child resources must specify the same condition as it does not cascade.
  - Conditional deployments can be used to create new resources or use existing ones

## Deployment Modes
There are two modes used when deploying resources.
- Incremental update
  - Default mode
  - Resources inside a resource group that aren't in the template are left unchanged
  - Redeployments should include all properties of resources and incremental mode will interpret their absence as a change
- Complete update
  - Resources inside a resource group that aren't in the template are deleted

The deployment mode is set using a command like this
``` bash
az deployment group create \
  --mode Complete \
  --name ExampleDeployment \
  --resource-group ExampleResourceGroup \
  --template-file storage.json
```

## Deploying using ARM
https://docs.microsoft.com/en-us/learn/modules/create-deploy-azure-resource-manager-templates/6-create-deploy-resource-manager-template

Code can be found in [Code/Azure ARM](Code/Azure%20ARM/)

## Resources
https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions


# Separate MS Learn Module
[Link](https://learn.microsoft.com/en-us/training/paths/deploy-manage-resource-manager-templates/)

## Deploying using ARM
- I'll be using the ARM template in [Code/Azure ARM/SingleStorageAccount.json](Code/Azure%20ARM/) for this.
- This template was originally created while working through stuff on Udemy (see storageaccount.json), but I've made a copy of it since I don't want to deploy three different storage accounts. I've modified it a bit while working though the MS Learn Module as well.
- The template deploys a storage account to a resource group and does the following:
  -  Uses a function to make the account have a unique name.
  -  It also uses parameters to prompt the user to select the SKU for the account.
  -  It stores the name of the storage account in a variable so that it can be referenced from multiple locations easily. 
  -  It stores the location to deploy the account in a variable that retrieves that location from the resource group the account is being deployed to.
  -  It references the tags associated with the storage account in parameters.
  -  It outputs the endpoints of the account after the deployment finishes.

## Notes about functions in ARM
- Strings are denoted with single quotes
- Literal values need to be escaped
- Since ARM uses JSON nulls can set as usual ("propertyName": null) or using a function ("propertyName": "[json('null')]")
- ARM template functions can be referenced [here](https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions)