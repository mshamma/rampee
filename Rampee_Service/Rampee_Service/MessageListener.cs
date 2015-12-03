using Rampee_Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rampee_Service
{
    internal class MessageListener : IDisposable
    {
        private int consumerId;
        private bool isDisposed = false;

        public MessageListener(int id, int eventId)
        {
            this.consumerId = id;
            LogClient.Record(eventId, string.Format("Listener {0} created.", id));
        }

        public void OnMessage(Apache.NMS.IMessage message)
        {
            try
            {
                var textMessage = message as Apache.NMS.ITextMessage;
                MessageRecord mr = new MessageRecord();
                mr.MessageId = textMessage.NMSMessageId;
                mr.Body = textMessage.Text;
                mr.Type = textMessage.NMSType;
                mr.TimeStamp = textMessage.NMSTimestamp;
                mr.CorrelationId = textMessage.NMSCorrelationID;
                mr.Expiration = textMessage.NMSTimeToLive;
                mr.Redelivered = textMessage.NMSRedelivered;
                
                // Not yet implemented
                //mr.Destination = textMessage.NMSDestination;
                //mr.ReplyTo = textMessage.NMSReplyTo;
                //mr.DeliveryMode = textMessage.NMSDeliveryMode;
                //mr.Priority = textMessage.NMSPriority;

                // save message to the database
                SaveMessage(mr);

                LogClient.Record(this.consumerId, string.Format("Message ID: {0} received", textMessage.NMSMessageId));
            }
            catch (Exception e)
            {
                LogClient.Record(this.consumerId, e.Message);
            }
        }

        public void SaveMessage(MessageRecord mr)
        {
            using (var rampeeContext = new RampeeDbContext())
            {
                var consumer = rampeeContext.ConsumerRecords.Where(c => c.Id == this.consumerId).FirstOrDefault();
                if (consumer.Messages.Count < 1)
                {
                    consumer.Messages = new List<MessageRecord>();
                }
                consumer.Messages.Add(mr);
                rampeeContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            this.isDisposed = true;
        }
    }
}