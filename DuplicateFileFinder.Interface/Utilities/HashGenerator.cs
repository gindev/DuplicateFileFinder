using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DuplicateFileFinder.Enums;
using DuplicateFileFinder.Models;
using System.Threading.Tasks;

namespace DuplicateFileFinder.Utilities
{
    public static class HashGenerator
    {
        public async static Task<string> FileHash(FileSystemEntity file, HashSet<int> methods)
        {
            string fileNameHash = String.Empty;
            string fileContentHash = String.Empty;
            string fileSizeHash = String.Empty;
            string hashString = String.Empty;


            foreach (int method in methods)
            {
                if (method == (int)CompareMethods.Name)
                {
                    fileNameHash = await CalculateMD5Hash(file.Name);
                }

                if (method == (int)CompareMethods.Content)
                {
                    using (FileStream fileStream = new FileStream($"{file.Path}\\{file.Name}", FileMode.Open))
                    {
                        fileContentHash = await CalculateMD5Hash(fileStream);
                    }
                }

                if (method == (int)CompareMethods.Size)
                {
                    fileSizeHash = await CalculateMD5Hash(file.Size.ToString());
                }
            }

            return await CalculateMD5Hash($"{fileNameHash}{fileContentHash}{fileSizeHash}");
        }

        private async static Task<string> CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.Default.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        private async static Task<string> CalculateMD5Hash(FileStream input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(input);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
        
    }
}
