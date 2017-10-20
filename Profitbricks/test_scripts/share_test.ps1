Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newGroup = New-PBGroup -Name "test_ps_1" -CreateDataCenter 1

$groupStatus = get-PBRequestStatus -RequestUrl $newGroup.Request

"Group status"
$groupStatus.Metadata.Status
$groupStatus.Metadata.Message

$datacenter = New-PBDatacenter -Name "test_ps_1" -Description "PS Unit Testing" -Location "us/las"
"Datacenter created"

$share = New-PBShare -GroupId $newGroup.Id -ResourceId $datacenter.id -EditPrivilege 1
"Share created"

$updatedShare = Set-PBShare -GroupId $newGroup.Id -ShareId $datacenter.id -EditPrivilege 0
"Share updated"

start-sleep -seconds 10

Remove-PBShare -GroupId $newGroup.Id -ShareId $share.Id
Remove-PBDatacenter -DatacenterId $datacenter.Id
Remove-PbGroup -GroupId $newGroup.Id

#Remove-Module Profitbricks
