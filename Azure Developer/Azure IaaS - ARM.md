# ARM
Azure Resource Manager is the deployment and management system for Azure. It is a management layer allowing for creation, updates, and deletion of Azure resources in your subscription.
- ARM has a declarative syntax that allows for any Azure resource to be deployed
- Results are repeatable using templates
- ARM orchestrates ordering operations for you

## Templates [MS Documentation on Template Functions](https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions)
- Template files can be written to extend JSON and use functions provided by ARM. Templates have the following sections
  - $schema - Describes the version of the template language. Required.
  - contentVersion - Version of the template. Specified by you to track changes in the template. Required.
  - apiProfile - An API version that serves as a collection of API versions for resource types. Used to avoid having to specify API versions for each resource in the template. Not required.
  - parameters - Allows templates to be used in different environments by specifying values that will be provided during deployment. Not required.
  - variables - Values that can be reused in your templates. Can be constructed from parameters. Not required.
  - User-defined functions (functions is how it appears in the template)- Customized functions used to simplify your template. Not required.
  - resources - The resources you want to deploy. Required.
  - outputs - Values that need to be returned from the deployed resources. Not required.
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

### Template structure
- An ARM template is formatted as JSON with this structure:
  ``` json
  {
    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
    "contentVersion": "",
    "apiProfile": "",
    "parameters": {  },
    "variables": {  },
    "functions": [  ],
    "resources": [  ],
    "outputs": {  }
  }
  ```
- Parameters are defined using the following format (only parameter-name and type are required):
  ``` json
  "parameters": {
    "<parameter-name>" : {
      "type" : "<type-of-parameter-value>",
      "defaultValue": "<default-value-of-parameter>",
      "allowedValues": [ "<array-of-allowed-values>" ],
      "minValue": <minimum-value-for-int>,
      "maxValue": <maximum-value-for-int>,
      "minLength": <minimum-length-for-string-or-array>,
      "maxLength": <maximum-length-for-string-or-array-parameters>,
      "metadata": {
        "description": "<description-of-the-parameter>"lhnjksdfvd
      }
    }
  }
  ```
- Variables are defined using the following format:
  ``` json
  "variables": {
    "stringVar": "example value"
  }
  ```
  - They can also be defined from parameters:
    ``` json
    "parameters": {
      "inputValue": {
        "defaultValue": "deployment parameter",
        "type": "string"
      }
    },
    "variables": {
      "stringVar": "myVariable",
      "concatToVar": "[concat(variables('stringVar'), '-addtovar') ]",
      "concatToParam": "[concat(parameters('inputValue'), '-addtoparam')]"
    }
    ```
  - They can be defined from template functions:
    ``` json
    "variables": {
      "storageName": "[concat(toLower(parameters('storageNamePrefix')), uniqueString(resourceGroup().id))]"
    },
    ```
  - They're used like this:
    ``` json
    "variables": {
      "storageName": "[concat(toLower(parameters('storageNamePrefix')), uniqueString(resourceGroup().id))]"
    },
    "resources": [
      {
        "type": "Microsoft.Storage/storageAccounts",
        "name": "[variables('storageName')]",
        ...
      }
    ]
    ```
- Functions are defined using the following format:
  - Functions can't access variables.
  - Functions can only use parameters that are defined in the functions.
  - Functions can't call other user-defined functions.
  - Functions can't use the reference function.
  - Parameters for functions can't have default values.
  - The namespace, function-name, output-type, and output-value elements are required.
  - They're called in your ARM template like this: `"some property here": "[<namespace>.<function name>(parameters)]"`
  ``` json
  "functions": [
    {
      "namespace": "<namespace-for-functions>",
      "members": {
        "<function-name>": {
          "parameters": [
            {
              "name": "<parameter-name>",
              "type": "<type-of-parameter-value>"
            }
          ],
          "output": {
            "type": "<type-of-output-value>",
            "value": "<function-return-value>"
          }
        }
      }
    }
  ],
  ```
- Resources are defined using the following format:
  - The only required elements are type, apiVersion, name, and the location (when required by a resource).
  ``` json
  "resources": [
    {
        "condition": "<true-to-deploy-this-resource>",
        "type": "<resource-provider-namespace/resource-type-name>",
        "apiVersion": "<api-version-of-resource>",
        "name": "<name-of-the-resource>",
        "comments": "<your-reference-notes>",
        "location": "<location-of-resource>",
        "dependsOn": [
            "<array-of-related-resource-names>"
        ],
        "tags": {
            "<tag-name1>": "<tag-value1>",
            "<tag-name2>": "<tag-value2>"
        },
        "identity": {
          "type": "<system-assigned-or-user-assigned-identity>",
          "userAssignedIdentities": {
            "<resource-id-of-identity>": {}
          }
        },
        "sku": {
            "name": "<sku-name>",
            "tier": "<sku-tier>",
            "size": "<sku-size>",
            "family": "<sku-family>",
            "capacity": <sku-capacity>
        },
        "kind": "<type-of-resource>",
        "scope": "<target-scope-for-extension-resources>",
        "copy": {
            "name": "<name-of-copy-loop>",
            "count": <number-of-iterations>,
            "mode": "<serial-or-parallel>",
            "batchSize": <number-to-deploy-serially>
        },
        "plan": {
            "name": "<plan-name>",
            "promotionCode": "<plan-promotion-code>",
            "publisher": "<plan-publisher>",
            "product": "<plan-product>",
            "version": "<plan-version>"
        },
        "properties": {
            "<settings-for-the-resource>",
            "copy": [
                {
                    "name": ,
                    "count": ,
                    "input": {}
                }
            ]
        },
        "resources": [
            "<array-of-child-resources>"
        ]
    }
  ]
  ```
- Outputs are defined using the following format:
  - The output-name element is the only required element.
  ``` json
  "outputs": {
    "<output-name>": {
      "condition": "<boolean-value-whether-to-output-value>",
      "type": "<type-of-output-value>",
      "value": "<output-value-expression>",
      "copy": {
        "count": <number-of-iterations>,
        "input": <values-for-the-variable>
      }
    }
  }
  ```

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

# Misc
- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-develop-azure-compute-solutions-1-of-5 for information on what may appear on the exam.
  - Be familiar with ARM template files and what can appear inside of them
  - Be aware of how to deploy an ARM template file using the az cli and Powershell