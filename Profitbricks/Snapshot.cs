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
    /// <para type="synopsis">This commandlet retrieves a list of available snapshots within your ProfitBricks instance.</para>
    /// <para type="synopsis">If the SnapshotId parameter is provided,  it will return only the specified snapshot.</para>
    /// </summary>
    ///<example>
    /// <para type="example">Get-PBSnapshot</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBSnapshot")]
    [OutputType(typeof(Snapshot))]
    public class GetSnapshot : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Snapshot ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Snapshot Id", ValueFromPipeline = true)]
        public string SnapshotId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {

                var snapshotApi = new SnapshotApi(Utilities.Configuration);

                if (!string.IsNullOrEmpty(SnapshotId))
                {
                    var snapshot = snapshotApi.FindById(SnapshotId, depth: 5);
                    WriteObject(snapshot);
                }
                else
                {
                    var snapshots = snapshotApi.FindAll(depth: 5);

                    WriteObject(snapshots.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet creates a snapshot of a volume within the data center. </para>
    /// <para type="synopsis">You can use a snapshot to create a new storage volume or to restore a storage volume.</para>
    /// </summary>
    ///<example>
    /// <para type="example">New-PBSnapshot -DatacenterId [UUID] -VolumeId [UUID] -Name [snapshotname]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBSnapshot")]
    [OutputType(typeof(Snapshot))]
    public class NewSnapshot : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DatacenterId { get; set; }

        /// <summary>
        /// <para type="description">Volume ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Volume Id", Mandatory = true, ValueFromPipeline = true)]
        public string VolumeId { get; set; }

        /// <summary>
        /// <para type="description">The name of the snapshot.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Name", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The description of the snapshot.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Description", ValueFromPipeline = true)]
        public string Description { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var snapshotApi = new VolumeApi(Utilities.Configuration);

                var snapshot = new Snapshot { Properties = new SnapshotProperties() };
                if (!string.IsNullOrEmpty(Name))
                {
                    snapshot.Properties.Name = Name;
                }

                if (!string.IsNullOrEmpty(Description))
                {
                    snapshot.Properties.Description = Description;
                }

                var resp = snapshotApi.CreateSnapshot(DatacenterId, VolumeId, string.IsNullOrEmpty(Name) ? null : Name, string.IsNullOrEmpty(Description) ? null : Description);

                WriteObject(resp);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet deletes the specified snapshot.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBSnapshot -SnapshotId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBSnapshot")]
    [OutputType(typeof(Snapshot))]
    public class RemoveSnapshot : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Snapshot ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Snapshot Id", Mandatory = true, ValueFromPipeline = true)]
        public string SnapshotId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var snapshotApi = new SnapshotApi(Utilities.Configuration);

                var resp = snapshotApi.Delete(SnapshotId);

                WriteObject(resp);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update the snapshot properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBSnapshot -SnapshotId [UUID] -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBSnapshot")]
    [OutputType(typeof(Snapshot))]
    public class SetSnapshot : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Snapshot ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Snapshot Id", Mandatory = true, ValueFromPipeline = true)]
        public string SnapshotId { get; set; }

        /// <summary>
        /// <para type="description">Snapshot name</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Snapshot Name", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Description</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Description", ValueFromPipeline = true)]
        public string Description { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of CPU hot plug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 3, ValueFromPipeline = true)]
        public bool? CpuHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of CPU hot unplug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 4, ValueFromPipeline = true)]
        public bool? CpuHotUnplug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of RAM hot plug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 5, ValueFromPipeline = true)]
        public bool? RamHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of RAM hot unplug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 6, ValueFromPipeline = true)]
        public bool? RamHotUnplug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of NIC hot plug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 7, ValueFromPipeline = true)]
        public bool? NicHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of NIC hot unplug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 8, ValueFromPipeline = true)]
        public bool? NicHotUnplug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of Virt-IO drive hot plug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 9, ValueFromPipeline = true)]
        public bool? DiscVirtioHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of Virt-IO drive hot unplug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 10, ValueFromPipeline = true)]
        public bool? DiscVirtioHotUnplug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of SCSI drive hot plug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 11, ValueFromPipeline = true)]
        public bool? DiscScsiHotPlug { get; set; }

        /// <summary>
        /// <para type="description">This volume is capable of SCSI drive hot unplug (no reboot required).	</para>
        /// </summary>
        [Parameter(Position = 12, ValueFromPipeline = true)]
        public bool? DiscScsiHotUnplug { get; set; }

        /// <summary>
        /// <para type="description">The snapshot's license type: LINUX, WINDOWS, or UNKNOWN.	</para>
        /// </summary>
        [Parameter(Position = 13, ValueFromPipeline = true)]
        public string LicenceType { get; private set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var newProps = new SnapshotProperties();

                if (!string.IsNullOrEmpty(this.Name))
                {
                    newProps.Name = this.Name;
                }
                if (!string.IsNullOrEmpty(this.LicenceType))
                {
                    newProps.LicenceType = this.LicenceType;
                }
                if (!string.IsNullOrEmpty(this.LicenceType))
                {
                    newProps.LicenceType = this.LicenceType;
                }

                newProps.CpuHotPlug = CpuHotPlug;
                newProps.CpuHotUnplug = CpuHotUnplug;
                newProps.DiscScsiHotPlug = DiscScsiHotPlug;
                newProps.DiscScsiHotUnplug = DiscScsiHotUnplug;
                newProps.DiscVirtioHotPlug = DiscVirtioHotPlug;
                newProps.DiscVirtioHotUnplug = DiscVirtioHotUnplug;

                var snapshotApi = new SnapshotApi(Utilities.Configuration);

                var resp = snapshotApi.Update(SnapshotId, new Snapshot { Properties = newProps });

                WriteObject(resp);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This will restore a snapshot onto a volume. </para>
    /// <para type="synopsis">A snapshot is created as just another image that can be used to create new volumes or to restore an existing volume.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Restore-PBSnapshot -DatacenterId [UUID] -VolumeId [UUID]</para>
    /// </example>
    [Cmdlet("Restore", "PBSnapshot")]
    [OutputType(typeof(Snapshot))]
    public class RestoreSnapshot : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DatacenterId { get; set; }

        /// <summary>
        /// <para type="description">Volume ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Volume Id", Mandatory = true, ValueFromPipeline = true)]
        public string VolumeId { get; set; }

        /// <summary>
        /// <para type="description">Snapshot ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Snapshot Id", Mandatory = true, ValueFromPipeline = true)]
        public string SnapshotId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var volumeApi = new VolumeApi(Utilities.Configuration);

                var resp = volumeApi.RestoreSnapshot(this.DatacenterId, this.VolumeId, name: this.SnapshotId);


                WriteObject(resp);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
