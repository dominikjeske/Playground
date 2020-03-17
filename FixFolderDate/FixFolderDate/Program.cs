using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace FixFolderDate
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var path = @"z:\";
                //var path = @"\\192.168.0.106\Public\Photo\Zdjecia";
                var cred = new NetworkCredential("admin", "Dnf;12345678");

                //using (var net = new NetworkConnection(path, cred))
                //{

                    //var path = @"z:\Photo\Zdjecia\";
                    

                    var dirs = Directory.GetDirectories(path);

                    foreach (var dir in dirs)
                    {
                        var files = Directory.GetFiles(dir);
                        if (files.Length == 0)
                        {
                            foreach (var subDir in Directory.GetDirectories(dir))
                            {
                                files = Directory.GetFiles(dir);
                                if (files.Length > 0)
                                {
                                    break;
                                }
                            }
                        }

                        if (files.Length == 0) continue;

                        var date = files.Min(f => new FileInfo(f).LastWriteTime);

                        var dirInfo = new DirectoryInfo(dir);
                    dirInfo.LastWriteTime = date.Date;
                   
                            //File.SetCreationTime(dir, date.Date);
                            //File.SetLastWriteTime(dir, date.Date);
                            //File.SetLastAccessTime(dir, date.Date);
                        

                        Console.WriteLine($"{dir}: {date}");
                    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            


            

        }
    }


    public class NetworkConnection : IDisposable
    {
        string _networkName;

        public NetworkConnection(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            var userName = string.IsNullOrEmpty(credentials.Domain)
                ? credentials.UserName
                : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            var result = WNetAddConnection2(
                netResource,
                credentials.Password,
                userName,
                0);

            if (result != 0)
            {
                throw new Win32Exception(result);
            }
        }

        ~NetworkConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        public ResourceScope Scope;
        public ResourceType ResourceType;
        public ResourceDisplaytype DisplayType;
        public int Usage;
        public string LocalName;
        public string RemoteName;
        public string Comment;
        public string Provider;
    }

    public enum ResourceScope : int
    {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    };

    public enum ResourceType : int
    {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8,
    }

    public enum ResourceDisplaytype : int
    {
        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }
}
