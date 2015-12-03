namespace Rampee_Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RampeeDbContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConnectionRecord",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Uri = c.String(),
                        Username = c.String(maxLength: 50),
                        Password = c.Binary(),
                        ClearPass = c.String(maxLength: 50),
                        Salt = c.Binary(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConsumerRecord",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Type = c.String(),
                        Destination = c.String(),
                        Active = c.Boolean(nullable: false),
                        Connection_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConnectionRecord", t => t.Connection_Id)
                .Index(t => t.Connection_Id);
            
            CreateTable(
                "dbo.MessageRecord",
                c => new
                    {
                        MessageId = c.String(nullable: false, maxLength: 128),
                        Body = c.String(storeType: "ntext"),
                        Destination = c.String(),
                        ReplyTo = c.String(),
                        Type = c.String(),
                        DeliveryMode = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                        CorrelationId = c.String(),
                        Expiration = c.Time(nullable: false, precision: 7),
                        Redelivered = c.Boolean(nullable: false),
                        ConsumerRecord_Id = c.Int(),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.ConsumerRecord", t => t.ConsumerRecord_Id)
                .Index(t => t.ConsumerRecord_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageRecord", "ConsumerRecord_Id", "dbo.ConsumerRecord");
            DropForeignKey("dbo.ConsumerRecord", "Connection_Id", "dbo.ConnectionRecord");
            DropIndex("dbo.MessageRecord", new[] { "ConsumerRecord_Id" });
            DropIndex("dbo.ConsumerRecord", new[] { "Connection_Id" });
            DropTable("dbo.MessageRecord");
            DropTable("dbo.ConsumerRecord");
            DropTable("dbo.ConnectionRecord");
        }
    }
}
