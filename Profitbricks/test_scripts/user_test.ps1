Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$email = "noreply"+(Get-Date).Ticks+"@example.com"

$newUser = New-PBUser -FirstName "John" -LastName "Doe" -Email $email -Password "secretpassword123"
$userStatus = get-PBRequestStatus -RequestUrl $newUser.Request

"User status" 
$userStatus.Metadata.Status
$userStatus.Metadata.Message

$user = Get-PBUser -UserId $newUser.Id

$updatedUser = Set-PBUser -UserId $newUser.Id -FirstName "John" -LastName "Doe" -Email $email -Administrator $false

start-sleep -seconds 10

if(!$updatedUser.Properties.Administrator){
"Update - Check"
}
else
{
"Update failed"
}

$newGroup = New-PBGroup -Name "PowerShell SDK Test" -CreateDataCenter 1

Add-PBGroupMember -GroupId $newGroup.Id -UserId $newUser.Id

Remove-PBGroupMember -GroupId $newGroup.Id -UserId $newUser.Id

Remove-PBUser -UserId $newUser.Id

Remove-PBGroup -GroupId $newGroup.Id

#Remove-Module Profitbricks