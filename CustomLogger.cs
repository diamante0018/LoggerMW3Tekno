using System;
using System.IO;

namespace LoggingUtil
{
    class CustomLogger
    {
        private String currentPath;
        private static DateTime StartTime;

        /// <summary>constructor <c>CustomLogger</c> Initializes the logger.</summary>
        public CustomLogger()
        {
            createDirectory();
            currentPath = Directory.GetCurrentDirectory() + @"\DiavoloLogs\AdvancedLogger.txt";
            StartTime = DateTime.Now;
            Info("------------------------------------------------------------");
        }

        /// <summary>function <c>Info</c> Appends the string to the file with formatting.</summary>
        public void Info(string format, params object[] args)
        {
            if (!File.Exists(currentPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(currentPath))
                {
                    sw.WriteLine("Advanced Logger for MW3. Made by Diavolo");
                }
            }


            using (StreamWriter sw = File.AppendText(currentPath))
            {
                sw.Write(FormatTime(DateTime.Now - StartTime));
                sw.Write(" ");
                sw.WriteLine(string.Format(format, args));
                sw.Flush();
            }
        }

        private void createDirectory()
        {
            try
            {
                DirectoryInfo di = Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\DiavoloLogs");
            }

            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        private static string FormatTime(TimeSpan duration)
        {
            int totalSeconds = (int)duration.TotalSeconds;
            return string.Format("{0}:{1}", totalSeconds / 60, (totalSeconds % 60).ToString().PadLeft(2, '0')).PadLeft(6, ' ');
        }

    }
}