using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace AnalyzerSystemDiagnostics.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        [TestMethod]
        public void UseHavitContracts_DoesntTriggerOnEmptyClass()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void UseHavitContracts_DoesntTriggerOnMethodWithoutContract()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            void TestMethod(object arg) {
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void UseHavitContracts_DoesntTriggerOnHavitContractsFullNamespace()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            void TestMethod(object arg) {
                Havit.Diagnostics.Contracts.Contract.Requires(arg != null, nameof(arg));
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void UseHavitContracts_DoesntTriggerOnHavitContracts()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Havit.Diagnostics.Contracts;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            void TestMethod(object arg) {
                Contract.Requires(arg != null, nameof(arg));
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void UseHavitContracts_TriggersOnFullNamespaceUsage()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            void TestMethod(object arg) {
                System.Diagnostics.Contracts.Contract.Requires(arg != null, nameof(arg));
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "UseHavitContracts",
                Message = "Nahraïte použití System.Diagnostics.Contracts za Havit.Diagnostics.Contracts",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 13, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void UseHavitContracts_TriggersOnNamespaceInUsingUsage()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics.Contracts;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            void TestMethod(object arg) {
                Contract.Requires(arg != null, nameof(arg));
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "UseHavitContracts",
                Message = "Nahraïte použití System.Diagnostics.Contracts za Havit.Diagnostics.Contracts",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 14, 17)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AnalyzerSystemDiagnosticsCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AnalyzerSystemDiagnosticsAnalyzer();
        }
    }
}
