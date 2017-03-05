using JetBrains.Annotations;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ATZ.CecilExtensions
{
    public static class ModuleDefinitionExtensions
    {
        private static bool AreAssemblyNamesEqual(AssemblyDefinition assemblyDefinition, string assemblyName)
        {
            var name = assemblyDefinition?.Name?.Name;
            return string.Compare(name, assemblyName, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public static AssemblyDefinition AssemblyDefinition(this ModuleDefinition moduleDefinition)
        {
            if (moduleDefinition == null)
            {
                throw new ArgumentNullException(nameof(moduleDefinition));
            }

            return moduleDefinition.Assembly;
        }

        public static TypeDefinition ImportDefinition(this ModuleDefinition moduleDefinition, TypeReference typeRefence)
        {
            var assemblies = moduleDefinition?.Assembly?.ReferencedAssemblies() ??
                             Enumerable.Empty<AssemblyDefinition>();
            var modules = assemblies.SelectMany(a => a.ModuleDefinitions()).Where(m => m != null);
            var types = modules.SelectMany(m => m.Types);
            var typeDefinitions = types.Where(t => t?.FullName == typeRefence?.FullName).Take(2).ToList();
            return typeDefinitions.Count != 1 ? null : typeDefinitions[0];
        }

        public static AssemblyDefinition ReferencedAssembly(this ModuleDefinition moduleDefinition, string assemblyName)
        {
            var referencedAssemblies = moduleDefinition?.Assembly?.ReferencedAssemblies();
            return referencedAssemblies?.FirstOrDefault(ad => AreAssemblyNamesEqual(ad, assemblyName));
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<TypeDefinition> TypeDefinitionsWithAttribute(
            [NotNull] [ItemNotNull] this IEnumerable<ModuleDefinition> moduleDefinitions,
            TypeReference typeReference)
        {
            return
                moduleDefinitions.SelectMany(md => md.Types)
                    .Where(t => t != null)
                    .Where(t => t.HasAttribute(typeReference));
        }
    }
}
