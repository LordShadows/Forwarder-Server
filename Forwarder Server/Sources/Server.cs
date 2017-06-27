using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Forwarder_Server.Sources
{
    class Server
    {
        public static int countUsers = 0;
        public static List<User> USERSLIST = new List<User>();
        public static Socket SERVERSOCKET;
        public const String HOST = "127.0.0.1";
        public const int PORT = 22222;

        public static bool Work = true;

        public static void StartServer()
        {
            IPAddress address = IPAddress.Parse(HOST);
            SERVERSOCKET = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            SERVERSOCKET.Bind(new IPEndPoint(address, PORT));
            SERVERSOCKET.Listen(100);
            Functions.AddJournalEntry($": __SERVER__ Сервер готов к работе. Хост/порт: {HOST}:{PORT}");
            DatabaseWork.Test();
            while (Work)
            {
                Socket handle = SERVERSOCKET.Accept();
                new User(handle);

            }
            Functions.AddJournalEntry(": __SERVER__ Сервер завершил работу.");
        }

        public static void NewUser(User user)
        {
            if (USERSLIST.Contains(user))
                return;
            USERSLIST.Add(user);
            Functions.AddJournalEntry($": {user.UserID} Пользователь подключился.");
            countUsers++;
        }

        public static void EndUser(User user)
        {
            if (!USERSLIST.Contains(user))
                return;
            USERSLIST.Remove(user);
            user.End();
            Functions.AddJournalEntry($": {user.UserID} {user.UserName} Пользователь отключился.");
            countUsers--;
        }

        public static User GetUser(string ID)
        {
            for (int i = 0; i < countUsers; i++)
            {
                if (USERSLIST[i].UserID == ID)
                    return USERSLIST[i];
            }
            return null;
        }

        public static void SendGlobalMessage(String text) 
        {
            for (int i = 0; i < countUsers; i++)
            {
                USERSLIST[i].SendMessage("GlobalMessage", new String[] { text });
            }
        }

        public static void Shutdown()
        {
            Work = false;
            for (int i = 0; i < countUsers; i++)
            {
                USERSLIST[i].End();
            }
        }
    }
}
