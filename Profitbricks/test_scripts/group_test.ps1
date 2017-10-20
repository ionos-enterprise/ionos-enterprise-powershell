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

$group = Get-PBGroup $newGroup.Id

$newname = $newGroup.Properties.Name + "updated"
 
$updatedgroup = Set-PBGroup -GroupId $group.Id -Name $newname

start-sleep -seconds 10

if($updatedgroup.Properties.Name -eq $newname){
"Update - Check"
}
else
{
"Update failed"
}

Remove-PBGroup -GroupId $group.Id

#Remove-Module Profitbricks