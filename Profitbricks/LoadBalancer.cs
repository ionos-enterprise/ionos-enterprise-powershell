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
    /// <para type="synopsis">This commandlet retrieves a list of load balancers within the virtual data center.</para>
    /// <para type="synopsis">If the LoadbalancerId parameter is provided then it will return only the specified load balancer.</para>
    /// </summary>
    ///<example>
    /// <para type="example">Get-PBLoadbalancer -DataCenterId [UUID] -LoadbalancerId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBLoadbalancer")]
    [OutputType(typeof(Loadbalancer))]
    public class GetLoadbalancer : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Loadbalancer ID.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Loadbalancer Id", ValueFromPipeline = true)]
        public string LoadbalancerId { get; set; } 

        #endregion

        protected override void BeginProcessing()
        {
            try
            {

                var loadbalancerApi = new LoadBalancerApi(Utilities.Configuration);

                if (!string.IsNullOrEmpty(LoadbalancerId))
                {
                    var loadbalancer = loadbalancerApi.FindById(DataCenterId, LoadbalancerId, depth: 5);
                    WriteObject(loadbalancer);
                }
                else
                {
                    var loadbalancers = loadbalancerApi.FindAll(DataCenterId, depth: 5);

                    WriteObject(loadbalancers.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet creates a load balancer within the data center. </para>
    /// <para type="synopsis">Load balancers can be used for public or private IP traffic.</para>
    /// </summary>
    ///<example>
    /// <para type="example">New-PBLoadbalancer -DataCenterId [UUID] -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBLoadbalancer")]
    [OutputType(typeof(Loadbalancer))]
    public class NewLoadbalancer : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0,HelpMessage = "Virtual data center ID", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Load balancer name. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1,HelpMessage = "Loadbalancer Name", Mandatory = true, ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">IPv4 address of the load balancer. All attached NICs will inherit this IP.	</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "IPv4 address of the loadbalancer.", ValueFromPipeline = true)]
        public string Ip { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the load balancer will reserve an IP using DHCP.	</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Indicates if the loadbalancer will reserve an IP using DHCP.", ValueFromPipeline = true)]
        public bool? Dhcp { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var loadbalancerApi = new LoadBalancerApi(Utilities.Configuration);
                var newProps = new LoadbalancerProperties
                {
                    Name = this.Name,
                    Dhcp = this.Dhcp
                };

                if (!string.IsNullOrEmpty(this.Ip))
                {
                    newProps.Ip = this.Ip;
                }

                var loadbalancer = loadbalancerApi.Create(DataCenterId, new Loadbalancer { Properties = newProps });

                WriteObject(loadbalancer);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet deletes the specified load balancer.</para>
    /// </summary>
    ///<example>
    /// <para type="example">Remove-PBLoadbalancer -DataCenterId [UUID] -LoadbalancerId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBLoadbalancer")]
    [OutputType(typeof(Loadbalancer))]
    public class RemoveLoadbalancer : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Loadbalancer ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Loadbalancer Id", Mandatory = true, ValueFromPipeline = true)]
        public string LoadbalancerId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var loadBalancerApi = new LoadBalancerApi(Utilities.Configuration);

                var resp = loadBalancerApi.Delete(this.DataCenterId, this.LoadbalancerId);
                WriteObject(resp);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update load balancer properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBLoadbalancer -DataCenterId [UUID] -LoadbalancerId [UUID] -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBLoadbalancer")]
    [OutputType(typeof(Loadbalancer))]
    public class SetLoadbalancer : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Load balancer ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Loadbalancer Id", Mandatory = true, ValueFromPipeline = true)]
        public string LoadbalancerId { get; set; }

        /// <summary>
        /// <para type="description">Load balancer name.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Loadbalancer Name", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The IP of the load balancer.	</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "The IP of the loadbalancer.	", ValueFromPipeline = true)]
        public string Ip { get; set; }

        /// <summary>
        /// <para type="description">Indicates if the load balancer will reserve an IP using DHCP.</para>
        /// </summary>
        [Parameter(Position =  4,HelpMessage = "Indicates if the loadbalancer will reserve an IP using DHCP.")]
        public bool? Dhcp { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var loadbalancerApi = new LoadBalancerApi(Utilities.Configuration);

                var newProps = new LoadbalancerProperties { Dhcp = Dhcp };
                if (!string.IsNullOrEmpty(Ip))
                {
                    newProps.Ip = Ip;
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    newProps.Name = Name;
                }

                var resp = loadbalancerApi.PartialUpdate(DataCenterId, LoadbalancerId, newProps);
                WriteObject(resp);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will retrieve a list of NICs associated with the load balancer.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBBalancedNics -DataCenterId [UUID] -LoadbalancerId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBBalancedNics")]
    [OutputType(typeof(BalancedNics))]
    public class GetBalancedNics : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center Id. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Load balancer ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Loadbalancer Id", Mandatory = true, ValueFromPipeline = true)]
        public string LoadbalancerId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var loadbalancerApi = new LoadBalancerApi(Utilities.Configuration);

                var resp = loadbalancerApi.FindAll(DataCenterId, LoadbalancerId);
                WriteObject(resp.Items);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will associate a NIC to a load balancer, enabling the NIC to participate in load balancing.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBNicToLoadbalancer -DataCenterId [UUID] -LoadbalancerId [UUID] -NicId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBNicToLoadbalancer")]
    [OutputType(typeof(BalancedNics))]
    public class SetNicToLoadbalancer : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Load balancer ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Loadbalancer Id", Mandatory = true, ValueFromPipeline = true)]
        public string LoadbalancerId { get; set; }

        /// <summary>
        /// <para type="description">Network interface ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Nic Id", Mandatory = true, ValueFromPipeline = true)]
        public string NicId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var networkInterfacesApi = new NetworkInterfacesApi(Utilities.Configuration);

                var resp = networkInterfacesApi.AttachNic(DataCenterId, LoadbalancerId, new Nic { Id = NicId });
                WriteObject(resp);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove the association of a NIC with a load balancer.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBNicFromLoadbalancer -DataCenterId [UUID] -LoadbalancerId [UUID] -NicId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBNicFromLoadbalancer")]
    [OutputType(typeof(BalancedNics))]
    public class RemoveNicFromLoadbalancer : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Load balancer ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Loadbalancer Id", Mandatory = true, ValueFromPipeline = true)]
        public string LoadbalancerId { get; set; }

        /// <summary>
        /// <para type="description">Network interface ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Nic Id", Mandatory = true, ValueFromPipeline = true)]
        public string NicId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var networkInterfacesApi = new NetworkInterfacesApi(Utilities.Configuration);

                var resp = networkInterfacesApi.DetachNic(DataCenterId, LoadbalancerId, NicId);

                WriteObject(resp);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
