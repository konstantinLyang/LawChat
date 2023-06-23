﻿using System.Collections.Generic;
using LawChat.Server.Data.Model;

namespace LawChat.Client.Services
{
    public interface IClientData
    {
        public User? UserData { get; set; }

        public List<User>? FriendList { get; set; }
    }
}
