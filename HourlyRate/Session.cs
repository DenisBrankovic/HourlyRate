using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourlyRate
{
    class Session : INotifyPropertyChanged
    {
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged ?. Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int sessionID;

        public int SessionId
        {
            get { return sessionID; }
            set { 
                sessionID = value;
                NotifyPropertyChanged("SessionId"); 
            }
        }

        private DateTime sessionDate;

        public DateTime SessionDate
        {
            get {return sessionDate; }
            set { 
                sessionDate = value;
                NotifyPropertyChanged("SessionDate");
            }
        }

        private string stakes;

        public string Stakes
        {
            get { return stakes; }
            set { 
                stakes = value;
                NotifyPropertyChanged("Stakes");
            }
        }

        private int handsPlayed;

        public int HandsPlayed
        {
            get { return handsPlayed; }
            set { 
                handsPlayed = value;
                NotifyPropertyChanged("HandsPlayed");
            }
        }

        private decimal result;
                
        public decimal Result
        {
            get { return result; }
            set { 
                result = value;
                NotifyPropertyChanged("Result");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
