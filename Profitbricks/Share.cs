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
    /// <para type="synopsis">This commandlet will get one or all resources that are shared through specific group and lists the permissions granted to the group members for each shared resource.</para>
    /// <para type="synopsis">If ResourceId parameter is provided then it will return the details of a specific shared resource available to the specified group.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBShare </para>
    /// <para type="description">Get-PBShare -GroupId [UUID] -ResourceId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBShare")]
    [OutputType(typeof(Share))]
    public class GetShare : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID.</para>
        /// </summary>
        [Parameter(HelpMessage = "Group Id", Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        /// <summary>
        /// <para type="description">Resource ID.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Resource Id", ValueFromPipeline = true)]
        public string ResourceId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var shareApi = new ShareApi(Utilities.Configuration);

                if (!string.IsNullOrEmpty(this.ResourceId))
                {
                    var share = shareApi.FindById(this.GroupId, this.ResourceId, depth: 5);

                    WriteObject(share);
                }
                else
                {
                    var share = shareApi.FindAll(this.GroupId, depth: 5);

                    WriteObject(share.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will add specific resource to the group.</para>
    /// </summary>
    /// <example>
    /// <para type="description">New-PBShare -GroupId [UUID] -ResourceId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBShare")]
    [OutputType(typeof(Share))]
    public class NewShare : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Group Id", Mandatory = true, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        /// <summary>
        /// <para type="description">Resource ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Resource Id", Mandatory = true, ValueFromPipeline = true)]
        public string ResourceId { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to edit privileges on this resource.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Indicates if the group will be allowed to edit privileges on this resource.", ValueFromPipeline = true)]
        public bool? EditPrivilege { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to to share this resource.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Indicates if the group will be allowed to to share this resource.", ValueFromPipeline = true)]
        public bool? SharePrivilege { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            var shareApi = new ShareApi(Utilities.Configuration);

            var share = new Share();
            share.Properties = new ShareProperties()
            {
                SharePrivilege = this.SharePrivilege,
                EditPrivilege = this.EditPrivilege
            };

            var shareAdded = shareApi.Add(this.GroupId, this.ResourceId, share, depth: 5);

            if (shareAdded != null)
            {
                WriteObject(shareAdded);
            }

            return;
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update specific resource within the group.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBShare -GroupId [UUID] -ShareId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBShare")]
    [OutputType(typeof(Share))]
    public class SetShare : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Group Id", Mandatory = true, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        /// <summary>
        /// <para type="description">Share ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Share Id", Mandatory = true, ValueFromPipeline = true)]
        public string ShareId { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to edit privileges on this resource.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Indicates if the group will be allowed to edit privileges on this resource.", ValueFromPipeline = true)]
        public bool? EditPrivilege { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to to share this resource.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Indicates if the group will be allowed to to share this resource.", ValueFromPipeline = true)]
        public bool? SharePrivilege { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            var shareApi = new ShareApi(Utilities.Configuration);

            var share = new Share();
            share.Properties = new ShareProperties()
            {
                SharePrivilege = this.SharePrivilege,
                EditPrivilege = this.EditPrivilege
            };

            var shareUpdated = shareApi.Update(this.GroupId, this.ShareId, share, depth: 5);

            if (shareUpdated != null)
            {
                WriteObject(shareUpdated);
            }

            return;
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove a share.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBShare -GroupId [UUID] -ShareId [ShareId] </para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBShare", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(Share))]
    public class RemoveShare : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID. </para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Group Id", Mandatory = true, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        /// <summary>
        /// <para type="description">Share ID. </para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Share Id", Mandatory = true, ValueFromPipeline = true)]
        public string ShareId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var shareApi = new ShareApi(Utilities.Configuration);

                var resp = shareApi.Remove(this.GroupId, this.ShareId);

                WriteObject("Share successfully removed ");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
