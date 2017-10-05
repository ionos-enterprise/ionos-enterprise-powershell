Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newDc = New-PBDatacenter -Name "test_ps_22" -Description "PS Unit Testing" -Location "us/las"
start-sleep -seconds 30

"Get all resources"
$resources = Get-PBResources
"Found " + $resources.Count + " resources."

"Get all resources of Datacenter type"
$resources = Get-PBResources -ResourceType Datacenter
"Found " + $resources.Count + " resources."

"Get resource of Datacenter type"
$resource = Get-PBResources -ResourceType Datacenter -ResourceId $newDc.Id 
"Found resource " + $resource.Id + " of type " + $resource.Type

Remove-PBDatacenter -DatacenterId $newDc.Id 
start-sleep -seconds 120

Remove-Module Profitbricks

"Done"

