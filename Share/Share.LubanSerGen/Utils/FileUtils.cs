using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Share.LubanSerGen.Utils;

public static class FileUtils {
    
    public static string CalculateMD5(string filePath)
    {
        try
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            using (var md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(fileBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        catch (FileNotFoundException ex)
        {
            return $"File not found: {ex.Message}";
        }
        catch (IOException ex)
        {
            return $"File access error: {ex.Message}";
        }
        catch (UnauthorizedAccessException ex)
        {
            return $"Permission denied: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
        
    public static string ComputeHash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}