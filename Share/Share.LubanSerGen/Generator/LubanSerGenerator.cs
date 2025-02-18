using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ET.Analyzer;
using ET.Generator.Luban;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ET.Generator;

/// <summary>
/// 解析[LubanSer], 生成Luban解析数据和代码的 Xml文件，然后执行Luabn.dll生成CSharp 和 表格数据
/// </summary>
[Generator(LanguageNames.CSharp)]
public class LubanSerGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {
        //context.RegisterForSyntaxNotifications((() => LubanSerSyntaxContextReceiver.Create()));
        context.RegisterForSyntaxNotifications(() => new LubanSerSyntaxCompositeReceiver(
            LubanSerEditorSyntaxContextReceiver.Create(),
            LubanSerSyntaxContextReceiver.Create(),
            LubanSerTableSyntaxContextReceiver.Create()
            ));
    }

    public void Execute(GeneratorExecutionContext context) {
        try {
            
            if (context.SyntaxContextReceiver is not LubanSerSyntaxCompositeReceiver receivers) {
                return;
            }
            
            TryInitEnv(context);
            if (!envInfo.isValid()) {
                return;
            }

            //AnalyzerReceiver_LubanSerEditor(receivers, context);
			//执行先后顺序要求
            AnalyzerReceiver_LubanSer(receivers, context);
            AnalyzerReceiver_LubanSerTable(receivers, context);
            
            //Debugger.Launch();
            LubanSerUtils.LogInContext(context, $"run cmd: {envInfo.cmdPath} {envInfo.arguments}", "001");
            
            /*
            if (CmdUtils.IsEnvUnix()) {
                var (res, msg) = CmdUtils.AddExecutePermission(envInfo.cmdPath);
                LubanSerUtils.LogInContext(context, $"run cmd in unix, add exe permission: {res}, {msg}", "001");
            }
            */
            
            var (okMsg, errMsg) = CmdUtils.RunCmd(envInfo.cmdPath, envInfo.arguments);
            string fileContent = string.Empty;
            bool isLubanGenOk = true;
            if (!string.IsNullOrEmpty(errMsg)) {
                fileContent = "errMsg: " + errMsg;
                isLubanGenOk = false;
            }
            else {
                fileContent = "okMsg: " + okMsg;
            }

            LubanSerUtils.LogInContext(context, $"run cmd res: {fileContent}", "001");

            Dictionary<string, string> genFileInfos = new Dictionary<string, string>();
            if (isLubanGenOk) {
                if (generateXmlSingle) {
                    foreach (var xmlName in outputXmlFileNames) {
                        var lubanOutputFilePath = Path.Combine(envInfo.lubanCodeCSharpOutPath, xmlName.Replace(LubanSer.s_outputXmlFileNameExt, ".cs"));
                        var content = LubanSerUtils.GetLubanGenScriptContent(lubanOutputFilePath);
                        genFileInfos[xmlName] = content;
                    }

                    if (this.outputXmlFileNames.Count > 0) {
                        //单独处理Luban生成的Tables.cs
                        var lubanOutputTablePath = Path.Combine(envInfo.lubanCodeCSharpOutPath, "Tables.cs");
                        if (File.Exists(lubanOutputTablePath)) {
                            var content = LubanSerUtils.GetLubanGenScriptContent(lubanOutputTablePath);
                            genFileInfos["Tables"] = content;
                        }
                    }
                }
                else {
                    var content = LubanSerUtils.GetLubanGenScriptContent(envInfo.lubanCodeCSharpOutPath, context);
                    if (!string.IsNullOrEmpty(content)) {
                        foreach (var xmlName in outputXmlFileNames) {
                            genFileInfos[xmlName] = content;
                        }
                    }
                }
            }
            
            foreach (var infoPair in genFileInfos) {
                context.AddSource($"Luban.{infoPair.Key}.g.cs", infoPair.Value);
            }
        }
        catch (Exception e) {
            LubanSerUtils.LogInContext(context, e.ToString(), "001", logLevel: DiagnosticSeverity.Error);
        }
    }
    
    private EnvInfo envInfo = null;
    private List<string> outputXmlFileNames = new List<string>();
    private bool generateXmlSingle = true;//一个个生成xml中间文件

    private void TryInitEnv(GeneratorExecutionContext context) {
        this.envInfo = new EnvInfo(context);
        if (!envInfo.isValid()) {
            return;
        }
        this.envInfo.PrintPaths(context);
    }

    //弃用：[LubanSerEditor(resPath = "Config/Editor/Bullet1")]
    //代替：[CreateAssetMenu(menuName = "ET/GameConfig/Bullet", fileName = "Bullet1", order = 0)]
    [Obsolete("弃用，用解析CreateAssetMenu代替")]
    private bool AnalyzerReceiver_LubanSerEditor(LubanSerSyntaxCompositeReceiver receivers, GeneratorExecutionContext context) {
         var contextReceiver = receivers.GetReceiver(0);
         if (contextReceiver != null && contextReceiver is LubanSerEditorSyntaxContextReceiver receiverEditor) {
             
             if (receiverEditor.Candidates.Count > 0) {
                 var compilation = context.Compilation;
                 foreach (var candidate in receiverEditor.Candidates) {
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

                     LubanSerTableGenXml(classSymbol, resPathValue);
                 }

                 string signalCode = $@"
namespace LubanSchemaGenIntermediate{{
    public static class LubanSchemaEditorGenIntermediateClass
    {{
        public static string info => ""{LubanSer.s_lubanSerEditor_genFinalMsg}"";
    }}
}}
                    ";
                 context.AddSource("LubanSerEditor.Intermediate_Generated.g.cs", signalCode);

                 return true;
             }
         }
         
         return false;
    }
    
    /// <summary>
    /// 分析器：解析[LubanSer]，数据结构
    /// 解析数据结构，生成Xml，供Luban使用
    /// </summary>
    /// <param name="receivers"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    private bool AnalyzerReceiver_LubanSer(LubanSerSyntaxCompositeReceiver receivers, GeneratorExecutionContext context) {
        var contextReceiver = receivers.GetReceiver(1);
        if (contextReceiver != null && contextReceiver is LubanSerSyntaxContextReceiver receiver) {

            if (receiver.AllBeanInfos.Count > 0) {
                //监听luban ser editor 生成器完成
                var lubanEditorIntermediateClass = context.Compilation.GetTypeByMetadataName(LubanSer.s_lubanSerEditor_genCsFile);
                if (lubanEditorIntermediateClass == null) {
                }
                
                //监听luban ser table 生成器完成
                var lubanTableIntermediateClass = context.Compilation.GetTypeByMetadataName(LubanSer.s_lubanSerTable_genCsFile);
                if (lubanTableIntermediateClass == null) {
                }
                
                //清理XML生成环境
                LubanSerUtils.ClearDirectory(envInfo.outputXmlForCodePath, context);
                this.outputXmlFileNames.Clear();

                //写入Bean内容
                //解析语法beans，生成中间文件xml
                bool generateXmlSingle = true;
                List<string> genXmlFilePaths = new List<string>();
                if (generateXmlSingle) {
                    //每个bean单独生成xml
                    List<(string, LubanModuleInfo)> moduleInfos = new List<(string, LubanModuleInfo)>();
                    foreach (var infoPair in receiver.AllBeanInfos) {
                        var info = infoPair.Value;
                        var moduleInfo = new LubanModuleInfo {
                            //TODO 命名空间是否需要传递给Luban
                            //Name = "",
                        };
                        List<LubanTypeInfo> typeList = new List<LubanTypeInfo>();
                        List<LubanTypeKind> typeKindList = new List<LubanTypeKind>();
                        LubanSerUtils.CollectTypeInfo(info, moduleInfo, typeList, typeKindList);
                        moduleInfos.Add((info.Name, moduleInfo));
                    }

                    foreach (var info in moduleInfos) {
                        var outputFilePath = LubanSerUtils.GetAndCreateXmlOutputPath(envInfo.outputXmlForCodePath, info.Item1);
                        LubanSerUtils.SerializeToXml(info.Item2, outputFilePath);
                        outputXmlFileNames.Add(Path.GetFileNameWithoutExtension(outputFilePath));
                        genXmlFilePaths.Add(outputFilePath);
                    }
                }
                else {
                    var allBeanInfos = receiver.AllBeanInfos.Values;
                    //生成唯一的xml
                    var moduleInfo = new LubanModuleInfo {
                        //TODO 命名空间是否需要传递给Luban
                        //Name = "",
                        LubanTypes = allBeanInfos.Select(beanInfo => new LubanTypeInfo {
                            Name = beanInfo.Name,
                            LubanFields = beanInfo.Fields.Select(field => new LubanFieldInfo {
                                Name = field.Name,
                                Type = field.Type,
                                Value = field.Value
                            }).ToList()
                        }).ToArray(),
                        LubanTypeIdentifiers = allBeanInfos.Select(beanInfo => beanInfo.LubanTypeKind).ToArray()
                    };
                    var xmlFileName = moduleInfo.Name;
                    if (string.IsNullOrEmpty(xmlFileName)) {
                        xmlFileName = "all";
                    }

                    var outputFilePath = LubanSerUtils.GetAndCreateXmlOutputPath(envInfo.outputXmlForCodePath, xmlFileName);
                    LubanSerUtils.SerializeToXml(moduleInfo, outputFilePath);
                    outputXmlFileNames.Add(Path.GetFileNameWithoutExtension(outputFilePath));
                    genXmlFilePaths.Add(outputFilePath);
                }

                if (genXmlFilePaths.Count > 0) {
                    LubanSerUtils.ClearDirectory(envInfo.outputXmlForDataPath, context);
                    foreach (var file in genXmlFilePaths) {
                        File.Copy(file, Path.Combine(envInfo.outputXmlForDataPath, Path.GetFileName(file)), true);
                    }
                }

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 分析器：解析[LubanSerTable]，数据表
    /// </summary>
    /// <param name="receivers"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    private bool AnalyzerReceiver_LubanSerTable(LubanSerSyntaxCompositeReceiver receivers, GeneratorExecutionContext context) {
        var contextReceiver = receivers.GetReceiver(2);
        if (contextReceiver != null && contextReceiver is LubanSerTableSyntaxContextReceiver receiverTable) {
            
            if (receiverTable.Candidates.Count > 0) {
                var compilation = context.Compilation;
                foreach (var candidate in receiverTable.Candidates) {
                    var model = compilation.GetSemanticModel(candidate.SyntaxTree);
                    var classSymbol = model.GetDeclaredSymbol(candidate);
                    if (classSymbol == null || !classSymbol.HasAttribute(Definition.UnityCreateAssetMenuAttribute)) {
                        continue;
                    }
                    
                    var baseType = classSymbol.BaseType;
                    if (baseType == null || !baseType.IsGenericType ||
                        baseType.OriginalDefinition.ToDisplayString() != Definition.LubanSoGenericType) {
                        continue;
                    }
                    
                    var genericArgument = baseType.TypeArguments.FirstOrDefault();

                    if (genericArgument != null) {
                    }

                    if (genericArgument == null || !genericArgument.HasAttribute(Definition.LubanSerAttribute) ||
                        !genericArgument.HasAttribute(Definition.LubanSerTableAttribute)) {
                        continue;
                    }

                    var resPathValue = LubanSerUtils.FindKeyValueFromAttribute(classSymbol, Definition.UnityCreateAssetMenuAttributeName,
                        LubanSer.s_lubanSerTable_AttrKey_ResPath);
                    
                    if (string.IsNullOrEmpty(resPathValue)) {
                        resPathValue = LubanSer.s_lubanSerEditor_UnitySo_DefResPath;
                    }
                    else {
                        resPathValue = $"{LubanSer.s_lubanSerEditor_UnitySo_DefResPathRoot}{resPathValue}";
                    }

                    LubanSerTableGenXml(classSymbol, resPathValue);
                }

                string signalCode = $@"
namespace LubanSchemaGenIntermediate{{
    public static class LubanSchemaTableGenIntermediateClass
    {{
        public static string info => ""{LubanSer.s_lubanSerTable_genFinalMsg}"";
    }}
}}
                ";
                context.AddSource("LubanSerTable.Intermediate_Generated.g.cs", signalCode);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 解析数据表，生成Xml，供Luban使用
    /// </summary>
    /// <param name="classSymbol"></param>
    /// <param name="resPathValue"></param>
    private void LubanSerTableGenXml(INamedTypeSymbol classSymbol, string resPathValue) {
        //供 luban gen code 使用
        var xmlContent1 = LubanSerUtils.GenerateXmlForLubanSerEditor(classSymbol, resPathValue);
        var outputXmlFilePath = LubanSerUtils.GetAndCreateXmlOutputPath(envInfo.outputXmlForCodePath, classSymbol.Name);
        File.WriteAllText(outputXmlFilePath, xmlContent1);
        //供 luban gen data 使用
        var xmlContent2 = LubanSerUtils.GenerateXmlForLubanSerEditor(classSymbol, resPathValue);
        var outputXmlManualFilePath = LubanSerUtils.GetAndCreateXmlOutputPath(envInfo.outputXmlForDataPath, classSymbol.Name);
        File.WriteAllText(outputXmlManualFilePath, xmlContent2);
        var fileName = Path.GetFileNameWithoutExtension(outputXmlFilePath);
        outputXmlFileNames.Add(fileName);
        outputXmlFileNames.Add(LubanSer.s_lubanSerEditor_TableFilePrefix + fileName);
    }
}