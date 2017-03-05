using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;

namespace ATZ.CecilExtensions.Tests
{
    [TestFixture]
    public class AssemblyDefinitionExtensionsShould
    {
        private AssemblyDefinition _assemblyDefinition;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var moduleDefinition =
                ModuleDefinition.ReadModule(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AssemblyToProcess.dll"));
            _assemblyDefinition = moduleDefinition?.Assembly;
        }

        [Test]
        public void ReturnEmptySequenceOfModuleDefinitionsIfAssemblyDefinitionIsNull()
        {
            Assert.AreEqual(0, AssemblyDefinitionExtensions.ModuleDefinitions(null).Count());
        }

        [Test]
        public void ReturnCorrectModules()
        {
            var modules = _assemblyDefinition.ModuleDefinitions().ToList();
            Assert.AreEqual(1, modules.Count);
            Assert.IsNotNull(modules[0]);
            Assert.AreEqual("AssemblyToProcess.dll", Path.GetFileName(modules[0].FullyQualifiedName));
        }

        [Test]
        public void ReturnCorrectReferencedAssemblies()
        {
            Assert.IsNotNull(_assemblyDefinition);

            var referencedAssemblies = _assemblyDefinition.ReferencedAssemblies();
            foreach (var assembly in referencedAssemblies)
            {
                Console.WriteLine(assembly.FullName);
            }
        }
    }
}
