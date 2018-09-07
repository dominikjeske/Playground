using Proto;
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
    }
}