using System;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckAcadVersion
    {
        private static AcadVersionResult CheckVersion(
            string version,
            string fileName
        )
        {
            return new AcadVersionResult
            {
                FileName = fileName,
                Version = version
            };
        }

        private static string GetAcadVersion()
        {
            Version version = Autodesk.AutoCAD.ApplicationServices.Application.Version;

            int major = version.Major;
            int minor = version.Minor;

            // 2019
            if (major == 23 && minor == 0)
                return "AutoCAD 2019";

            // 2020
            if (major == 23 && minor == 1)
                return "AutoCAD 2020";

            // 2021
            if (major == 24 && minor == 0)
                return "AutoCAD 2021";

            // 2022
            if (major == 24 && minor == 1)
                return "AutoCAD 2022";

            // 2023
            if (major == 24 && minor == 2)
                return "AutoCAD 2023";

            // 2024
            if (major == 24 && minor == 3)
                return "AutoCAD 2024";

            // 2025
            if (major == 25 && minor == 0)
                return "AutoCAD 2025";

            // 2026
            if (major == 25 && minor == 1)
                return "AutoCAD 2026";

            // return
            return $"Unknown ({major}.{minor})";
        }

        public class AcadVersionResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }
            public string Version { get; set; }
        }

        public static AcadVersionResult AnalyzeVersion(
            Database db,
            string fileName
        )
        {
            // Obtenemos version
            string version = 
                Autodesk.AutoCAD.ApplicationServices.Application.Version.ToString();
            // return
            return CheckVersion(version, fileName);
        }

        public static AcadVersionResult AnalyzeVersionMapped(
            Database db,
            string fileName
        )
        {
            // Obtenemos version
            string version = GetAcadVersion();
            // return
            return CheckVersion(version, fileName);
        }

        



    }
}
