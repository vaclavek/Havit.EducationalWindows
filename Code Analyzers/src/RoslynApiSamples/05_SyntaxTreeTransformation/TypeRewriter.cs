using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace _05_SyntaxTreeTransformation
{
    public class TypeRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel SemanticModel;

        public TypeRewriter(SemanticModel semanticModel) => SemanticModel = semanticModel;

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            if (node.Declaration.Variables.Count > 1)
            {
                return node;
            }
            if (node.Declaration.Variables[0].Initializer == null)
            {
                return node;
            }

            var declarator = node.Declaration.Variables.First();
            var variableTypeName = node.Declaration.Type;

            var variableType = (ITypeSymbol)SemanticModel
                .GetSymbolInfo(variableTypeName)
                .Symbol;

            var initializerInfo = SemanticModel.GetTypeInfo(declarator.Initializer.Value);

            if (Equals(variableType, initializerInfo.Type))
            {
                TypeSyntax varTypeName = SyntaxFactory.IdentifierName("var")
                    .WithLeadingTrivia(variableTypeName.GetLeadingTrivia())
                    .WithTrailingTrivia(variableTypeName.GetTrailingTrivia());

                return node.ReplaceNode(variableTypeName, varTypeName);
            }
            else
            {
                return node;
            }
        }
    }
}
