using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Rampee_Service
{
    public partial class RampeeService : ServiceBase
    {
        int eventId = 0;

        public RampeeService()
        {
            InitializeComponent();
        }

        #region Member methods
        protected override void OnStart(string[] args)
        {
            try
            {
                ServiceStatus serviceStatus = new ServiceStatus()
                {
                    dwCurrentState = ServiceState.SERVICE_START_PENDING,
                    dwWaitHint = 100000
                };
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                // Inspect Connection and protect if necessary
                ProtectConnection();

                // Load consumers
                LoadConsumersAsync();

            }
            catch (Exception e)
            {

            }
        }

        private void LoadConsumersAsync()
        {
            throw new NotImplementedException();
        }

        private void ProtectConnection()
        {
            eventId++;
            LogClient.Record(eventId, "Data protection check.");
            try
            {
                using (JmsDbContext jmsContext = new JmsDbContext())
                {
                    var connections = jmsContext.ConnectionRecords.ToList();
                    if (connections.Count() > 0)
                    {
                        foreach (ConnectionRecord cr in connections)
                        {
                            if (!String.IsNullOrEmpty(cr.ClearPass))
                            {
                                if (cr.Salt == null)
                                {
                                    cr.Salt = DataProtection.GetEntropy();
                                }
                                byte[] clearPassBytes = Encoding.UTF8.GetBytes(cr.ClearPass);
                                cr.Password = DataProtection.Protect(clearPassBytes, cr.Salt);
                                cr.ClearPass = null;
                            }
                        }
                    }
                    else
                    {
                        LogClient.Record(eventId, "No connections to protect.");
                    }
                    jmsContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogClient.Record(eventId, string.Format("Error on event {0}: {1}", eventId, ex.Message, ex.InnerException));
            }
        }

        protected override void OnStop()
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);


            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }
        #endregion

        #region System level boiler plate
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };
        #endregion
    }
}
