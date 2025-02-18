using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LubanSerUtils{
    public static class CustomEditorUtils {
        public static readonly string ETUnityLubanSystemTempDirName = "ET_Unity_LubanSer_Temp";
        public static readonly string ETUnityLubanUnityProjectDirCodeName = "UnityProjectPath.cs";
        public static readonly string ETUnityGeneratedCodePath = "_Generated/";
        public static readonly string ETUnityLubanGeneratedCodePath = "_Generated/LubanSer/";
        
        public static readonly string ETUnityCodePath_ModelView = "Scripts/ModelView/Client/Module/";
		public static readonly string ETUnityCodePath_Model = "Scripts/Model/Client/Module/";
		public static readonly string ETUnityCodePath_HotfixView = "Scripts/HotfixView/Client/Module/";
		public static readonly string ETUnityCodePath_Hotfix = "Scripts/Hotfix/Client/Module/";
		public static readonly string ETUnityCodePath_BaseEditor = "Editor/";
		public static readonly string ETUnityCodePath_BaseScript = "Scripts/";

        public static List<string> ETUnityLubanUnityProjectDirCodePaths = new List<string> () {
            ETUnityCodePath_ModelView + ETUnityLubanGeneratedCodePath,
            ETUnityCodePath_Model + ETUnityLubanGeneratedCodePath,
            ETUnityCodePath_HotfixView + ETUnityLubanGeneratedCodePath,
            ETUnityCodePath_Hotfix + ETUnityLubanGeneratedCodePath,
            ETUnityCodePath_BaseEditor + ETUnityLubanGeneratedCodePath,
            ETUnityCodePath_BaseScript + ETUnityLubanGeneratedCodePath,
        };
        
        public static void SetProjectPathToTemp() {
            string projectPath = Application.dataPath.Replace("/Assets", "");

            Process currentProcess = Process.GetCurrentProcess();
            var unityProjectDirectory= Environment.GetEnvironmentVariable($"UNITY_PROJECT_DIR");
            if (string.IsNullOrEmpty(unityProjectDirectory)) {
                Environment.SetEnvironmentVariable($"UNITY_PROJECT_DIR", projectPath, EnvironmentVariableTarget.Process);
            }

            var tempRootPath = Path.Combine(Path.GetTempPath(), ETUnityLubanSystemTempDirName);
            if (!Directory.Exists(tempRootPath)) {
                Directory.CreateDirectory(tempRootPath);
            }
            string tempPath = Path.Combine(tempRootPath, $"unity_project_mapping_{currentProcess.Id}.txt");
            File.WriteAllText(tempPath, projectPath);
            
            Debug.Log($"[LubanSerUtils] Unity Project Directory Saved: {unityProjectDirectory}, {currentProcess.Id}, {currentProcess.ProcessName}");
        }

        public static void GenerateProjectPathToCode() {
            SetProjectPathToTemp();
            
            string projectPath = Application.dataPath.Replace("/Assets", "");
            // 生成静态类 UnityProjectPath.cs
            string classContent = $@"
namespace UnityGenerated
{{
    public static class UnityProjectPath
    {{
        public const string Path = ""{projectPath}"";
    }}
}}";

            bool hasNew = false;
            foreach (var path in ETUnityLubanUnityProjectDirCodePaths) {
                string outputPath = Path.Combine(Application.dataPath, path);
                string outputCodePath = Path.Combine(outputPath, ETUnityLubanUnityProjectDirCodeName);
                if (File.Exists(outputCodePath)) {
                    continue;
                }
                if (!Directory.Exists(outputPath)) {
                    Directory.CreateDirectory(outputPath);
                }
                File.WriteAllText(outputCodePath, classContent);
                Debug.Log($"[LubanSerUtils] Generated UnityProjectPath.cs: {outputCodePath}");
                hasNew = true;
            }

            if (hasNew) {
                UnityEditor.AssetDatabase.Refresh();
            }
        }
    }
}