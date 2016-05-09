using Api;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Profitbricks
{
    /// <summary>
    /// <para type="synopsis">This commandlet will get a list of volumes attached to the server.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBAttachedVolume -DataCenterId [UUID] -ServerId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBAttachedVolume")]
    [OutputType(typeof(Volume))]
    public class GetAttachedVolume : Cmdlet
    {
        #region Parameters 
        
        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Server ID. If this parameter is not passed the commandlet will return a list of all servers from the virtual data center passed in the previous parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Server Id", Mandatory = true, ValueFromPipeline = true)]
        public string ServerId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var attachedVolumesApi = new AttachedVolumesApi(Utilities.Configuration);

                var volumes = attachedVolumesApi.FindAll(this.DataCenterId, this.ServerId, depth: 5);
                WriteObject(volumes.Items);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will get one or a list of volumes attached from a given virtual data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBVolume -DataCenterId [UUID] -VolumeId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBVolume")]
    [OutputType(typeof(Volume))]
    public class GetVolume : Cmdlet
    {
        #region Parameters 

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Volume ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Volume Id", ValueFromPipeline = true)]
        public string VolumeId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var volumeApi = new VolumeApi(Utilities.Configuration);

                if (string.IsNullOrEmpty(VolumeId))
                {
                    var volumes = volumeApi.FindAll(DataCenterId, depth: 5);
                    WriteObject(volumes.Items);
                    return;
                }
                else
                {
                    var volume = volumeApi.FindById(DataCenterId, VolumeId, depth: 5);
                    WriteObject(volume);
                    return;
                }

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will create a volume on a given virtual data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">New-PBVolume -DataCenterId [UUID] -Size [size] -Type [type]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBVolume")]
    [OutputType(typeof(Volume))]
    public class NewVolume : Cmdlet
    {
        #region Parameters 
        
        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">The size of the volume in GB. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "The size of the volume in GB. ", Mandatory = true, ValueFromPipeline = true)]
        public int Size { get; set; }

        /// <summary>
        /// <para type="description">The disk type. HDD, SSD. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "The disk type. HDD, SSD", Mandatory = true, ValueFromPipeline = true)]
        public string Type { get; set; }

        /// <summary>
        /// <para type="description">The bus type of the volume (VIRTIO or IDE). Default: VIRTIO.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "The bus type of the volume (VIRTIO or IDE). Default: VIRTIO.", ValueFromPipeline = true)]
        public string Bus { get; set; }

        /// <summary>
        /// <para type="description">The image or snapshot ID.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "The image or snapshot ID.", ValueFromPipeline = true)]
        public string ImageId { get; set; }

        /// <summary>
        /// <para type="description">The licence type of the volume.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "The licence type of the volume. Options: LINUX, WINDOWS, UNKNOWN, OTHER", ValueFromPipeline = true)]
        public string LicenceType { get; set; }

        /// <summary>
        /// <para type="description">One-time password for the image. Only these characters are allowed: [abcdefghjkmnpqrstuvxABCDEFGHJKLMNPQRSTUVX23456789]</para>
        /// </summary>
        [Parameter(Position = 6, HelpMessage = "One-time password for the Image. Only these characters are allowed: [abcdefghjkmnpqrstuvxABCDEFGHJKLMNPQRSTUVX23456789]", ValueFromPipeline = true)]
        public string ImagePassword { get; set; }

        /// <summary>
        /// <para type="description">SSH key to allow access to the volume via SSH</para>
        /// </summary>
        [Parameter(Position = 7, HelpMessage = "SSH key to allow access to the volume via SSH", ValueFromPipeline = true)]
        public string SshKey { get; set; }


        /// <summary>
        /// <para type="description">The name of the volume.</para>
        /// </summary>
        [Parameter(Position = 7, HelpMessage = "The name of the volume.", ValueFromPipeline = true)]
        public string Name { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var volumeApi = new VolumeApi(Utilities.Configuration);

                if (string.IsNullOrEmpty(this.ImageId) && string.IsNullOrEmpty(this.LicenceType))
                {
                    WriteWarning("Please provide ImageId or LicenceType.");
                    return;
                }

                var volume = new Volume
                {
                    Properties = new VolumeProperties
                    {
                        Size = this.Size,
                        Type = this.Type
                    }
                };

                if (!string.IsNullOrEmpty(this.Name))
                {
                    volume.Properties.Name = this.Name;
                }
                if (!string.IsNullOrEmpty(this.Bus))
                {
                    volume.Properties.Bus = this.Bus;
                }
                if (!string.IsNullOrEmpty(this.ImagePassword))
                {
                    volume.Properties.ImagePassword = this.ImagePassword;
                }
                if (!string.IsNullOrEmpty(this.ImageId))
                {
                    volume.Properties.Image = this.ImageId;
                }
                if (!string.IsNullOrEmpty(LicenceType))
                {
                    volume.Properties.LicenceType = LicenceType;
                }
                if (!string.IsNullOrEmpty(SshKey))
                {
                    volume.Properties.SshKeys = new List<string> { SshKey };
                }

                var newVolume = volumeApi.Create(this.DataCenterId, volume);

                WriteObject(newVolume);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will attach a pre-existing storage volume to the server.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Attach-PBVolume -DataCenterId [UUID] -ServerId [UUID] -VolumeId [UUID]</para>
    /// </example>
    [Cmdlet("Attach", "PBVolume")]
    [OutputType(typeof(Volume))]
    public class AttachVolume : Cmdlet
    {
        #region Parameters 

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Server ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Server Id", Mandatory = true, ValueFromPipeline = true)]
        public string ServerId { get; set; }

        /// <summary>
        /// <para type="description">Volume ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Volume Id", Mandatory = true, ValueFromPipeline = true)]
        public string VolumeId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var attachedVolumesApi = new AttachedVolumesApi(Utilities.Configuration);

                var newVolume = attachedVolumesApi.AttachVolume(this.DataCenterId, this.ServerId, new Volume { Id = this.VolumeId });

                WriteObject(newVolume);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will detach the volume from the server.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Detach-PBVolume -DataCenterId [UUID] -ServerId [UUID] -VolumeId [UUID]</para>
    /// </example>
    [Cmdlet("Detach", "PBVolume")]
    [OutputType(typeof(Volume))]
    public class DetachVolume : Cmdlet
    {
        #region Parameters 

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Server ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Server Id", Mandatory = true, ValueFromPipeline = true)]
        public string ServerId { get; set; }

        /// <summary>
        /// <para type="description">Volume ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Volume Id", Mandatory = true, ValueFromPipeline = true)]
        public string VolumeId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var attachedVolumesApi = new AttachedVolumesApi(Utilities.Configuration);

                var newVolume = attachedVolumesApi.DetachVolume(this.DataCenterId, this.ServerId, this.VolumeId);

                WriteObject("Volume detached");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will delete the specified volume from the virtual data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBVolume -DataCenterId [UUID] -VolumeId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBVolume", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    [OutputType(typeof(Volume))]
    public class RemoveVolume : Cmdlet
    {
        #region Parameters 

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Volume ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Volume Id", Mandatory = true, ValueFromPipeline = true)]
        public string VolumeId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var volumeApi = new VolumeApi(Utilities.Configuration);

                var newVolume = volumeApi.Delete(this.DataCenterId, this.VolumeId);

                WriteObject("Volume detached");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update volume properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBVolume -DataCenterId [UUID] -VolumeId [UUID] -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBVolume")]
    [OutputType(typeof(Volume))]
    public class SetVolume : Cmdlet
    {
        #region Parameters 

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Volume ID. Mandatory parameter. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Volume Id", Mandatory = true, ValueFromPipeline = true)]
        public string VolumeId { get; set; }

        /// <summary>
        /// <para type="description">Volume name. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Volume Name", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The disk type. HDD, SSD.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "The disk type. HDD, SSD", ValueFromPipeline = true)]
        public string Type { get; set; }

        /// <summary>
        /// <para type="description">The bus type of the volume (VIRTIO or IDE). Default: VIRTIO.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "The bus type of the volume (VIRTIO or IDE). Default: VIRTIO.", ValueFromPipeline = true)]
        public string Bus { get; set; }

        /// <summary>
        /// <para type="description">The image or snapshot ID.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "The image or snapshot ID.", ValueFromPipeline = true)]
        public string ImageId { get; set; }

        /// <summary>
        /// <para type="description">The license type of the volume. Options: LINUX, WINDOWS, UNKNOWN, OTHER</para>
        /// </summary>
        [Parameter(Position = 6, HelpMessage = "The licence type of the volume. Options: LINUX, WINDOWS, UNKNOWN, OTHER", ValueFromPipeline = true)]
        public string LicenceType { get; set; }

        /// <summary>
        /// <para type="description">One-time password for the image. Only these characters are allowed: [abcdefghjkmnpqrstuvxABCDEFGHJKLMNPQRSTUVX23456789]</para>
        /// </summary>
        [Parameter(Position = 7, HelpMessage = "One-time password for the Image. Only these characters are allowed: [abcdefghjkmnpqrstuvxABCDEFGHJKLMNPQRSTUVX23456789]", ValueFromPipeline = true)]
        public string ImagePassword { get; set; }

        /// <summary>
        /// <para type="description">SSH key to allow access to the volume via SSH</para>
        /// </summary>
        [Parameter(Position = 8, HelpMessage = "SSH key to allow access to the volume via SSH", ValueFromPipeline = true)]
        public string SshKey { get; set; }

        /// <summary>
        /// <para type="description">The size of the volume in GB.</para>
        /// </summary>
        [Parameter(Position = 9, HelpMessage = "The size of the volume in GB.", ValueFromPipeline = true)]
        public int Size { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of CPU hot plug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 10, ValueFromPipeline = true)]
        public bool? CpuHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of CPU hot unplug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 11, ValueFromPipeline = true)]
        public bool? CpuHotUnplug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of memory hot plug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 12, ValueFromPipeline = true)]
        public bool? RamHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of memory hot unplug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 13, ValueFromPipeline = true)]
        public bool? RamHotUnplug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of NIC hot plug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 14, ValueFromPipeline = true)]
        public bool? NicHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of NIC hot unplug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 15, ValueFromPipeline = true)]
        public bool? NicHotUnplug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of Virt-IO drive hot plug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 16, ValueFromPipeline = true)]
        public bool? DiscVirtioHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of Virt-IO drive hot unplug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 17, ValueFromPipeline = true)]
        public bool? DiscVirtioHotUnplug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of SCSI drive hot plug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 18, ValueFromPipeline = true)]
        public bool? DiscScsiHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of SCSI drive hot unplug (no reboot required).</para>
        /// </summary>
        [Parameter(Position = 19, ValueFromPipeline = true)]
        public bool? DiscScsiHotUnplug { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var newProps = new VolumeProperties();

                if (!string.IsNullOrEmpty(this.Name))
                {
                    newProps.Name = this.Name;
                }
                if (!string.IsNullOrEmpty(this.Type))
                {
                    newProps.Type = this.Type;
                }
                if (!string.IsNullOrEmpty(this.Bus))
                {
                    newProps.Bus = this.Bus;
                }
                if (!string.IsNullOrEmpty(this.ImageId))
                {
                    newProps.Image = this.ImageId;
                }
                if (!string.IsNullOrEmpty(this.LicenceType))
                {
                    newProps.LicenceType = this.LicenceType;
                }
                if (!string.IsNullOrEmpty(this.ImagePassword))
                {
                    newProps.ImagePassword = this.ImagePassword;
                }
                if (!string.IsNullOrEmpty(this.SshKey))
                {
                    newProps.SshKeys = new List<string> { SshKey };
                }
                if (this.Size != 0)
                {
                    newProps.Size = this.Size;
                }

                newProps.CpuHotPlug = CpuHotPlug;
                newProps.CpuHotUnplug = CpuHotUnplug;
                newProps.DiscScsiHotPlug = DiscScsiHotPlug;
                newProps.DiscScsiHotUnplug = DiscScsiHotUnplug;
                newProps.DiscVirtioHotPlug = DiscVirtioHotPlug;
                newProps.DiscVirtioHotUnplug = DiscVirtioHotUnplug;


                var volumeApi = new VolumeApi(Utilities.Configuration);

                var resp = volumeApi.PartialUpdate(DataCenterId, VolumeId, newProps);

                WriteObject(resp);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
