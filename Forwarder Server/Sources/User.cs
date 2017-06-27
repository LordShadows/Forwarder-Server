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
        private String USERLOGIN;
        private String USERROLE;
        private String USERID;
        private String USERSNAPPING;

        private bool AUTHSUCCESS = false;

        private Cryptography cryptography;
        private Functions functions;

        public string UserID
        {
            get { return USERID; }
        }

        public bool AuthSuccess
        {
            get { return AUTHSUCCESS; }
        }

        public string UserName
        {
            get { return USERNAME; }
        }

        public string UserRole
        {
            get { return USERROLE; }
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
                    Functions.AddJournalEntry($"<<<<<! {Encoding.UTF8.GetString(buffer, 0, bytesReceive)}");
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

        public void Authentication(String login, String name, String role, String snapping)
        {
            USERNAME = name;
            USERLOGIN = login;
            USERROLE = role;
            USERSNAPPING = snapping;
            AUTHSUCCESS = true;
        }

        public void HandleCommand(Message message)
        {
            try
            {
                Functions.AddJournalEntry($"<<<<<Key {message.Keyword}");
                if (cryptography.GetHash(message.TextArguments) != message.Signature)
                {
                    Functions.AddJournalEntry($": __ERROR__ {USERID} {USERNAME} Сообщение повреждено.");
                    return;
                }

                switch (message.Keyword)
                {
                    case "AuthenticationAttempt":
                        Functions.AuthenticationAttempt(message.TextArguments[0], message.TextArguments[1], this);
                        break;
                    case "UpdateUsersData":
                        Functions.UpdateUsersData(this);
                        break;
                    case "UpdateEngineersData":
                        Functions.UpdateEngineersData(this);
                        break;
                    case "UpdateForwardersData":
                        Functions.UpdateForwardersData(this);
                        break;
                    case "UpdateAllData":
                        switch (USERROLE)
                        {
                            case "Администратор":
                                Functions.UpdateEngineersData(this);
                                Functions.UpdateForwardersData(this);
                                Functions.UpdateCompaniesData(this);
                                Functions.UpdateRequestsData(this);

                                Functions.UpdateUsersData(this);
                                break;
                            case "Инженер":
                                break;
                            case "Экспедитор":
                                break;
                            case "Руководитель экспедиторов":
                                break;
                        }
                        break;
                }
            }
            catch (NullReferenceException)
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
            catch (NullReferenceException)
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
            Functions.AddJournalEntry($": >>>>>> {Encoding.UTF8.GetBytes(cryptography.Encrypt_AES_String(json)).Length} >>> { cryptography.Encrypt_AES_String(json).Length } >>> { json.Length } >>> {json}");
            USERHANDLE.Send(Encoding.UTF8.GetBytes(cryptography.Encrypt_AES_String(json) + "$END$"));
        }

        public void Send(string Buffer)
        {
            USERHANDLE.Send(Encoding.UTF8.GetBytes(Buffer + "$END$"));
        }

        public void End()
        {
            USERHANDLE.Close();
        }
    }
}
