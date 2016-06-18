Import-Module ..\bin\Debug\Profitbricks.dll
$SecurePassword = [Environment]::GetEnvironmentVariable("PB_PASSWORD","User") | ConvertTo-SecureString -AsPlainText -Force
$username = [Environment]::GetEnvironmentVariable("PB_USERNAME","User")

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks $credentials

$newDc = New-PBDatacenter -Name "load balancer test" -Description "PS Unit Testing" -Location "us/las"

$dcstatus = get-PBRequestStatus -RequestUrl $newDc.Request

"DC status" 
$dcstatus.Metadata.Status
$dcstatus.Metadata.MEssage

$datacenter = Get-PBDatacenter $newDc.Id

    $loadbalancer = New-PBLoadbalancer -DataCenterId $datacenter.Id -Name "test LB"


Do{
"Waiting on load balancer to provision"
 start-sleep -seconds 5

$status = Get-PBRequestStatus -RequestUrl $loadbalancer.Request

}While($status.Metadata.Status -ne "DONE")


$newname = $loadbalancer.Properties.Name + "updated"

$updated = Set-PBLoadbalancer -DataCenterId $datacenter.Id -LoadbalancerId $loadbalancer.Id -Name $newname

if($updated.Properties.Name -eq $newname){
"Update load balancer - check"
}
else{
"Update load balancer - fail"
}


$newServer = New-PBServer -DataCenterId $datacenter.Id -Name "server_test" -Cores 1 -Ram 1024 

Do{
"Waiting on server to provision"

$serverstatus = Get-PBRequestStatus -RequestUrl $newServer.Request

 start-sleep -seconds 5
}While($serverstatus.Metadata.Status -ne "DONE")

$server = Get-PBServer -DataCenterId $newDc.Id -ServerId $newServer.Id


$nic = New-PBNic -DataCenterId $datacenter.Id -ServerId $server.Id -LanId 1 -Name "Test"
 start-sleep -seconds 30

 $newname = $nic.Properties.Name + "updated"

 $updatedNic = Set-PBNic -DataCenterId $datacenter.Id -ServerId $server.Id -NicId $nic.Id -Name $newname
 start-sleep -seconds 10

 if($updatedNic.Properties.Name -eq $newname){
"Update nic - check"
}
else{
"Update nic - fail"
}

$fwRule = New-PBFirewallRule -DataCenterId $datacenter.Id -ServerId $server.Id -NicId $nic.Id -Protocol TCP -SourceIp 192.168.1.1

Do{
"Waiting on fire wall rule to provision"

$status = Get-PBRequestStatus -RequestUrl $fwRule.Request

 start-sleep -seconds 5
}While($status.Metadata.Status -ne "DONE")

$fwRule = Set-PBFirewallRule -DataCenterId $datacenter.Id -ServerId $server.Id -NicId $nic.Id -FirewallRuleId $fwRule.Id -SourceIp 192.168.1.2
 
start-sleep -seconds 20

$fwRule = Get-PBFirewallRule -DataCenterId $datacenter.Id -ServerId $server.Id -NicId $nic.Id -FirewallRuleId $fwRule.Id
$fwRule.Properties.SourceIp 
if($fwRule.Properties.SourceIp -eq "192.168.1.2"){
"Firewall Rule updated check"
}
else{
"FW fail"
}

$balancedNic = Set-PBNicToLoadbalancer -DataCenterId $datacenter.Id -LoadbalancerId $loadbalancer.Id -NicId $nic.Id

 start-sleep -seconds 30

$balancedNics = Get-PBBalancedNics -DataCenterId $datacenter.Id -LoadbalancerId $loadbalancer.Id 

if($balancedNics -ne $null -and $balancedNics.Id -eq $balancedNic.Id){
"successfully ed a nic"
}
else {
"nic attach failed"
}

$reqUrl = Remove-PBNicFromLoadbalancer -DataCenterId $datacenter.Id -LoadbalancerId $loadbalancer.Id -NicId $balancedNic.Id

Do{
"Waiting on nic to detach"
 start-sleep -seconds 5

$status = Get-PBRequestStatus -RequestUrl $reqUrl

}While($status.Metattachadata.Status -ne "DONE")

"nic detached"

Remove-PBFirewallRule -DataCenterId $datacenter.Id -ServerId $server.Id -NicId $nic.Id -FirewallRuleId $fwRule.Id

Remove-PBNic -DataCenterId $datacenter.Id -ServerId $server.Id -NicId $nic.Id
"NIC removed"

Remove-PBDatacenter -DatacenterId $datacenter.Id

#Remove-Module Profitbricks