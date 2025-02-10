using System.Collections.Generic;
using ET.Analyzer;
using ET.Generator.Luban;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ET.Generator;

/// <summary>
/// [LubanSerTable]语法分析入口
/// </summary>
public class LubanSerTableSyntaxContextReceiver : ISyntaxContextReceiver
{
    internal static ISyntaxContextReceiver Create()
    {
        return new LubanSerTableSyntaxContextReceiver();
    }

    public List<ClassDeclarationSyntax> Candidates { get; } = new();
    
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
        SyntaxNode node = context.Node;
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
            if (!typeSymbol.HasAttribute(Definition.UnityCreateAssetMenuAttribute))
            {
                return;
            }
            Candidates.Add(classDeclarationSyntax);
        }
    }
}