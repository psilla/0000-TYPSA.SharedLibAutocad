using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckAcadVersion;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckBlockAttr;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckByLayerProp;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckEntInLayerZero;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckLayersInUse;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckPaperTextFont;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckProjUnits;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckRevClouds;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckXrefs;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainGetPlogTag;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class EntityTypes
    {
        public string BlockReference { get; set; } = "INSERT";
        public string Polyline { get; set; } = "LWPOLYLINE";
        public string Text { get; set; } = "TEXT";
        public string MText { get; set; } = "MTEXT";

        public static EntityTypes GetDefaultEntityTypes()
        {
            return new EntityTypes();
        }
    }

    public class EntityFilterTypes
    {
        public string BlockReference { get; set; } = "AcDbBlockReference";
        public string Polyline { get; set; } = "AcDbPolyline";
        public string Line { get; set; } = "AcDbLine";

        public static EntityFilterTypes GetDefaultFilterTypes()
        {
            return new EntityFilterTypes();
        }
    }

    public class ModelCheckerResults
    {
        public List<ProjectUnitsResult> ProjectUnits { get; set; } = new List<ProjectUnitsResult>();

        public List<LayerUsageResult> LayersInUse { get; set; } = new List<LayerUsageResult>();

        public List<LayerZeroUsageResult> LayerZero { get; set; } = new List<LayerZeroUsageResult>();

        public List<AcadVersionResult> Version { get; set; } = new List<AcadVersionResult>();

        public List<XrefStatusResult> Xrefs { get; set; } = new List<XrefStatusResult>();

        //public List<CoordinateSystemResult> CoordSystem { get; set; } = new List<CoordinateSystemResult>();

        public List<PaperTextFontResult> PaperTextFont { get; set; } = new List<PaperTextFontResult>();

        public List<ByLayerEntityResult> ByLayer { get; set; } = new List<ByLayerEntityResult>();

        public List<RevisionCloudResult> RevisionClouds { get; set; } = new List<RevisionCloudResult>();

        public List<BlockAttributesResult> BlockAttributes { get; set; } = new List<BlockAttributesResult>();

        public List<PlotInfoResult> PlotTags { get; set; } = new List<PlotInfoResult>();
    }

    public class CivilSessionInfo
    {
        // =========================
        // Revit Info (inputs reales)
        // =========================

        public string CivilVersion { get; set; }
        public string UserName { get; set; }
        public string DateTimeNow { get; set; }
        public string CivilLanguage { get; set; }
        public string AteneaVersion { get; set; }
        public string SerapisMetrics { get; set; }
        public bool IsProductionUse { get; set; }

        // =========================
        // Base URL (calculada)
        // =========================

        public string AteneaBaseUrl =>
            IsProductionUse
            ? "https://atenea.api.typsa.com:3000"
            : "https://atenea.api.typsadev.com:3000";

        // =========================
        // Endpoints (calculados)
        // =========================

        public string EndpointProjectDataUrl =>
            $"{AteneaBaseUrl}/api/project/validate-revit-element-data";

        public string EndpointValidateSetUrl =>
            $"{AteneaBaseUrl}/api/revit-project-param-set/validate-status-revit-param-set";

        public string EndpointGetSetUrl =>
            $"{AteneaBaseUrl}/api/revit-project-param-set/get-revit-param-set";

        public string EndpointDeleteUrl =>
            $"{AteneaBaseUrl}/api/project/revit-element-data";

        public string EndpointPostUrl =>
            $"{AteneaBaseUrl}/api/project/revit-element-data-direct";

        public string EndpointSetUrl =>
            $"{AteneaBaseUrl}/api/revit-project-param-set/revit-param-set";

        public string EndpointDataByFileUrl =>
            $"{AteneaBaseUrl}/api/revit-file-param-config/revit-param";

        public string EndpointGetCustomSetUrl =>
            $"{AteneaBaseUrl}/api/revit-project-param-set-config/get-revit-param-set";

        public string EndpointPostCustomSetUrl =>
            $"{AteneaBaseUrl}/api/revit-project-param-set-config/revit-param-set";

        public string EndpointDataByFileElementUrl =>
            $"{AteneaBaseUrl}/api/revit-file-element/revit-element";

        // =========================
        // Root Folder
        // =========================

        public string RootFolderName => "AteneaCivilWeb";

        // =========================
        // Safe Values
        // =========================

        public string SafeVersion => MakeSafe(CivilVersion);
        public string SafeUser => MakeSafe(UserName);
        public string SafeLanguage => MakeSafe(CivilLanguage);

        // =========================
        // Json File Names
        // =========================

        public string JsonFileNameDataExtraction => $"ParamElemExp_{SafeUser}_{SafeVersion}_{SafeLanguage}.json";
        public string JsonFileNameParamCheckSet => $"ParamCheckSet_{SafeUser}_{SafeVersion}_{SafeLanguage}.json";
        public string JsonFileNameParamCheckExp => $"ParamCheckExp_{SafeUser}_{SafeVersion}_{SafeLanguage}.json";
        public string JsonFileNameParamCheckExpInv => $"ParamCheckExpInv_{SafeUser}_{SafeVersion}_{SafeLanguage}.json";
        public string JsonFileNameParamDataExp => $"ParamDataExp_{SafeUser}_{SafeVersion}_{SafeLanguage}.json";

        // =========================
        // Helper
        // =========================

        private string MakeSafe(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            var invalidChars = Path.GetInvalidFileNameChars();
            var result = new StringBuilder(input.Length);
            // Iteramos
            foreach (char c in input)
            {
                if (invalidChars.Contains(c) || c == ' ')
                    result.Append('_');
                else
                    result.Append(c);
            }
            // return
            return result.ToString();
        }
    }

    public class cls_00_CivilInfoHelper
    {
        

        public static CivilSessionInfo GetCivilSessionInfo()
        {
            return new CivilSessionInfo
            {

                CivilVersion = "Civil " + Application.Version.ToString(),
                UserName = Environment.UserName,
                DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CivilLanguage = System.Globalization.CultureInfo.InstalledUICulture.DisplayName,
                AteneaVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                SerapisMetrics = "69c59742e4dfd32366266eb8",
                IsProductionUse = false
            };
        }

        public static Dictionary<string, object> GetProjectDataDictionary(
            string projectCode
        )
        {
            CivilSessionInfo sessionInfo = GetCivilSessionInfo();
            // return
            return new Dictionary<string, object>
            {
                { AteneaJson.User, sessionInfo.UserName },
                { AteneaJson.ProjectCode, projectCode },
                { AteneaJson.CivilVersion, sessionInfo.CivilVersion },
                { AteneaJson.CivilLanguage , sessionInfo.CivilLanguage },
                { AteneaJson.AteneaVersion, sessionInfo.AteneaVersion }
            };
        }

        public static Dictionary<string, object> GetFinalJsonDictionary(
            string projectCode,
            List<Dictionary<string, object>> dataJsonByModel
        )
        {
            Dictionary<string, object> baseDict = GetProjectDataDictionary(projectCode);
            // Añadimos info
            baseDict[AteneaJson.DataByFileName] = dataJsonByModel;
            // return
            return baseDict;
        }

        public static Dictionary<string, object> GetSetStatusDictionary(
            string projectCode
        )
        {
            CivilSessionInfo sessionInfo = GetCivilSessionInfo();
            // return
            return new Dictionary<string, object>
            {
                { AteneaJson.User, sessionInfo.UserName },
                { AteneaJson.ProjectCode, projectCode },
                { AteneaJson.AteneaVersion, sessionInfo.AteneaVersion }
            };
        }

        public static class AteneaJson
        {
            // -----------------------------
            // PROPERTY SET DATA (Civil)
            // -----------------------------

            public const string PsetName = "PsetName";
            public const string PsetId = "PsetId";
            public const string PropId = "PropId";
            public const string PropName = "PropName";
            public const string DataType = "DataType";
            public const string PropDefaultValue = "PropDefaultValue";
            public const string PropDescription = "PropDescription";
            public const string PropIsAutomatic = "PropIsAutomatic";
            public const string PropIsVisible = "PropIsVisible";
            public const string PropIsReadOnly = "PropIsReadOnly";
            public const string PropUnitType = "PropUnitType";
            // Categories
            public const string Categories = "Categories";
            public const string CategoryName = "categoryName";
            public const string CategoryValue = "categoryValue";

            // Información principal del proyecto
            public const string ProjectCode = "ProjectCode";
            public const string CivilVersion = "CivilVersion";
            public const string User = "User";
            public const string CivilLanguage = "CivilLanguage";
            public const string AteneaVersion = "AteneaVersion";

            // Estado actual del conjunto (1, 2 o 3)
            public const string Status = "Status";

            // Diccionario con la información agrupada por archivo
            public const string DataByFileName = "DataByFileName";
            public const string FileName = "FileName";
            public const string CivilElementData = "CivilElementData";
            public const string CivilParamData = "CivilParamData";
            public const string CivilParamCheck = "CivilParamCheck";
        }

        

        public static void SaveJsonToDesktop(
            bool isSpanish,
            string jsonContent,
            string projectCode,
            string rootFolderName,
            string jsonFileName
        )
        {
            // try
            try
            {
                // Obtener la ruta del escritorio
                string desktopPath =
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                // Construir la ruta completa de la carpeta destino
                string rootFolderPath =
                    Path.Combine(desktopPath, rootFolderName);
                // Crear la carpeta si no existe
                if (!Directory.Exists(rootFolderPath))
                {
                    Directory.CreateDirectory(rootFolderPath);
                }

                // Construir la ruta de la carpeta del proyecto
                string projectFolderPath = Path.Combine(rootFolderPath, projectCode);
                // Crear la carpeta si no existe
                if (!Directory.Exists(projectFolderPath))
                {
                    Directory.CreateDirectory(projectFolderPath);
                }

                // Construir la ruta completa del archivo JSON
                string jsonPath = Path.Combine(projectFolderPath, jsonFileName);

                // Guardar el archivo
                File.WriteAllText(jsonPath, jsonContent);
            }
            // catch
            catch (Exception ex)
            {
                // Mensaje
                MessageBox.Show(
                    isSpanish
                        ? $"Error: No se pudo guardar el archivo JSON.\n\n{ex.Message}"
                        : $"Error: The JSON file could not be saved.\n\n{ex.Message}",
                    isSpanish ? "Error de Exportación" : "Export Error"
                );
            }
        }



    }
}
