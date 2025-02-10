using System;
using System.Collections.Generic;
using System.IO;
using ET.Analyzer;
using ET.Generator.Luban;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ET.Generator;

/// <summary>
/// 弃用
/// 解析[LubanSerEditor], 生成Luban解析数据用的Xml文件
/// </summary>
[Obsolete("弃用")]
[Generator(LanguageNames.CSharp)]
public class LubanSchemaEditorGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications((() => LubanSerEditorSyntaxContextReceiver.Create()));
    }

    public void Execute(GeneratorExecutionContext context) {
        try {
            if (context.SyntaxContextReceiver is not LubanSerEditorSyntaxContextReceiver receiver) {
                return;
            }
            
            if (receiver.Candidates.Count == 0) {
                return;
            }
            
            var envInfo = new EnvInfo();
            envInfo.PrintPaths(context);
            
            var compilation = context.Compilation;
            foreach (var candidate in receiver.Candidates) {
                var model = compilation.GetSemanticModel(candidate.SyntaxTree);
                var classSymbol = model.GetDeclaredSymbol(candidate);
                if (classSymbol == null || !classSymbol.HasAttribute(Definition.LubanSerEditorAttribute)) {
                    continue;
                }
                var resPathValue = LubanSerUtils.FindKeyValueFromAttribute(classSymbol, Definition.LubanSerEditorAttributeName,
                    LubanSer.s_lubanSerEditor_AttrKey_ResPath);
                if (string.IsNullOrEmpty(resPathValue)) {
                    resPathValue = LubanSer.s_lubanSerEditor_UnitySo_DefResPath;
                }
                
                var xmlContent = LubanSerUtils.GenerateXmlForLubanSerEditor(classSymbol, resPathValue);
                var outputXmlFilePath = LubanSerUtils.GetAndCreateXmlOutputPath(envInfo.outputXmlForCodePath, classSymbol.Name);
                File.WriteAllText(outputXmlFilePath, xmlContent);
            }
            
            string signalCode = $@"
            public static class LubanSchemaEditorGenIntermediateClass
            {{
                public static string info => ""{LubanSer.s_lubanSerEditor_genFinalMsg}"";
            }}
            ";
            context.AddSource("LubanSerEditor.Intermediate_Generated.g.cs", signalCode);
        }
        catch (Exception e) {
            LubanSerUtils.LogInContext(context, e.ToString(), "002", "Luban Editor Generator executed", logLevel:DiagnosticSeverity.Error);
        }
    }
    
    
}
