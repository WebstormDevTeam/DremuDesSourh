using UnityEngine;

namespace Utils.Helper.DebugHelper
{
    public class DremuDebuger
    {
        public string LogFilePath;

        public DremuDebuger()
        {
            LogFilePath = "Assets/Logs/Debuger.dlog";
        }

        public DremuDebuger(string FileName)
        {
            LogFilePath = "Assets/Logs/" + FileName + ".log";
        }

        ~DremuDebuger()
        {

        }
    }
}