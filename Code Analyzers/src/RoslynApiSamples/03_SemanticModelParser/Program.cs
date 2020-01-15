using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace _03_SemanticModelParser
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
            Console.WriteLine(""Hello, World!"");
        }
    }
}";

        private static void Main()
        {
            // use syntax parser
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            // create "compilation" for semantic model
            var compilation = CSharpCompilation.Create("HelloWorld") // name of "our new" assembly
                .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))  // references for "string" a "Console" (System.Private.CoreLib.dll)
                                                                                                    // .AddReferences(new[] { MetadataReference.CreateFromFile(@"c:\Program Files\dotnet\shared\Microsoft.NETCore.App\3.0.0-preview6-27804-01\System.Private.CoreLib.dll") })
                .AddSyntaxTrees(tree);

            SemanticModel model = compilation.GetSemanticModel(tree);

            // Use the syntax tree to find "using System;"
            UsingDirectiveSyntax usingSystem = root.Usings[0];
            NameSyntax systemName = usingSystem.Name;

            // Use the semantic model for symbol information:
            SymbolInfo nameInfo = model.GetSymbolInfo(systemName);

            // Now semantic analysis "understands" that "System" is namepsace and we are able to write its memebers
            var systemSymbol = (INamespaceSymbol)nameInfo.Symbol;
            foreach (INamespaceSymbol ns in systemSymbol.GetNamespaceMembers())
            {
                Console.WriteLine(ns);
            }
            // only NS declared in this Compilation are written ...

            // Use the syntax model to find the literal string "Hello world!"
            LiteralExpressionSyntax helloWorldString = root.DescendantNodes()
                .OfType<LiteralExpressionSyntax>()
                .Single();

            // Use the semantic model for type information:
            TypeInfo literalInfo = model.GetTypeInfo(helloWorldString);

            var stringTypeSymbol = (INamedTypeSymbol)literalInfo.Type;

            Debugger.Break();
        }
    }
}
