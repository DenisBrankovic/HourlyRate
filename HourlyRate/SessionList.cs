using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourlyRate
{
    class SessionList : ObservableCollection<Session>
    {
        public SessionList()
        {
            foreach (Session s in SessionDal.GetAllSessions())
            {
                Add(s); 
            }
        }

        private ObservableCollection<Session> allSessions;

        public ObservableCollection<Session> AllSessions
        {
            get { return allSessions; } 
        }
    }
}
