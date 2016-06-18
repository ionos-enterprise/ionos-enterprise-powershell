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
    /// <para type="synopsis">This commandlet will get one or a list of firewall rules associated with a particular NIC.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBFirewallRule -DataCenterId [UUID] -ServerId [UUID] -NicId [UUID]</para>
    /// <para type="description">Get-PBFirewallRule -DataCenterId [UUID] -ServerId [UUID] -NicId [UUID] -FirewallRuleId[UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBFirewallRule")]
    [OutputType(typeof(FirewallRule))]
    public class GetFirewallRule : Cmdlet
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
        [Parameter(Position = 0, HelpMessage = "Server Id", Mandatory = true, ValueFromPipeline = true)]
        public string ServerId { get; set; }

        /// <summary>
        /// <para type="description">Nic ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Nic Id", Mandatory = true, ValueFromPipeline = true)]
        public string NicId { get; set; }

        /// <summary>
        /// <para type="description">Firewall rule ID. If this parameter is not passed, the commandlet will return a list of all firewall rules associated with the NIC in question.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Firewall Rule Id", ValueFromPipeline = true)]
        public string FirewallRuleId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var fwApi = new FirewallRuleApi(Utilities.Configuration);
                if (!string.IsNullOrEmpty(FirewallRuleId))
                {
                    var fw = fwApi.FindById(DataCenterId, ServerId, NicId, FirewallRuleId, depth: 5);

                    WriteObject(fw);
                }
                else
                {
                    var fws = fwApi.FindAll(DataCenterId, ServerId, NicId, depth: 5);

                    WriteObject(fws.Items);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will get one or a list of firewall rules associated with a particular NIC.</para>
    /// </summary>
    /// <example>
    /// <para type="description">New-PBFirewallRule -DataCenterId [UUID] -ServerId [UUID] -NicId [UUID]</para>
    /// <para type="description">Get-PBFirewallRule -DataCenterId [UUID] -ServerId [UUID] -NicId [UUID] -FirewallRuleId[UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBFirewallRule")]
    [OutputType(typeof(FirewallRule))]
    public class NewFirewallRule : Cmdlet
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
        /// <para type="description">The protocol for the rule: TCP, UDP, ICMP, ANY. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "The protocol for the rule: TCP, UDP, ICMP, ANY.", ValueFromPipeline = true)]
        public string Protocol { get; set; }

        /// <summary>
        /// <para type="description">The name of the firewall rule.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "The name of the Firewall Rule.", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Only traffic originating from the respective MAC address is allowed. Valid format: aa:bb:cc:dd:ee:ff. Value "null" allows all source MAC addresses.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "Only traffic originating from the respective MAC address is allowed. Valid format: aa:bb:cc:dd:ee:ff. Value null allows all source MAC address.", ValueFromPipeline = true)]
        public string SourceMac { get; set; }

        /// <summary>
        /// <para type="description">Only traffic originating from the respective IPv4 address is allowed. Value "null" allows all source IPs.</para>
        /// </summary>
        [Parameter(Position = 6, HelpMessage = "Only traffic originating from the respective IPv4 address is allowed. Value null allows all source IPs.", ValueFromPipeline = true)]
        public string SourceIp { get; set; }

        /// <summary>
        /// <para type="description">In case the target NIC has multiple IP addresses, only traffic directed to the IP address of the NIC is allowed. Value "null" allows all target IPs.</para>
        /// </summary>
        [Parameter(Position = 7, HelpMessage = "In case the target NIC has multiple IP addresses, only traffic directed to the respective IP address of the NIC is allowed. Value null allows all target IPs.", ValueFromPipeline = true)]
        public string TargetIp { get; set; }

        /// <summary>
        /// <para type="description">Defines the start range of the allowed port (from 1 to 65534) if protocol TCP or UDP is chosen. Leave portRangeStart and portRangeEnd value "null" to allow all ports.</para>
        /// </summary>
        [Parameter(Position = 8, HelpMessage = "Defines the start range of the allowed port (from 1 to 65534) if protocol TCP or UDP is chosen. Leave portRangeStart and portRangeEnd value null to allow all ports.", ValueFromPipeline = true)]
        public int? PortRangeStart { get; set; }

        /// <summary>
        /// <para type="description">Defines the start range of the allowed port (from 1 to 65534) if protocol TCP or UDP is chosen. Leave portRangeStar and portRangeEnd value "null" to allow all ports.</para>
        /// </summary>
        [Parameter(Position = 9, HelpMessage = "Defines the end range of the allowed port (from 1 to 65534) if the protocol TCP or UDP is chosen. Leave portRangeStart and portRangeEnd null to allow all ports.", ValueFromPipeline = true)]
        public int? PortRangeEnd { get; set; }

        /// <summary>
        /// <para type="description">Defines the allowed type (from 0 to 254) if the protocol ICMP is chosen. Value "null" allows all types.</para>
        /// </summary>
        [Parameter(Position = 10, HelpMessage = "Defines the allowed type (from 0 to 254) if the protocol ICMP is chosen. Value null allows all types.", ValueFromPipeline = true)]
        public int? IcmpType { get; set; }

        /// <summary>
        /// <para type="description">Defines the allowed code (from 0 to 254) if the protocol ICMP is chosen. Value "null" allows all types.</para>
        /// </summary>
        [Parameter(Position = 11, HelpMessage = "Defines the allowed code (from 0 to 254) if protocol ICMP is chosen. Value null allows all codes.", ValueFromPipeline = true)]
        public int? IcmpCode { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var fwApi = new FirewallRuleApi(Utilities.Configuration);

                var newProps = new FirewallruleProperties
                {
                    PortRangeStart = PortRangeStart == null ? null : PortRangeStart,
                    PortRangeEnd = PortRangeEnd == null ? null : PortRangeEnd,
                    IcmpType = IcmpType == null ? null : IcmpType,
                    IcmpCode = IcmpCode == null ? null : IcmpCode
                };

                if (!string.IsNullOrEmpty(Protocol))
                    newProps.Protocol = Protocol;
                if (!string.IsNullOrEmpty(Name))
                    newProps.Name = Name;
                if (!string.IsNullOrEmpty(SourceMac))
                    newProps.SourceMac = SourceMac;
                if (!string.IsNullOrEmpty(SourceIp))
                    newProps.SourceIp = SourceIp;
                if (!string.IsNullOrEmpty(TargetIp))
                    newProps.TargetIp = TargetIp;

                var fw = fwApi.Create(DataCenterId, ServerId, NicId, new FirewallRule { Properties = newProps });

                WriteObject(fw);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update firewall rule properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBFirewallRule -DataCenterId [UUID] -ServerId [UUID] -NicId [UUID] -FirewallRuleId [UUID] -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBFirewallRule")]
    [OutputType(typeof(FirewallRule))]
    public class SetFirewallRule : Cmdlet
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
        /// <para type="description">Firewall rule ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Firewall Rule Id", Mandatory = true, ValueFromPipeline = true)]
        public string FirewallRuleId { get; set; }

        /// <summary>
        /// <para type="description">The protocol for the rule: TCP, UDP, ICMP, ANY. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "The protocol for the rule: TCP, UDP, ICMP, ANY.", ValueFromPipeline = true)]
        public string Protocol { get; set; }

        /// <summary>
        /// <para type="description">The name of the firewall rule.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "The name of the Firewall Rule.", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Only traffic originating from the respective MAC address is allowed. Valid format: aa:bb:cc:dd:ee:ff. Value "null" allows all source MAC addresses.</para>
        /// </summary>
        [Parameter(Position = 6, HelpMessage = "Only traffic originating from the respective MAC address is allowed. Valid format: aa:bb:cc:dd:ee:ff. Value null allows all source MAC address.", ValueFromPipeline = true)]
        public string SourceMac { get; set; }

        /// <summary>
        /// <para type="description">Only traffic originating from the respective IPv4 address is allowed. Value "null" allows all source IPs.</para>
        /// </summary>
        [Parameter(Position = 7, HelpMessage = "Only traffic originating from the respective IPv4 address is allowed. Value null allows all source IPs.", ValueFromPipeline = true)]
        public string SourceIp { get; set; }

        /// <summary>
        /// <para type="description">In case the target NIC has multiple IP addresses, only traffic directed to the IP address of the NIC is allowed. Value "null" allows all target IPs.</para>
        /// </summary>
        [Parameter(Position = 8, HelpMessage = "In case the target NIC has multiple IP addresses, only traffic directed to the respective IP address of the NIC is allowed. Value null allows all target IPs.", ValueFromPipeline = true)]
        public string TargetIp { get; set; }

        /// <summary>
        /// <para type="description">Defines the start range of the allowed port (from 1 to 65534) if protocol TCP or UDP is chosen. Leave portRangeStart and portRangeEnd value "null" to allow all ports.</para>
        /// </summary>
        [Parameter(Position = 9, HelpMessage = "Defines the start range of the allowed port (from 1 to 65534) if protocol TCP or UDP is chosen. Leave portRangeStart and portRangeEnd value null to allow all ports.", ValueFromPipeline = true)]
        public int? PortRangeStart { get; set; }

        /// <summary>
        /// <para type="description">Defines the end range of the allowed port (from 1 to 65534) if the protocol TCP or UDP is chosen. Leave portRangeStart and portRangeEnd "null" to allow all ports.</para>
        /// </summary>
        [Parameter(Position = 10, HelpMessage = "Defines the end range of the allowed port (from 1 to 65534) if the protocol TCP or UDP is chosen. Leave portRangeStart and portRangeEnd null to allow all ports.", ValueFromPipeline = true)]
        public int? PortRangeEnd { get; set; }

        /// <summary>
        /// <para type="description">Defines the allowed type (from 0 to 254) if the protocol ICMP is chosen. Value "null" allows all types.</para>
        /// </summary>
        [Parameter(Position = 11, HelpMessage = "Defines the allowed type (from 0 to 254) if the protocol ICMP is chosen. Value null allows all types.", ValueFromPipeline = true)]
        public int? IcmpType { get; set; }

        /// <summary>
        /// <para type="description">Defines the allowed code (from 0 to 254) if protocol ICMP is chosen. Value "null" allows all codes.</para>
        /// </summary>
        [Parameter(Position = 12, HelpMessage = "Defines the allowed code (from 0 to 254) if protocol ICMP is chosen. Value null allows all codes.", ValueFromPipeline = true)]
        public int? IcmpCode { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var fwApi = new FirewallRuleApi(Utilities.Configuration);

                var newProps = new FirewallruleProperties
                {
                    PortRangeStart = PortRangeStart,
                    PortRangeEnd = PortRangeEnd,
                    IcmpType = IcmpType,
                    IcmpCode = IcmpCode
                };

                if (!string.IsNullOrEmpty(Name))
                    newProps.Name = Name;
                if (!string.IsNullOrEmpty(SourceMac))
                    newProps.SourceMac = SourceMac;
                if (!string.IsNullOrEmpty(SourceIp))
                    newProps.SourceIp = SourceIp;
                if (!string.IsNullOrEmpty(TargetIp))
                    newProps.TargetIp = TargetIp;
                if (!string.IsNullOrEmpty(Protocol))
                    newProps.Protocol = Protocol;

                var fw = fwApi.PartialUpdate(DataCenterId, ServerId, NicId, FirewallRuleId, newProps);

                WriteObject(fw);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove the specified firewall rule </para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBFirewallRule -DataCenterId [UUID] -ServerId [UUID] -NicId [UUID] -FirewallRuleId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBFirewallRule")]
    [OutputType(typeof(FirewallRule))]
    public class RemoveFirewallRule : Cmdlet
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
        /// <para type="description">Firewall rule ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "Firewall Rule Id", ValueFromPipeline = true)]
        public string FirewallRuleId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var fwApi = new FirewallRuleApi(Utilities.Configuration);

                var fws = fwApi.Delete(DataCenterId, ServerId, NicId, FirewallRuleId, depth: 5);

                WriteObject("Firewall Rule successfuly deleted.");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
