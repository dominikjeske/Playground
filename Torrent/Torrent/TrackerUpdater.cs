using BencodeNET.Parsing;
using BencodeNET.Torrents;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TorrentTrackerUpdater
{
    public class TrackerUpdater
    {
        private string _trackerList;
        private string _outputDirectory;
        private string _inputDirectory;

        private BencodeParser _parser = new BencodeParser();
        private List<string> _trackers = new List<string>();
        private Dictionary<string, string> _workQuee = new Dictionary<string, string>();
        private object locker = new object();

        public void Start()
        {
            TryReadConfig();

            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = _inputDirectory;

                watcher.NotifyFilter = NotifyFilters.LastWrite;

                watcher.Filter = "*.torrent";

                watcher.Changed += Watcher_Changed;

                watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
                Console.WriteLine("Press 'q' to quit");
                while (Console.Read() != 'q') ;
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (locker)
            {
                if (!_workQuee.ContainsKey(e.FullPath))
                {
                    _workQuee.Add(e.FullPath, e.Name);

                    Task.Run(() =>
                    {
                        try
                        {
                            var filePath = e.FullPath;
                            var fileName = e.Name;

                            Console.WriteLine($"Wait for file {fileName}");

                            Thread.Sleep(1000);
                            //AwaitFile(filePath);

                            Console.WriteLine($"Start processing {fileName}");

                            FixTorrent(filePath).GetAwaiter().GetResult();

                            Console.WriteLine($"File {fileName} was fixed");

                            File.Delete(filePath);

                            _workQuee.Remove(e.FullPath);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"ERROR: {e.Message}");
                        }
                       
                    });
                }
            }
        }

        public async Task FixTorrent(string fileName)
        {
            try
            {
                var torrent = _parser.Parse<Torrent>(fileName);

                await TryLoadTrackerList();

                foreach (var tracker in _trackers)
                {
                    torrent.Trackers.Add(new List<string> { tracker });
                }

                var result = torrent.EncodeAsBytes();

                var newFileName = Path.GetFileNameWithoutExtension(fileName) + "_fixed.torrent";

                File.WriteAllBytes(Path.Combine(_outputDirectory, newFileName), result);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
        }

        private async Task TryLoadTrackerList()
        {
            if (_trackers.Count > 0) return;

            var trackers = await File.ReadAllLinesAsync(_trackerList);
            foreach (var tracker in trackers)
            {
                if (tracker.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    _trackers.Add(tracker);
                }
            }
        }

        private void TryReadConfig()
        {
            if (!string.IsNullOrWhiteSpace(_outputDirectory)) return;

            var configuration = GetConfiguration();

            _outputDirectory = configuration.GetValue<string>("OutputDir");
            _trackerList = configuration.GetValue<string>("TrackerList");
            _inputDirectory = configuration.GetValue<string>("InputDir");
        }

        private IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            return configuration;
        }
    }
}