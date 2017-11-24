using Lascarizador.Core.Models;
using System;
using System.Data.Entity;
using System.Data.Common;
using System.Linq;

namespace Lascarizador
{
    public class LascarizadorDbContext : DbContext
    {
        // Your context has been configured to use a 'LascarizadorDbContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Lascarizador.LascarizadorDbContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'LascarizadorDbContext' 
        // connection string in the application configuration file.

        public LascarizadorDbContext() : base("name=LascarizadorDbContext") { }
        public LascarizadorDbContext(DbConnection connection) : base(connection, true) { }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardBrand> CardBrands { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        public static LascarizadorDbContext Create()
        {
            return new LascarizadorDbContext();
        }
    }
    
}