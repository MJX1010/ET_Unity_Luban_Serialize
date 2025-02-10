using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ET.Generator.Luban;

public class CmdUtils {
    #region CMD Luban

    public static string GetCmdEnvPath(string cmdEnvCfgPath) {
        if (string.IsNullOrEmpty(cmdEnvCfgPath)) return string.Empty;
        if(!File.Exists(cmdEnvCfgPath)) return string.Empty;
        
        var config = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var lines = File.ReadAllLines(cmdEnvCfgPath);
        foreach (var line in lines)
        {
            string trimmed = line.Trim();
            // 跳过空行和注释
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;

            // 按 = 分割键值
            var parts = trimmed.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                config[parts[0].Trim()] = parts[1].Trim();
            }
        }
        // 根据操作系统读取相应路径
        string key = IsEnvWindows() ? "WINDOWS_DOTNET_PATH" : "MACOS_DOTNET_PATH";
        if (config.TryGetValue(key, out string dotnetPath))
        {
            return dotnetPath;
        }
        return string.Empty;
    }

    public static bool IsEnvWindows() {
        string os = Environment.OSVersion.Platform.ToString();
        return os.StartsWith("Win");
    }
    
    public static bool IsEnvUnix() {
        string os = Environment.OSVersion.Platform.ToString();
        return os.StartsWith("Unix");
    }

    public static (string, string) RunCmd(string exeFilePath, string arguments) {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        //设置要启动的应用程序
        p.StartInfo.FileName = exeFilePath;
        p.StartInfo.Arguments = arguments;
        //是否使用操作系统shell启动
        p.StartInfo.UseShellExecute = false;
        // 接受来自调用程序的输入信息
        p.StartInfo.RedirectStandardInput = true;
        //输出信息
        p.StartInfo.RedirectStandardOutput = true;
        // 输出错误
        p.StartInfo.RedirectStandardError = true;
        //不显示程序窗口
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        
        // 设置输出和错误流的编码为UTF-8
        p.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
        p.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;

        // 如果你使用的是GB2312编码，可以使用下面这两行替换上面的设置
        // p.StartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding("GB2312");
        // p.StartInfo.StandardErrorEncoding = System.Text.Encoding.GetEncoding("GB2312");
        
        //启动程序
        p.Start();
 
        //向cmd窗口发送输入信息
        p.StandardInput.WriteLine("Luban &exit");
 
        p.StandardInput.AutoFlush = true;
 
        //获取输出信息
        string strOuput = p.StandardOutput.ReadToEnd();
        string strErrOuput = p.StandardError.ReadToEnd();
        //等待程序执行完退出进程
        p.WaitForExit();
        p.Close();
        return (strOuput, strErrOuput);
    }
    
    #endregion CMD Luban
    
    
    public static (bool, string) AddExecutePermission(string filePath)
    {
        try
        {
            // 创建一个 Process 来执行 chmod 命令
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"+x \"{filePath}\"", // 添加执行权限
                    RedirectStandardOutput = true, // 可选：重定向输出
                    RedirectStandardError = true,  // 可选：重定向错误信息
                    UseShellExecute = false,       // 必须为 false，以重定向
                    CreateNoWindow = true          // 防止弹出窗口
                }
            };

            // 启动进程并等待完成
            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"Successfully added execute permission to {filePath}");
                return (true, "ok");
            }
            else
            {
                string error = process.StandardError.ReadToEnd();
                return (false, $"Failed to add execute permission. Error: {error}");
            }
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred: {ex.Message}");
        }
    }
}