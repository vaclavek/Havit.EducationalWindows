using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace _02_SyntaxWalker
{
    internal class UsingCollector : CSharpSyntaxWalker
    {
        public ICollection<UsingDirectiveSyntax> Usings { get; } = new List<UsingDirectiveSyntax>();

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (node.Name.ToString() != "System"
                && !node.Name.ToString().StartsWith("System."))
            {
                this.Usings.Add(node);
            }
        }
    }
}
