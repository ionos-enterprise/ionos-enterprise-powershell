﻿Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newDc = New-PBDatacenter -Name "PowerShell SDK Test" -Description "PowerShell SDK Test datacenter" -Location "us/las"

$datacenter = Get-PBDatacenter $newDc.Id

$old_name = $datacenters.Properties.Name

$new_name =  "PowerShell SDK Test RENAME"

$s = Set-PBDatacenter -DatacenterId $datacenter.Id -Name  $new_name

$updateddc = Get-PBDatacenter -DataCenterId $datacenter.Id


if($updateddc.Properties.Name -eq $new_name)
{
    "Success"
    $updateddc.Properties.Name
}else{
    "Failed"
    "The name is " + $updateddc.Properties.Name
}

Remove-PBDatacenter -DatacenterId $datacenter.Id

Remove-Module Profitbricks
