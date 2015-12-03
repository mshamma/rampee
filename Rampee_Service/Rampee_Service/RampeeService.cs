using Apache.NMS.ActiveMQ;
using Rampee_Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
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
            // Define the cancellation token.
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            List<Task> tasks = new List<Task>();
            TaskFactory factory = new TaskFactory(token);

            eventId++;
            LogClient.Record(eventId, string.Format("Interval started, launching listeners."));
            try
            {
                using (RampeeDbContext rampeeContext = new RampeeDbContext())
                {
                    var consumers = rampeeContext.ConsumerRecords.Where(b => b.Active).ToList();
                    if (consumers.Count() > 0)
                    {
                        foreach (ConsumerRecord cr in consumers)
                        {
                            var brokerUri = cr.Connection.Uri;
                            // construct the client ID from the hostname and the Rampee Db 
                            // consumer ID 
                            var clientId = string.Format("{0}{1}", Dns.GetHostName(), cr.Id);
                            var userName = cr.Connection.Username;
                            var password = GetSecret(cr.Connection);
                            var dest = cr.Destination;
                            var cf = new ConnectionFactory(brokerUri, clientId);
                            Apache.NMS.ISession session;
                            Apache.NMS.IConnection connection = cf.CreateConnection(userName, password);
                            connection.Start();
                            session = connection.CreateSession();
                            var ml = new MessageListener(cr.Id, eventId);
                            var tps = new TopicSubscriber(session, dest);

                            tasks.Add(factory.StartNew(() =>
                            {
                                tps.Start(clientId);
                                tps.OnMessageReceived += m => ml.OnMessage(m);
                            }, token));

                            LogClient.Record(eventId, string.Format("Consumer {0} started.", clientId));
                        }
                    }
                    else
                    {
                        LogClient.Record(eventId, string.Format("No queue records found."));
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Record(eventId, string.Format("Error on event {0}: {1}", eventId, ex.Message, ex.InnerException));
            }
        }

        private string GetSecret(ConnectionRecord cn)
        {
            var secretBytes = DataProtection.Unprotect(cn.Password, cn.Salt);
            var secretText = Encoding.UTF8.GetString(secretBytes);
            var secrets = secretText.Split('|');
            return secrets[0];
        }

        private void ProtectConnection()
        {
            eventId++;
            LogClient.Record(eventId, "Data protection check.");
            try
            {
                // load connection information from the database
                using (RampeeDbContext rampeeContext = new RampeeDbContext())
                {
                    var connections = rampeeContext.ConnectionRecords.ToList();
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
                    rampeeContext.SaveChanges();
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
