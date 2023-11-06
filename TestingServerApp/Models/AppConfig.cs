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

        public static DbExternalFiles DbExternalFiles { get; set; } = new DbExternalFiles();
        public static string LogFileName { get; set; } = string.Empty;

    }
}
