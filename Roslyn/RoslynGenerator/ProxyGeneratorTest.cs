using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading.Tasks;

namespace RoslynGenerator
{
    public class ProxyGeneratorTest
    {
        public async Task<string> Generate(string code)
        {
            var generator = new ProxyGenerator();
            var models = await GetModels(code);

            var syntaxTree = models.syntaxTree;
            var semanticModel = models.semanticModel;

            foreach (var classModel in models.syntaxTree.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var classSemantic = semanticModel.GetDeclaredSymbol(classModel);
                if (classSemantic.BaseType.Name == "Actor")
                {
                    var proxyClass = generator.GenerateProxy(classModel, semanticModel);
                    var @namespace = classModel.Parent as NamespaceDeclarationSyntax;
                    var newNamespace = @namespace.AddMembers(proxyClass);
                    syntaxTree = syntaxTree.ReplaceNode(@namespace, newNamespace);
                }
            }

            return FormatResut(syntaxTree);
        }

        private string FormatResut(CompilationUnitSyntax root)
        {
            var r = root.NormalizeWhitespace();
            return r.ToFullString();
        }

        private async Task<(CompilationUnitSyntax syntaxTree, SemanticModel semanticModel)> GetModels(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var syntaxTree = await tree.GetRootAsync().ConfigureAwait(false) as CompilationUnitSyntax;

            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var comp = CSharpCompilation.Create("Demo").AddSyntaxTrees(tree).AddReferences(mscorlib);

            var semanticModel = comp.GetSemanticModel(tree);

            return (syntaxTree, semanticModel);
        }
    }
}