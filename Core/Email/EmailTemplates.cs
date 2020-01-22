using System;

namespace GrowRoomEnvironment.Core.Email
{
    public static class EmailTemplates
    {
        static string testEmailTemplate;
        static string plainTextTestEmailTemplate;

        public static string GetTestEmail(string recepientName, DateTime testDate)
        {
            if (testEmailTemplate == null)
                testEmailTemplate = StoragePath.ReadPhysicalFile("Email/Templates/TestEmail.template");

            string emailMessage = testEmailTemplate
                .Replace("{user}", recepientName)
                .Replace("{testDate}", testDate.ToString());

            return emailMessage;
        }

        public static string GetPlainTextTestEmail(DateTime date)
        {
            if (plainTextTestEmailTemplate == null)
                plainTextTestEmailTemplate = StoragePath.ReadPhysicalFile("Email/Templates/PlainTextTestEmail.template");

            string emailMessage = plainTextTestEmailTemplate
                .Replace("{date}", date.ToString());

            return emailMessage;
        }
    }
}
