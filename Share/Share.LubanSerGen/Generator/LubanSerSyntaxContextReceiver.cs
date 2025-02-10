using System.Collections.Generic;
using ET.Analyzer;
using ET.Generator.Luban;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ET.Generator;

/// <summary>
/// [LubanSer]语法分析入口
/// </summary>
public class LubanSerSyntaxContextReceiver : ISyntaxContextReceiver
{
    internal static ISyntaxContextReceiver Create()
    {
        return new LubanSerSyntaxContextReceiver();
    }
    
    public Dictionary<string, BeanInfo> AllBeanInfos { get; } = new();
    public Dictionary<string, INamedTypeSymbol> BeanTypeSymbols { get; } = new();
    
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        SyntaxNode node = context.Node;
        
        //定义Bean内容
        TypeKind? typeKind = null;
        LubanTypeKind lubanTypeKind = LubanTypeKind.Bean;
        if (node is ClassDeclarationSyntax) {
            typeKind = TypeKind.Class;
            lubanTypeKind = LubanTypeKind.Bean;
        }
        else if (node is StructDeclarationSyntax) {
            typeKind = TypeKind.Struct;
            lubanTypeKind = LubanTypeKind.Bean;
        }
        else if (node is EnumDeclarationSyntax) {
            typeKind = TypeKind.Enum;
            lubanTypeKind = LubanTypeKind.Enum;
        }
        if (typeKind == null) {
            return;
        }

        INamedTypeSymbol? typeSymbol = null;
        if (node is ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (classDeclarationSyntax.AttributeLists.Count == 0)
            {
                return;
            }
            typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
            if (typeSymbol==null)
            {
                return;
            }
            if (!typeSymbol.HasAttribute(Definition.LubanSerAttribute))
            {
                return;
            }
        }
        else if (node is EnumDeclarationSyntax enumDeclarationSyntax) {
            if (enumDeclarationSyntax.AttributeLists.Count == 0) {
                return;
            }
            typeSymbol = context.SemanticModel.GetDeclaredSymbol(enumDeclarationSyntax) as INamedTypeSymbol;
            if (typeSymbol==null)
            {
                return;
            }
            if (!typeSymbol.HasAttribute(Definition.LubanSerAttribute))
            {
                return;
            }
        }
        else if (node is StructDeclarationSyntax structDeclarationSyntax) {
            if (structDeclarationSyntax.AttributeLists.Count == 0) {
                return;
            }
            typeSymbol = context.SemanticModel.GetDeclaredSymbol(structDeclarationSyntax) as INamedTypeSymbol;
            if (typeSymbol==null)
            {
                return;
            }
            if (!typeSymbol.HasAttribute(Definition.LubanSerAttribute))
            {
                return;
            }
        }

        if (typeSymbol == null) {
            return;
        }

        var mainBeanInfo = new BeanInfo
        {
            Name = typeSymbol.Name,
            TypeKind = typeKind.Value,
            LubanTypeKind = lubanTypeKind
        };

        LubanSerUtils.CollectBeanTypeInfo(typeSymbol, mainBeanInfo);
        AllBeanInfos[typeSymbol.Name] = mainBeanInfo;
        if (mainBeanInfo.TypeKind != TypeKind.Enum) {
            BeanTypeSymbols[typeSymbol.Name] = typeSymbol;
        }
    }
}