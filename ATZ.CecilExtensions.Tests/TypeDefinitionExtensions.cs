using System;
using System.IO;
using AssemblyToProcess;
using Mono.Cecil;
using NUnit.Framework;

namespace ATZ.CecilExtensions.Tests
{
    [TestFixture]
    public class TypeDefinitionExtensions
    {
        private readonly string _assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AssemblyToProcess.dll");

        [Test]
        public void ProperlyDetectIfTypeHasAnAttribute()
        {
            var assemblyToProcessModuleDefinition = ModuleDefinition.ReadModule(_assemblyPath);
            var testFixtureClassReference = assemblyToProcessModuleDefinition?.Import(typeof(TestClass));
            var testFixtureClassDefinition = assemblyToProcessModuleDefinition.ImportDefinition(testFixtureClassReference);
            var testFixtureAttribute = assemblyToProcessModuleDefinition?.Import(typeof(TestFixtureAttribute));

            Assert.IsTrue(testFixtureClassDefinition?.HasAttribute(testFixtureAttribute));
        }

        [Test]
        public void ProperlyDetectIfTypeDoesNotHaveAnAttribute()
        {
            var assemblyToProcessModuleDefinition = ModuleDefinition.ReadModule(_assemblyPath);
            var testFixtureClassReference = assemblyToProcessModuleDefinition?.Import(typeof(TestClass));
            var testFixtureClassDefinition = assemblyToProcessModuleDefinition.ImportDefinition(testFixtureClassReference);
            var timeoutAttribute = assemblyToProcessModuleDefinition?.Import(typeof(TimeoutAttribute));

            Assert.IsFalse(testFixtureClassDefinition?.HasAttribute(timeoutAttribute));
        }
    }
}
