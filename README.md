
# Table of Contents

* [Concepts](#concepts)
* [Getting Started](#getting-started)
* [Installation](#installation)
* [Overview](#overview)
* [Usage](#usage)
    * [Create Data Center](#create-data-center)
    * [Create Server](#create-server)
    * [Update Server](#update-server)
    * [List Servers](#list-servers)
    * [Create Volume](#create-volume)
    * [Attach Volume](#attach-volume)
    * [List Volumes](#list-volumes)
    * [Create Snapshot](#create-snapshot)
    * [List Snapshots](#list-snapshots)
    * [Update Snapshot](#update-snapshot)
    * [Delete Snapshot](#delete-snapshot)
    * [List All Commandlets](#list-all-commandlets)
    * [Get Help for a Commandlet](#get-help-for-a-commandlet)

## Concepts

Profitbricks Poweshell module wraps the [ProfitBricks REST API](https://devops.profitbricks.com/api/rest/), allowing you to interact with it from a command-line interface.

## Getting Started

Before you begin you will need to have [signed up](https://www.profitbricks.com/signup/) for a ProfitBricks account. The credentials you establish during sign-up will be used to authenticate against the [ProfitBricks API](https://devops.profitbricks.com/api/rest/).

## Installation

Download the [Profitbricks.zip](https://github.com/StackPointCloud/profitbricks-powershell/releases/download/v1.0.0/Profitbricks.zip) file and extract all. Use one of the following options to make the module available for PowerShell:

1. Place the resulting folder `Profitbricks` (contains 3 files) in `%USERPROFILE%\Documents\WindowsPowerShell\Modules\` to auto-load the module on PowerShell start for the user.
2. Place the resulting folder `Profitbricks` (contains 3 Files) in `%SYSTEMROT%\System32\WindowsPowerShell\v1.0\Modules\` to make the module available system-wide (not recommended).
3. Place the resulting folder in any folder of your choice and extend the environment variable `PSModulePath` by this folder to make the module available system-wide. 

## Configuration

Before using the ProfitBricks Powershell module to perform any operations, we'll need to set our credentials:

```
$SecurePassword = "PB_PASSWORD" | ConvertTo-SecureString -AsPlainText -Force
$username = "PB_USERNAME","User"

$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $SecurePassword

$credentials = Get-Credential $Credentials

Set-Profitbricks -Credential $credentials
Authorization successful
```


You will be notified with the following message if you have provided incorrect credentials:

```
At line:9 char:1
+ Set-Profitbricks $credentials
+ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : AuthenticationError: (:) [Set-Profitbricks], Exception
    + FullyQualifiedErrorId : 406,Profitbricks.SetProfitbricks
```

After successful authentication the credentials will be stored for the duration of  the PowerShell session.


## Usage

These examples assume that you don't have any resources provisioned under your account. The first thing we will want to do is create a data center to hold all of our resources.

### Create Data Center

We need to supply some parameters to get our first data center created. In this case, we will set the location to `us/las` so that this data center is created under the [DevOps Data Center](https://devops.profitbricks.com/tutorials/devops-data-center-information/). Other valid locations can be determined by reviewing the [REST API Documentation](https://devops.profitbricks.com/api/rest/#locations). That documentation is an excellent resource since that is what the Profitbricks Powershell module is calling to complete these operations.

```
>$datacenter =  New-PBDatacenter -Name "Example datacenter" -Description "Example description" -Location "us/las"
>$datacenter
```

```
Id         : dbe936f8-a536-49c5-b864-cec842e3ee65
Type       : datacenter
Href       : https://api.profitbricks.com/rest/v2/datacenters/dbe936f8-a536-49c5-b864-cec842e3ee65
Metadata   : class DatacenterElementMetadata {
               CreatedDate: 5/5/2016 10:38:28 AM
               CreatedBy: jasmin.gacic@gmail.com
               Etag: 75c469dc35faf7223943f4c6fe6d0689
               LastModifiedDate: 5/5/2016 10:38:28 AM
               LastModifiedBy: user@domain.com
               State: BUSY
             }
             
Properties : class DatacenterProperties {
               Name: Example datacenter
               Description: Example description
               Location: us/las
               Version: 
             }
             
Entities   : 
Request    : https://api.profitbricks.com/rest/v2/requests/d2cee0fc-a984-4d88-b56e-68203ad61660/status
```

Voila, we have successfully provisioned a data center. 

Notice the "Id" which was returned. That UUID was assigned to our new data center and will be needed for other operations. The "RequestID" that was returned can be used to check on the status of any `create` or `update` operations.

```
Get-Datacenter -DataCenterId [dbe936f8-a536-49c5-b864-cec842e3ee65]		
```

### Create Server

Next we will create a server in the data center. This time we have to pass the 'Id' for the data center, along with some other relevant properties (processor cores, memory, boot volume or boot CD-ROM) for the new server.

```
$server = New-PBServer -DataCenterId $datacenter.Id -Name "server_test" -Cores 1 -Ram 256 
$server
```

```
Id         : 3e7c254f-aeb6-481e-a594-1267cc19cac5
Type       : server
Href       : https://api.profitbricks.com/rest/v2/datacenters/dbe936f8-a536-49c5-b864-cec842e3ee65/servers/3e7c254f-aeb6-481e-a594-1267cc19cac5
Metadata   : class DatacenterElementMetadata {
               CreatedDate: 5/5/2016 10:45:29 AM
               CreatedBy: jasmin.gacic@gmail.com
               Etag: b4046bb75c48ce7bce9cb6ab3e82ae91
               LastModifiedDate: 5/5/2016 10:45:29 AM
               LastModifiedBy: user@domain.com
               State: BUSY
             }
             
Properties : class ServerProperties {
               Name: server_test
               Cores: 1
               Ram: 256
               AvailabilityZone: 
               VmState: 
               BootCdrom: 
               BootVolume: 
             }
             
Entities   : 
Request    : https://api.profitbricks.com/rest/v2/requests/3e72e83b-6e8b-43c4-bccb-97ce084292bf/status

```

### Update Server

Whoops, we didn't assign enough memory to our instance. Let's go ahead and update the server to increase the amount of memory it has assigned. We will need the DataCenterId, the ID of the server we are updating, and the parameters we want to change.

```
$server = Set-PBServer -DataCenterId $datacenter.Id -ServerId $server.Id -Ram 1024
$server.Properties.Ram
1024
```

### List Servers

Let's take a look at the list of servers in our data center. There are a few more listed here for demonstration purposes.

```
Get-PBServer -DataCenterId dbe936f8-a536-49c5-b864-cec842e3ee65 | Format-Table

Id                                   Type   Href                                                                                                                          
--                                   ----   ----                                                                                                                          
cf2b3019-351c-438b-a5f8-b4f323fc9f23 server https://api.profitbricks.com/rest/v2/datacenters/dbe936f8-a536-49c5-b864-cec842e3ee65/servers/cf2b3019-351c-438b-a5f8-b4f32...
3e7c254f-aeb6-481e-a594-1267cc19cac5 server https://api.profitbricks.com/rest/v2/datacenters/dbe936f8-a536-49c5-b864-cec842e3ee65/servers/3e7c254f-aeb6-481e-a594-1267c...
```

### Create Volume

Now that we have a server provisioned, it needs some storage. We will specify a size for this storage volume in GB as well as set the `bus` and `licencetype`. 

* The `bus` setting can have a serious performance impact. You will want to use VIRTIO when possible. Using VIRTIO may require drivers to be installed depending on the OS you plan to install. 
* The `licencetype` impacts billing rate, as there is a surcharge for running certain OS types.

```
$volume = New-PBVolume -DataCenterId $datacenter.Id -Size 5 -Type HDD -ImageId 646023fb-f7bd-11e5-b7e8-52540005ab80 -Name "test_volume"
$volume | Format-Table

Id                                   Type   Href                                                                                                                          
--                                   ----   ----                                                                                                                          
12629ab1-20e4-4751-9d92-94986d424eec volume https://api.profitbricks.com/rest/v2/datacenters/dbe936f8-a536-49c5-b864-cec842e3ee65/volumes/12629ab1-20e4-4751-9d92-94986...

```

### Attach Volume

The volume we created is not yet connected or attached to a server. To accomplish that, use the `dcid` and `serverid` values returned from the previous commands:

```
$attachedvolume = Attach-PBVolume -DataCenterId $datacenter.Id -ServerId $newServer.Id -VolumeId $volume.Id
$attachedvolume | Format-Table

Id                                   Type   Href                                                                                                                          
--                                   ----   ----                                                                                                                          
12629ab1-20e4-4751-9d92-94986d424eec volume https://api.profitbricks.com/rest/v2/datacenters/dbe936f8-a536-49c5-b864-cec842e3ee65/volumes/12629ab1-20e4-4751-9d92-94986...
```

### List Volumes

Let's take a look at all the volumes in the data center:

```
Get-PBVolume -DataCenterId $datacenter.Id | Format-Table

Id                                   Type   Href                                                                                                                          
--                                   ----   ----                                                                                                                          
12629ab1-20e4-4751-9d92-94986d424eec volume https://api.profitbricks.com/rest/v2/datacenters/dbe936f8-a536-49c5-b864-cec842e3ee65/volumes/12629ab1-20e4-4751-9d92-94986...
```

### Create Snapshot

If we have a volume we would like to keep a copy of, perhaps as a backup, we can take a snapshot:

```
New-PBSnapshot -DatacenterId $datacenter.Id -VolumeId $newvolume.Id -Name "test snapshot"
```

### List Snapshots

Here is a list of the snapshots in our account:

```

$snapshots = Get-PBSnapshot 
$snapshots| Format-Table

Id                                   Type     Href                                                                                Metadata                                
--                                   ----     ----                                                                                --------                                
91c0a13b-2bc3-4628-851f-7f30c34997f9 snapshot https://api.profitbricks.com/rest/v2/snapshots/91c0a13b-2bc3-4628-851f-7f30c34997f9 class DatacenterElementMetadata {...    
c46f5183-7ee4-4317-ab2a-e9320eac57d6 snapshot https://api.profitbricks.com/rest/v2/snapshots/c46f5183-7ee4-4317-ab2a-e9320eac57d6 class DatacenterElementMetadata {...    

```

### Update Snapshot

Now that we have a snapshot created, we can change the name to something more descriptive:

```
Set-PBSnapshot -SnapshotId $snapshots.Item(0).Id -Name "new_name
```

### Delete Snapshot

We can delete our snapshot when we are done with it:

```
Remove-PBSnapshot -SnapshotId $snapshots.Item(0).Id
```

### List Commandlets

Now we have had a taste of working with the Profitbricks Powershell module. To get more details on every commandlet contained in Profitbricks Powershell module you can do this:

```
Get-Command -Module Profitbricks

CommandType     Name                                               Version    Source                                                                                      
-----------     ----                                               -------    ------                                                                                      
Cmdlet          Attach-PBVolume                                    1.0.0.0    Profitbricks                                                                                
Cmdlet          Detach-PBVolume                                    1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBAttachedVolume                               1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBBalancedNics                                 1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBDatacenter                                   1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBFirewallRule                                 1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBImage                                        1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBIPBlock                                      1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBLan                                          1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBLoadbalancer                                 1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBLocation                                     1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBNic                                          1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBRequestStatus                                1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBServer                                       1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBSnapshot                                     1.0.0.0    Profitbricks                                                                                
Cmdlet          Get-PBVolume                                       1.0.0.0    Profitbricks                                                                                
Cmdlet          New-PBDatacenter                                   1.0.0.0    Profitbricks                                                                                
Cmdlet          New-PBFirewallRule                                 1.0.0.0    Profitbricks                                                                                
Cmdlet          New-PBIPBlock                                      1.0.0.0    Profitbricks                                                                                
Cmdlet          New-PBLan                                          1.0.0.0    Profitbricks                                                                                
Cmdlet          New-PBLoadbalancer                                 1.0.0.0    Profitbricks                                                                                
Cmdlet          New-PBNic                                          1.0.0.0    Profitbricks                                                                                
Cmdlet          New-PBServer                                       1.0.0.0    Profitbricks                                                                                
Cmdlet          New-PBSnapshot                                     1.0.0.0    Profitbricks                                                                                
Cmdlet          New-PBVolume                                       1.0.0.0    Profitbricks                                                                                
Cmdlet          Reboot-PBServer                                    1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBDatacenter                                1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBFirewallRule                              1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBIPBlock                                   1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBLan                                       1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBLoadbalancer                              1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBNic                                       1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBNicFromLoadbalancer                       1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBServer                                    1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBSnapshot                                  1.0.0.0    Profitbricks                                                                                
Cmdlet          Remove-PBVolume                                    1.0.0.0    Profitbricks                                                                                
Cmdlet          Restore-PBSnapshot                                 1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-PBDatacenter                                   1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-PBFirewallRule                                 1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-PBLan                                          1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-PBLoadbalancer                                 1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-PBNic                                          1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-PBNicToLoadbalancer                            1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-PBServer                                       1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-PBSnapshot                                     1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-PBVolume                                       1.0.0.0    Profitbricks                                                                                
Cmdlet          Set-Profitbricks                                   1.0.0.0    Profitbricks                                                                                
Cmdlet          Start-PBServer                                     1.0.0.0    Profitbricks                                                                                
Cmdlet          Stop-PBServer                                      1.0.0.0    Profitbricks 
```

### Get Help for a Commandlet

To get help for a specific commandlet do the following:

```
Get-Help Set-Profitbricks -Full

NAME
    Set-Profitbricks
    
SYNOPSIS
    This is the cmdlet sets profitbricks credentials.
    
SYNTAX
    Set-Profitbricks [-Credential] <PSCredential> [<CommonParameters>]
    
    
DESCRIPTION
    

PARAMETERS
    -Credential <PSCredential>
        Required?                    true
        Position?                    0
        Default value                
        Accept pipeline input?       true (ByValue)
        Accept wildcard characters?  false
        
    <CommonParameters>
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see 
        about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).
INPUTS
    System.Management.Automation.PSCredential
        
    
    
    
OUTPUTS
    
----------  EXAMPLE 1  ----------
    
$credentials = Get-Credential -Message [message text] -UserName [user_name]
Set-Profitbricks -Credential $credential
```
