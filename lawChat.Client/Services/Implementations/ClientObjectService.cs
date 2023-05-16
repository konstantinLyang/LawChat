using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lawChat.Client.Services.Implementations
{
    public class ClientObjectService : IClientObject
    {
        private readonly IClientData _clientData;

        private Socket _clientSocket;

        private TcpClient _tcpClient;

        public static StreamReader StreamReader;

        public static StreamWriter StreamWriter;

        public static NetworkStream NetworkStream;

        public ClientObjectService(IClientData clientData)
        {
            _clientData = clientData;
        }

        public string OpenConnection(string login, string password)
        {
            try
            {
                using _tcpClient


                _clientSoc = new TcpClient();
                
                _clientSoc.Connect(IPAddress.Parse("10.10.11.47"), 8080);

                StreamReader = new StreamReader(_clientSoc.GetStream());
                
                StreamWriter = new StreamWriter(_clientSoc.GetStream());

                NetworkStream = _clientSoc.GetStream();

                StreamWriter.AutoFlush = true;

                return Authorization(login, password);
            }
            catch
            {
                return "server error";
            }
        }
        private string Authorization(string login, string password)
        {
            SendToServer($"{login};{password};");
            
            return GetMessageFromServer();
        }

        private void SendToServer(string message)
        {
            StreamWriter.WriteLine(message);
        }
        public void SendPrivateTextMessage(int recipient, string message)
        {
            SendToServer($"message;PRIVATE;text;{recipient};{message};");
        }
        public void SendPrivateFileMessage(int recipient, string filePath, string fileName)
        {
            Stream FileStream = File.OpenRead(filePath);

            byte[] FileBuffer = new byte[FileStream.Length];

            FileStream.Read(FileBuffer, 0, (int)FileStream.Length);

            NetworkStream.Write(FileBuffer, 0, FileBuffer.GetLength(0));

            NetworkStream.Close();
        }
        public void SendServerCommandMessage(string commandMessage)
        {
            SendToServer($"command;{commandMessage}");
        }
        public string GetMessageFromServer()
        {
            var result = StreamReader.ReadLine();

            return result;
        }
    }
}
