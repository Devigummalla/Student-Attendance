using System;
using System.IO;
using System.Web.Hosting;

namespace Student_Att.Logging
{
    // Writes errors to a text file: App_Data/Logs/errors-yyyy-MM-dd.log
    // App_Data is never served to the browser, so the log stays private.
    public static class Logger
    {
        // Two requests may fail at the same moment; the lock lets only one write at a time
        private static readonly object fileLock = new object();

        public static void LogError(Exception ex)
        {
            try
            {
                Exception realError = ex.GetBaseException();

                string folder = HostingEnvironment.MapPath("~/App_Data/Logs");
                Directory.CreateDirectory(folder);
                string filePath = Path.Combine(folder, "errors-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");

                string entry = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                               + " | " + realError.GetType().Name
                               + " | " + realError.Message + Environment.NewLine
                               + realError.StackTrace + Environment.NewLine
                               + new string('-', 70) + Environment.NewLine;

                lock (fileLock)
                {
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.Write(entry);
                    }
                }
            }
            catch
            {
                // Logging must never crash the application
            }
        }
    }
}
