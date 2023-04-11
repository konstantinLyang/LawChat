﻿using lawChat.Server.Data.Model;
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
            optionsBuilder.UseMySql("Server=188.127.239.143;User=lawchatserver;Password=cN6bV5tX4e;Database=lawchat;",
                new MySqlServerVersion(new Version(8, 0, 11)));
        }
    }
}
