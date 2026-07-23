using System;
using System.Collections.Generic;
using System.Linq;
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

        private static void ShowAllDwgVersions()
        {
            List<string> values = Enum.GetValues(typeof(DwgVersion))
                .Cast<DwgVersion>().Select(v => $"{v} = {(int)v}").ToList();
            
            string result = string.Join(
                Environment.NewLine, values
            );
            // Mostramos
            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(result);
        }

        private static string GetDwgSavedVersion(
            DwgVersion dwgVersion
        )
        {
            switch (dwgVersion)
            {
                case DwgVersion.AC1015:
                    return "AutoCAD 2000 / 2000i / 2002";

                case DwgVersion.AC1800:
                case DwgVersion.AC1800a:
                    return "AutoCAD 2004 / 2005 / 2006";

                case DwgVersion.AC1021:
                case DwgVersion.AC2100a:
                    return "AutoCAD 2007 / 2008 / 2009";

                case DwgVersion.AC1024:
                case DwgVersion.AC2400a:
                    return "AutoCAD 2010 / 2011 / 2012";

                case DwgVersion.AC1027:
                case DwgVersion.AC2700a:
                    return "AutoCAD 2013 / 2014 / 2015 / 2016 / 2017";

                case DwgVersion.AC1032:
                case DwgVersion.AC3200a:
                    return "AutoCAD 2018 / 2019 / 2020 / 2021 / 2022 / 2023 / 2024 / 2025 / 2026";

                case DwgVersion.Unknown:
                    return "Unknown";

                default:
                    return dwgVersion.ToString();
            }
        }

        private static string GetDwgSaveAsVersion(
            DwgVersion dwgVersion
        )
        {
            switch (dwgVersion)
            {
                case DwgVersion.AC1032:
                case DwgVersion.AC3200a:
                    return "AutoCAD 2018 (*.dwg)";

                case DwgVersion.AC1027:
                case DwgVersion.AC2700a:
                    return "AutoCAD 2013 (*.dwg)";

                case DwgVersion.AC1024:
                case DwgVersion.AC2400a:
                    return "AutoCAD 2010 (*.dwg)";

                case DwgVersion.AC1021:
                case DwgVersion.AC2100a:
                    return "AutoCAD 2007 (*.dwg)";

                case DwgVersion.AC1800:
                case DwgVersion.AC1800a:
                    return "AutoCAD 2004 (*.dwg)";

                case DwgVersion.AC1015:
                    return "AutoCAD 2000 (*.dwg)";

                default:
                    return dwgVersion.ToString();
            }
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
            //string version = GetAcadVersion();
            string version = GetDwgSaveAsVersion(db.OriginalFileVersion);
            // return
            return CheckVersion(version, fileName);
        }

        



    }
}
