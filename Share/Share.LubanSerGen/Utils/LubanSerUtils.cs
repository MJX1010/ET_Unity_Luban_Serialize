using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;

namespace ET.Generator.Luban;

public static class LubanSerUtils {

    public static void LogInContext(GeneratorExecutionContext context, string msg,string id = "000", string title = "LubanSer",  string category = "Usage",  DiagnosticSeverity logLevel = DiagnosticSeverity.Info) {
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                $"LUBAN_{id}", 
                !string.IsNullOrEmpty(title) ? title : "LubanSer Generator executed",
                msg, 
                !string.IsNullOrEmpty(category) ? category : "Usage",
                logLevel,
                true),
            Location.None));
    }

    public static void ClearDirectory(string directoryPath, GeneratorExecutionContext context) {
        if (Directory.Exists(directoryPath)) {
            StringBuilder deleteFileNames = new StringBuilder();
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                File.Delete(file);
                deleteFileNames.Append(Path.GetFileName(file));
                deleteFileNames.Append("\n");
            }
            LogInContext(context, $"clear directory: {directoryPath}, \n files: {deleteFileNames}", "005");
        }
    }

    public static void ClearDirectory(string directoryPath) {
        if (Directory.Exists(directoryPath)) {
            StringBuilder deleteFileNames = new StringBuilder();
            foreach (var file in Directory.GetFiles(directoryPath)) {
                File.Delete(file);
            }
        }
    }

    public static void CollectBeanTypeInfo(INamedTypeSymbol typeSymbol, BeanInfo beanInfo) {
        if (typeSymbol.TypeKind == TypeKind.Enum) {
            beanInfo.Fields.AddRange(typeSymbol.GetMembers().OfType<IFieldSymbol>().Select(field =>
                    new BeanFieldInfo() {
                        Name = field.Name,
                        Value = field.ConstantValue?.ToString()
                    }).ToList());
        }
        else {
            beanInfo.Fields.AddRange(typeSymbol.GetMembers().OfType<IFieldSymbol>().Select(field =>
                    new BeanFieldInfo {
                        Name = field.Name,
                        Type = GetISymbolTypeNameForLubanBean(field)
                    }).ToList());
        }
        // 递归处理嵌套类型
        foreach (var nestedType in typeSymbol.GetTypeMembers()) {
            CollectBeanTypeInfo(nestedType, beanInfo);
        }
    }
        
    /// <summary>
    /// Luban Bean需要特殊处理以获取为: float 和 BulletConfig
    /// 
    /// IFieldSymbol.ITypeSymbol.ToDisplayString(获取类型名会带上命名空间) : float 会获取成 Single
    /// ET.BulletConfig 会获取成 ET.BulletConfig\
    ///
    /// IFieldSymbol.ITypeSymbol.Name
    /// ET.BulletConfig 会获取成 BulletConfig
    /// </summary>
    /// <returns></returns>
    public static string GetISymbolTypeNameForLubanBean(IFieldSymbol field) {
        // 检查是否是内置类型
        if (field.Type.SpecialType!= SpecialType.None)
        {
            return field.Type.SpecialType switch
            {
                SpecialType.System_Int32 => "int",
                SpecialType.System_String => "string",
                SpecialType.System_Boolean => "bool",
                SpecialType.System_Char => "char",
                SpecialType.System_Byte => "byte",
                SpecialType.System_SByte => "sbyte",
                SpecialType.System_Int16 => "short",
                SpecialType.System_UInt16 => "ushort",
                SpecialType.System_Int64 => "long",
                SpecialType.System_UInt64 => "ulong",
                SpecialType.System_Single => "float",
                SpecialType.System_Double => "double",
                SpecialType.System_Decimal => "decimal",
                SpecialType.System_Object => "object",
                SpecialType.System_Void => "void",
                _ => field.Type.Name // 默认返回类型名
            };
        }

        return field.Type.Name;
    }
        
#region SerializeToXml

    public static string FindKeyValueFromAttribute(INamedTypeSymbol classSymbol, string attributeName, string findKey)
    {
        var attribute = classSymbol.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.Name == attributeName);
        if (attribute == null) {
            return string.Empty;
        }
        // 提取 `resPath` 参数
        foreach (var namedArgument in attribute.NamedArguments)
        {
            if (namedArgument.Key.Equals(findKey))
                return namedArgument.Value.Value?.ToString();
        }
        return string.Empty;
    }

    public static string GenerateXmlForLubanSerEditor(INamedTypeSymbol classSymbol, string resPath) {
        var builder = new StringBuilder();
        var fields = GetAllFields(classSymbol);
        builder.AppendLine("<module>");
        builder.AppendLine($"<bean name=\"{classSymbol.Name}\">");
        //foreach (var member in classSymbol.GetMembers().OfType<IFieldSymbol>())
        foreach (var field in fields) {
            builder.AppendLine($"    <var name=\"{field.Name}\" type=\"{GetISymbolTypeNameForLubanBean(field)}\"/>");
        }
        builder.AppendLine("</bean>");
        
        var genericType = GetGenericTypeArgument(classSymbol);
        if (genericType != null)
        {
            builder.AppendLine($"<!-- 泛型类型参数: {genericType} -->");
        }
        
        builder.AppendLine($"<table name=\"{LubanSer.s_lubanSerEditor_TableFilePrefix}{classSymbol.Name}\" value=\"{classSymbol.Name}\" input=\"{resPath}\"/>");
        builder.AppendLine("</module>");
        return builder.ToString();
    }
    
    public static List<IFieldSymbol> GetAllFields(INamedTypeSymbol classSymbol)
    {
        var fields = new List<IFieldSymbol>();
        // 获取当前类的字段
        fields.AddRange(classSymbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => !f.IsStatic)); // 排除静态字段

        // 递归解析基类
        if (classSymbol.BaseType != null)
        {
            fields.AddRange(GetAllFields(classSymbol.BaseType));
        }
        return fields;
    }

    public static string? GetGenericTypeArgument(INamedTypeSymbol classSymbol)
    {
        if (classSymbol.BaseType != null && classSymbol.BaseType.IsGenericType)
        {
            // 获取泛型类型的实际参数，例如 BulletConfig
            return classSymbol.BaseType.TypeArguments.FirstOrDefault()?.Name;
        }
        return null;
    }
    
    public static void CollectTypeInfo(BeanInfo beanInfo, LubanModuleInfo module, List<LubanTypeInfo> types,
    List<LubanTypeKind> typeKinds) {
        LubanTypeInfo? lubanTypeInfo = null;
        if (beanInfo.TypeKind == TypeKind.Enum) { 
            lubanTypeInfo = new LubanTypeInfo {
                Name = beanInfo.Name,
                //IsValueType = typeSymbol.TypeKind == TypeKind.Struct ? "1" : "0",
                LubanFields = beanInfo.Fields.Select(field => 
                        new LubanFieldInfo {
                            Name = field.Name,
                            Value = field.Value
                        }
                ).ToList()
            };
        }
        else {
            lubanTypeInfo = new LubanTypeInfo {
                Name = beanInfo.Name,
                IsValueType = beanInfo.TypeKind == TypeKind.Struct ? "1" : "0",
                LubanFields = beanInfo.Fields.Select(field => 
                        new LubanFieldInfo {
                            Name = field.Name,
                            Type = field.Type
                        }
                ).ToList()
            };
        }

        types.Add(lubanTypeInfo);
        module.LubanTypes = types.ToArray();

        // 添加到 TypeIdentifiers 以区分是 "bean" 还是 "enum"
        typeKinds.Add(beanInfo.TypeKind == TypeKind.Enum ? LubanTypeKind.Enum : LubanTypeKind.Bean);
        module.LubanTypeIdentifiers = typeKinds.ToArray();
    }
    
    public static void SerializeToXml(LubanModuleInfo module, string filePath) {
        var serializer = new XmlSerializer(typeof(LubanModuleInfo));
        using (var writer = new StreamWriter(filePath)) {
            serializer.Serialize(writer, module);
        }
    }

    public static string GetAndCreateXmlOutputPath(string outputFilePath, string fileName) {
        string outputFileName = $"{fileName}{LubanSer.s_outputXmlFileNameExt}{LubanSer.s_outputXmlFileExtension}";
        outputFilePath = Path.Combine(outputFilePath, outputFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
        return outputFilePath;
    }
    
    #endregion SerializeToXml
    
    #region LubanGen
    
    public static string GetLubanGenScriptContent(string lubanOutputPath, GeneratorExecutionContext context) {
        try
        {
            if (!Directory.Exists(lubanOutputPath)) {
                return string.Empty;
            }
            // 获取目录下所有 .cs 文件
            string[] csFiles = Directory.GetFiles(lubanOutputPath, "*.cs", SearchOption.AllDirectories);
            StringBuilder writer = new StringBuilder();
            foreach (string file in csFiles)
            {
                // 读取文件内容
                string content = File.ReadAllText(file);
                // 写入分隔符（可选，用于标记不同文件的内容）
                writer.Append($"// --- Start of {Path.GetFileName(file)} ---");
                writer.Append($"{LubanSer.s_macro_Start} {LubanSer.s_lubanSerIde_Macro}");
                writer.Append(content);
                writer.Append($"{LubanSer.s_macro_End}");
                writer.Append($"// --- End of {Path.GetFileName(file)} ---");
                writer.Append(""); // 空行分隔
            }
            return writer.ToString();
        }
        catch (Exception ex)
        {
            LubanSerUtils.LogInContext(context, $"get luban gen files content failed: {ex}", "005", logLevel: DiagnosticSeverity.Error);
        }

        return string.Empty;
    }

    public static string GetLubanGenScriptContent(string lubanOutputFile) {
        try
        {
            if (!File.Exists(lubanOutputFile)) {
                return string.Empty;
            }
            // 读取文件内容
            //UNITY_COMPILE
            StringBuilder writer = new StringBuilder();
            string content = File.ReadAllText(lubanOutputFile);
            writer.Append($"{LubanSer.s_macro_Start} {LubanSer.s_lubanSerIde_Macro}");
            writer.Append(content);
            writer.Append($"{LubanSer.s_macro_End}");
            return writer.ToString();
        }
        catch (Exception ex) {
            return $"[GetLubanGenScriptContent] failed: {ex}";
        }
    }

    #endregion LubanGen
    
    #region LubanCopy
    
    private static void CopyAddEditorLubanSerToUnityScript(string destPath, string fileName, string content)
    {
        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
        }
        string destinationPath = Path.Combine(destPath, fileName);
        string modifiedContent = $"{LubanSer.s_macro_Start} {LubanSer.s_unityEditor_Macro}{content}{LubanSer.s_macro_End}";
        modifiedContent = $"{LubanSer.s_macro_Start} {LubanSer.s_macro_Not}{LubanSer.s_lubanSerIde_Macro}{modifiedContent}{LubanSer.s_macro_End}";
        File.WriteAllText(destinationPath, modifiedContent, Encoding.UTF8);
    }

    private static void CopyAddEditorLubanGenToUnityScript(string srcPath, string destPath, GeneratorExecutionContext context) {
        if (!Directory.Exists(srcPath))
        {
            return;
        }

        bool isDestDirNew = false;
        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
            isDestDirNew = true;
        }
        
        Dictionary<string, string> expireCsFileInfos = new Dictionary<string, string>();
        if (!isDestDirNew) {
            string[] copyCsFiles = Directory.GetFiles(destPath, "*.cs", SearchOption.TopDirectoryOnly);
            foreach (string file in copyCsFiles) {
                string fileName = Path.GetFileName(file);
                expireCsFileInfos.Add(fileName, Path.GetFullPath(file));
            }
        }

        string[] csFiles = Directory.GetFiles(srcPath, "*.cs", SearchOption.TopDirectoryOnly);
        foreach (string file in csFiles)
        {
            string fileName = Path.GetFileName(file);
            string destinationPath = Path.Combine(destPath, fileName);
            File.Copy(file, destinationPath, overwrite: true);
            
            string originalContent = File.ReadAllText(destinationPath);
            string modifiedContent = $"{LubanSer.s_macro_Start} {LubanSer.s_unityEditor_Macro}{originalContent}{LubanSer.s_macro_End}";
            modifiedContent = $"{LubanSer.s_macro_Start} {LubanSer.s_macro_Not}{LubanSer.s_lubanSerIde_Macro}{modifiedContent}{LubanSer.s_macro_End}";
            
            File.WriteAllText(destinationPath, modifiedContent,  Encoding.UTF8);

            if (expireCsFileInfos.ContainsKey(fileName)) {
                expireCsFileInfos.Remove(fileName);
            }
        }

        StringBuilder deleteFiles = new StringBuilder();
        foreach (var expireCsFile in expireCsFileInfos) {
            if (File.Exists(expireCsFile.Value)) {
                File.Delete(expireCsFile.Value);
                deleteFiles.Append(expireCsFile.Key);
                deleteFiles.Append("\n");
            }
        }
    }

    #endregion LubanCopy
}