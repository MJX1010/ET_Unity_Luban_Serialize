using System;

namespace ET {
    /// <summary>
    /// 标记需要被Luban序列化的类
    /// 1. 目前暂时只支持Unity Script Object相关的
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct)]
    public class LubanSerAttribute : EnableClassAttribute {
    }
}