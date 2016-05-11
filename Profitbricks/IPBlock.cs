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
    /// <para type="synopsis">This commandlet will get one or a list of IP Blocks within Virtual Data Center.</para>
    /// <para type="synopsis">If IpBlockId parameter is provided then it will return only the specified IpBlock.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBIPBlock </para>
    /// <para type="description">Get-PBIPBlock -IpBlockId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBIPBlock")]
    [OutputType(typeof(IpBlock))]
    public class GetIPBlock : Cmdlet
    {

        #region Parameters 

        /// <summary>
        /// <para type="description">IP Block ID</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "IP Block Id", ValueFromPipeline = true)]
        public string IpBlockId { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var ipblockApi = new IPBlocksApi(Utilities.Configuration);

                if (!string.IsNullOrEmpty(IpBlockId))
                {
                    var ipblock = ipblockApi.FindById(IpBlockId, depth: 5);

                    WriteObject(ipblock);
                }
                else
                {
                    var ipblocks = ipblockApi.FindAll(depth: 5);
                    WriteObject(ipblocks.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will create an IP Block within the data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">New-PBIPBlock -Location [location] -Size [size]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBIPBlock")]
    [OutputType(typeof(IpBlock))]
    public class NewIPBlock : Cmdlet
    {

        #region Parameters 

        /// <summary>
        /// <para type="description">Location</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Location (see: Get-PBLocation)", Mandatory = true, ValueFromPipeline = true)]
        public string Location { get; set; }

        /// <summary>
        /// <para type="description">The size of the IP block</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "The size of the IP block", Mandatory = true, ValueFromPipeline = true)]
        public int Size { get; set; }
        
        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var ipblockApi = new IPBlocksApi(Utilities.Configuration);

                var newProps = new IpBlockProperties { Size = this.Size, Location = this.Location };

                var ipblock = ipblockApi.Create(new IpBlock { Properties = newProps }, depth: 5);
                WriteObject(ipblock);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove an IP Block from the data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">New-PBIPBlock -IpBlockId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBIPBlock", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(IpBlock))]
    public class RemoveIpBlock : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">IPBlock Id. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "IPBlock Id", Mandatory = true, ValueFromPipeline = true)]
        public string IpBlockId { get; set; }
        
        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var ipblockApi = new IPBlocksApi(Utilities.Configuration);

                var resp = ipblockApi.Delete(IpBlockId);

                WriteObject("IPBlock successfully removed ");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
