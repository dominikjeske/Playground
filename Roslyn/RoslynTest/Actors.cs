using Proto;
using System;
using System.Threading.Tasks;

namespace RoslynTest
{
    public class ProtoCluster
    {
        public static async Task Start()
        {
            var context = new RootContext();
            var props = Props.FromProducer(() => new DeviceProxy());
            var pid = context.Spawn(props);

            var result = await context.RequestAsync<int>(pid, new QueryCapabilities()).ConfigureAwait(false);
            Console.WriteLine($"Result from proxy {result}");

            Console.ReadLine();
        }
    }
}