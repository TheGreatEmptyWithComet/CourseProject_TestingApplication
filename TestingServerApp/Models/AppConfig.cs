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

        public static DbExternalFilesPath DbExternalFilesPath { get; set; } = new DbExternalFilesPath();


    }
}
