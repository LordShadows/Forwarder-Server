using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Forwarder_Server.Sources
{
    class Functions
    {
        public static MainWindow MAINWINDOW;

        public static void AddJournalEntry(String entry)
        {
            MAINWINDOW.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                MAINWINDOW.lbMessages.Items.Add($"> {DateTime.Now.ToString()} {entry}");
            }));
        }

        public void AuthenticationAttempt(String login, String password, User user)
        {
            System.Data.DataTable tempTable = DatabaseWork.ExecuteQuery("SELECT * FROM [Users] WHERE Login = '" + login + "' AND Password = '" + password + "'");
            if (DatabaseWork.ExecuteQuery("SELECT * FROM [Users] WHERE Login = '" + login + "' AND Password = '" + password + "'").Rows.Count > 0)
            {
                user.SendMessage("AuthenticationAttempt", new String[] { "Yes" });
                user.SendMessage("AccountData", new String[] { login, tempTable.Rows[0]["FIO User"].ToString(), tempTable.Rows[0]["Role"].ToString(), tempTable.Rows[0]["Snapping"].ToString() });
                user.Authentication(tempTable.Rows[0]["Login"].ToString(), tempTable.Rows[0]["FIO User"].ToString(), tempTable.Rows[0]["Role"].ToString(), tempTable.Rows[0]["Snapping"].ToString());
                Functions.AddJournalEntry($": {user.UserID} {user.UserName} Авторизация. Роль: {tempTable.Rows[0]["Role"].ToString()}.");
            }
            else
            {
                user.SendMessage("AuthenticationAttempt", new String[] { "No" });
                Functions.AddJournalEntry($": {user.UserID} Неудачная попытка авторизации.");
            }
        }

        public void UpdateUsersData(User user)
        {
            if(user.AuthSuccess == true && user.UserRole == "Администратор")
            {
                System.Data.DataTable tempTable = DatabaseWork.RunProcedure("GetAllUsersData");
                List<ClassResource.User> tempList = new List<ClassResource.User>();
                for(int i = 0; i < tempTable.Rows.Count; ++i)
                {
                    tempList.Add(new ClassResource.User(
                        tempTable.Rows[i]["Login"].ToString(),
                        tempTable.Rows[i]["FIO User"].ToString(),
                        tempTable.Rows[i]["Role"].ToString(),
                        tempTable.Rows[i]["Snapping"].ToString(),
                        tempTable.Rows[i]["Engineer"].ToString(),
                        tempTable.Rows[i]["Forwarder"].ToString(),
                        null));
                }

                tempTable = DatabaseWork.RunProcedure("GetAllUsers");
                List<String> tempStringList = new List<String>();
                for (int i = 0; i < tempTable.Rows.Count; ++i)
                {
                    tempStringList.Add(tempTable.Rows[i]["NAme"].ToString().Replace("_eng_", "Инженер").Replace("_for_", "Экспедитор"));
                }

                user.SendMessage("UpdateUsersData", new String[] { JsonConvert.SerializeObject(tempList), JsonConvert.SerializeObject(tempStringList) });
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateEngineersData(User user)
        {
            if (user.AuthSuccess == true)
            {
                System.Data.DataTable tempTable = DatabaseWork.ExecuteQuery("SELECT * FROM Engineers ORDER BY Name");
                List<ClassResource.Engineer> tempList = new List<ClassResource.Engineer>();
                for (int i = 0; i < tempTable.Rows.Count; ++i)
                {
                    tempList.Add(new ClassResource.Engineer(
                        tempTable.Rows[i]["ID engineer"].ToString(),
                        tempTable.Rows[i]["Name"].ToString(),
                        tempTable.Rows[i]["Сontact number"].ToString(),
                        tempTable.Rows[i]["Note"].ToString()));
                }

                user.SendMessage("UpdateEngineersData", new String[] { JsonConvert.SerializeObject(tempList) });
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateForwardersData(User user)
        {
            if (user.AuthSuccess == true)
            {
                System.Data.DataTable tempTable = DatabaseWork.ExecuteQuery("SELECT * FROM Forwarders ORDER BY Name");
                List<ClassResource.Forwarder> tempList = new List<ClassResource.Forwarder>();
                for (int i = 0; i < tempTable.Rows.Count; ++i)
                {
                    tempList.Add(new ClassResource.Forwarder(
                        tempTable.Rows[i]["ID forwarder"].ToString(),
                        tempTable.Rows[i]["Name"].ToString(),
                        tempTable.Rows[i]["Contact number"].ToString(),
                        tempTable.Rows[i]["Note"].ToString()));
                }

                user.SendMessage("UpdateForwardersData", new String[] { JsonConvert.SerializeObject(tempList) });
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateCompaniesData(User user)
        {
            if (user.AuthSuccess == true)
            {
                System.Data.DataTable tempTable = DatabaseWork.ExecuteQuery("SELECT * FROM Companies ORDER BY Name");
                List<ClassResource.Company> tempList = new List<ClassResource.Company>();
                for (int i = 0; i < tempTable.Rows.Count; ++i)
                {
                    tempList.Add(new ClassResource.Company(
                        tempTable.Rows[i]["ID company"].ToString(),
                        tempTable.Rows[i]["Name"].ToString(),
                        tempTable.Rows[i]["Country"].ToString(),
                        tempTable.Rows[i]["City"].ToString(),
                        tempTable.Rows[i]["Address"].ToString(),
                        tempTable.Rows[i]["Name contact person"].ToString(),
                        tempTable.Rows[i]["Phone contact person"].ToString()));
                }

                user.SendMessage("UpdateCompaniesData", new String[] { JsonConvert.SerializeObject(tempList) });
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateRequestsData(User user)
        {
            if (user.AuthSuccess == true)
            {
                System.Data.DataTable tempTable = DatabaseWork.ExecuteQuery("SELECT * FROM Requests ORDER BY Date DESC");
                List<ClassResource.Request> tempList = new List<ClassResource.Request>();
                for (int i = 0; i < tempTable.Rows.Count; ++i)
                {
                    tempList.Add(new ClassResource.Request(
                        tempTable.Rows[i]["ID request"].ToString(),
                        tempTable.Rows[i]["Number"].ToString(),
                        tempTable.Rows[i]["Product name"].ToString(),
                        tempTable.Rows[i]["Product weight"].ToString(),
                        tempTable.Rows[i]["Product dimensions"].ToString(),
                        tempTable.Rows[i]["Quantity"].ToString(),
                        tempTable.Rows[i]["ID company"].ToString(),
                        tempTable.Rows[i]["ID engineer"].ToString(),
                        tempTable.Rows[i]["Note"].ToString(),
                        tempTable.Rows[i]["Date"].ToString()));
                }

                user.SendMessage("UpdateRequestsData", new String[] { JsonConvert.SerializeObject(tempList) });
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateDestinationsData(User user)
        {
            if (user.AuthSuccess == true)
            {
                System.Data.DataTable tempTable = DatabaseWork.ExecuteQuery("SELECT * FROM [Destinations]");
                List<ClassResource.Destination> tempList = new List<ClassResource.Destination>();
                for (int i = 0; i < tempTable.Rows.Count; ++i)
                {
                    tempList.Add(new ClassResource.Destination(
                        tempTable.Rows[i]["ID destination"].ToString(),
                        tempTable.Rows[i]["Arrival date"].ToString(),
                        tempTable.Rows[i]["Note"].ToString(),
                        tempTable.Rows[i]["ID route"].ToString(),
                        tempTable.Rows[i]["ID request"].ToString(),
                        tempTable.Rows[i]["Number"].ToString()));
                }

                user.SendMessage("UpdateDestinationsData", new String[] { JsonConvert.SerializeObject(tempList) });
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateRoutesData(User user)
        {
            if (user.AuthSuccess == true)
            {
                System.Data.DataTable tempTable = DatabaseWork.ExecuteQuery("SELECT * FROM Routes ORDER BY [Route status] DESC, [Departure date], [Name]");
                List<ClassResource.Route> tempList = new List<ClassResource.Route>();
                for (int i = 0; i < tempTable.Rows.Count; ++i)
                {
                    tempList.Add(new ClassResource.Route(
                        tempTable.Rows[i]["ID route"].ToString(),
                        tempTable.Rows[i]["Name"].ToString(),
                        tempTable.Rows[i]["Departure date"].ToString(),
                        tempTable.Rows[i]["Car type"].ToString(),
                        tempTable.Rows[i]["Return date"].ToString(),
                        tempTable.Rows[i]["Route status"].ToString(),
                        tempTable.Rows[i]["City / Country departure"].ToString(),
                        tempTable.Rows[i]["Note"].ToString(),
                        tempTable.Rows[i]["ID forwarder"].ToString()));
                }

                user.SendMessage("UpdateRoutesData", new String[] { JsonConvert.SerializeObject(tempList) });
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void AddRequest(String stringRequest, User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Инженер")
            {
                ClassResource.Request request = JsonConvert.DeserializeObject<ClassResource.Request>(stringRequest);
                DatabaseWork.ExecuteUpdate("INSERT INTO Requests ([Number], [Product name], [Product weight], [Product dimensions], [Quantity], [ID company], [ID engineer], [Note]) VALUES (" +
                    "N'" + request.Number + "'," +
                    "N'" + request.ProductName +"'," +
                    "N'" + request.ProductWeight +"'," +
                    "N'" + request.ProductDimensions +"'," +
                    "'" + request.Quantity +"'," +
                    "'" + request.IDCompany +"'," +
                    "'" + user.UserSnapping +"'," +
                    "N'" + request.Note +"')");
                Server.UpdateRequests();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void AddCompany(String stringCompany, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Инженер" || user.UserRole == "Администратор"))
            {
                ClassResource.Company company = JsonConvert.DeserializeObject<ClassResource.Company>(stringCompany);
                DatabaseWork.ExecuteUpdate("INSERT INTO [Companies] ([Name], [Country], [City], [Address], [Name contact person], [Phone contact person]) VALUES (" +
                    "N'" + company.Name + "'," +
                    "N'" + company.Country + "'," +
                    "N'" + company.City + "'," +
                    "N'" + company.Address + "'," +
                    "N'" + company.NameСontactPerson + "'," +
                    "N'" + company.PhoneContactPerson + "')");
                Server.UpdateCompanies();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateCompany(String stringCompany, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Инженер" || user.UserRole == "Администратор"))
            {
                ClassResource.Company company = JsonConvert.DeserializeObject<ClassResource.Company>(stringCompany);
                DatabaseWork.ExecuteUpdate("UPDATE [Companies] SET" +
                    "[Name] = N'" + company.Name + "', " +
                    "[Country] = N'" + company.Country + "', " +
                    "[City] = N'" + company.City + "', " +
                    "[Address] = N'" + company.Address + "', " +
                    "[Name contact person] = N'" + company.NameСontactPerson + "', " +
                    "[Phone contact person] = N'" + company.PhoneContactPerson + "' " +
                    "WHERE [ID company] = '" + company.ID + "'");
                Server.UpdateCompanies();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void DeleteCompany(String companyID, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Инженер" || user.UserRole == "Администратор"))
            {
                if(DatabaseWork.ExecuteUpdate($"DELETE FROM Companies WHERE [ID company] = N'{companyID}'") == "REFERENCE Conflicted")
                {
                    user.SendMessage("ShowWarning", new String[] { "Удаление невозможно!", "Данная фирма связана с одной либо несколькими заявками. Сперва удалите заявки.", "Удаление невозможно" });
                }
                else
                {
                    Server.UpdateCompanies();
                }
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void AddForwarder(String stringForwarder, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                ClassResource.Forwarder company = JsonConvert.DeserializeObject<ClassResource.Forwarder>(stringForwarder);
                DatabaseWork.ExecuteUpdate("INSERT INTO [Forwarders] ([Name], [Contact number], [Note]) VALUES (" +
                    "N'" + company.Name + "'," +
                    "N'" + company.ContactNumber + "'," +
                    "N'" + company.Note + "')");
                Server.UpdateForwarders();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateForwarder(String stringForwarder, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                ClassResource.Forwarder forwarder = JsonConvert.DeserializeObject<ClassResource.Forwarder>(stringForwarder);
                DatabaseWork.ExecuteUpdate("UPDATE [Forwarders] SET " +
                    "[Name] = N'" + forwarder.Name + "'," +
                    "[Contact number] = N'" + forwarder.ContactNumber + "'," +
                    "[Note] = N'" + forwarder.Note + "' " +
                    "WHERE [ID forwarder] = '" + forwarder.ID + "'");
                Server.UpdateForwarders();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void DeleteForwarder(String forwarderID, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторовр" || user.UserRole == "Администратор"))
            {
                if (DatabaseWork.ExecuteUpdate($"DELETE FROM Forwarders WHERE [ID forwarder] = '{forwarderID}'") == "REFERENCE Conflicted")
                {
                    user.SendMessage("ShowWarning", new String[] { "Удаление невозможно!", "Данный экспедитор связан с одним либо несколькими маршрутами. Сперва удалите маршруты.", "Удаление невозможно" });
                }
                else
                {
                    Server.UpdateForwarders();
                }
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void AddEngineer(String stringEngineer, User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
            {
                ClassResource.Engineer engineer = JsonConvert.DeserializeObject<ClassResource.Engineer>(stringEngineer);
                DatabaseWork.ExecuteUpdate("INSERT INTO [Engineers] ([Name], [Сontact number], [Note]) VALUES (" +
                    "N'" + engineer.Name + "'," +
                    "N'" + engineer.ContactNumber + "'," +
                    "N'" + engineer.Note + "')");
                Server.UpdateEngineer();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateEngineer(String stringEngineer, User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
            {
                ClassResource.Engineer engineer = JsonConvert.DeserializeObject<ClassResource.Engineer>(stringEngineer);
                DatabaseWork.ExecuteUpdate("UPDATE [Engineers] SET " +
                    "[Name] = N'" + engineer.Name + "'," +
                    "[Сontact number] = N'" + engineer.ContactNumber + "'," +
                    "[Note] = N'" + engineer.Note + "' " +
                    "WHERE [ID engineer] = '" + engineer.ID + "'");
                Server.UpdateEngineer();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void DeleteEngineer(String engineerID, User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
            {
                if (DatabaseWork.ExecuteUpdate($"DELETE FROM Engineers WHERE [ID engineer] = '{engineerID}'") == "REFERENCE Conflicted")
                {
                    user.SendMessage("ShowWarning", new String[] { "Удаление невозможно!", "Данный инженер связан с одной либо несколькими заявками. Сперва удалите заявки.", "Удаление невозможно" });
                }
                else
                {
                    Server.UpdateEngineer();
                }
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void AddUser(String stringUser, User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
            {
                ClassResource.User tempUser = JsonConvert.DeserializeObject<ClassResource.User>(stringUser);
                if(tempUser.Snapping != "")
                    DatabaseWork.ExecuteUpdate("INSERT INTO [Users] ([Login], [Password], [FIO User], [Role], [Snapping]) VALUES (" +
                        "N'" + tempUser.Login + "'," +
                        "N'" + tempUser.Password + "'," +
                        "N'" + tempUser.Name + "'," +
                        "N'" + tempUser.Role + "'," +
                        "N'" + tempUser.Snapping + "')");
                else
                    DatabaseWork.ExecuteUpdate("INSERT INTO [Users] ([Login], [Password], [FIO User], [Role]) VALUES (" +
                        "N'" + tempUser.Login + "'," +
                        "N'" + tempUser.Password + "'," +
                        "N'" + tempUser.Name + "'," +
                        "N'" + tempUser.Role + "')");
                Server.UpdateUsers();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateUser(String stringUser, User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
            {
                ClassResource.User tempUser = JsonConvert.DeserializeObject<ClassResource.User>(stringUser);
                DatabaseWork.ExecuteUpdate("UPDATE [Users] SET" +
                    "[Login] = N'" + tempUser.Login + "'," +
                    "[FIO User] = N'" + tempUser.Name + "'," +
                    "[Role] = N'" + tempUser.Role + "'," +
                    (tempUser.Snapping != "" ? "[Snapping] = '" + tempUser.Snapping + "' " : "") +
                    "WHERE [Login] = '" + tempUser.Engineer + "'");
                Server.UpdateUsers();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }


        public void DeleteUser(String userLogin, User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
            {
                if (DatabaseWork.ExecuteUpdate($"DELETE FROM Users WHERE [Login] = '{userLogin}'") == "REFERENCE Conflicted")
                {
                    user.SendMessage("ShowWarning", new String[] { "Удаление невозможно!", "Данный пользователь не может быть удален.", "Удаление невозможно" });
                }
                else
                {
                    Server.UpdateUsers();
                }
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void AddRoute(User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                DatabaseWork.ExecuteUpdate("INSERT INTO [Routes] ([Name], [Car type], [Route status], [City / Country departure]) VALUES (" +
                    "N'000000'," +
                    "N'Легковой'," +
                    "N'Открыт'," +
                    "N'Барановичи')");
                Server.UpdateRoutes();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateRoute(String stringRoute, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                ClassResource.Route route = JsonConvert.DeserializeObject<ClassResource.Route>(stringRoute);
                DatabaseWork.ExecuteUpdate("UPDATE [Routes] SET " +
                    "[Name] = N'" + route.Name + "'," +
                    (route.DepartureDate != "" ? "[Departure date] = CONVERT(DATETIME, '" + route.DepartureDate + "', 104)," : "") +
                    "[Car type] = N'" + route.CarType + "'," +
                    (route.ReturnDate != "" ? "[Return date] = CONVERT(DATETIME, '" + route.ReturnDate + "', 104)," : "") +
                    (route.IDForwarder != null ? "[ID forwarder] = '" + route.IDForwarder + "', " : "") +
                    "[Note] = N'" + route.Note + "' " +
                    "WHERE [ID route] = '" + route.ID + "'");
                Server.UpdateRoutes();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void ChangeRouteStatus(String status, String route, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                DatabaseWork.ExecuteUpdate("UPDATE [Routes] SET " +
                    "[Route status] = N'" + status + "' " +
                    "WHERE [ID route] = '" + route + "'");
                Server.UpdateRoutes();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void DeleteRoute(String route, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                DatabaseWork.ExecuteUpdate($"DELETE FROM Destinations WHERE [ID route] = '{route}'");
                DatabaseWork.ExecuteUpdate($"DELETE FROM Routes WHERE [ID route] = '{route}'");
                Server.UpdateRoutes();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void UpdateDestination(String stringDestination, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                ClassResource.Destination destination = JsonConvert.DeserializeObject<ClassResource.Destination>(stringDestination);
                DatabaseWork.ExecuteUpdate("UPDATE [Destinations] SET " +
                    (destination.ArrivalDate != "" ? "[Arrival date] = CONVERT(DATETIME, '" + destination.ArrivalDate + "', 104)," : "") +
                    "[Note] = N'" + destination.Note + "' " +
                    "WHERE [ID destination] = '" + destination.ID + "'");
                Server.UpdateDestinations();
                Server.UpdateRoutes();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void ChangeDestinationNumber(String number, String destination, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                DatabaseWork.ExecuteUpdate("UPDATE [Destinations] SET " +
                    "[Number] = '" + number + "' " +
                    "WHERE [ID destination] = '" + destination + "'");
                Server.UpdateDestinations();
                Server.UpdateRoutes();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void DeleteDestination(String destination, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                DatabaseWork.ExecuteUpdate($"DELETE FROM Destinations WHERE [ID destination] = '{destination}'");
                Server.UpdateDestinations();
                Server.UpdateRoutes();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public void RequestDistribute(String requesID, String routeID, User user)
        {
            if (user.AuthSuccess == true && (user.UserRole == "Руководитель экспедиторов" || user.UserRole == "Администратор"))
            {
                DatabaseWork.ExecuteUpdate("INSERT INTO [Destinations] ([ID route], [ID request], [Number]) VALUES (" +
                   "'" + routeID + "'," +
                   "'" + requesID + "'," +
                   "'" + (DatabaseWork.ExecuteQuery("SELECT * FROM [Destinations] WHERE [ID route] = '" + routeID + "'").Rows.Count + 1).ToString() + "')");
                Server.UpdateDestinations();
                Server.UpdateRoutes();
            }
            else
            {
                Functions.AddJournalEntry($": __ATTENTION__ {user.UserID} {user.UserName} Попытка доступа без соответствующих прав.");
            }
        }

        public static void UpdateUserList()
        {

        }
    }
}
