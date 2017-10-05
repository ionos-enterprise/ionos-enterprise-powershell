using Api;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace ProfitBricks
{
    /// <summary>
    /// <para type="synopsis">This commandlet will get one or a list of resources and optionally their group associations.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBResources</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBResources")]
    [OutputType(typeof(ManagedResource))]
    public class GetResources : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="Type">Resource type</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Resource type.", ValueFromPipeline = true)]
        public ResourceType? ResourceType { get; set; }

        /// <summary>
        /// <para type="description">Resource ID.</para>
        /// </summary>
        [Parameter(HelpMessage = "Resource Id", Position = 1, ValueFromPipeline = true)]
        public string ResourceId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var resourceApi = new ResourceApi(Utilities.Configuration);

                if (!string.IsNullOrEmpty(ResourceId) && ResourceType.HasValue)
                {
                    var resource = resourceApi.FindSpecificByType(ResourceType.Value, ResourceId);
                    WriteObject(resource);
                }
                else if (ResourceType.HasValue)
                {
                    var resources = resourceApi.FindAllByType(ResourceType.Value, depth: 5);
                    WriteObject(resources.Items);
                }
                else
                {
                    var resources = resourceApi.FindAll(depth: 0);
                    WriteObject(resources.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
