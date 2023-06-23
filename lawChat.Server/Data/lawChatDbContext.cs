using LawChat.Server.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace LawChat.Server.Data
{
    public class LawChatDbContext : DbContext
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<User> Clients { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Model.File> Files { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;User=root;Password=Joinappbrother337;Database=lawchat;",
                new MySqlServerVersion(new Version(8, 0, 11)));
        }
    }
}
