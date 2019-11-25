using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace GrowRoomEnvironment.Core
{
    public static class StoragePath
    {
        static IWebHostEnvironment _hostingEnvironment;
        public static void Initialize(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            RootPath = _hostingEnvironment.ContentRootPath;
            WWWRootPath = Path.Combine(RootPath, "wwwroot");
            ClientAppPath = Path.Combine(RootPath, "ClientApp");
            EmailTemplatesPath = Path.Combine(RootPath, "EmailTemplates");
            if (_hostingEnvironment.IsDevelopment())
                DbFile = Path.Combine(RootPath, "GrowRoomEnvironment-Dev.db");
            else
                DbFile = Path.Combine(RootPath, "GrowRoomEnvironment.db");
            LogFile = Path.Combine(RootPath, "Log.txt");
        }

        public static string RootPath { get; private set; }

        public static string WWWRootPath { get; private set; }

        public static string ClientAppPath { get; private set; }
        public static string EmailTemplatesPath { get; private set; }
        public static string DbFile { get; private set; }
        public static string LogFile { get; set; }
        public static void QuickLog(string text)
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
            if (_hostingEnvironment == null)
                throw new InvalidOperationException($"{nameof(StoragePath)} is not initialized");

            IFileInfo fileInfo = _hostingEnvironment.ContentRootFileProvider.GetFileInfo(path);
            if (!fileInfo.Exists)
                throw new FileNotFoundException($"Cannot read file \"{path}\" because it was not found!");

            using (Stream fs = fileInfo.CreateReadStream())
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }
}
