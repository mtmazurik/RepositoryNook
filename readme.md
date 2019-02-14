RepositoryNook
==============

Content is copyright 2019 (c) Cloud Computing Associates authors, released under [CC-BY-4.0](https://creativecommons.org/licenses/by/4.0/), which gives you permission to use content for almost any purpose (but does not grant you any trademark permissions), so long as you note the license and give attribution (credit), such as follows:


Content based on https://github.com/mtmazurik/RepositoryNook used under the [CC-BY-4.0](https://creativecommons.org/licenses/by/4.0/) license.

Readme.md - Last updated 2/14/2019 mtm

### Overview
RepositoryNook is a service that exposes a RESTful API. It can run in any container, on any host-platform, that can host Linux Docker images.
REST API, controller and code written in ASPNETCore and C#.  It targest an Atlas MongoDB database for the data store.
We start with Azure container deployment (below).

It defaults to Port 8902.  

It is intended to be part of a set of services, brought together inside of a Microservice, which I call a Level II service. (The Microservice being a Level I service)

Swagger help can be seen, after running (locally) with uri: http://localhost:8902/swagger/index.html

I've deployed to Azure and AWS, and it stores repository info Mongo DB Atlas (hosted).

Unlike MongoDB itself, which will create a Database and Collection if it doesn't exist, I've instituted a check for the Database/Collection which
will throw an exception and return a 400 (Bad Request) if it doesn't exist.   We don't need typo's proliferating databases and collections out there in the wild.
It is easy to connect with MongoDB Compass to Atlas and create the Database\Collection before the services are put to use.

At present we do not support the programmatic creation of DB and collection, but that wouldn't be difficult to add.

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
