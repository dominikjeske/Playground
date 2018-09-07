using Proto;
using RoslynGenerator;
using System;
using System.Threading.Tasks;

namespace RoslynTest
{

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