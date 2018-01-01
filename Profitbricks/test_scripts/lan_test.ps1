Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newDc = New-PBDatacenter -Name "PowerShell SDK Test" -Description "PowerShell SDK Test datacenter" -Location "us/las"

$datacenter = Get-PBDatacenter $newDc.Id

$old_name = $datacenter.Properties.Name

$new_name =  "PowerShell SDK Test"

$newLan = New-PBLan -DataCenterId $datacenter.Id -Name "PowerShell SDK Test LAN" -Public $true


Do{
"Waiting on LAN to provision"

$status = Get-PBRequestStatus -RequestUrl $newLan.Request

 start-sleep -seconds 10
}While($status.Metadata.Status -ne "DONE")

$new_name = "PowerShell SDK Test RENAME" 

$l = Set-PBLan -DataCenterId $datacenter.Id -LanId $newLan.Id -Name $new_name

start-sleep -seconds 5

$lan = Get-PBLan -DataCenterId $datacenter.Id -LanId $newLan.Id

if($new_name -eq $lan.Properties.Name){
"Lan update check"
}
else{
"Failed"
}

Remove-PBLan -DataCenterId $datacenter.Id -LanId $lan.Id

Remove-PBDatacenter -DatacenterId $datacenter.Id 

Remove-Module Profitbricks
