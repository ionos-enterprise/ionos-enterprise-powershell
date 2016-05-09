Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newDc = New-PBDatacenter -Name "test_ps_1" -Description "PS Unit Testing" -Location "us/las"

$datacenter = Get-PBDatacenter $newDc.Id

$old_name = $datacenter.Properties.Name

$new_name =  "test_ps"

$newLan = New-PBLan -DataCenterId $datacenter.Id -Name "test_lan" -Public $true 


Do{
"Waiting on LAN to provision"

$status = Get-PBRequestStatus -RequestUrl $newLan.Request

 start-sleep -seconds 5
}While($status.Metadata.Status -ne "DONE")

$new_name = "test_lan_updated" 

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
