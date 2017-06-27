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

        public static void AuthenticationAttempt(String login, String password, User user)
        {
            System.Data.DataTable tempTable = DatabaseWork.ExecuteQuery("SELECT * FROM [Users] WHERE Login = '" + login + "' AND Password = '" + password + "'");
            if (DatabaseWork.ExecuteQuery("SELECT * FROM [Users] WHERE Login = '" + login + "' AND Password = '" + password + "'").Rows.Count > 0)
            {
                user.SendMessage("AuthenticationAttempt", new String[] { "Yes" });
                user.SendMessage("AccountData", new String[] { login, tempTable.Rows[0]["FIO User"].ToString(), tempTable.Rows[0]["Role"].ToString() });
                user.Authentication(tempTable.Rows[0]["Login"].ToString(), tempTable.Rows[0]["FIO User"].ToString(), tempTable.Rows[0]["Role"].ToString(), tempTable.Rows[0]["Snapping"].ToString());
                Functions.AddJournalEntry($": {user.UserID} {user.UserName} Авторизация. Роль: {tempTable.Rows[0]["Role"].ToString()}.");
            }
            else
            {
                user.SendMessage("AuthenticationAttempt", new String[] { "No" });
                Functions.AddJournalEntry($": {user.UserID} Неудачная попытка авторизации.");
            }
        }

        public static void UpdateUsersData(User user)
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

        public static void UpdateEngineersData(User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
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

        public static void UpdateForwardersData(User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
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

        public static void UpdateCompaniesData(User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
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

        public static void UpdateRequestsData(User user)
        {
            if (user.AuthSuccess == true && user.UserRole == "Администратор")
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

        public static void UpdateUserList()
        {

        }
    }
}
