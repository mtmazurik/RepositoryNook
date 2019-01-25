#!/bin/bash
# azure-deploy  mtm 1-25-2018
az group create --name repositorynookresgroup --location westus
#
az group deployment create --resource-group repositorynookresgroup --template-file azure-resmgr-template.json
#
az container show --resource-group repositorynookresgroup --name repositorynookcontainergroup