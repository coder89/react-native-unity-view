using ReactNative;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent;
using Reinforced.Typings.Generators;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Vieware;

public static class TypeScriptGenerator
{
    [MenuItem("Build/Export TypeScript %&a", false, 1)]
    public static void Generate()
    {
        var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies)
            .Select(m =>
            {
                try
                {
                    //Debug.Log("Processing:");
                    //Debug.Log(m.name);
                    //Debug.Log(m.outputPath);
                    return System.Reflection.Assembly.LoadFile(m.outputPath);
                }
                catch
                {
                    return null;
                }
            })
            .Where(m => m != null)
            .ToArray();

        var context = new Reinforced.Typings.ExportContext(assemblies)
        {
            ConfigurationMethod = Configure,
            TargetFile = "./dupa.ts"
        };

        new Reinforced.Typings.TsExporter(context).Export();
    }

    public static void Configure(ConfigurationBuilder builder)
    {
        var t = builder.Context.SourceAssemblies
            .SelectMany(m => m.GetTypes())
            .Where(m =>
            {
                return m.GetTypeInfo().GetCustomAttribute<UnityRequestAttribute>() != null;
            })
            .ToArray();

        builder.ExportAsInterfaces(t, ConfigureRequest);

        Debug.LogError("Done");
    }

    private static void ConfigureRequest(InterfaceExportBuilder builder)
    {
        // Ignore if given type is not IUnityRequest
        if (builder.Type.FindInterfaces(IsUnityRequestType, null).Length == 0)
        {
            builder.DontIncludeToNamespace();
        }

        builder.WithField("data", ConfigureRequestData);
        builder.WithFields(
            builder.Type.GetFields(BindingFlags.Public | BindingFlags.Instance));

        //builder.WithFields(
        //    null,
        //    (b) =>
        //    {
        //        b.InferType((mi, rttn) =>
        //        {
        //            rttn.ResolveTypeName().Children.First() 
        //        });
        //    });
    }

    private static void ConfigureRequestData(FieldExportBuilder builder)
    {
        builder.InferType((a) =>
        {
            return new RtAnonymousObject();
        });

    }

    private static bool IsUnityRequestType(Type type, object _)
    {
        return type == typeof(IUnityRequest<>);
    }
}

namespace Vieware
{

    [TsEnum]
    public enum TestEnum : int
    {
        Dupa,
        Cipa,
        Cyka
    }

    public class TestRequest : IUnityRequest<TestEnum>
    {
        public TestEnum Type() => TestEnum.Cipa;
    }
}


