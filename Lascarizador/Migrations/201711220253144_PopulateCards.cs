namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateCards : DbMigration
    {
        public override void Up()
        {
            // Cartão normal
            Sql("INSERT INTO Cards (CardHolderName, Cvv, Number, ExpirationDate, CardBrandId, Password, HashPassword, SaltPassword, CardTypeId, HasPassword, IsBlocked, ClientId) VALUES ('FREDERICO FLINTSTONE', 111, '1111222233334444', datetime(), 1, '123456', 'hash', 'salt', 2, true, false, 1)");
            // Cartão Bloqueado
            Sql("INSERT INTO Cards (CardHolderName, Cvv, Number, ExpirationDate, CardBrandId, Password, HashPassword, SaltPassword, CardTypeId, HasPassword, IsBlocked, ClientId) VALUES ('FREDERICO FLINTSTONE', 222, '5555666677778888', datetime(), 2, '222222', 'hash', 'salt', 2, true, true, 1)");
            // Cartão Expirado (Val:Jan/2017)
            Sql("INSERT INTO Cards (CardHolderName, Cvv, Number, ExpirationDate, CardBrandId, Password, HashPassword, SaltPassword, CardTypeId, HasPassword, IsBlocked, ClientId) VALUES ('FREDERICO FLINTSTONE', 333, '9999888899998888', datetime(1/1/2017), 3, '666666', 'hash', 'salt', 2, true, false, 1)");
        }

        public override void Down()
        {
        }
    }
}
