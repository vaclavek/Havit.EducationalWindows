using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace _05_SyntaxTreeTransformation
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Compilation test = CreateTestCompilation();

            foreach (SyntaxTree sourceTree in test.SyntaxTrees)
            {
                SemanticModel model = test.GetSemanticModel(sourceTree);

                TypeRewriter rewriter = new TypeRewriter(model);

                SyntaxNode newSource = rewriter.Visit(sourceTree.GetRoot());

                if (newSource != sourceTree.GetRoot())
                {
                    Console.WriteLine(newSource.ToFullString());
                    // File.WriteAllText(sourceTree.FilePath, newSource.ToFullString());
                }
            }
        }

        private static Compilation CreateTestCompilation()
        {
            String programPath = @"..\..\..\Program.cs";
            String programText = File.ReadAllText(programPath);
            SyntaxTree programTree = CSharpSyntaxTree.ParseText(programText).WithFilePath(programPath);

            String rewriterPath = @"..\..\..\TypeRewriter.cs";
            String rewriterText = File.ReadAllText(rewriterPath);
            SyntaxTree rewriterTree = CSharpSyntaxTree.ParseText(rewriterText).WithFilePath(rewriterPath);

            SyntaxTree[] sourceTrees = { programTree, rewriterTree };

            MetadataReference mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            MetadataReference codeAnalysis = MetadataReference.CreateFromFile(typeof(SyntaxTree).Assembly.Location);
            MetadataReference csharpCodeAnalysis = MetadataReference.CreateFromFile(typeof(CSharpSyntaxTree).Assembly.Location);

            MetadataReference[] references = { mscorlib, codeAnalysis, csharpCodeAnalysis };

            return CSharpCompilation.Create("TransformationCS",
                sourceTrees,
                references,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        }
    }
}
