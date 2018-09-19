using Proto;
using Proto.Mailbox;
using RoslynGenerator;
using System;
using System.Threading.Tasks;

namespace RoslynTest
{
    public class Actor : IActor
    {
        public virtual Task ReceiveAsync(IContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual async Task<bool> HandleSystemMessages(IContext context)
        {
            var msg = context.Message;
            if (msg is Started)
            {
                await OnStarted(context).ConfigureAwait(false);
                return true;
            }
            else if (msg is Restarting)
            {
                await OnRestarting(context).ConfigureAwait(false);
                return true;
            }
            else if (msg is Restart)
            {
                await OnRestart(context).ConfigureAwait(false);
                return true;
            }
            else if (msg is Stop)
            {
                await OnStop(context).ConfigureAwait(false);
                return true;
            }
            else if (msg is Stopped)
            {
                await OnStopped(context).ConfigureAwait(false);
                return true;
            }
            else if (msg is Stopping)
            {
                await Stopping(context).ConfigureAwait(false);
                return true;
            }
            else if (msg is SystemMessage)
            {
                await OtherSystemMessage(context).ConfigureAwait(false);
                return true;
            }

            return false;
        }

        protected virtual Task OnStarted(IContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnRestarting(IContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnRestart(IContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnStop(IContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnStopped(IContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task Stopping(IContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OtherSystemMessage(IContext context)
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

    [ProxyCodeGenerator]
    public class HttpService : Actor
    {
        private readonly PID _router;

        protected async Task<string> Handle(HttpQuery query)
        {
            Console.WriteLine($"Actor: [{nameof(HttpService)}] | Actor:{query.Context.Self.Id} | Sender:{query.Context.Sender.Id}");

            //query.Context.Forward(_router);

            //return Task.FromResult("");
            var result = await query.Context.RequestAsync<string>(_router, query);

            //query.Context.Forward(handler);
            return result;
        }

        protected override Task OnStarted(IContext context)
        {
            //var robin = Router.NewRoundRobinPool(Props.FromProducer(() => new HttpServiceHandlerProxy()), 2);

            //_router = context.SpawnNamed(robin, "ROUTER");

            return base.OnStarted(context);
        }
    }

    [ProxyCodeGenerator]
    public class HttpServiceHandler : Actor
    {
        protected async Task<string> Handle(HttpQuery query)
        {
            await Task.Delay(3000);

            Console.WriteLine($"Actor [{nameof(HttpServiceHandler)}] | Actor:{query.Context.Self.Id} | Sender:{query.Context.Sender.Id}");

            return "Test";
        }
    }

    public interface IExecutionContext
    {
        Proto.IContext Context { get; set; }
    }

    public class Command : IExecutionContext
    {
        public Proto.IContext Context { get; set; }
    }

    public class TurnOffCommand : Command
    {
    }

    public class TurnOnCommand : Command
    {
    }

    public class Query : IExecutionContext
    {
        public Proto.IContext Context { get; set; }
    }

    public class HttpQuery : Query
    {
        public string Uri { get; set; }
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
}