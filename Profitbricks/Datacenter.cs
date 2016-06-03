using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Model;
using Client;
using Api;
using System.Runtime.InteropServices;

namespace Profitbricks
{
    /// <summary>
    /// <para type="synopsis">This commandlet will get one or a list of virtual data centers from your ProfitBricks instance.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBDatacenter </para>
    /// <para type="description">Get-PBDatacenter -DataCenterId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBDatacenter")]
    [OutputType(typeof(Datacenter))]
    public class GetDatacenter : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. If this parameter is not passed, the commandlet will return a list of all datacenters from your ProfitBricks instance.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id")]
        public string DataCenterId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var dcApi = new DataCenterApi(Utilities.Configuration);

                if (string.IsNullOrEmpty(DataCenterId))
                {
                    var dcs = dcApi.FindAll(depth: 5);
                    WriteObject(dcs.Items);
                }
                else
                {
                    var dc = dcApi.FindById(DataCenterId, depth: 5);
                    WriteObject(dc);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will create a virtual data center in your ProfitBricks instance.</para>
    /// </summary>
    /// <example>
    /// <para type="example">New-PBDatacenter -Name [name] -Location [location id]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBDatacenter")]
    [OutputType(typeof(Datacenter))]
    public class NewDatacenter : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">The name of the data center.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Name", Mandatory = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Virtual data center location (us/las, de/fkb, de/fra). Mandatory parameter</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Virtual data center Location (us/las, de/fkb, de/fra)", ValueFromPipeline = true,Mandatory = true)]
        public string Location { get; set; }

        /// <summary>
        /// <para type="description">Virtual data center description</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Virtual data center Description", Mandatory = false)]
        public string Description { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var dcApi = new DataCenterApi(Utilities.Configuration);

                var dc = dcApi.Create(new Datacenter
                {
                    Properties = new DatacenterProperties
                    {
                        Name = this.Name,
                        Description = this.Description,
                        Location = this.Location
                    }
                });

                WriteObject(dc);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove a virtual data center from your ProfitBricks instance.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBDatacenter -DataCenterId [UUID] </para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBDatacenter", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(Datacenter))]
    public class RemoveDatacenter : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. </para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var dcApi = new DataCenterApi(Utilities.Configuration);

                var resp = dcApi.Delete(this.DataCenterId);

                WriteObject("Virtual data center successfully removed ");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update virtual data center properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBDatacenter -DataCenterId [UUID] -Name [name] -Description [description]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBDatacenter")]
    [OutputType(typeof(Datacenter))]
    public class SetDatacenter : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter. </para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Virtual data center new name. </para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Virtual data center Name", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Virtual data center description. </para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Virtual data center Description", ValueFromPipeline = true)]
        public string Description { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var dcApi = new DataCenterApi(Utilities.Configuration);

                if ((string.IsNullOrWhiteSpace(Name) || string.IsNullOrEmpty(Name)) &&
                    (string.IsNullOrWhiteSpace(Description) || string.IsNullOrEmpty(Description)))
                {
                    WriteError(new ErrorRecord(new Exception("Please provide Name or Description to update Virtual data center."), "", ErrorCategory.InvalidArgument, ""));
                }
                else
                {
                    var dc = new DatacenterProperties();

                    if (!(string.IsNullOrWhiteSpace(Name) && string.IsNullOrEmpty(Name)))
                    {
                        dc.Name = Name;
                    }

                    if (!(string.IsNullOrWhiteSpace(Description) && string.IsNullOrEmpty(Description)))
                    {
                        dc.Description = Description;
                    }

                    var resp = dcApi.PartialUpdate(this.DataCenterId, dc);
                    WriteObject(resp);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
