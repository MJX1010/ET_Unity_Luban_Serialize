using System;

namespace ET {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct)]
    public class LubanSerEditorAttribute : EnableClassAttribute {
        public string resPath { get; set; }
    }
}