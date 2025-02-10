using System.Collections.Generic;
using ET.Analyzer;
using ET.Generator.Luban;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ET.Generator;

/// <summary>
/// 语法分析组合器
/// </summary>
public class LubanSerSyntaxCompositeReceiver : ISyntaxContextReceiver
{
    private readonly List<ISyntaxContextReceiver> _receivers = new();
    
    public LubanSerSyntaxCompositeReceiver(params ISyntaxContextReceiver[] receivers)
    {
        _receivers.AddRange(receivers);
    }

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
        foreach (var receiver in _receivers)
        {
            receiver.OnVisitSyntaxNode(context);
        }
    }

    public ISyntaxContextReceiver GetReceiver(int index) {
        if (index < 0 || index >= _receivers.Count) return null;
        return _receivers[index];
    }
}