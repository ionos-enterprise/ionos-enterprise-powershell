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
    /// <para type="synopsis">This commandlet returns information about the resource limits for a particular contract and the current resource usage.</para>
    /// <para type="synopsis">If the ContractNumber parameter is provided,  it will return only the specified contract.</para>
    /// </summary>
    ///<example>
    /// <para type="example">Get-PBContractResource</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBContractResources")]
    [OutputType(typeof(ContractResources))]
    public class GetContractResources : Cmdlet
    {
        protected override void BeginProcessing()
        {
            try
            {
                var contractResourcesApi = new ContractResourcesApi(Utilities.Configuration);

                var contract = contractResourcesApi.List(null, 5);
                WriteObject(contract.Properties);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
