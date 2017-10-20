Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newDc = New-PBDatacenter -Name "test_ps_22" -Description "PS Unit Testing" -Location "us/las"

$ip_block = New-PBIPBlock -Location us/las -Size 1

$images = Get-PBImage
$images = $images | Where-Object {$_.Properties.ImageType -eq 'HDD' -and $_.Properties.LicenceType -eq 'LINUX' -and $_.Properties.Location -eq 'us/las' -and $_.Properties.Public -eq 1}
$image = $images[0]

$newServer = New-PBServer -DataCenterId $newDc.Id -Name "server_test_ps" -DiskType $image.Properties.ImageType -ImageId $image.Id -SshKey keysshtest -Cores 1 -Ram 1073741824 -PublicIp 0 -StaticIp 1

$newLan = New-PBLan -DataCenterId $newDc.Id -Name "test_lan" -Public $true

$newNic = New-PBNic -DataCenterId $newDc.Id -ServerId $newServer.Id -LanId $newLan.Id -Ips $ip_block.Properties.Ips

Do{
"Waiting on nic to provision"

$nicstatus = Get-PBRequestStatus -RequestUrl $newNic.Request

 start-sleep -seconds 5
}While($nicstatus.Metadata.Status -ne "DONE")

$ipFailovers = @([pscustomobject]@{Ip=$ip_block.Properties.Ips[0];NicUuid=$newNic.Id})
$updatedLan = Set-PBLan -DataCenterId $newDc.Id -LanId $newLan.Id -Name "test_lan" -Public $true -IpFailover $ipFailovers

start-sleep -seconds 20

$newServer2 = New-PBServer -DataCenterId $newDc.Id -Name "server_test_ps_2" -DiskType $image.Properties.ImageType -ImageId $image.Id -SshKey keysshtest -Cores 1 -Ram 1073741824 -PublicIp 0 -StaticIp 1

$newNic2 = New-PBNic -DataCenterId $newDc.Id -ServerId $newServer2.Id -LanId $newLan.Id -Ips $ip_block.Properties.Ips

Remove-PBDatacenter -DatacenterId $newDc.Id
start-sleep -seconds 120
Remove-PBIPBlock -IpBlockId $ip_block.Id

Remove-Module Profitbricks
