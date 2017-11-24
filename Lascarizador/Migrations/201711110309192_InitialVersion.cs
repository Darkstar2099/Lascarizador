namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CardBrands",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Name = c.String(nullable: false),
                        ApiName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CardHolderName = c.String(nullable: false, maxLength: 255),
                        Cvv = c.Int(nullable: false),
                        Number = c.String(nullable: false, maxLength: 19),
                        ExpirationDate = c.DateTime(nullable: false),
                        CardBrandId = c.Byte(nullable: false),
                        Password = c.String(maxLength: 6),
                        HashPassword = c.Binary(),
                        SaltPassword = c.Binary(),
                        CardTypeId = c.Byte(nullable: false),
                        HasPassword = c.Boolean(nullable: false),
                        IsBlocked = c.Boolean(nullable: false),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CardBrands", t => t.CardBrandId, cascadeDelete: true)
                .ForeignKey("dbo.CardTypes", t => t.CardTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.CardBrandId)
                .Index(t => t.CardTypeId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.CardTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Name = c.String(nullable: false),
                        PasswordAvailable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CPF = c.String(nullable: false, maxLength: 11),
                        Name = c.String(nullable: false, maxLength: 255),
                        CreditLimit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Email = c.String(nullable: false),
                        Saldo = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransactionTypeId = c.Byte(nullable: false),
                        CardId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Installments = c.Int(nullable: false),
                        CreationTimestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cards", t => t.CardId, cascadeDelete: true)
                .ForeignKey("dbo.TransactionTypes", t => t.TransactionTypeId, cascadeDelete: true)
                .Index(t => t.TransactionTypeId)
                .Index(t => t.CardId);
            
            CreateTable(
                "dbo.TransactionTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Type = c.String(nullable: false),
                        InstallmentsAvailable = c.Boolean(nullable: false),
                        ApiName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "TransactionTypeId", "dbo.TransactionTypes");
            DropForeignKey("dbo.Transactions", "CardId", "dbo.Cards");
            DropForeignKey("dbo.Cards", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Cards", "CardTypeId", "dbo.CardTypes");
            DropForeignKey("dbo.Cards", "CardBrandId", "dbo.CardBrands");
            DropIndex("dbo.Transactions", new[] { "CardId" });
            DropIndex("dbo.Transactions", new[] { "TransactionTypeId" });
            DropIndex("dbo.Cards", new[] { "ClientId" });
            DropIndex("dbo.Cards", new[] { "CardTypeId" });
            DropIndex("dbo.Cards", new[] { "CardBrandId" });
            DropTable("dbo.TransactionTypes");
            DropTable("dbo.Transactions");
            DropTable("dbo.Clients");
            DropTable("dbo.CardTypes");
            DropTable("dbo.Cards");
            DropTable("dbo.CardBrands");
        }
    }
}
