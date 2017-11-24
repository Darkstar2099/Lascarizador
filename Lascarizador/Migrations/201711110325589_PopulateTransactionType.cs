namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateTransactionType : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO TransactionTypes(Id, Type, InstallmentsAvailable, ApiName) VALUES(1, 'Crédito', 0, 'credito')");
            Sql("INSERT INTO TransactionTypes(Id, Type, InstallmentsAvailable, ApiName) VALUES(2, 'Credito Parcelado', 1, 'credito_parcelado')");
        }

        public override void Down()
        {
        }
    }
}
