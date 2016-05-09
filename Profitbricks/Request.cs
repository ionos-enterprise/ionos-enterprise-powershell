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
    /// <para type="synopsis">This commandlet will retrieve the status of the request.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBRequestStatus -RequestUrl [url]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBRequestStatus")]
    [OutputType(typeof(RequestStatus))]
    public class GetRequest : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Request URL</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Request Url", Mandatory = true, ValueFromPipeline = true)]
        public string RequestUrl { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var requestApi = new RequestApi(Utilities.Configuration);

                var sub = RequestUrl.Substring(RequestUrl.IndexOf("requests/") + 9, 36);

                var request = requestApi.GetStatus(sub);

                WriteObject(request);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }

    }
}
