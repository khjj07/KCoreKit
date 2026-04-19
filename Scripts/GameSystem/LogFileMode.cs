
using System.IO;

namespace KCoreKit
{
    public class LogFileMode : GameSubModeBase
    {
        public void CreateLogFile(string path, string text)
        {
            File.WriteAllText(path,text);
        }
    }
}