using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace _06_Emit
{
    internal class Program
    {
        private const string programText =
@"using System;
using System.Collections.Generic;
using System.Text;
 
namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine(""Hello, World!"");
        }
    }
}";
        private static void Main(string[] args)
        {
            // use syntax parser
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);

            // create "compilation" for semantic model
            var compilation = CSharpCompilation.Create("HelloWorld") // name of "our new" assembly
                .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location))  // references for "string" a "Console" (System.Private.CoreLib.dll)
                                                                                      // .AddReferences(new[] { MetadataReference.CreateFromFile(@"c:\Program Files\dotnet\shared\Microsoft.NETCore.App\3.0.0-preview6-27804-01\System.Private.CoreLib.dll") })
                .AddSyntaxTrees(tree);

            var emitResult = compilation.Emit("HelloWorld.exe", "HelloWorld.pdb");
            // compilation.EmitDifference()

            //If our compilation failed, we can discover exactly why.
            if (!emitResult.Success)
            {
                foreach (var diagnostic in emitResult.Diagnostics)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
            }
        }
    }
}
