using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckFileSize
    {
        private static List<FileSizeResult> CheckFileSize(
            string filePath,
            string fileName
        )
        {
            List<FileSizeResult> results = new List<FileSizeResult>();

            FileInfo fileInfo = new FileInfo(filePath);
            double sizeMB = fileInfo.Length / (1024.0 * 1024.0);

            results.Add(new FileSizeResult
            {
                FileName = fileName,
                SizeMB = sizeMB,
                ExceedsLimit = sizeMB > 200.0
            });

            return results;
        }

        public class FileSizeResult
        {
            [JsonIgnore]
            public string FileName { get; set; }
            public double SizeMB { get; set; }
            public bool ExceedsLimit { get; set; }
        }

        public static List<FileSizeResult> AnalyzeFileSize(
            string filePath,
            string fileName
        )
        {
            return CheckFileSize(
                filePath, fileName
            );
        }
    }
}
