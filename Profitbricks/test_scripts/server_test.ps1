Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newDc = New-PBDatacenter -Name "test_ps_1" -Description "PS Unit Testing" -Location "us/las"

$dcstatus = get-PBRequestStatus -RequestUrl $newDc.Request

"DC status" 
$dcstatus.Metadata.Status
$dcstatus.Metadata.MEssage

$datacenter = Get-PBDatacenter $newDc.Id

$newServer = New-PBServer -DataCenterId $datacenter.Id -Name "server_test" -Cores 1 -Ram 1024 

Do{
"Waiting on server to provision"

$serverstatus = Get-PBRequestStatus -RequestUrl $newServer.Request

 start-sleep -seconds 5
}While($serverstatus.Metadata.Status -ne "DONE")

$server = Get-PBServer -DataCenterId $newDc.Id -ServerId $newServer.Id

$new_name = $server.Properties.Name + "updated"

$updatedServer = Set-PBServer -DataCenterId $datacenter.Id -ServerId $server.Id -Name $new_name

if($updatedServer.Properties.Name -eq $new_name)
{
    "Success"
    $updatedServer.Properties.Name
}else{
    "Failed"
    "The name is " + $updatedServer.Properties.Name
}

Reboot-PBServer -DataCenterId $datacenter.Id -ServerId $server.Id
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