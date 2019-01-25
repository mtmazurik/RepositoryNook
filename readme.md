RepositoryNook
==============

Content is copyright 2019 (c) Cloud Computing Associates authors, released under [CC-BY-4.0](https://creativecommons.org/licenses/by/4.0/), which gives you permission to use content for almost any purpose (but does not grant you any trademark permissions), so long as you note the license and give attribution (credit), such as follows:


Content based on https://github.com/mtmazurik/RepositoryNook used under the [CC-BY-4.0](https://creativecommons.org/licenses/by/4.0/) license.

### Overview
RepositoryNook is a RESTful API data store service. It can run in any container, on any host-platform, that can host Linux Docker images.
REST API, controller and code written in ASPNETCore and C#.  It targest an Atlas MongoDB database for the data store.
We start with Azure container deployment (below).

#### Azure 
Step 1:

Creat an Azure Container Registry (ACR) and set the service principle (and jot down the service principle ids, for use later)
See: https://github.com/mtmazurik/devops-handy/  for some Azure Container Registry (ACR) creation scripts.

Step 2:

Build-n-push using Azure DevOps pipeline, which uses azure-pipelines.yml, to do CI build on GitHub checkin and push to an Azure Container Registry (called NooksAndCrannies in sample code).

Step 3:

Deploy:  Running image in an Azure Container Instance (ACI) from Azure CMD Shell

Note: in Azure the Azure CMD Shell has storage, pull source down from GitHub to this storage.
(so you can run the script examples .sh bash code)

a) Create Container Group from Azure Cloud Shell, NOTE: res mgr file in source code under /AZURE directory

Az$> az group deployment create --resource-group repositorynookresgroup --template-file azure-resmgr-template.json

b) Deploy the container

Az$> ./azure-deploy.sh     

Step 4:  Cleanup your test   (so as not to incur costs on Azure, if testing things out)

Your Container Registry can be cleaned out with https://github.com/mtmazurik/devops-handy/blob/master/ACR-delete-all-images.sh

Note: Currently in testing the tag on the image in the repository is always :Latest, but in real-world, you will have
versions, and may NOT want to use this script, lest you delete all previous image instances !

 




"# RepositoryNook" 
