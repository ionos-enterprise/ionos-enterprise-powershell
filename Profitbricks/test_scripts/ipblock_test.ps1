Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

$ip = New-PBIPBlock -Location us/las -Size 1 

start-sleep -seconds 10

$ipblock = Get-PBIPBlock -IpBlockId $ip.Id

$ipblock

Remove-PBIPBlock -IpBlockId $ipblock.Id
