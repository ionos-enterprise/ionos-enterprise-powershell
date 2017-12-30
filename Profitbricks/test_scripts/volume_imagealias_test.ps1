Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newDc = New-PBDatacenter -Name "PowerShell SDK Test" -Description "PowerShell SDK Test datacenter" -Location "us/las"

$newServer = New-PBServer -DataCenterId $newDc.Id -Name "PowerShell SDK Test" -DiskType "HDD" -ImageAlias "ubuntu:latest" -Password "Vol44lias" -Cores 1 -Ram 1073741824 -PublicIp 0 -StaticIp 1

start-sleep -seconds 30

Remove-PBDatacenter -DatacenterId $newDc.Id 
start-sleep -seconds 120

Remove-Module Profitbricks
