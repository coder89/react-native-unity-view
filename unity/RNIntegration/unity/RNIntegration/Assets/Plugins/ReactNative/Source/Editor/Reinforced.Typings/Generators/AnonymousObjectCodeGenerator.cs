using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings.Generators
{
    /// <summary>
    ///     Default code generator for interfaces. Derived from class generator since interfaces are very similar to classes in
    ///     TypeScript
    /// </summary>
    public class AnonymousObjectCodeGenerator : ClassAndInterfaceGeneratorBase<RtAnonymousObject>
    {
        /// <summary>
        ///     Main code generator method. This method should write corresponding TypeScript code for element (1st argument) to
        ///     WriterWrapper (3rd argument) using TypeResolver if necessary
        /// </summary>
        /// <param name="element">Element code to be generated to output</param>
        /// <param name="result">Resulting node</param>
        /// <param name="resolver">Type resolver</param>
        public override RtAnonymousObject GenerateNode(Type element, RtAnonymousObject result, TypeResolver resolver)
        {
            var tc = Context.Project.Blueprint(element).Attr<TsInterfaceAttribute>();
            if (tc == null) throw new ArgumentException("TsInterfaceAttribute is not present", "element");
            Export(result, element, resolver, tc);
            return result;
        }

        /// <summary>
        ///     Exports entire class to specified writer
        /// </summary>
        /// <param name="result">Exporting result</param>
        /// <param name="type">Exporting class type</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void Export(RtAnonymousObject result, Type type, TypeResolver resolver, IAutoexportSwitchAttribute swtch)
        {
            ExportMembers(type, resolver, result, swtch);
        }

        /// <summary>
        ///     Exports all type members sequentially
        /// </summary>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="typeMember">Placeholder for members</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportMembers(Type element, TypeResolver resolver, RtAnonymousObject typeMember,
            IAutoexportSwitchAttribute swtch)
        {
            ExportFields(typeMember, element, resolver, swtch);
            ExportProperties(typeMember, element, resolver, swtch);
        }

        /// <summary>
        ///     Exports type fields
        /// </summary>
        /// <param name="typeMember">Output writer</param>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportFields(RtAnonymousObject typeMember, Type element, TypeResolver resolver, IAutoexportSwitchAttribute swtch)
        {
            GenerateMembers(element, resolver, typeMember, Context.Project.Blueprint(element).GetExportedFields());
        }

        /// <summary>
        ///     Exports type properties
        /// </summary>
        /// <param name="typeMember">Output writer</param>
        /// <param name="element">Type itself</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="swtch">Pass here type attribute inherited from IAutoexportSwitchAttribute</param>
        protected virtual void ExportProperties(RtAnonymousObject typeMember, Type element, TypeResolver resolver, IAutoexportSwitchAttribute swtch)
        {
            GenerateMembers(element, resolver, typeMember, Context.Project.Blueprint(element).GetExportedProperties());
        }

        /// <summary>
        ///     Exports list of type members
        /// </summary>
        /// <typeparam name="T">Type member type</typeparam>
        /// <param name="element">Exporting class</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="typeMember">Output writer</param>
        /// <param name="members">Type members to export</param>
        protected virtual void GenerateMembers<T>(Type element, TypeResolver resolver, RtAnonymousObject typeMember,
            IEnumerable<T> members) where T : MemberInfo
        {
            foreach (var m in members)
            {
                var generator = Context.Generators.GeneratorFor(m);
                var member = generator.Generate(m, resolver);
                if (member != null) typeMember.Members.Add(member);
            }
        }
    }
}