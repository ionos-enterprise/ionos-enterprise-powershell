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

$newvolume = New-PBVolume -DataCenterId $datacenter.Id -Size 5 -Type HDD -ImageId 646023fb-f7bd-11e5-b7e8-52540005ab80 -Name "test_volume"

Do{
"Waiting on volume to provision"

$volumestatus = Get-PBRequestStatus -RequestUrl $newvolume.Request

 start-sleep -seconds 5
}While($volumestatus.Metadata.Status -ne "DONE")

New-PBSnapshot -DatacenterId $datacenter.Id -VolumeId $newvolume.Id -Name "test snapshot"

Start-Sleep -Seconds 60

$newname = $newvolume.Properties.Name + "updated"

$snapshots = Get-PBSnapshot 
 
$updatedsnapshot = Set-PBSnapshot -SnapshotId $snapshots.Item(0).Id -Name $newname

start-sleep -seconds 10

$snapshot = Get-PBSnapshot -SnapshotId $snapshots.Item(0).Id

if($snapshot.Properties.Name -eq $newname){
"Update - Check"
}
else
{
"Update failed"
}

Restore-PBSnapshot -DatacenterId $datacenter.Id -SnapshotId $snapshots.Item(0).Id

Remove-PBSnapshot -SnapshotId $snapshots.Item(0).Id

Remove-PBDatacenter -DatacenterId $datacenter.Id

#Remove-Module Profitbricks