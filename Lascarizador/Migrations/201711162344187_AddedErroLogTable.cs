namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedErroLogTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Error_code = c.Int(nullable: false),
                        Error_message = c.String(),
                        TransactionLogId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TransactionLogs", t => t.TransactionLogId, cascadeDelete: true)
                .Index(t => t.TransactionLogId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ErrorLogs", "TransactionLogId", "dbo.TransactionLogs");
            DropIndex("dbo.ErrorLogs", new[] { "TransactionLogId" });
            DropTable("dbo.ErrorLogs");
        }
    }
}
