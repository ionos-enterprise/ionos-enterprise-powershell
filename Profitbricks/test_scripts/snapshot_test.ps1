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

$images = Get-PBImage
$images = $images | Where-Object {$_.Properties.ImageType -eq 'HDD' -and $_.Properties.LicenceType -eq 'LINUX' -and $_.Properties.Location -eq 'us/las' -and $_.Properties.Public -eq 1}
$image = $images[0]
start-sleep -seconds 10


$newvolume = New-PBVolume -DataCenterId $datacenter.Id -Size 20 -Type HDD -ImageId $image.Id -Name "PowerShell SDK Test" -ImagePassword "Vol44lias" -AvailabilityZone "AUTO"
Start-Sleep -Seconds 20
Do{
"Waiting on volume to provision"

$volumestatus = Get-PBRequestStatus -RequestUrl $newvolume.Request

 start-sleep -seconds 10
}While($volumestatus.Metadata.Status -ne "DONE")

New-PBSnapshot -DatacenterId $datacenter.Id -VolumeId $newvolume.Id -Name "PowerShell SDK Test"

Start-Sleep -Seconds 60

$newname = $newvolume.Properties.Name + " RENAME"

$snapshots = Get-PBSnapshot 
 
$updatedsnapshot = Set-PBSnapshot -SnapshotId $snapshots.Item(0).Id -Name $newname

start-sleep -seconds 10

$snapshot = Get-PBSnapshot -SnapshotId $snapshots.Item(0).Id

if($updatedsnapshot.Properties.Name -eq $newname){
"Update - Check"
}
else
{
"Update failed"
}

Restore-PBSnapshot -DatacenterId $datacenter.Id -SnapshotId $snapshots.Item(0).Id -VolumeId $newvolume.Id

start-sleep -seconds 20

Remove-PBSnapshot -SnapshotId $snapshots.Item(0).Id

start-sleep -seconds 20

Remove-PBDatacenter -DatacenterId $datacenter.Id

Remove-Module Profitbricks