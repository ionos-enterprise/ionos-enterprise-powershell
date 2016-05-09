using Api;
using Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Text;

namespace Profitbricks
{
    /// <summary>
    /// <para type="synopsis">This is the cmdlet which sets ProfitBricks credentials.</para>
    /// </summary>
    /// <example>
    ///   <code>
    ///   $credentials = Get-Credential -Message [message text] -UserName [user_name]
    ///   Set-Profitbricks -Credential $credential
    ///   </code>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "Profitbricks")]
    [Alias("Set Profitbricks Credentials")]
    public class SetProfitbricks : Cmdlet
    {
        #region Parameters

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Profitbricks username")]
        public PSCredential Credential { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var dcApi = new LocationApi(new Configuration { Username = Credential.UserName, Password = Utilities.SecureStringToString(Credential.Password) });

                var dcs = dcApi.FindAll(depth: 5);

                Utilities.Configuration = new Configuration
                {
                    Username = Credential.UserName,
                    Password = Utilities.SecureStringToString(Credential.Password)
                };

                WriteObject("Authorization successful" );
            }
            catch (ApiException ex)
            {
                WriteError(new ErrorRecord(new Exception("Authentication failed"),ex.ErrorCode.ToString(), ErrorCategory.AuthenticationError, null));
            }
           
        }
    }
}
