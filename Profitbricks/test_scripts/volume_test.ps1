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

$newvolume = New-PBVolume -DataCenterId $datacenter.Id -Size 5 -Type HDD -ImageId 646023fb-f7bd-11e5-b7e8-52540005ab80 -Name "test_volume"


Do{
"Waiting on volume to provision"

$volumestatus = Get-PBRequestStatus -RequestUrl $newvolume.Request

 start-sleep -seconds 5
}While($volumestatus.Metadata.Status -ne "DONE")

$newname = $newvolume.Properties.Name + "updated"

$updatedvolume = Set-PBVolume -DataCenterId $datacenter.Id -VolumeId $newvolume.Id -Name $newname

start-sleep -seconds 10

$volume = Get-PBVolume -DataCenterId $datacenter.Id -VolumeId $newvolume.Id

if($volume.Properties.Name -eq $newname){
"Update - Check"
}
else
{
"Update failed"
}

#attach the volume
$attachedvolume = Attach-PBVolume -DataCenterId $datacenter.Id -ServerId $newServer.Id -VolumeId $volume.Id

Do{
"Waiting on volume to attach"

$volumestatus = Get-PBRequestStatus -RequestUrl $attachedvolume.Request

 start-sleep -seconds 5
}While($volumestatus.Metadata.Status -ne "DONE")

$volumes = Get-PBAttachedVolume -DataCenterId $datacenter.Id -ServerId $newServer.Id

if( $volumes.Item(0).Id -eq $attachedvolume.Id)
{
"Successfully attached to the server" 
}
else
{
"It failed"
}

Detach-PBVolume -DataCenterId $datacenter.Id -ServerId $newServer.Id -VolumeId $attachedvolume.Id

Remove-PBDatacenter -DatacenterId $datacenter.Id

#Remove-Module Profitbricks