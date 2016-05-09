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
    /// <para type="synopsis">This commandlet will get one or a list of Network interfaces (NIC).</para>
    /// <para type="synopsis">If the NicId parameter is provided then it will return only the specified NIC.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBLocation </para>
    /// <para type="description">Get-PBLocation -LocationId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBNic")]
    [OutputType(typeof(Nic))]
    public class GetNic : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Server ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Server Id", Mandatory = true, ValueFromPipeline = true)]
        public string ServerId { get; set; }

        /// <summary>
        /// <para type="description">NIC ID.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Nic Id", ValueFromPipeline = true)]
        public string NicId { get; set; }


        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var nicApi = new NetworkInterfacesApi(Utilities.Configuration);

                if (!string.IsNullOrWhiteSpace(this.NicId))
                {
                    var nic = nicApi.FindById(DataCenterId, ServerId, NicId, depth: 5);

                    WriteObject(nic);
                }
                else
                {
                    var nics = nicApi.FindAll(DataCenterId, ServerId, depth: 5);

                    WriteObject(nics.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will add a NIC to the target server.</para>
    /// </summary>
    /// <example>
    /// <para type="description">New-PBNic -DataCenterId [UUID] -ServerId [UUID] -LanId [UUID] </para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBNic")]
    [OutputType(typeof(Nic))]
    public class NewNic : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Server ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Server Id", Mandatory = true, ValueFromPipeline = true)]
        public string ServerId { get; set; }

        /// <summary>
        /// <para type="description">The LAN ID the NIC will sit on. If the LAN ID does not exist it will be created.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "The LAN ID the NIC will sit on. If the LAN ID does not exist it will be created.", Mandatory = true, ValueFromPipeline = true)]
        public int LanId { get; set; }

        /// <summary>
        /// <para type="description">The name of the NIC.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "The name of the NIC.", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">IPs assigned to the NIC. This can be a collection.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "IPs assigned to the NIC. This can be a collection.", ValueFromPipeline = true)]
        public List<string> Ips { get; set; }

        /// <summary>
        /// <para type="description">Set to FALSE if you wish to disable DHCP on the NIC. Default: TRUE.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "Set to FALSE if you wish to disable DHCP on the NIC. Default: TRUE.", ValueFromPipeline = true)]
        public bool? DHCP { get; set; }

        /// <summary>
        /// <para type="description">Once you add a firewall rule, this will reflect a true value.</para>
        /// </summary>
        [Parameter(Position = 6, HelpMessage = "Once you add a firewall rule this will reflect a true value.", ValueFromPipeline = true)]
        public bool? FirewallActive { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var nicApi = new NetworkInterfacesApi(Utilities.Configuration);

                var newProps = new NicProperties { Dhcp = DHCP, FirewallActive = FirewallActive };

                if (Ips != null && Ips.Count > 0)
                {
                    newProps.Ips = Ips;
                }
                if (LanId != 0)
                {
                    newProps.Lan = LanId;
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    newProps.Name = Name;
                }
                var nic = nicApi.Create(DataCenterId, ServerId, new Nic { Properties = newProps }, depth: 5);

                WriteObject(nic);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove the specified NIC </para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBNic -DataCenterId [UUID] -ServerId [UUID] -NicId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBNic", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(Datacenter))]
    public class RemoveNic : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Server ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Server Id", Mandatory = true, ValueFromPipeline = true)]
        public string ServerId { get; set; }

        /// <summary>
        /// <para type="description">NIC ID.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Nic Id", Mandatory = true, ValueFromPipeline = true)]
        public string NicId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var nicApi = new NetworkInterfacesApi(Utilities.Configuration);

                var resp = nicApi.Delete(DataCenterId, ServerId, NicId);

                WriteObject("Nic successfully removed ");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update NIC properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBNic -DataCenterId [UUID] -ServerId [UUID] -NicId[UUID] -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBNic")]
    [OutputType(typeof(Nic))]
    public class SetNic : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Server ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Server Id", Mandatory = true, ValueFromPipeline = true)]
        public string ServerId { get; set; }

        /// <summary>
        /// <para type="description">NIC ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "Nic Id", Mandatory = true, ValueFromPipeline = true)]
        public string NicId { get; set; }

        /// <summary>
        /// <para type="description">The name of the NIC.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "The name of the NIC.", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">IPs assigned to the NIC. This can be a collection.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "IPs assigned to the NIC. This can be a collection.", ValueFromPipeline = true)]
        public List<string> Ips { get; set; }

        /// <summary>
        /// <para type="description">Set to FALSE if you wish to disable DHCP on the NIC. Default: TRUE.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "Set to FALSE if you wish to disable DHCP on the NIC. Default: TRUE.", ValueFromPipeline = true)]
        public bool? DHCP { get; set; }

        /// <summary>
        /// <para type="description">The LAN ID the NIC sits on.</para>
        /// </summary>
        [Parameter(Position = 6, HelpMessage = "The LAN ID the NIC sits on.", ValueFromPipeline = true)]
        public int LanId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var nicApi = new NetworkInterfacesApi(Utilities.Configuration);

                var newProps = new NicProperties { Dhcp = DHCP };

                if (Ips != null && Ips.Count > 0)
                {
                    newProps.Ips = Ips;
                }
                if(LanId != 0)
                {
                    newProps.Lan = LanId;
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    newProps.Name = Name;
                }
                var nic = nicApi.PartialUpdate(DataCenterId, ServerId, NicId, newProps, depth: 5);

                WriteObject(nic);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
