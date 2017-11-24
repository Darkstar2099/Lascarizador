namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateCardBrand : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO CardBrands (Id, Name,  ApiName) VALUES (1, 'Bedrock Visa', 'bedrock_visa')");
            Sql("INSERT INTO CardBrands (Id, Name, ApiName) VALUES (2, 'Bedrock Master', 'bedrock_master')");
            Sql("INSERT INTO CardBrands (Id, Name, ApiName) VALUES (3, 'Bedrock Express', 'bedrock_express')");
        }

        public override void Down()
        {
        }
    }
}
