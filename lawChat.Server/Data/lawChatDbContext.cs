using lawChat.Server.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace lawChat.Server.Data
{
    public class LawChatDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=188.127.239.144;User=user615945;Password=R7P-tfQ-mFW-bu9;Database=lawchatdb;",
                new MySqlServerVersion(new Version(8, 0, 11)));
        }
    }
}
