namespace Lascarizador.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateClients : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Clients (CPF, Name, CreditLimit, Email, Saldo) VALUES ('11122233344', 'Fred Flintstone', 3000.00, 'fred.flinstone@gmail.com', 0 )");
            Sql("INSERT INTO Clients (CPF, Name, CreditLimit, Email, Saldo) VALUES ('55566677788', 'Barney Rubble', 1500.00, 'barney.rubble@gmail.com', 0 )");
            Sql("INSERT INTO Clients (CPF, Name, CreditLimit, Email, Saldo) VALUES ('99900099900', 'Wilma Flintstone', 20000.00, 'wilma.flintstone@gmail.com', 0 )");
            Sql("INSERT INTO Clients (CPF, Name, CreditLimit, Email, Saldo) VALUES ('00099900099', 'Betty Rubble', 1000.00, 'betty.rubble@gmail.com', 0 )");
        }

        public override void Down()
        {
        }
    }
}
