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
    }
}