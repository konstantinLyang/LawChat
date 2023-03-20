using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            optionsBuilder.UseSqlServer("Data Source=PW-421-02-21;Initial catalog=LawChatBd;Integrated Security=true;TrustServerCertificate=True;");
        }
    }
}
