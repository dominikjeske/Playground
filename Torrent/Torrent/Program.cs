using MinimizeCapture;
using System;
using System.Threading.Tasks;
using TorrentTrackerUpdater;

namespace TorrentTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Torrent Tracker monitor...");

            var mailWatcher = new MailWatcher();
            mailWatcher.Start();

            var screen = new ScreenCapture();
            screen.Start();

            var updater = new TrackerUpdater();
            updater.Start();

            Console.ReadLine();
        }
    }
}