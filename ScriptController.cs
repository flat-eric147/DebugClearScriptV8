using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestClearScript
{
    public class ScriptController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        // property bound to view
        string logText;
        public string LogText
        {
            get
            {
                return logText;
            }
            set
            {
                if (logText != value)
                {
                    logText = value;
                    OnPropertyChanged("LogText");
                }
            }
        }

        public ScriptController()
        {
        
        }
        

        // invoked from ClearScript
        public void WriteLog(dynamic message)
        {
            string messageStr = Convert.ToString(message);
            if( messageStr != null )
                LogText += messageStr;
        }

        internal void ClearLog()
        {
            LogText = "";
        }
    }
}
