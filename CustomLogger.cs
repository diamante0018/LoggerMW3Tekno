using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace LoggingUtil
{
    class CustomLogger
    {
        private readonly string currentPath;
        private static DateTime StartTime;
        private CultureInfo culture;

        /// <summary>constructor <c>CustomLogger</c> Initializes the logger.</summary>
        public CustomLogger(string hostName)
        {
            DateTime dateTime = DateTime.UtcNow.Date;
            culture = CultureInfo.CreateSpecificCulture("en-GB");
            string formattedTime = dateTime.ToString("dd/MM/yyyy").Replace("/", "");
            CreateDirectory();
            hostName = CleanHostName(hostName);
            currentPath = Directory.GetCurrentDirectory() + $"\\DiavoloLogs\\{hostName}{formattedTime}.txt";
            StartTime = DateTime.Now;
            dateTime = DateTime.UtcNow;
            Info("Script loaded at {0}. Will now proceed to log every player that joins this server instance", dateTime.ToString("r", culture));
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

        private void CreateDirectory()
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

        private string CleanHostName(string hostName) => Regex.Replace(hostName, @"\^([0-9]|:|;)", "");
    }
}