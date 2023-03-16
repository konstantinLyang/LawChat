﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using lawChat.Server.Model;

IPEndPoint endPoint = new(IPAddress.Any, 5555);
Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

List<Client> clientList = new();

serverSocket.Bind(endPoint);

while (true)
{
    serverSocket.Listen(5);

    Task.Factory.StartNew(() =>
    {
        Client client = new();

        client.Socket = serverSocket.Accept();

        clientList.Add(client);

        var buffer = new byte[4026];
        int size = 0;
        var receiveMessage = new StringBuilder();

        size = client.Socket.Receive(buffer);

        client.Name = receiveMessage.Append(Encoding.Unicode.GetString(buffer, 0, size)).ToString();

        Console.WriteLine(client.Name + " подключился");

        client.Socket.Send(Encoding.Unicode.GetBytes("Успешное подключение!"));

        try
        {
            while (client.Socket.Connected)
            {
                size = client.Socket.Receive(buffer);
                receiveMessage = new StringBuilder(Encoding.Unicode.GetString(buffer, 0, size));

                Console.WriteLine(client.Name + ": " + receiveMessage);
            }
        }
        catch
        {
            Console.WriteLine(client.Name + ": разрыв соединения");
        }

        client.Socket.Shutdown(SocketShutdown.Both);
        client.Socket.Close();

        clientList.Remove(client);
    });
    Thread.Sleep(100);
}