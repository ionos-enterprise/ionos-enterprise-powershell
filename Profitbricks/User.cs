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
    /// <para type="synopsis">This commandlet will get a list of all the users that have been created under a contract.</para>
    /// <para type="synopsis">If UserId parameter is provided then it will return only the specified user.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBUser </para>
    /// <para type="description">Get-PBUser -UserId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBUser")]
    [OutputType(typeof(User))]
    public class GetUser : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">User ID.</para>
        /// </summary>
        [Parameter(HelpMessage = "User Id", Position = 0, ValueFromPipeline = true)]
        public string UserId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var userApi = new UserApi(Utilities.Configuration);

                if (!string.IsNullOrEmpty(this.UserId))
                {
                    var user = userApi.FindById(this.UserId, depth: 5);

                    WriteObject(user);
                }
                else
                {
                    var users = userApi.FindAll(depth: 5);

                    WriteObject(users.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will create a user.</para>
    /// </summary>
    /// <example>
    /// <para type="example">New-PBUser -FirstName [firstName] -LastName [lastName] -Email [email] -Password [password]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBUser")]
    [OutputType(typeof(User))]
    public class NewUser : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">The first name of the user.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "First Name", Mandatory = true)]
        public string FirstName { get; set; }

        /// <summary>
        /// <para type="description">The last name of the user.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Last Name", Mandatory = true)]
        public string LastName { get; set; }

        /// <summary>
        /// <para type="description">The email of the user.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Email", Mandatory = true)]
        public string Email { get; set; }

        /// <summary>
        /// <para type="description">The password.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Password", Mandatory = true)]
        public string Password { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the user will have administrative rights.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "Indicates if the user will have administrative rights", ValueFromPipeline = true)]
        public bool? Administrator { get; set; }

        /// <summary>
        /// <para type="description">Indicates if secure (two-factor) authentication should be forced for the user.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "Indicates if secure (two-factor) authentication should be forced for the user.", ValueFromPipeline = true)]
        public bool? ForceSecAuth { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var userApi = new UserApi(Utilities.Configuration);

                var user = userApi.Create(new User()
                {
                    Properties = new UserProperties()
                    {
                        FirstName = this.FirstName,
                        LastName = this.LastName,
                        Password = this.Password,
                        Email = this.Email,
                        Administrator = this.Administrator,
                        ForceSecAuth = this.ForceSecAuth
                    }
                });

                WriteObject(user);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update the user properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBUser -FirstName [firstName] -LastName [lastName] -Email [email]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBUser")]
    [OutputType(typeof(User))]
    public class SetUser : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">User ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "User Id", Mandatory = true, ValueFromPipeline = true)]
        public string UserId { get; set; }

        /// <summary>
        /// <para type="description">The first name of the user.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "First Name", ValueFromPipeline = true)]
        public string FirstName { get; set; }

        /// <summary>
        /// <para type="description">The last name of the user.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Last Name", ValueFromPipeline = true)]
        public string LastName { get; set; }

        /// <summary>
        /// <para type="description">The email of the user.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Email", ValueFromPipeline = true)]
        public string Email { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the user will have administrative rights.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "Indicates if the user will have administrative rights", ValueFromPipeline = true)]
        public bool? Administrator { get; set; }

        /// <summary>
        /// <para type="description">Indicates if secure (two-factor) authentication should be forced for the user.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "Indicates if secure (two-factor) authentication should be forced for the user.", ValueFromPipeline = true)]
        public bool? ForceSecAuth { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var newProps = new UserProperties();

                if (!string.IsNullOrEmpty(this.FirstName))
                {
                    newProps.FirstName = this.FirstName;
                }
                if (!string.IsNullOrEmpty(this.LastName))
                {
                    newProps.LastName = this.LastName;
                }
                if (!string.IsNullOrEmpty(this.Email))
                {
                    newProps.Email = this.Email;
                }
                if (this.Administrator.HasValue)
                {
                    newProps.Administrator = this.Administrator;
                }
                if (this.ForceSecAuth.HasValue)
                {
                    newProps.ForceSecAuth = this.ForceSecAuth;
                }

                var userApi = new UserApi(Utilities.Configuration);

                var resp = userApi.Update(this.UserId, new User { Properties = newProps });

                WriteObject(resp);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove a user.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBUser -User [UUID] </para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBUser", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(User))]
    public class RemoveUser : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">User ID. </para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "User Id", Mandatory = true, ValueFromPipeline = true)]
        public string UserId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var userApi = new UserApi(Utilities.Configuration);

                var resp = userApi.Delete(this.UserId);

                WriteObject("User successfully removed ");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will list users from the goup.</para>
    /// </summary>
    /// <example>
    /// <para type="description">ListUserGroup-PBUser -GroupId [UUID]</para>
    /// </example>
    [Cmdlet("ListUserGroup", "PBUser")]
    [OutputType(typeof(User))]
    public class ListUserGroup : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Group Id", Mandatory = true, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            var groupApi = new GroupApi(Utilities.Configuration);

            var users = groupApi.FindAllGroupUsers(this.GroupId);

            WriteObject(users.Items);
            return;
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will add user to the group.</para>
    /// </summary>
    /// <example>
    /// <para type="description">AddToGroup-PBUser -GroupId [UUID] -UserId [UUID]</para>
    /// </example>
    [Cmdlet("AddToGroup", "PBUser")]
    [OutputType(typeof(User))]
    public class AddUserToGroup : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Group Id", Mandatory = true, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        /// <summary>
        /// <para type="description">User ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "User Id", Mandatory = true, ValueFromPipeline = true)]
        public string UserId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            var groupApi = new GroupApi(Utilities.Configuration);


            var user = groupApi.AddGroupUser(this.GroupId, this.UserId, depth: 5);

            if (user != null)
            {
                WriteObject(user);
            }

            return;
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove user from the group.</para>
    /// </summary>
    /// <example>
    /// <para type="description">RemoveFromGroup-PBUser -GroupId [UUID] -UserId [UUID]</para>
    /// </example>
    [Cmdlet("RemoveFromGroup", "PBUser")]
    [OutputType(typeof(User))]
    public class RemoveUserFromGroup : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Group ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Group Id", Mandatory = true, ValueFromPipeline = true)]
        public string GroupId { get; set; }

        /// <summary>
        /// <para type="description">User ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "User Id", Mandatory = true, ValueFromPipeline = true)]
        public string UserId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            var groupApi = new GroupApi(Utilities.Configuration);


            var resp = groupApi.RemoveGroupUser(this.GroupId, this.UserId, depth: 5);

            WriteObject("User successfully removed from the group.");

            return;
        }
    }
}
