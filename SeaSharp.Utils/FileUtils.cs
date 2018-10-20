using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Web;

namespace SeaSharp.Utils
{
    public static class FileUtils
    {
        #region 上传附件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="files">HttpContext.Current.Request.Files</param>
        /// <param name="savePath">保存地址</param>
        /// <returns>保存地址用逗号分隔</returns>
        public static string Upload(this HttpFileCollection files, string savePath)
        {
            var str = string.Empty;
            foreach (HttpPostedFile file in files)
            {
                var ext = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString("N") + ext;
                if (file.ContentLength > 0 || !string.IsNullOrEmpty(file.FileName))
                {
                    string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + savePath);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    file.SaveAs(filePath + "\\" + fileName);
                    str += savePath + "\\" + fileName + ",";
                }
            }
            return str.Length > 0 ? str.Substring(0, str.Length - 1) : "";
        }
        #endregion

        public static bool CopyBig(this string sourceFileName, string destFileName, long size = 1048576)
        {
            if (size <= 0)
            {
                throw new Exception("size参数必须大于0");
            }
            try
            {
                using (var read = File.OpenRead(sourceFileName))
                {
                    using (var write =File.OpenWrite(destFileName))
                    {
                        var bytes = new byte[size];
                        while (true)
                        {
                            var length = read.Read(bytes, 0, bytes.Length);
                            if (length == 0)
                            {
                                break;
                            }
                            write.Write(bytes, 0, length);
                        }
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }


        public static string GetExtension(this string fileName)
        {
            return string.IsNullOrEmpty(fileName) ? "" : Path.GetExtension(fileName);
        }

        public static string GetFileName(this string filePath)
        {
            return string.IsNullOrEmpty(filePath) ? "" : Path.GetFileName(filePath);
        }

        public static string GetFileNameWithoutExtension(this string filePath)
        {
            return string.IsNullOrEmpty(filePath) ? "" : Path.GetFileNameWithoutExtension(filePath);
        }

        public static string GetDirectoryName(this string filePath)
        {
            return string.IsNullOrEmpty(filePath) ? "" : Path.GetDirectoryName(filePath);
        }

        public static long GetFileLength(this string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo == null ? 0 : fileInfo.Length;
        }

        public static bool HasPermission(this string path, FileIOPermissionAccess access)
        {
            var permissionSet = new PermissionSet(PermissionState.None);
            var writePermission = new FileIOPermission(access, path);
            permissionSet.AddPermission(writePermission);
            return permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
        }

        public static Process OpenFile(this string filePath)
        {
            return Process.Start(filePath);
        }

        public static bool DeleteFile(this string filePath)
        {
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void CreateDirectory(this string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }

        public static string GetFileMd5(this string fileName, bool appendLength = false)
        {
            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var length = fileStream.Length;
                    byte[] array = new MD5CryptoServiceProvider().ComputeHash(fileStream);
                    var sb = new StringBuilder();
                    for (int i = 0; i < array.Length; i++)
                    {
                        sb.Append(array[i].ToString("x2"));
                    }
                    if (appendLength)
                    {
                        sb.AppendFormat("{0:x2}", length);
                    }
                    return sb.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Encoding GetEncoding(this string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            Encoding encoding1 = Encoding.Default;
            if (File.Exists(filePath))
            {
                try
                {
                    using (FileStream stream1 = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        if (stream1.Length > 0)
                        {
                            using (StreamReader reader1 = new StreamReader(stream1, true))
                            {
                                char[] chArray1 = new char[1];
                                reader1.Read(chArray1, 0, 1);
                                encoding1 = reader1.CurrentEncoding;
                                reader1.BaseStream.Position = 0;
                                if (encoding1 == Encoding.UTF8)
                                {
                                    byte[] buffer1 = encoding1.GetPreamble();
                                    if (stream1.Length >= buffer1.Length)
                                    {
                                        byte[] buffer2 = new byte[buffer1.Length];
                                        stream1.Read(buffer2, 0, buffer2.Length);
                                        for (int num1 = 0; num1 < buffer2.Length; num1++)
                                        {
                                            if (buffer2[num1] != buffer1[num1])
                                            {
                                                encoding1 = Encoding.Default;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        encoding1 = Encoding.Default;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                if (encoding1 == null)
                {
                    encoding1 = Encoding.UTF8;
                }
            }
            return encoding1;
        }

        public static string ReadFileText(this string filePath)
        {
            using (var sr = File.OpenText(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public static bool IsDirectoryExists(this string path)
        {
            return Directory.Exists(path);
        }

        public static IEnumerable<string> LoadAllFolders(this string filePath)
        {
            if (!IsDirectoryExists(filePath))
            {
                yield break;
            }
            foreach (var folder in Directory.GetDirectories(filePath))
            {
                yield return folder;
            }
        }

        public static IEnumerable<string> LoadAllFiles(this string filePath)
        {
            if (!IsDirectoryExists(filePath))
            {
                yield break;
            }
            foreach (var file in System.IO.Directory.GetFiles(filePath))
            {
                yield return file;
            }
        }

        public static bool IsFileLocked(this string filePath)
        {
            try
            {
                using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }


    }
}
