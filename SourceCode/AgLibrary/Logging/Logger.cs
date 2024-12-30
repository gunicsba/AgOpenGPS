using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AgLibrary.Logging
{
    public class Logger
    {
        private string _logsDirectory;
        private string _logFilename;
        private StringBuilder _logs = new StringBuilder();

        public string RawLogs => _logs.ToString();

        public void Initialize(string logsDirectory, string logFilename)
        {
            _logsDirectory = logsDirectory;
            _logFilename = logFilename;

            FileInfo logFile = new FileInfo(Path.Combine(_logsDirectory, _logFilename));
            if (logFile.Exists)
            {
                if (logFile.Length > (500000))       // ## NOTE: 0.5MB max file size
                {
                    _logs.Append("Log File Reduced by 100Kb\r");
                    StringBuilder sbF = new StringBuilder();
                    long lines = logFile.Length - 450000;

                    //create some extra space
                    lines /= 30;

                    using (StreamReader reader = new StreamReader(Path.Combine(_logsDirectory, _logFilename)))
                    {
                        try
                        {
                            //Date time line
                            for (long i = 0; i < lines; i++)
                            {
                                reader.ReadLine();
                            }

                            while (!reader.EndOfStream)
                            {
                                sbF.AppendLine(reader.ReadLine());
                            }
                        }
                        catch { }
                    }

                    using (StreamWriter writer = new StreamWriter(Path.Combine(_logsDirectory, _logFilename)))
                    {
                        writer.WriteLine(sbF);
                    }
                }
            }
            else
            {
                _logs.Append("Events Log File Created\r");
            }
        }

        public void Write(string message)
        {
            _logs.Append(DateTime.Now.ToString("T"));
            _logs.Append("-> ");
            _logs.Append(message);
            _logs.Append("\r");
        }

        public void WriteRaw(string message)
        {
            _logs.Append(message);
        }

        public void SaveLogs()
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(_logsDirectory, _logFilename), true))
            {
                writer.Write(_logs);
            }
        }

        public void ClearLogs()
        {
            _logs.Clear();
        }

        public void OpenLogFileWithNotepad()
        {
            FileInfo txtfile = new FileInfo(Path.Combine(_logsDirectory, _logFilename));
            if (txtfile.Exists)
            {
                Process.Start("notepad.exe", txtfile.FullName);
            }
        }
    }
}
