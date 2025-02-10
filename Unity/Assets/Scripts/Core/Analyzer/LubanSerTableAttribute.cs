using System;

namespace ET {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct)]
    public class LubanSerTableAttribute : EnableClassAttribute {
        public string dataPath { get; set; }
    }
}