namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriaTabelaTransactionLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Transaction_id = c.Int(nullable: false),
                        Transaction_type = c.String(),
                        Card_brand = c.String(),
                        Card_number_first = c.String(),
                        Card_number_last = c.String(),
                        Card_holder_name = c.String(),
                        Amount = c.Int(nullable: false),
                        Installments = c.Int(nullable: false),
                        Creation_timestamp = c.DateTime(nullable: false),
                        Status_code = c.String(),
                        Status_reason = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Transactions", "TransactionLogId", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "TransactionLogId");
            AddForeignKey("dbo.Transactions", "TransactionLogId", "dbo.TransactionLogs", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "TransactionLogId", "dbo.TransactionLogs");
            DropIndex("dbo.Transactions", new[] { "TransactionLogId" });
            DropColumn("dbo.Transactions", "TransactionLogId");
            DropTable("dbo.TransactionLogs");
        }
    }
}
