using System;
using System.IO;

namespace GrowRoomEnvironment.Core
{
    public static class StoragePath
    {
        public static void Initialize(string rootPath)
        {
            RootPath = rootPath; 
            WWWRootPath = Path.Combine(RootPath, "wwwroot");
            ClientAppPath = Path.Combine(RootPath, "ClientApp");
            EmailTemplatesPath = Path.Combine(RootPath, "EmailTemplates");
            DbFile = Path.Combine(RootPath, "GrowRoomEnvironment.db");
            LogFile = Path.Combine(RootPath, "Log.txt");
        }

        public static string RootPath { get; private set; }

        public static string WWWRootPath { get; private set; }

        public static string ClientAppPath { get; private set; }
        public static string EmailTemplatesPath { get; private set; }
        public static string DbFile { get; private set; }
        public static string LogFile { get; set; }
        public static void AppendLogFile(string text)
        {
            string dirPath = Path.GetDirectoryName(LogFile);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using (StreamWriter writer = File.AppendText(LogFile))
            {
                writer.WriteLine($"{DateTime.Now} - {text}");
            }
        }

        public static string ReadPhysicalFile(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
                throw new FileNotFoundException($"Cannot read file \"{path}\" because it was not found!");

            using (Stream fs = fileInfo.OpenRead())
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }
}
