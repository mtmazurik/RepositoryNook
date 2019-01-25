# AzureDeploy
# for RepositoryNook, created: mtm 1/25/2018
#
# resource group
New-AzureRmResourceGroup -Name RepositoryNookResGrp -Location WestUS
#
New-AzureRmContainerGroup -ResourceGroupName RepositoryNookResGrp -TemplateFile azure-resmgr-template.json
#
Get-AzureRmContainerGroup -ResourceGroupName RepositoryNookResGrp -Name repositorynookcontainergroup