using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;

namespace ATZ.CecilExtensions.Tests
{
    [TestFixture]
    public class ModuleDefinitionExtensionsShould
    {
        private readonly ModuleDefinition _moduleDefinition = ModuleDefinition.ReadModule(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AssemblyToProcess.dll"));

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [Test]
        public void ThrowArgumentNullExceptionIfModuleDefinitionIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ModuleDefinitionExtensions.AssemblyDefinition(null));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: moduleDefinition", ex?.Message);
        }

        [Test]
        public void EnumerateTheCorrectNumberOfClassesWithTestFixtureAttribute()
        {
            var testFixtureAttribute = _moduleDefinition?.Import(typeof(TestFixtureAttribute));
            var assemblyDefinition = _moduleDefinition.AssemblyDefinition();
            var moduleDefinitions = assemblyDefinition.ModuleDefinitions();
            var types = moduleDefinitions.TypeDefinitionsWithAttribute(testFixtureAttribute);
            Assert.AreEqual(1, types.Count());
        }

        [Test]
        public void ResolveAssemblyCorrectlyIfExists()
        {
            var assembly = _moduleDefinition.ReferencedAssembly("nunit.framework");
            Assert.IsNotNull(assembly);
        }

        [Test]
        public void ResolveAssemblyCorrectlyIfExistsButWithDifferentCase()
        {
            Assert.IsNotNull(_moduleDefinition.ReferencedAssembly("NUniT.FrAmEwOrK"));
        }

        [Test]
        public void UnresolveNonExistingReference()
        {
            Assert.IsNull(_moduleDefinition.ReferencedAssembly("This.Should.Not.Be.A.Valid.Assembly"));
        }

    }
}
