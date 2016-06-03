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
    /// <para type="synopsis">This commandlet will get one or a list of LANs within the data center.</para>
    /// <para type="synopsis">If the LanId parameter is provided then it will return only the specified LAN.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBLan -DatacenterId [UUID]</para>
    /// <para type="description">Get-PBLan -DatacenterId [UUID]-LanId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBLan")]
    [OutputType(typeof(Lan))]
    public class GetLan : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Lan ID.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Lan Id", ValueFromPipeline = true)]
        public string LanId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var lanApi = new LanApi(Utilities.Configuration);

                if (!string.IsNullOrEmpty(LanId))
                {
                    var lan = lanApi.FindById(DataCenterId, LanId, depth: 5);

                    WriteObject(lan);
                }
                else
                {
                    var lans = lanApi.FindAll(DataCenterId, depth: 5);

                    WriteObject(lans.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet creates a LAN within a data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">New-PBLan -DatacenterId [UUID]-Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBLan")]
    [OutputType(typeof(Lan))]
    public class NewLan : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">The name of your LAN.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "The name of your LAN.", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Boolean indicating if the LAN faces the public Internet or not.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Boolean indicating if the LAN faces the public Internet or not.", ValueFromPipeline = true)]
        public bool? Public { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var lanApi = new LanApi(Utilities.Configuration);

                var newProps = new LanProperties { Public = this.Public };

                if (!string.IsNullOrEmpty(Name))
                {
                    newProps.Name = Name;
                }

                var newLan = lanApi.Create(DataCenterId, new Lan { Properties = newProps });

                WriteObject(newLan);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet removes the specified LAN froma a data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBLan -DatacenterId [UUID] -LanId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBLan")]
    [OutputType(typeof(Lan))]
    public class RemoveLan : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Lan ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "LAN Id", Mandatory = true, ValueFromPipeline = true)]
        public string LanId { get; set; }
        
        #endregion
        protected override void BeginProcessing()
        {
            try
            {
                var lanApi = new LanApi(Utilities.Configuration);

                var lan = lanApi.Delete(DataCenterId, LanId);
                WriteObject(lan);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update LAN properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBLan -DataCenterId [UUID] -LanId [UUID] -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBLan")]
    [OutputType(typeof(Lan))]
    public class SetLan : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Lan ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "LAN Id", Mandatory = true, ValueFromPipeline = true)]
        public string LanId { get; set; }

        /// <summary>
        /// <para type="description">The name of your LAN.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "The name of your LAN.", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Boolean indicating if the LAN faces the public Internet or not.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Boolean indicating if the LAN faces the public Internet or not.", ValueFromPipeline = true)]
        public bool? Public { get; set; }

        #endregion
        protected override void BeginProcessing()
        {
            try
            {
                var lanApi = new LanApi(Utilities.Configuration);
                var newProps = new LanProperties { Public = this.Public};

                if (!string.IsNullOrEmpty(Name))
                {
                    newProps.Name = Name;
                }

                var resp = lanApi.PartialUpdate(DataCenterId, LanId,newProps);

                WriteObject("Lan successfully removed.");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
