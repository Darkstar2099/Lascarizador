namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTransIDFromTransLog : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TransactionLogs", "Transaction_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransactionLogs", "Transaction_id", c => c.Int(nullable: false));
        }
    }
}
