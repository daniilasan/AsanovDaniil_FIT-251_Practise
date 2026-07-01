using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace task11
{
    public class ClassGenerator
    {
        public ICalculator CreateCalculator(string SourceCode)
        {
            SyntaxTree OriginalTree = CSharpSyntaxTree.ParseText(SourceCode);
            CompilationUnitSyntax? Root = OriginalTree.GetRoot() as CompilationUnitSyntax;

            if (Root == null)
            {
                throw new Exception("Failed to parse source code");
            }

            ClassDeclarationSyntax OriginalClass = Root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .First();

            // Добавляем наследование от ICalculator
            ClassDeclarationSyntax ModifiedClass = OriginalClass.AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("task11.ICalculator"))
            );

            CompilationUnitSyntax ModifiedRoot = Root.ReplaceNode(OriginalClass, ModifiedClass);

            string ModifiedCode = ModifiedRoot.ToFullString();
            SyntaxTree ModifiedTree = CSharpSyntaxTree.ParseText(ModifiedCode);

            // Получаем ссылки на нужные сборки
            MetadataReference[] References = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
            };

            CSharpCompilation Compilation = CSharpCompilation.Create(
                "DynamicCalculator",
                new[] { ModifiedTree },
                References,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using (MemoryStream Stream = new MemoryStream())
            {
                Microsoft.CodeAnalysis.Emit.EmitResult Result = Compilation.Emit(Stream);

                if (Result.Success == false)
                {
                    string Errors = "";

                    foreach (Microsoft.CodeAnalysis.Diagnostic Diagnostic in Result.Diagnostics)
                    {
                        if (Diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                        {
                            Errors = Errors + Diagnostic.GetMessage() + "\n";
                        }
                    }

                    throw new Exception("Compilation failed:\n" + Errors);
                }

                Stream.Seek(0, SeekOrigin.Begin);
                Assembly DynamicAssembly = Assembly.Load(Stream.ToArray());

                Type[] Types = DynamicAssembly.GetTypes();
                Type? CalculatorType = null;

                foreach (Type CurrentType in Types)
                {
                    if (CurrentType.Name == "Calculator")
                    {
                        CalculatorType = CurrentType;
                        break;
                    }
                }

                if (CalculatorType == null)
                {
                    throw new Exception("Calculator class not found in compiled assembly");
                }

                object? Instance = Activator.CreateInstance(CalculatorType);

                if (Instance == null)
                {
                    throw new Exception("Failed to create Calculator instance");
                }

                // Приводим к интерфейсу — теперь можно вызывать без рефлексии
                ICalculator Calculator = (ICalculator)Instance;
                return Calculator;
            }
        }
    }
}
