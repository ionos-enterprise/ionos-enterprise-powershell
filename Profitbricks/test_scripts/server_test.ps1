Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newDc = New-PBDatacenter -Name "PowerShell SDK Test" -Description "PowerShell SDK Test datacenter" -Location "us/las"

$dcstatus = get-PBRequestStatus -RequestUrl $newDc.Request

"DC status" 
$dcstatus.Metadata.Status
$dcstatus.Metadata.MEssage

$datacenter = Get-PBDatacenter $newDc.Id

$newServer = New-PBServer -DataCenterId $datacenter.Id -Name "PowerShell SDK Test" -ImageAlias "ubuntu:latest" -Password "Vol44lias" -Cores 1 -Ram 1073741824 -PublicIp 0 -StaticIp 1
start-sleep -seconds 30

$server = Get-PBServer -DataCenterId $newDc.Id -ServerId $newServer.Id

$new_name = $server.Properties.Name + " RENAME"

$updatedServer = Set-PBServer -DataCenterId $datacenter.Id -ServerId $server.Id -Name $new_name

if($updatedServer.Properties.Name -eq $new_name)
{
    "Success"
    $updatedServer.Properties.Name
}else{
    "Failed"
    "The name is " + $updatedServer.Properties.Name
}

Reset-PBServer -DataCenterId $datacenter.Id -ServerId $server.Id
 start-sleep -seconds 40

$server = Get-PBServer -DataCenterId $newDc.Id -ServerId $newServer.Id
$server.Metadata.VmState

$server = Get-PBServer -DataCenterId $newDc.Id -ServerId $newServer.Id

$server.properties.vmstate 

 start-sleep -seconds 10

Stop-PBServer -DataCenterId $datacenter.Id -ServerId $server.Id
 start-sleep -seconds 10

$server = Get-PBServer -DataCenterId $newDc.Id -ServerId $newServer.Id

$server.properties.vmstate 

Start-PBServer -DataCenterId $datacenter.Id -ServerId $server.Id
 start-sleep -seconds 20

$server = Get-PBServer -DataCenterId $newDc.Id -ServerId $newServer.Id

$server.properties.vmstate 

$server.Id
$datacenter.Id

Remove-PBDatacenter -DatacenterId $datacenter.Id

Remove-Module Profitbricks