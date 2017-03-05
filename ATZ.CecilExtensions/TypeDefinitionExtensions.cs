using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ATZ.CecilExtensions
{
    public static class TypeDefinitionExtensions
    {
        [NotNull]
        private static IEnumerable<CustomAttribute> Attributes([NotNull] this ICustomAttributeProvider typeDefinition,
            MemberReference attributeType)
        {
            var customAttributes = typeDefinition.CustomAttributes;
            return customAttributes?.Where(ca => ca?.AttributeType?.FullName == attributeType?.FullName) ??
                Enumerable.Empty<CustomAttribute>();
        }

        public static CustomAttribute Attribute([NotNull] this TypeDefinition typeDefinition,
            TypeReference attributeType)
        {
            var attributes = Attributes(typeDefinition, attributeType).Take(2).ToList();
            return attributes.Count != 1 ? null : attributes[0];
        }

        public static bool HasAttribute([NotNull] this TypeDefinition typeDefinition, TypeReference attributeType)
        {
            return Attributes(typeDefinition, attributeType).Any();
        }
    }
}