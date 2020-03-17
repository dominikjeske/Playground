using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace TorrentTrackerUpdater
{
    public class MailWatcher
    {
        private string _torrentReadyDir;
        private string _mailList;
        private string _mailPassword;
        private string _inputDir;

        private List<string> _workQuee = new List<string>();
        private object locker = new object();

        public void Start()
        {
            TryReadConfig();

            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = _torrentReadyDir;

            watcher.NotifyFilter = NotifyFilters.LastWrite;
            
            

            watcher.Filter = "*.*";

            watcher.Changed += Watcher_Changed;

            watcher.EnableRaisingEvents = true;
        }


        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (locker)
            {
                var ifFile = File.Exists(e.FullPath);
                var isDir = Directory.Exists(e.FullPath);

                var inputDir = Path.GetFileName(_inputDir);

                if ((isDir || ifFile) && e.Name.IndexOf(inputDir) == -1)
                {
                    if (!_workQuee.Contains(e.Name))
                    {
                        _workQuee.Add(e.Name);

                        Task.Run(() =>
                        {
                            try
                            {
                                var fileName = e.Name;

                                var mailList = File.ReadAllLines(_mailList);
                                foreach(var mail in mailList)
                                {
                                    SendMail(mail, fileName);
                                }

                                Thread.Sleep(1000);

                                _workQuee.Remove(e.Name);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"ERROR: {ex.Message}");
                            }

                        });
                    }
                }
            }
        }

        private void TryReadConfig()
        {
            if (!string.IsNullOrWhiteSpace(_torrentReadyDir)) return;

            var configuration = GetConfiguration();

            _torrentReadyDir = configuration.GetValue<string>("ReadyTorrentDir");
            _mailList = configuration.GetValue<string>("MailList");
            _mailPassword = configuration.GetValue<string>("MailPassword");
            _inputDir = configuration.GetValue<string>("InputDir");

        }

        private IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            return configuration;
        }

        public void SendMail(string address, string name)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.live.com");
                var mail = new MailMessage();
                mail.From = new MailAddress("gerart.wiedzmin@outlook.com");
                mail.To.Add(address);
                mail.Subject = "New warez";
                mail.IsBodyHtml = true;
                string htmlBody;
                htmlBody = name;
                mail.Body = htmlBody;
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("gerart.wiedzmin@outlook.com", _mailPassword);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
        }
    }
}