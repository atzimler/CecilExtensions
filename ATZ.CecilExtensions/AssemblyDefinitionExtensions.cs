using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ATZ.CecilExtensions
{
    public static class AssemblyDefinitionExtensions
    {
        [NotNull]
        [ItemNotNull]
        private static readonly List<string> AssemblySearchDirectories = new List<string>
        {
            Directory.GetCurrentDirectory(),
            AppDomain.CurrentDomain.BaseDirectory
        };

        [NotNull]
        [ItemNotNull]
        public static List<string> SearchDirectories => AssemblySearchDirectories.ToList();

        private static void MapAssemblyReferences([NotNull] string assemblyFullName, [NotNull] Queue<string> unprocessedAssemblies)
        {
            var assembly = ResolveAssemblyNameReference(assemblyFullName);
            var mainModule = assembly?.MainModule;
            var references = mainModule?.AssemblyReferences ?? Enumerable.Empty<AssemblyNameReference>();

            references.Where(r => r != null).Select(r => r.FullName).ToList().ForEach(r =>
            {
                if (!unprocessedAssemblies.Contains(r))
                {
                    unprocessedAssemblies.Enqueue(r);
                }
            });
        }

        private static AssemblyDefinition ResolveAssemblyNameReference([NotNull] string assemblyFullName)
        {
            return AssemblySearchDirectories
                .Select(d => ResolveAssemblyNameReferenceFromDirectory(assemblyFullName, d))
                .FirstOrDefault(ad => ad != null);
        }

        private static AssemblyDefinition ResolveAssemblyNameReferenceFromDirectory([NotNull] string assemblyFullName, [NotNull] string directory)
        {
            var assemblyName = new AssemblyName(assemblyFullName);
            return Directory
                .GetFiles(directory, $"{assemblyName.Name}.dll", SearchOption.AllDirectories)
                .Select(ModuleDefinition.ReadModule)
                .Where(md => md != null)
                .Select(md => md.Assembly)
                .Where(a => a != null)
                .FirstOrDefault(a => a.FullName == assemblyFullName);
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<ModuleDefinition> ModuleDefinitions(this AssemblyDefinition assemblyDefinition)
        {
            var modules = assemblyDefinition?.Modules ?? Enumerable.Empty<ModuleDefinition>();
            return modules.Where(m => m != null);
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<AssemblyDefinition> ReferencedAssemblies([NotNull] this AssemblyDefinition assemblyDefinition)
        {
            var unprocessedAssemblies = new Queue<string>();
            var processedAssemblies = new List<string>();

            unprocessedAssemblies.Enqueue(assemblyDefinition.FullName);
            while (unprocessedAssemblies.Count > 0)
            {
                var assemblyFullName = unprocessedAssemblies.Dequeue();
                if (assemblyFullName == null || processedAssemblies.Contains(assemblyFullName))
                {
                    continue;
                }

                MapAssemblyReferences(assemblyFullName, unprocessedAssemblies);

                processedAssemblies.Add(assemblyFullName);
                var item = ResolveAssemblyNameReference(assemblyFullName);
                if (item != null)
                {
                    yield return item;
                }
            }
        }

    }
}

