using DataBase.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Data
{
    public class LawChatDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=188.127.239.143;User=ppomidorka;Password=nPi-6uk-7kR-3Y7;Database=lawchat;",
                new MySqlServerVersion(new Version(8, 0, 11)));
        }
    }
}
