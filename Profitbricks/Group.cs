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
    /// <para type="synopsis">This commandlet will get one or a list of groups.</para>
    /// <para type="synopsis">If GroupId parameter is provided then it will return only the specified group.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBGroup </para>
    /// <para type="description">Get-PBGroup -GroupId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBGroup")]
    [OutputType(typeof(Group))]
    public class GetGroup : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID.</para>
        /// </summary>
        [Parameter(HelpMessage = "Group Id", Position = 0, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var groupApi = new GroupApi(Utilities.Configuration);

                if (!string.IsNullOrEmpty(this.GroupId))
                {
                    var group = groupApi.FindById(this.GroupId, depth: 5);

                    WriteObject(group);
                }
                else
                {
                    var groups = groupApi.FindAll(depth: 5);

                    WriteObject(groups.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will create a group.</para>
    /// </summary>
    /// <example>
    /// <para type="example">New-PBGroup -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBGroup")]
    [OutputType(typeof(Group))]
    public class NewGroup : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">The name of the group.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Group Name", Mandatory = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to create virtual data centers.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Indicates if the group will be allowed to create virtual data centers.", ValueFromPipeline = true)]
        public bool? CreateDataCenter { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to create snapshots.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Indicates if the group will be allowed to create snapshots.", ValueFromPipeline = true)]
        public bool? CreateSnapshot { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to reserve IP addresses.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Indicates if the group will be allowed to reserve IP addresses.", ValueFromPipeline = true)]
        public bool? ReserveIp { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to access the activity log.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "Indicates if the group will be allowed to access the activity log.", ValueFromPipeline = true)]
        public bool? AccessActivityLog { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var groupApi = new GroupApi(Utilities.Configuration);

                var group = groupApi.Create(new Group()
                {
                    Properties = new GroupProperties()
                    {
                        Name = this.Name,
                        CreateDataCenter = this.CreateDataCenter,
                        CreateSnapshot = this.CreateSnapshot,
                        ReserveIp = this.ReserveIp,
                        AccessActivityLog = this.AccessActivityLog
                    }
                });

                WriteObject(group);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update the group properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBGroup -GroupId [UUID] -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBGroup")]
    [OutputType(typeof(Group))]
    public class SetGroup : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Group Id", Mandatory = true, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        /// <summary>
        /// <para type="description">Group name</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Group Name", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to create virtual data centers.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Indicates if the group will be allowed to create virtual data centers.", ValueFromPipeline = true)]
        public bool? CreateDataCenter { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to create snapshots.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Indicates if the group will be allowed to create snapshots.", ValueFromPipeline = true)]
        public bool? CreateSnapshot { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to reserve IP addresses.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "Indicates if the group will be allowed to reserve IP addresses.", ValueFromPipeline = true)]
        public bool? ReserveIp { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the group will be allowed to access the activity log.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "Indicates if the group will be allowed to access the activity log.", ValueFromPipeline = true)]
        public bool? AccessActivityLog { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var newProps = new GroupProperties();

                if (!string.IsNullOrEmpty(this.Name))
                {
                    newProps.Name = this.Name;
                }
                newProps.CreateDataCenter = this.CreateDataCenter;
                newProps.CreateSnapshot = this.CreateSnapshot;
                newProps.ReserveIp = this.ReserveIp;
                newProps.AccessActivityLog = this.AccessActivityLog;

                var groupApi = new GroupApi(Utilities.Configuration);

                var resp = groupApi.Update(this.GroupId, new Group { Properties = newProps });

                WriteObject(resp);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove a group.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBGroup -GroupId [UUID] </para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBGroup", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(Group))]
    public class RemoveGroup : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID. </para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Group Id", Mandatory = true, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var groupApi = new GroupApi(Utilities.Configuration);

                var resp = groupApi.Delete(this.GroupId);

                WriteObject("Group successfully removed ");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
