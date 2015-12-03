using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;

namespace Rampee_Service
{
    internal class TopicSubscriber : IDisposable
    {
        private readonly ISession session;
        private readonly ITopic topic;
        private readonly string destination;
        private bool disposed = false;

        public TopicSubscriber(ISession session, string destination)
        {
            this.session = session;
            this.destination = destination;
            topic = new ActiveMQTopic(this.destination);
        }

        public event MessageReceivedDelegate OnMessageReceived;

        public IMessageConsumer Consumer { get; private set; }

        public string ConsumerId { get; private set; }

        public void Start(string consumerId)
        {
            ConsumerId = consumerId;
            Consumer = session.CreateDurableConsumer(topic, consumerId, null, false);
            Consumer.Listener += (message =>
            {
                var textMessage = message as ITextMessage;
                if (textMessage == null) throw new InvalidCastException();
                if (OnMessageReceived != null)
                {
                    OnMessageReceived(textMessage);
                }
            });
        }

        public void Dispose()
        {
            if (disposed) return;
            if (Consumer != null)
            {
                Consumer.Close();
                Consumer.Dispose();
            }
            disposed = true;
        }
    }
}