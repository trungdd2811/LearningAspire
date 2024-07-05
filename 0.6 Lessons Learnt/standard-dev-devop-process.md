# Lessons learned from applying standard process between DEVs and DEVOPTs to develop, deploy ASPIRE project

![standard-dev-devops-process](./Images/standard-dev-devops-process.PNG)

Basic steps are below:

	1 - Using Github to create a Development Repository
	2 - Using Azure DevOps to create a Deployment Repository
	3 - Using Github Action to create a simple CI to mirror Github repository to Azure DevOp repository
	4 - Using Azure DevOps to deploy codes to Deployment Environment (Azure) via IaC

## 1 - Using Github to create a Development Repository
It is easy, please see the github website to know how to create a repository

## 2 - Using Azure DevOps to create a Deployment Repository

* Create a Azure DevOps account
* Create a deployment project
	![azure-devops-create-project](./Images/azure-devops-create-project.PNG)
* Create a empty deployment repository
	![azure-devops-create-repository](./Images/azure-devops-create-repository.PNG)
* Generate the URL will be used to mirror codes from the development repository
	* format: **https://{user-name}:{password}@dev.azure.com/trungdd2811/AsipreDeployment/_git/AsipreDeployment**

	![azure-devops-repository-generate-url-1](./Images/azure-devops-repository-generate-url-1.PNG)
	![azure-devops-repository-generate-url-2](./Images/azure-devops-repository-generate-url-2.PNG)

## 3 - Using Github Action to create a simple CI to mirror Github repository to Azure DevOp repository

```yaml
name: "Mirror repository to Azure DevOps"

on:
  push:
	branches:
	  - master

jobs:
  mirror:
	name: Mirror repository to Azure DevOps
	runs-on: ubuntu-latest

	steps:
	  - name: Checkout repository
		uses: actions/checkout@v4
		with:
		  fetch-depth: 0
		  
	  - name: Setup dotnet cli
		uses: actions/setup-dotnet@v4
		with:
		  dotnet-version: '8.0.x'
		  
	  - name: Install Aspire workload
		run: dotnet workload restore 
		
	  - name: Build whole .net solution before mirroring
		run: dotnet build
		
	  - name: Mirror development repository to the deployment repository on Azure DevOps
		run: git push --mirror {your deployment repo's URL which is created at step 2}
	  
```
	![github-mirror-to-azure-devops](./Images/github-mirror-to-azure-devops.PNG)

	![github-mirror-to-azure-devops-result](./Images/github-mirror-to-azure-devops-result.PNG)

## 4 - Using Azure DevOps to deploy codes to Deployment Environment (Azure) via IaC

Please see the link below to understand basic steps:
* https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/aca-deployment-github-actions?tabs=windows&pivots=azure-pipelines

Basic steps:
* 4.1 - Initialize the template for .NET Aspire using azd (azure developer cli)
* 4.2 - Create and configure an Azure Pipelines workflow
* 4.3 - Create a connection between Azure and Azure DevOps to create necessary azure resources
* 4.4 - Run Azure Pipelines

### 4.1 - Initialize the template for .NET Aspire using azd (azure developer cli)
* Clone the deployment repository
* Using azd init to create necessary template files
	![azure-devops-azd-init](./Images/azure-devops-azd-init.PNG)

### 4.2 - Create and configure an Azure Pipelines workflow
#### 4.2.1 - Create an Azure Pipelines workflow:

using the code below to create a yml file and put it in .azdo/pipelines folder (create necessary folder if not exists).
example: .azdo/pipelines/

```yaml
trigger:
  - main
  - master

pool:
  vmImage: ubuntu-latest

steps:

  - task: Bash@3
	displayName: Install azd
	inputs:
	  targetType: 'inline'
	  script: |
		curl -fsSL https://aka.ms/install-azd.sh | bash

  # azd delegate auth to az to use service connection with AzureCLI@2
  - pwsh: |
	  azd config set auth.useAzCliAuth "true"
	displayName: Configure `azd` to Use AZ CLI Authentication.

  - task: Bash@3
	displayName: Install .NET Aspire workload
	inputs:
	  targetType: 'inline'
	  script: |
		dotnet workload install aspire

  - task: AzureCLI@2
	displayName: Provision Infrastructure
	inputs:
	  azureSubscription: azconnection
	  scriptType: bash
	  scriptLocation: inlineScript
	  inlineScript: |
		azd provision --no-prompt
	env:
	  AZURE_SUBSCRIPTION_ID: $(AZURE_SUBSCRIPTION_ID)
	  AZURE_ENV_NAME: $(AZURE_ENV_NAME)
	  AZURE_LOCATION: $(AZURE_LOCATION)

  - task: AzureCLI@2
	displayName: Deploy Application
	inputs:
	  azureSubscription: azconnection
	  scriptType: bash
	  scriptLocation: inlineScript
	  inlineScript: |
		azd deploy --no-prompt
	env:
	  AZURE_SUBSCRIPTION_ID: $(AZURE_SUBSCRIPTION_ID)
	  AZURE_ENV_NAME: $(AZURE_ENV_NAME)
	  AZURE_LOCATION: $(AZURE_LOCATION)
```

#### 4.2.2 - Configure the workflow file (Azure pipelines)
This step is very important. You should have an active azure subscription first. It will do:

* You will need a Azure DevOps Personal Access Token (PAT) with necessary permissions. AZD will need this token
	to create necessary things such as service connection when configuring the workflow locally. You don't need this token if you already know how to create these things manually by yourself

	![azure-devops-pat](./Images/azure-devops-pat.PNG)
	![azure-devops-pat-create](./Images/azure-devops-pat-create.PNG)

* using active azure subscription to create a service connection automatically (based on the yml file) between Azure and Azure DevOps to create necessary azure resources.
* config and deploy the applications: just run the command below locally
	```
	PS D:\Projects\AsipreDeployment> azd pipeline config --provider azdo
	```

	![azure-devops-azd-pipeline-config-locally](./Images/azure-devops-azd-pipeline-config-locally.PNG)

Some necessary things are created automatically:

* An App Registration is created automatically on Azure to allow Azure DevOps manages necessary resouces

	![azure-app-registration-created](./Images/azure-app-registration-created.PNG)

* A Service Connection is created automatically on Azure DevOps to allow connection between Azure and Azure DevOps
	![azure-devops-service-connection-created](./Images/azure-devops-service-connection-created.PNG)

* A pipelines workflow is created and run
	![aazure-devops-pipelines-created](./Images/azure-devops-pipelines-created.PNG)


# Troubleshooting

The created pipelines run failed because the password is not filled
![azure-depops-pipelines-provision-failed-password](./Images/azure-depops-pipelines-provision-failed-password.PNG)

The solution is generating IaC files (bicep files) and correct them

* generating bicep files by running commands below

```
D:\Projects\AsipreDeployment> azd config set alpha.infraSynth on
D:\Projects\AsipreDeployment> azd infra synth
```

* change the code to do the hot-fix, will find another way to update bicep file and use the keyvault
	![azure-depops-pipelines-provision-failed-password-hotfix](./Images/azure-depops-pipelines-provision-failed-password-hotfix.PNG)