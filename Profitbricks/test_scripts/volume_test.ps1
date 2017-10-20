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

$newServer = New-PBServer -DataCenterId $datacenter.Id -Name "server_test" -ImageAlias "ubuntu:latest" -Password "Vol44lias" -Cores 1 -Ram 1073741824 -PublicIp 0 -StaticIp 1
start-sleep -seconds 30

$images = Get-PBImage
$images = $images | Where-Object {$_.Properties.ImageType -eq 'HDD' -and $_.Properties.LicenceType -eq 'LINUX' -and $_.Properties.Location -eq 'us/las' -and $_.Properties.Public -eq 1}
$image = $images[0]
start-sleep -seconds 10

$newvolume = New-PBVolume -DataCenterId $datacenter.Id -Size 20 -Type HDD -ImageId $image.Id -Name "test_volume" -ImagePassword "Vol44lias" -AvailabilityZone "AUTO"
start-sleep -seconds 20
"New volume add with id "+$newvolume.Id

$newname = $newvolume.Properties.Name + "updated"

$updatedvolume = Set-PBVolume -DataCenterId $datacenter.Id -VolumeId $newvolume.Id -Name $newname

$volume = Get-PBVolume -DataCenterId $datacenter.Id -VolumeId $newvolume.Id

if($updatedvolume.Properties.Name -eq $newname){
"Update - Check"
}
else
{
"Update failed"
}

#connect the volume
$connectedvolume = Connect-PBVolume -DataCenterId $datacenter.Id -ServerId $newServer.Id -VolumeId $volume.Id
start-sleep -seconds 30
Do{
"Waiting on volume to conect"

$volumestatus = Get-PBRequestStatus -RequestUrl $connectedvolume.Request

 start-sleep -seconds 5
}While($volumestatus.Metadata.Status -ne "DONE")

$volumes = Get-PBAttachedVolume -DataCenterId $datacenter.Id -ServerId $newServer.Id

if( $volumes.Item(0).Id -eq $connectedvolume.Id)
{
"Successfully connected to the server" 
}
else
{
"It failed"
}

Disconnect-PBVolume -DataCenterId $datacenter.Id -ServerId $newServer.Id -VolumeId $connectedvolume.Id

Remove-PBDatacenter -DatacenterId $datacenter.Id

#Remove-Module Profitbricks