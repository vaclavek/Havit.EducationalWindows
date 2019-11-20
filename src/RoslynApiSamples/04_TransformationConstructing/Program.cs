using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace _04_TransformationConstructing
{
    internal class Program
    {
        private const string sampleCode =
@"using System;
using System.Collections;
using System.Linq;
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
            NameSyntax name = SyntaxFactory.IdentifierName("System");
            Console.WriteLine(name.ToString());

            name = SyntaxFactory.QualifiedName(name, SyntaxFactory.IdentifierName("Collections"));
            Console.WriteLine(name.ToString());

            name = SyntaxFactory.QualifiedName(name, SyntaxFactory.IdentifierName("Generic"));
            Console.WriteLine(name.ToString());

            Console.WriteLine();
            Console.WriteLine();

            // System.Collections.Generic
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sampleCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            // Second using = System.Collections
            var oldUsing = root.Usings[1];

            // Create a new node with new name
            var newUsing = oldUsing.WithName(name);
            // Root is still the same (immutable)
            Console.WriteLine(root.ToString());

            Console.WriteLine();
            Console.WriteLine();

            // Replace old node with new node
            root = root.ReplaceNode(oldUsing, newUsing);
            Console.WriteLine(root.ToString());
        }
    }
}
