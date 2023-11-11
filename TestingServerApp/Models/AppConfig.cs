using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public static class AppConfig
    {
        public const string Config = "AppConfig";

        // Storage path for files (images) that are stored in database as referenses
        public static DbExternalFiles DbExternalFiles { get; set; } = new DbExternalFiles();
        
        public static string LogFileName { get; set; } = string.Empty;

        // Server settings
        public static string ServerIpAdress { get; set; } = string.Empty;
        public static int ServerPort { get; set; }

    }
}
