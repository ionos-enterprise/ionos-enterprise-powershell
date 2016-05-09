using Api;
using Client;
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
    /// <para type="synopsis">This commandlet will get one or a list of servers associated with the virtual data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Get-PBServer -DataCenterId [UUID]</para>
    /// <para type="description">Get-PBDatacenter -DataCenterId [UUID] -ServerId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PBServer")]
    [OutputType(typeof(Server))]
    public class GetServer : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">Server ID. If this parameters is not passed, the commandlet will return a list of all servers from the virtual data center passed in the previous parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "Server Id", ValueFromPipeline = true)]
        public string ServerId { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var serverApi = new ServerApi(Utilities.Configuration);

                if (string.IsNullOrEmpty(ServerId))
                {
                    var servers = serverApi.FindAll(DataCenterId, depth: 5);
                    WriteObject(servers.Items);
                }
                else
                {
                    var server = serverApi.FindById(DataCenterId, ServerId, depth: 5);
                    WriteObject(server);
                }

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will create a server within a virtual data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">New-PBServer -DataCenterId [UUID] -Name [name] -Cores [cores] -Ram [ram]</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "PBServer")]
    [OutputType(typeof(Server))]
    public class NewServer : Cmdlet
    {
        #region Parameters

        /// <summary>
        /// <para type="description">Virtual data center ID. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Virtual data center Id", Mandatory = true, ValueFromPipeline = true)]
        public string DataCenterId { get; set; }

        /// <summary>
        /// <para type="description">The hostname of the server. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 1, HelpMessage = "The hostname of the server.", Mandatory = true, ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The total number of cores for the server. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "The total number of cores for the server.", Mandatory = true, ValueFromPipeline = true)]
        public int Cores { get; set; }

        /// <summary>
        /// <para type="description">The amount of memory for the server in MB, e.g. 2048. Size must be specified in multiples of 256 MB with a minimum of 256 MB. However, if you set ramHotPlug to TRUE then you must use a minimum of 1024 MB. Mandatory parameter.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "The amount of memory for the server in MB, e.g. 2048.Size must be specified in multiples of 256 MB with a minimum of 256 MB; however, if you set ramHotPlug to TRUE then you must use a minimum of 1024 MB.",
            Mandatory = true, ValueFromPipeline = true)]
        public int Ram { get; set; }

        /// <summary>
        /// <para type="description">The availability zone in which the server should exist. AUTO, ZONE_1, ZONE_2.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "The availability zone in which the server should exist. AUTO, ZONE_1, ZONE_2", ValueFromPipeline = true)]
        public string AvailabilityZone { get; set; }

        /// <summary>
        /// <para type="description">Reference to a volume used for booting. If not ‘null’ then bootCdrom has to be ‘null’.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "Reference to a Volume used for booting. If not ‘null’ then bootCdrom has to be ‘null’.", ValueFromPipeline = true)]
        public string BootVolume { get; set; }

        /// <summary>
        /// <para type="description">Reference to a CD-ROM used for booting. If not 'null' then bootVolume has to be 'null'.</para>
        /// </summary>
        [Parameter(Position = 6, HelpMessage = "Reference to a CD-ROM used for booting. If not 'null' then bootVolume has to be 'null'.", ValueFromPipeline = true)]
        public string BootCDRom { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var serverApi = new ServerApi(Utilities.Configuration);

                var server = new Server
                {
                    Properties = new ServerProperties
                    {
                        Name = this.Name,
                        Cores = this.Cores,
                        Ram = this.Ram
                    }
                };

                if (!string.IsNullOrEmpty(this.AvailabilityZone))
                {
                    server.Properties.AvailabilityZone = this.AvailabilityZone;
                }

                if (!string.IsNullOrEmpty(this.BootVolume))
                {
                    server.Properties.BootVolume = new ResourceReference { Id = this.BootVolume };
                }

                if (!string.IsNullOrEmpty(this.BootCDRom))
                {
                    server.Properties.BootCdrom = new ResourceReference { Id = this.BootCDRom };
                }

                var resp = serverApi.Create(this.DataCenterId, server);
                WriteObject(resp);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }

    }

    /// <summary>
    /// <para type="synopsis">This commandlet will remove the specified server from the specified virtual data center.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Remove-PBServer -DataCenterId [UUID] -ServerId [UUID]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "PBServer")]
    [OutputType(typeof(Server))]
    public class RemoveServer : Cmdlet
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

        #endregion

        protected override void BeginProcessing()
        {
            try
            {
                var serverApi = new ServerApi(Utilities.Configuration);

                var resp = serverApi.Delete(this.DataCenterId, this.ServerId);
                WriteObject(resp);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "", ErrorCategory.NotSpecified, null));
            }
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will update server properties.</para>
    /// <para type="synopsis">Only parameters passed in the commandlet will be updated.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Set-PBServer -DataCenterId [UUID] -ServerId [UUID] -Name [name]</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "PBServer")]
    [OutputType(typeof(Server))]
    public class SetServer : Cmdlet
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
        /// <para type="description">The hostname of the server.</para>
        /// </summary>
        [Parameter(Position = 2, HelpMessage = "The hostname of the server.", ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The total number of cores for the server.</para>
        /// </summary>
        [Parameter(Position = 3, HelpMessage = "The total number of cores for the server.", ValueFromPipeline = true)]
        public int Cores { get; set; }

        /// <summary>
        /// <para type="description">The amount of memory for the server in MB, e.g. 2048. Size must be specified in multiples of 256 MB with a minimum of 256 MB. However, if you set ramHotPlug to TRUE then you must use a minimum of 1024 MB.</para>
        /// </summary>
        [Parameter(Position = 4, HelpMessage = "The amount of memory for the server in MB, e.g. 2048.Size must be specified in multiples of 256 MB with a minimum of 256 MB; however, if you set ramHotPlug to TRUE then you must use a minimum of 1024 MB.", ValueFromPipeline = true)]
        public int Ram { get; set; }

        /// <summary>
        /// <para type="description">The availability zone in which the server should exist.</para>
        /// </summary>
        [Parameter(Position = 5, HelpMessage = "The availability zone in which the server should exist.", ValueFromPipeline = true)]
        public string AvailabilityZone { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            var serverApi = new ServerApi(Utilities.Configuration);

            var server = new ServerProperties();

            if (!string.IsNullOrEmpty(this.Name) && !string.IsNullOrWhiteSpace(this.Name))
            {
                server.Name = this.Name;
            }
            if (this.Cores != 0)
            {
                server.Cores = this.Cores;
            }
            if (Ram != 0)
            {
                server.Ram = this.Ram;
            }
            if (!string.IsNullOrEmpty(this.AvailabilityZone) && !string.IsNullOrWhiteSpace(this.AvailabilityZone))
            {
                server.AvailabilityZone = this.AvailabilityZone;
            }

            var resp = serverApi.PartialUpdate(this.DataCenterId, this.ServerId, server);
            WriteObject(resp);
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will reboot the server instance.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Stop-PBServer -DataCenterId [UUID] -ServerId [UUID]</para>
    /// </example>
    [Cmdlet("Reboot", "PBServer")]
    [OutputType(typeof(Server))]
    public class RebootServer : Cmdlet
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

        #endregion

        protected override void BeginProcessing()
        {
            var serverApi = new ServerApi(Utilities.Configuration);


            var err = serverApi.Reboot(this.DataCenterId, this.ServerId);

            if (err != null)
            {
                WriteObject(err);
                return;
            }
            WriteObject("Server rebooting...");
        }
    }

    /// <summary>
    /// <para type="synopsis">This commandlet will start a server instance.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Start-PBServer -DataCenterId [UUID] -ServerId [UUID]</para>
    /// </example>
    [Cmdlet("Start", "PBServer")]
    [OutputType(typeof(Server))]
    public class StartServer : Cmdlet
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

        #endregion

        protected override void BeginProcessing()
        {
            var serverApi = new ServerApi(Utilities.Configuration);

            var err = serverApi.Start(this.DataCenterId, this.ServerId);

            if (err != null)
            {
                WriteObject(err);
                return;
            }
            WriteObject("Server starting...");
        }
    }
    /// <summary>
    /// <para type="synopsis">This commandlet will stop a server instance.</para>
    /// </summary>
    /// <example>
    /// <para type="description">Stop-PBServer -DataCenterId [UUID] -ServerId [UUID]</para>
    /// </example>
    [Cmdlet("Stop", "PBServer")]
    [OutputType(typeof(Server))]
    public class StopServer : Cmdlet
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

        #endregion

        protected override void BeginProcessing()
        {
            var serverApi = new ServerApi(Utilities.Configuration);


            var err = serverApi.Stop(this.DataCenterId, this.ServerId);

            if (err != null)
            {
                WriteObject(err);
                return;
            }
            WriteObject("Server stopping...");
            return;
        }
    }
}
