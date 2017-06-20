using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            AddJournalEntry(login);
            AddJournalEntry(password);
            if(login == password)
            {
                user.SendMessage("AuthenticationAttempt", new String[] { "Yes" });
            }
            else
            {
                user.SendMessage("AuthenticationAttempt", new String[] { "No" });
            }
        }

        public static void UpdateUserList()
        {

        }
    }
}
