using CodeGeneration.Roslyn;
using System;
using System.Diagnostics;

namespace RoslynGenerator
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute(typeof(ProxyCodeGenerator))]
    [Conditional("CodeGeneration")]
    public class ProxyCodeGeneratorAttribute : Attribute
    {
    }
}