using Proto;
using Proto.Router;
using RoslynGenerator;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RoslynTest
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            // Runtime
            //await ProtoCluster.Start();

            await ProtoTest.Start();

            // Debug
            //await GenerateDebuging().ConfigureAwait(false);

            Console.ReadLine();
        }

        private static async Task GenerateDebuging()
        {
            try
            {
                var code = await File.ReadAllTextAsync(@"..\..\..\Models.cs").ConfigureAwait(false);

                var externalRefs = new Assembly[] { typeof(IContext).Assembly, typeof(Proto.Mailbox.UnboundedMailbox).Assembly, typeof(Router).Assembly };

                var result = await new ProxyGeneratorTest().Generate(code, externalRefs).ConfigureAwait(false);

                Console.ReadLine();

                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}