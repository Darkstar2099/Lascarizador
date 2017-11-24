namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateCardTypes : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO CardTypes (Id, Name, PasswordAvailable) VALUES (1, 'Chip', 0)");
            Sql("INSERT INTO CardTypes (Id, Name, PasswordAvailable) VALUES (2, 'Tarja Magnética', 1)");
        }

        public override void Down()
        {
        }
    }
}
