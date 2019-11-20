using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnalyzerSystemDiagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AnalyzerSystemDiagnosticsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "UseHavitContracts";
        private const string Category = "Usage";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = "Use Havit.Diagnostics.Contracts instead of System.Diagnostics.Contracts.";
        private static readonly LocalizableString MessageFormat = "Replace usage of System.Diagnostics.Contracts to Havit.Diagnostics.Contracts.";
        private static readonly LocalizableString Description = "Due to performance reasons during the build process we replaced System.Diagnostics.Contracts for Havit.Diagnostics.Contracts.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(c =>
            {
                var invocationOperation = c.Operation as Microsoft.CodeAnalysis.Operations.IInvocationOperation;
                if (invocationOperation == null)
                {
                    return;
                }

                if (invocationOperation?.TargetMethod?.ContainingNamespace?.ContainingNamespace?.ContainingNamespace == null)
                {
                    return;
                }

                // https://github.com/microsoft/vs-threading/blob/master/src/Microsoft.VisualStudio.Threading.Analyzers/Utils.cs
                if (invocationOperation.TargetMethod.Name == nameof(System.Diagnostics.Contracts.Contract.Requires) &&
                    invocationOperation.TargetMethod.ContainingType.Name == nameof(System.Diagnostics.Contracts.Contract) &&
                    invocationOperation.TargetMethod.ContainingNamespace.Name == nameof(System.Diagnostics.Contracts) &&
                    invocationOperation.TargetMethod.ContainingNamespace.ContainingNamespace.Name == nameof(System.Diagnostics) &&
                    invocationOperation.TargetMethod.ContainingNamespace.ContainingNamespace.ContainingNamespace.Name == nameof(System))
                {
                    var diagnostic = Diagnostic.Create(Rule, invocationOperation.Syntax.GetLocation());
                    c.ReportDiagnostic(diagnostic);
                }
            }, OperationKind.Invocation);
        }
    }
}
