using RoslynGenerator;
using System;
using System.Threading.Tasks;

namespace RoslynTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await ProtoCluster.Start();

            //await GenerateDebuging().ConfigureAwait(false);

            Console.ReadLine();
        }

        private static async Task GenerateDebuging()
        {
            var code =
                        @"  using System;

                namespace HomeCenter
                {
                        public class Actor : IActor
                        {
                            public virtual Task ReceiveAsync(IContext context)
                            {
                                return Task.CompletedTask;
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

                        internal class Device : Actor
                        {
                            protected Task Invoke(TurnOnCommand command)
                            {
                                return Task.CompletedTask;
                            }

                            protected Task Invoke(TurnOffCommand command)
                            {
                                return Task.CompletedTask;
                            }

                            protected Task<int> Get(QueryCapabilities command)
                            {
                                return Task.FromResult(1);
                            }

                            protected Task<Result> Get(QueryStuff command)
                            {
                                return Task.FromResult(new Result());
                            }
                        }
                }";

            try
            {
                var result = await new ProxyGeneratorTest().Generate(code);
                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}