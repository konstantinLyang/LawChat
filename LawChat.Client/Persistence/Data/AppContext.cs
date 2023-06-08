using LawChat.Client.Persistence.Tables;
using Microsoft.EntityFrameworkCore;

namespace LawChat.Client.Persistence.Data
{
    public class AppContext : DbContext
    {
        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<File> Files => Set<File>();

        public AppContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=lawChatClientCash.db");
        }
    }
}
