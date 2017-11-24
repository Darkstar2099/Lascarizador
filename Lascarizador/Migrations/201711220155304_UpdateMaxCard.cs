namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMaxCard : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cards", "Number", c => c.String(nullable: false, maxLength: 16));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cards", "Number", c => c.String(nullable: false, maxLength: 19));
        }
    }
}
