using System.Linq;
using System.Text;
using ET.Generator.Luban;
using Microsoft.CodeAnalysis;

namespace ET.Generator;

/*
 * 解析[LubanSer], 生成partial类，添加[EnableClass] / [Serializable]
 */
[Generator(LanguageNames.CSharp)]
public class LubanUnitySerGenerator : ISourceGenerator {
    
    public void Initialize(GeneratorInitializationContext context) {
        context.RegisterForSyntaxNotifications((() => LubanSerSyntaxContextReceiver.Create()));
    }

    public void Execute(GeneratorExecutionContext context) {
        if (context.SyntaxContextReceiver is not LubanSerSyntaxContextReceiver receiver) {
            return;
        }
        if (receiver.BeanTypeSymbols.Count == 0) {
            return;
        }

        var envInfo = new EnvInfo(context);
        if (!envInfo.isValid()) {
            return;
        }
        envInfo.PrintPaths(context);
        
        var sourceBuilder = new StringBuilder();
        sourceBuilder.AppendLine("// <auto-generated />");
        sourceBuilder.AppendLine("using System;");
        sourceBuilder.AppendLine("using ET;");
        sourceBuilder.AppendLine();

        var firstTypeNamespace = receiver.BeanTypeSymbols.Values.First().ContainingNamespace;
        if (firstTypeNamespace != null) {
            sourceBuilder.AppendLine($"namespace {firstTypeNamespace.ToDisplayString()}");
            sourceBuilder.AppendLine("{");
        }
        
        int genIndex = 0;
        foreach (var typeSymbolPair in receiver.BeanTypeSymbols) {
            var typeSymbol = typeSymbolPair.Value;
            var typeName = typeSymbol.Name;
            var namespaceName = typeSymbol.ContainingNamespace.ToDisplayString();
            var typeKind = typeSymbol.TypeKind.ToString().ToLower(); // "class", "struct", etc.
            
            if (genIndex == 0) {
                //sourceBuilder.AppendLine($"namespace {namespaceName}");
                //sourceBuilder.AppendLine("{");
                //sourceBuilder.AppendLine($"namespace {namespaceName};");
            }
            if (typeKind == "class" || typeKind == "struct") {
                // 如果是类（class），额外添加 [EnableClass]
                if (typeKind == "class")
                {
                    sourceBuilder.AppendLine("    [EnableClass]");
                }
                sourceBuilder.AppendLine("    [Serializable]");
                sourceBuilder.AppendLine($"    public partial {typeKind} {typeName}");
                sourceBuilder.AppendLine("    {");
                sourceBuilder.AppendLine("    }");
            }
            else {
                continue;
            }
            genIndex++;
        }
        if (firstTypeNamespace != null && receiver.BeanTypeSymbols.Count > 0) {
            sourceBuilder.AppendLine("}");
        }
        string lubanSerGenFileName = "LubanSer.UnitySo_Generated.g.cs";
        string lubanSerContent = sourceBuilder.ToString();
        
        lubanSerContent = $"{LubanSer.s_macro_Start} {LubanSer.s_lubanSerIde_Macro} {lubanSerContent}{LubanSer.s_macro_End}";
        context.AddSource(lubanSerGenFileName, lubanSerContent);
    }
}