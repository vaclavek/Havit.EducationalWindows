using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AnalyzerSystemDiagnostics
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnalyzerSystemDiagnosticsCodeFixProvider))]
    [Shared]
    public class AnalyzerSystemDiagnosticsCodeFixProvider : CodeFixProvider
    {
        private const string title = "Replace for Havit Contracts";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(AnalyzerSystemDiagnosticsAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // var node = root.FindNode(diagnosticSpan);

            // Register a code action that will invoke the fix.
            // context.RegisterCodeFix(
            //     CodeAction.Create(
            //         title: title,
            //         createChangedDocument: (c) => ReplaceSystemDiagnostics(context.Document, root, diagnosticSpan, c),
            //         equivalenceKey: title),
            //     diagnostic);
        }

        private async Task<Document> ReplaceSystemDiagnostics(Document document, SyntaxNode root, TextSpan diagnosticSpan, CancellationToken cancellationToken)
        {
            var token = root.FindToken(diagnosticSpan.Start);
            var systemIdentifier = token.Parent as IdentifierNameSyntax;
            if (systemIdentifier.Identifier.ValueText == "System")
            {
                var diagnostics = systemIdentifier.Parent as MemberAccessExpressionSyntax;
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
                var symbolInfo = semanticModel.GetSymbolInfo(systemIdentifier, cancellationToken);
                var solution = document.Project.Solution;

                var x = SyntaxFactory.ParseTokens("Havit");
                var replaced = root.ReplaceToken(token, x.First());

                return document.WithSyntaxRoot(replaced.WithAdditionalAnnotations());

                //var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, symbolInfo.Symbol, "Havit.Diagnostics", solution.Workspace.Options);
                //return newSolution;
            }
            // token.FullSpan

            //TypeDeclarationSyntax typeDecl;
            // Compute new uppercase name.
            //var identifierToken = typeDecl.Identifier;
            //var newName = identifierToken.Text.ToUpperInvariant();
            //
            //// Get the symbol representing the type to be renamed.
            //var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            //var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);
            //
            //// Produce a new solution that has all references to that type renamed, including the declaration.
            //var originalSolution = document.Project.Solution;
            //var optionSet = originalSolution.Workspace.Options;
            //var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.

            // return newSolution;
            return document;
        }

    }
}
