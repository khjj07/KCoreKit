
using System.IO;

namespace KCoreKit
{
    public class LogFileSystem : GameSubSystemBase
    {
        public void CreateLogFile(string path, string text)
        {
            File.WriteAllText(path,text);
        }
    }
}