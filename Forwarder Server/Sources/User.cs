using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using System.ComponentModel;


namespace Forwarder_Server.Sources
{
    class User
    {
        private Thread USERTHREAD;
        private Socket USERHANDLE;
        private String USERNAME;
        private String USERID;

        private bool AuthSuccess = false;

        private Cryptography cryptography;
        private Functions functions;

        public string UserID
        {
            get { return USERID; }
        }

        public string UserName
        {
            get { return USERNAME; }
        }

        public User(Socket handle)
        {
            USERHANDLE = handle;
            USERID = handle.RemoteEndPoint.ToString();
            USERTHREAD = new Thread(Listner)
            {
                IsBackground = true
            };
            USERTHREAD.Start();
            Server.NewUser(this);

            cryptography = new Cryptography();
            functions = new Functions();
            Send("$Directives$" + "$RSAPublicKey$" + cryptography.GenerateRSAKeys());
        }

        private void Listner()
        {
            try
            {
                while (USERHANDLE.Connected)
                {
                    byte[] buffer = new byte[4096];
                    int bytesReceive = USERHANDLE.Receive(buffer);
                    String message = Encoding.UTF8.GetString(buffer, 0, bytesReceive);
                    if (message.Contains("$Directives$"))
                        HandleDirectiveCommand(message);
                    else
                        HandleCommand(JsonConvert.DeserializeObject<Message>(cryptography.Decrypt_AES_String(message)));
                }
            }
            catch (SocketException exp)
            {
                if(exp.Message != "Удаленный хост принудительно разорвал существующее подключение")
                {
                    Functions.AddJournalEntry($": __ERROR__ {USERID} {USERNAME} Ошибка при получении сообщения: {exp.ToString()}.");
                }
                Server.EndUser(this);
            }
            catch (Exception exp)
            {
                Functions.AddJournalEntry($": __ERROR__ {USERID} {USERNAME} Ошибка при получении сообщения: {exp.ToString()}.");
                Server.EndUser(this);
            }
        }

        private bool setName(string Name) //Переписать
        {
            //Тут можно добавить различные проверки
            USERNAME = Name;
            AuthSuccess = true;
            return true;
        }

        public void HandleCommand(Message message)
        {
            try
            {
                if(cryptography.GetHash(message.TextArguments) != message.Signature)
                {
                    Functions.AddJournalEntry($": __ERROR__ {USERID} {USERNAME} Сообщение повреждено.");
                    return;
                }

                switch (message.Keyword)
                {
                    case "AuthenticationAttempt":
                        Functions.AuthenticationAttempt(message.TextArguments[0], message.TextArguments[1], this);
                        break;
                    case "Message":
                        Functions.AddJournalEntry(message.TextArguments[0]);
                        break;
                }
            }
            catch (NullReferenceException ignore)
            {
                Functions.AddJournalEntry($": __ERROR__ {USERID} {USERNAME} Утрата соединения с пользователем.");
                Server.EndUser(this);
            }
            catch (Exception exp)
            {
                Functions.AddJournalEntry($": __ERROR__ {USERID} {USERNAME} Ошибка в обработке команды пользователя: {exp.Message}");
            }
        }

        public void HandleDirectiveCommand(String message)
        {
            try
            {
                message = message.Replace("$Directives$", "");
                if (message.Contains("$AESKeys$"))
                {
                    cryptography.GetAESKey(message.Replace("$AESKeys$", ""));
                }
            }
            catch (NullReferenceException ignore)
            {
                Functions.AddJournalEntry($": __ERROR__ {USERID} {USERNAME} Утрата соединения с пользователем.");
                Server.EndUser(this);
            }
            catch (Exception exp)
            {
                Functions.AddJournalEntry($": __ERROR__ {USERID} {USERNAME} Ошибка в обработке команды пользователя: {exp.Message}");
            }
        }

        public void SendMessage(String keyword, String[] textArguments)
        {
            String signature = cryptography.GetHash(textArguments);
            Message message = new Message(keyword, textArguments, signature);
            String json = JsonConvert.SerializeObject(message);
            USERHANDLE.Send(Encoding.UTF8.GetBytes(cryptography.Encrypt_AES_String(json)));
        }

        public void Send(string Buffer)
        {
            USERHANDLE.Send(Encoding.UTF8.GetBytes(Buffer));
        }

        public void End()
        {
            USERHANDLE.Close();
        }
    }
}
