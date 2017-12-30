Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newGroup = New-PBGroup -Name "PowerShell SDK Test" -CreateDataCenter 1

$groupStatus = get-PBRequestStatus -RequestUrl $newGroup.Request

"Group status"
$groupStatus.Metadata.Status
$groupStatus.Metadata.Message

$datacenter = New-PBDatacenter -Name "PowerShell SDK Test" -Description "PowerShell SDK Test datacenter" -Location "us/las"
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
