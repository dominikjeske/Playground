using Proto;
using RoslynGenerator;
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

    internal class Hello
    {
        public string Who { get; }

        public Hello(string who)
        {
            Who = who;
        }
    }

    public class Command
    {
    }

    public class TurnOnCommand : Command
    {
    }

    public class TurnOffCommand : Command
    {
    }

    public class Query
    {
    }

    public class QueryCapabilities : Query
    {
    }

    public class QueryStuff : Query
    {
    }

    public class Result
    {
    }

    public class Actor : IActor
    {
        public virtual Task ReceiveAsync(IContext context)
        {
            return Task.CompletedTask;
        }
    }

    [ProxyCodeGenerator]
    public class Device : Actor
    {
        protected Task Invoke(TurnOnCommand command)
        {
            return Task.CompletedTask;
        }

        protected Task Invoke(TurnOffCommand command)
        {
            return Task.CompletedTask;
        }

        protected Task<int> Get(QueryCapabilities query)
        {
            return Task.FromResult(1);
        }

        protected Task<Result> Get(QueryStuff query)
        {
            return Task.FromResult(new Result());
        }
    }
}