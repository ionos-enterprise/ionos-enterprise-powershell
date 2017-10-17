Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$email = "pstestuser"+(Get-Date).Ticks+"@example.com"

$newUser = New-PBUser -FirstName PSTest_FirstName -LastName PSTest_LastName -Email $email -Password pstestpass111

$userStatus = get-PBRequestStatus -RequestUrl $newUser.Request

"User status" 
$userStatus.Metadata.Status
$userStatus.Metadata.Message

$user = Get-PBUser -UserId $newUser.Id

$newuserfirstname = $user.Properties.FirstName + "_updated"
$newuserlasttname = $user.Properties.Last + "_updated"
$newuseremail = "update_" +$user.Properties.Email

$updatedUser = Set-PBUser -UserId $newUser.Id -FirstName $newuserfirstname -LastName $newuserlasttname -Email $newuseremail

start-sleep -seconds 10

if($updatedUser.Properties.FirstName -eq $newuserfirstname){
"Update - Check"
}
else
{
"Update failed"
}

$newGroup = New-PBGroup -Name "test_ps_1" -CreateDataCenter 1

AddToGroup-PBUser -GroupId $newGroup.Id -UserId $newUser.Id

RemoveFromGroup-PBUser -GroupId $newGroup.Id -UserId $newUser.Id

Remove-PBUser -UserId $newUser.Id

Remove-PBGroup -GroupId $newGroup.Id

#Remove-Module Profitbricks