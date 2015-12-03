using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rampee_Data.Models
{
    // Poco for containing Jms Messages from ActiveMQ
    // http://activemq.apache.org/activemq-message-properties.html
    public class MessageRecord
    {
        // unique identifier for the message
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string MessageId { get; set; }

        [Column(TypeName = "ntext")]
        public string Body { get; set; }

        // Destination used by the producer
        public string Destination { get; set; }

        // user defined
        public string ReplyTo { get; set; }

        // user defined
        public string Type { get; set; }

        // indicator if messages should be persisted
        public int DeliveryMode { get; set; }

        // value from 0-9
        public MessagePriority Priority { get; set; }

        // time in milliseconds
        public DateTime TimeStamp { get; set; }

        // user defined
        public string CorrelationId { get; set; }

        // time in milliseconds to expire the message - 0 means never expire
        public TimeSpan Expiration { get; set; }

        // true if the message is being resent to the consumer, persisted via persistJMSRedelivered
        public bool Redelivered { get; set; }
    }
}