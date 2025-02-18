using System;
using System.IO;
using System.Runtime.CompilerServices;
using ET.Generator.Luban;

namespace ET {
    public static class PathUtils {
        public static string GetCurrentSourceFileDirectory([CallerFilePath] string sourceFilePath = "")
        {
            return Path.GetDirectoryName(sourceFilePath) ?? string.Empty;
        }   
        
        public static string GetFullProjectDir(string projectDir) {
            string currDir = PathUtils.GetCurrentSourceFileDirectory();
            if (string.IsNullOrEmpty(projectDir)) {
                if (CmdUtils.IsEnvWindows())
                {
                    projectDir = Path.GetFullPath(Path.Combine(currDir, "../../../"));
                    //projectDir = Directory.GetCurrentDirectory();
                }
                else
                {
                    projectDir = Path.GetFullPath(Path.Combine(currDir, "../../../"));
                    //projectDir = Directory.GetCurrentDirectory();
                }
            }
            return projectDir;
        }
        
        public static string GetUnityProjectRootFromAssemblyPath(string dllPath)
        {
            // 统一路径分隔符
            string normalizedPath = dllPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            // 查找 "Assets" 目录的位置
            string assetsFolder = Path.Combine("Assets", "Plugins", "Editor"); // 假设 DLL 位于 Assets/Plugins/Editor
            int assetsIndex = normalizedPath.IndexOf(assetsFolder, StringComparison.OrdinalIgnoreCase);

            if (assetsIndex == -1)
            {
                throw new InvalidOperationException("The DLL is not located in the expected Unity project structure (Assets/Plugins/Editor).");
            }

            // 截取项目根目录
            string projectRoot = normalizedPath.Substring(0, assetsIndex).TrimEnd(Path.DirectorySeparatorChar);
            return projectRoot;
        }
    }
}