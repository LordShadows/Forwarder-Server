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
        public Functions functions;

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

        public string UserSnapping
        {
            get { return USERSNAPPING; }
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
                if (cryptography.GetHash(message.TextArguments) != message.Signature)
                {
                    Functions.AddJournalEntry($": __ERROR__ {USERID} {USERNAME} Сообщение повреждено.");
                    return;
                }

                switch (message.Keyword)
                {
                    case "AuthenticationAttempt":
                        functions.AuthenticationAttempt(message.TextArguments[0], message.TextArguments[1], this);
                        break;
                    case "UpdateUsersData":
                        functions.UpdateUsersData(this);
                        break;
                    case "UpdateEngineersData":
                        functions.UpdateEngineersData(this);
                        break;
                    case "UpdateForwardersData":
                        functions.UpdateForwardersData(this);
                        break;
                    case "AddRequest":
                        functions.AddRequest(message.TextArguments[0], this);
                        break;
                    case "AddCompany":
                        functions.AddCompany(message.TextArguments[0], this);
                        break;
                    case "UpdateCompany":
                        functions.UpdateCompany(message.TextArguments[0], this);
                        break;
                    case "DeleteCompany":
                        functions.DeleteCompany(message.TextArguments[0], this);
                        break;
                    case "AddForwarder":
                        functions.AddForwarder(message.TextArguments[0], this);
                        break;
                    case "UpdateForwarder":
                        functions.UpdateForwarder(message.TextArguments[0], this);
                        break;
                    case "DeleteForwarder":
                        functions.DeleteForwarder(message.TextArguments[0], this);
                        break;
                    case "AddEngineer":
                        functions.AddEngineer(message.TextArguments[0], this);
                        break;
                    case "UpdateEngineer":
                        functions.UpdateEngineer(message.TextArguments[0], this);
                        break;
                    case "DeleteEngineer":
                        functions.DeleteEngineer(message.TextArguments[0], this);
                        break;
                    case "AddUser":
                        functions.AddUser(message.TextArguments[0], this);
                        break;
                    case "UpdateUser":
                        functions.UpdateUser(message.TextArguments[0], this);
                        break;
                    case "DeleteUser":
                        functions.DeleteUser(message.TextArguments[0], this);
                        break;
                    case "AddRoute":
                        functions.AddRoute(this);
                        break;
                    case "DeleteRoute":
                        functions.DeleteRoute(message.TextArguments[0], this);
                        break;
                    case "UpdateRoute":
                        functions.UpdateRoute(message.TextArguments[0], this);
                        break;
                    case "ChangeRouteStatus":
                        functions.ChangeRouteStatus(message.TextArguments[0],  message.TextArguments[1], this);
                        break;
                    case "UpdateDestination":
                        functions.UpdateDestination(message.TextArguments[0], this);
                        break;
                    case "ChangeDestinationNumber":
                        functions.ChangeDestinationNumber(message.TextArguments[0], message.TextArguments[1], this);
                        break;
                    case "DeleteDestination":
                        functions.DeleteDestination(message.TextArguments[0], this);
                        break;
                    case "RequestDistribute":
                        functions.RequestDistribute(message.TextArguments[0], message.TextArguments[1], this);
                        break;
                    case "DeleteRequest":
                        functions.DeleteRequest(message.TextArguments[0], this);
                        break;
                    case "UpdateRequest":
                        functions.UpdateRequest(message.TextArguments[0], this);
                        break;
                    case "UpdateAllData":
                        switch (USERROLE)
                        {
                            case "Администратор":
                                functions.UpdateEngineersData(this);
                                functions.UpdateForwardersData(this);
                                functions.UpdateCompaniesData(this);
                                functions.UpdateRequestsData(this);
                                functions.UpdateDestinationsData(this);
                                functions.UpdateRoutesData(this);
                                functions.UpdateUsersData(this);
                                break;
                            case "Инженер":
                                functions.UpdateEngineersData(this);
                                functions.UpdateForwardersData(this);
                                functions.UpdateCompaniesData(this);
                                functions.UpdateRequestsData(this);
                                functions.UpdateDestinationsData(this);
                                functions.UpdateRoutesData(this);
                                break;
                            case "Экспедитор":
                                functions.UpdateEngineersData(this);
                                functions.UpdateForwardersData(this);
                                functions.UpdateCompaniesData(this);
                                functions.UpdateRequestsData(this);
                                functions.UpdateDestinationsData(this);
                                functions.UpdateRoutesData(this);
                                break;
                            case "Руководитель экспедиторов":
                                functions.UpdateEngineersData(this);
                                functions.UpdateForwardersData(this);
                                functions.UpdateCompaniesData(this);
                                functions.UpdateRequestsData(this);
                                functions.UpdateDestinationsData(this);
                                functions.UpdateRoutesData(this);
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
