using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

[InitializeOnLoad]
public class LubanSerMacroAdder {
    static LubanSerMacroAdder() {
        //AddLubanSerMacro();
    }

    public static void AddLubanSerMacro() {

        BuildTargetGroup activeBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(activeBuildTargetGroup);
        string currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

        if (currentDefines.Contains("LUBAN_SER")) {
            Debug.Log("[LubanSerUtils] LUBAN_SER macro already exists.");
            return;
        }

        string newDefines = string.IsNullOrEmpty(currentDefines) ? "LUBAN_SER" : currentDefines + ";LUBAN_SER";
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newDefines);

        Debug.Log("[LubanSerUtils] LUBAN_SER macro added. Recompiling...");
        AssetDatabase.Refresh();
    }
}
