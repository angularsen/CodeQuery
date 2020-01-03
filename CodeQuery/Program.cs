using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeQuery
{
    public static class Program
    {
        /// <summary>
        /// Example to query all methods in CodeQuery solution with a parameter of type System.DateTime -or- System.DateTime?:
        /// $ CodeQuery.exe methods "C:/dev/CodeQuery/CodeQuery.sln" --projects "^(CodeQuery)$" --method-param-type "System.DateTime\??"
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            if (args.Length == 0 || args[0] != "methods")
            {
                Console.WriteLine("Supported commands:");
                Console.WriteLine("methods solution-or-project-or-source-file [--projects regex] [--namespaces regex] [--method-param-type regex]");
                Console.WriteLine();
                Console.WriteLine("Examples:\n---\n");
                Console.WriteLine("Query all methods with a parameter of type System.DateTime -or- System.DateTime?");
                Console.WriteLine(@"CodeQuery.exe methods ""C:/dev/CodeQuery/CodeQuery.sln"" --projects ""^(CodeQuery)$"" --method-param-type ""System.DateTime\??""");
                return;
            }

            var file = args[1];
            if (!File.Exists(file))
            {
                Console.WriteLine("Solution file not found: " + file);
                return;
            }

            if (Path.GetExtension(file) != ".sln")
            {
                Console.WriteLine("File not supported: " + file);
                Console.WriteLine("Only solution files currently supported, must have extension .sln.");
                return;
            }

            string solutionFile = file;

            // Locate and register the default instance of MSBuild installed on this machine.
            MSBuildLocator.RegisterDefaults();

            // The test solution is copied to the output directory when you build this sample.
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            // Open the solution within the workspace.
            Solution originalSolution = await workspace.OpenSolutionAsync(solutionFile);

            // Declare a variable to store the intermediate solution snapshot at each step.
            Solution newSolution = originalSolution;

            Regex namespaceRegex = null;
            Regex projectRegex = null;
            Regex methodParamTypeRegex = null;
            for (int j = 0; j < args.Length; j++)
            {
                var arg = args[j];
                switch (arg)
                {
                    case "--namespaces":
                        namespaceRegex = new Regex(args[++j]);
                        break;
                    case "--projects":
                        projectRegex = new Regex(args[++j]);
                        break;
                    case "--method-param-type":
                        methodParamTypeRegex = new Regex(args[++j]);
                        break;
                }
            }

            Console.WriteLine("Find methods.");
            Console.WriteLine($"File: {file}");
            Console.WriteLine($"Projects: {projectRegex?.ToString() ?? "(all)"}");
            Console.WriteLine($"Namespaces: {namespaceRegex?.ToString() ?? "(all)"}");
            Console.WriteLine($"Method param type: {methodParamTypeRegex?.ToString() ?? "(all)"}");

            var sw = Stopwatch.StartNew();
            int i = 0;
            // Note how we can't simply iterate over originalSolution.Projects or project.Documents
            // because it will return objects from the unmodified originalSolution, not from the newSolution.
            // We need to use the ProjectIds and DocumentIds (that don't change) to look up the corresponding
            // snapshots in the newSolution.
            foreach (ProjectId projectId in originalSolution.ProjectIds)
            {
                // Look up the snapshot for the original project in the latest forked solution.
                Project project = newSolution.GetProject(projectId);

                if (projectRegex != null && !projectRegex.IsMatch(project.Name))
                {
                    // Console.WriteLine($"Skip project: {project.Name}");
                    continue;
                }

                foreach (DocumentId documentId in project.DocumentIds)
                {
                    // Look up the snapshot for the original document in the latest forked solution.
                    Document document = newSolution.GetDocument(documentId);
                    var semanticModel = await document.GetSemanticModelAsync();

                    var root = await document.GetSyntaxRootAsync();
                    var allMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

                    foreach (MethodDeclarationSyntax method in allMethods)
                    {
                        IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(method);
                        
                        // Is the public declared public?
                        if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
                            continue;

                        // Is the method in the right namespace?
                        if (namespaceRegex != null && namespaceRegex.IsMatch(methodSymbol.ContainingNamespace.ToString()))
                            continue;

                        // Does it have any of the required parameter types?
                        if (methodParamTypeRegex != null && !methodSymbol.Parameters.Any(p => methodParamTypeRegex.IsMatch(p.Type.ToString())))
                            continue;

                        Console.WriteLine($"{sw.Elapsed} {++i:D3} {methodSymbol}");
                    }
                }
            }
        }
    }
}