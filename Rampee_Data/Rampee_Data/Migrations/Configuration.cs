namespace Rampee_Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Security.Cryptography;

    internal sealed class Configuration : DbMigrationsConfiguration<Rampee_Data.Models.RampeeDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Rampee_Data.Models.RampeeDbContext context)
        {
            var createdDate = DateTime.Now;
            // connection records
            var conn = new Rampee_Data.Models.ConnectionRecord()
            {
                Id = 1001,
                Username = "admin",
                CreatedOn = createdDate,
                Salt = GetEntropy(),
                ClearPass = string.Format("{0}|{2}|{1}", "admin", createdDate, "password"),
                Password = null,
                Uri = "tcp://localhost:61616"
            };
            context.ConnectionRecords.Add(conn);

            // consumer records
            var consumer1 = new Rampee_Data.Models.ConsumerRecord()
            {
                Id = 3001,
                Type = null,
                Destination = "ActiveMQ.Advisory.Consumer.Queue.TestQ2",
                Active = true,
                Connection = conn
            };
            var consumer2 = new Rampee_Data.Models.ConsumerRecord()
            {
                Id = 3002,
                Type = null,
                Destination = "ActiveMQ.Advisory.Consumer.Queue.TestQ3",
                Active = true,
                Connection = conn
            };

            conn.ConsumerRecords = new List<Models.ConsumerRecord>();
            conn.ConsumerRecords.Add(consumer1);
            conn.ConsumerRecords.Add(consumer2);

            // message records
            var message = new Rampee_Data.Models.MessageRecord()
            {
                MessageId = "M-2-asdfasf-1234123412341234",
                Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer posuere lorem nisl, et sodales purus tincidunt id. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Aliquam sed neque lectus. Integer et urna pretium augue eleifend volutpat. Aliquam malesuada libero quis imperdiet dapibus. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Proin libero dui, posuere pulvinar lorem scelerisque, commodo porta lectus. Maecenas dignissim lacus sem, sed fermentum justo vehicula vel. Morbi diam nulla, sagittis scelerisque metus non, luctus tincidunt orci. Duis quam mi, efficitur nec dignissim a, porta a magna. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. In id facilisis metus. Suspendisse eu iaculis ipsum, sed pellentesque nisi.Nullam a auctor augue,"
                + "vel venenatis dui.Aliquam erat volutpat.Aenean sit amet mauris sit amet dui finibus sagittis ac ut felis.Nunc in congue felis.Cras et vulputate felis,"
                + "elementum lacinia quam.Vivamus id mattis ipsum.Ut consectetur suscipit leo sed consectetur.Nulla dolor metus,"
                + "faucibus sed lobortis vitae,"
                + "semper nec neque.Mauris ac commodo diam.Sed fringilla felis at leo tincidunt,"
                + "nec suscipit ligula lacinia.Mauris id vestibulum tellus,"
                + "vel dignissim erat. nibh odio tincidunt augue, ut hendrerit dui nunc at nisi.Suspendisse molestie auctor leo, in commodo ex congue in. Suspendisse potenti.Aliquam hendrerit metus vel consectetur facilisis.Ut ornare egestas arcu ut egestas.Phasellus bibendum nibh vitae purus condimentum pellentesque.Donec fringilla lacinia euismod."
                + "vel venenatis dui.Aliquam erat volutpat.Aenean sit amet mauris sit amet dui finibus sagittis ac ut felis.Nunc in congue felis.Cras et vulputate felis,"
                + "elementum lacinia quam.Vivamus id mattis ipsum.Ut consectetur suscipit leo sed consectetur.Nulla dolor metus,"
                + "faucibus sed lobortis vitae,"
                + "semper nec neque.Mauris ac commodo diam.Sed fringilla felis at leo tincidunt,"
                + "nec suscipit ligula lacinia.Mauris id vestibulum tellus,"
                + "vel dignissim erat. nibh odio tincidunt augue, ut hendrerit dui nunc at nisi.Suspendisse molestie auctor leo, in commodo ex congue in. Suspendisse potenti.Aliquam hendrerit metus vel consectetur facilisis.Ut ornare egestas arcu ut egestas.Phasellus bibendum nibh vitae purus condimentum pellentesque.Donec fringilla lacinia euismod."
                + "vel venenatis dui.Aliquam erat volutpat.Aenean sit amet mauris sit amet dui finibus sagittis ac ut felis.Nunc in congue felis.Cras et vulputate felis,"
                + "elementum lacinia quam.Vivamus id mattis ipsum.Ut consectetur suscipit leo sed consectetur.Nulla dolor metus,"
                + "faucibus sed lobortis vitae,"
                + "semper nec neque.Mauris ac commodo diam.Sed fringilla felis at leo tincidunt,"
                + "nec suscipit ligula lacinia.Mauris id vestibulum tellus,"
                + "vel dignissim erat. nibh odio tincidunt augue, ut hendrerit dui nunc at nisi.Suspendisse molestie auctor leo, in commodo ex congue in. Suspendisse potenti.Aliquam hendrerit metus vel consectetur facilisis.Ut ornare egestas arcu ut egestas.Phasellus bibendum nibh vitae purus condimentum pellentesque.Donec fringilla lacinia euismod.",
                DeliveryMode = 1,
                CorrelationId = "correlation0001",
                Destination = "destination",
                Expiration = System.DateTime.Now - System.DateTime.Now.Date,
                TimeStamp = System.DateTime.Now,
                ReplyTo = "reply-to-001",
                Priority = Models.MessagePriority.AboveLow,
                Redelivered = true
            };

            consumer1.Messages = new List<Models.MessageRecord>();
            consumer1.Messages.Add(message);

            context.SaveChanges();
        }

        public static byte[] Protect(byte[] data, byte[] salt)
        {
            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                return ProtectedData.Protect(data, salt, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not encrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        internal static byte[] GetEntropy()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[20];
            rnd.NextBytes(b);
            return b;
        }

    }
}
