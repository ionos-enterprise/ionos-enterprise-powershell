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

The Profitbricks Powershell module wraps the [ProfitBricks Cloud API](https://devops.profitbricks.com/api/cloud/v2/), allowing you to interact with it from a command-line interface.

## Getting Started

Before you begin you will need to have [signed up](https://www.profitbricks.com/signup/) for a ProfitBricks account. The credentials you establish during sign-up will be used to authenticate against the [ProfitBricks Cloud API](https://devops.profitbricks.com/api/cloud/v2/).

## Installation

Profitbricks Powershell Module requires .NET Framework 4.5 or higher.  You can download .NET Framework version 4.5 from [here](https://www.microsoft.com/en-us/download/details.aspx?id=40779).

Download the [Profitbricks.zip](https://github.com/profitbricks/profitbricks-powershell/releases/download/v1.0.0/Profitbricks.zip) file and extract all. Use one of the following options to make the module available inside PowerShell:

1. Place the resulting folder `Profitbricks` in `%USERPROFILE%\Documents\WindowsPowerShell\Modules\` to auto-load the module on PowerShell start for a specific user.
2. Place the resulting folder `Profitbricks` in `%SYSTEMROOT%\System32\WindowsPowerShell\v1.0\Modules\` to make the module available system-wide (not recommended).
3. Place the resulting folder `Profitbricks` in the folder of your choice and add that folder to the environment variable `PSModulePath` to make the module available system-wide.

## Configuration

Before using the ProfitBricks Powershell module to perform any operations, we need to set our credentials:

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

After successful authentication the credentials will be stored locally for the duration of the PowerShell session.

## Usage

These examples assume that you do not have any resources provisioned under your account. The first thing we will want to do is create a data center to hold all of our resources.

### Create Data Center

We need to supply some parameters to get our first data center created. In this case, we will set the location to `us/las`. Other valid locations can be determined by reviewing the [Cloud API Documentation](https://devops.profitbricks.com/api/cloud/v2/#locations). That documentation is an excellent resource since the Cloud API is what the Profitbricks Powershell module is calling to complete these operations.

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
               CreatedBy: user@domain.tld
               Etag: 75c469dc35faf7223943f4c6fe6d0689
               LastModifiedDate: 5/5/2016 10:38:28 AM
               LastModifiedBy: user@domain.tld
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

We have successfully provisioned a data center.

Notice the "Id" which was returned. That UUID was assigned to our new data center and will be needed for other operations. The "RequestID" that was returned can be used to check on the status of any `create` or `update` operations.

```
Get-Datacenter -DataCenterId [dbe936f8-a536-49c5-b864-cec842e3ee65]
```

### Create Server

Next we will create a server in the data center. This time we have to pass the 'Id' for the data center, along with some other relevant properties (processor cores, memory, boot volume or boot CD-ROM) for the new server.

```
$server = New-PBServer -DataCenterId [UUID] -ImageId [UUID] -Cores 1 -Ram 1gb -Name test1234 -PublicIp $true -StaticIp $true -Verbose
$server
```

```
VERBOSE: Creating the server...
VERBOSE: Creating the volume...
VERBOSE: Creating the static IP address
VERBOSE: Creating the nic.

Id         : 5bcba46e-47cc-42fa-9c7d-52852088c5b5
Type       : server
Href       : https://api.profitbricks.com/rest/v2/datacenters/e12e545e-1f27-4cb0-87ac-97078dbb9923/servers/5bcba46e-47cc-42fa-9c7d-52852088c5b5
Metadata   : class DatacenterElementMetadata {
               CreatedDate: 5/11/2016 10:51:11 AM
               CreatedBy: user@domain.tld
               Etag: 23732a5ff98b1b9873f19e520730d553
               LastModifiedDate: 5/11/2016 10:51:11 AM
               LastModifiedBy: user@domain.tld
               State: AVAILABLE
             }

Properties : class ServerProperties {
               Name: test1234
               Cores: 1
               Ram: 1024
               AvailabilityZone: AUTO
               VmState: RUNNING
               BootCdrom:
               BootVolume:
             }

Entities   : class ServerEntities {
               Cdroms: class Cdroms {
               Id: 5bcba46e-47cc-42fa-9c7d-52852088c5b5/cdroms
               Type: collection
               Href: https://api.profitbricks.com/rest/v2/datacenters/e12e545e-1f27-4cb0-87ac-97078dbb9923/servers/5bcba46e-47cc-42fa-9c7d-52852088c5b5/cdroms
               Items: System.Collections.Generic.List`1[Model.Image]
             }

               Volumes: class AttachedVolumes {
               Id: 5bcba46e-47cc-42fa-9c7d-52852088c5b5/volumes
               Type: collection
               Href: https://api.profitbricks.com/rest/v2/datacenters/e12e545e-1f27-4cb0-87ac-97078dbb9923/servers/5bcba46e-47cc-42fa-9c7d-52852088c5b5/volumes
               Items: System.Collections.Generic.List`1[Model.Volume]
             }

               Nics: class Nics {
               Id: 5bcba46e-47cc-42fa-9c7d-52852088c5b5/nics
               Type: collection
               Href: https://api.profitbricks.com/rest/v2/datacenters/e12e545e-1f27-4cb0-87ac-97078dbb9923/servers/5bcba46e-47cc-42fa-9c7d-52852088c5b5/nics
               Items: System.Collections.Generic.List`1[Model.Nic]
             }

             }

Request    :
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
Set-PBSnapshot -SnapshotId $snapshots.Item(0).Id -Name "new_name"
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