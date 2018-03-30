using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JavaScript.Manager.Extensions
{
    internal static class StringExtensions
    {
        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (input.Length == 1)
            {
                return input.ToLowerInvariant();
            }

            return input.Substring(0, 1).ToLowerInvariant() + input.Substring(1);
        }

        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (input.Length == 1)
            {
                return input.ToUpperInvariant();
            }

            return input.Substring(0, 1).ToUpperInvariant() + input.Substring(1);

        }

        /// <summary>
        /// 相对路径转成绝对路径
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static string GetAbsolutePath(this string relativePath, String basePath)
        {
            if (relativePath == null)
            {
                return null;
            }
            if (relativePath.ToLower().StartsWith("http://") || relativePath.ToLower().StartsWith("https://"))
            {
                return relativePath;
            }
            if (basePath == null)
                basePath = Path.GetFullPath("."); // quick way of getting current working directory
            else
                basePath = GetAbsolutePath(basePath, null); // to be REALLY sure ;)
            String path;
            // specific for windows paths starting on \ - they need the drive added to them.
            // I constructed this piece like this for possible Mono support.
            if (!Path.IsPathRooted(relativePath) || "\\".Equals(Path.GetPathRoot(relativePath)))
            {
                if (relativePath.StartsWith(Path.DirectorySeparatorChar.ToString()))
                    path = Path.Combine(Path.GetPathRoot(basePath), relativePath.TrimStart(Path.DirectorySeparatorChar));
                else
                    path = Path.Combine(basePath, relativePath);
            }
            else
                path = relativePath;
            // resolves any internal "..\" to get the true full path.
            return Path.GetFullPath(path);
        }

        public static string GetFolderPath(this string path)
        {
            if (File.Exists(path))
            {
                return new FileInfo(path).DirectoryName;
            }

            if (Directory.Exists(path))
            {
                return path;
            }

            return string.Empty;
        }
        /// <summary>
        /// MD5 16位加密 加密后密码为大写
        /// </summary>
        /// <param name="ConvertString"></param>
        /// <returns></returns>
        public static string MD516(this string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
    }
}