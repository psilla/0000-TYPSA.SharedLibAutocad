using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckAcadVersion;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckBlockAttr;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckBlockRefsInLayouts;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckBlocksInUse;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckByLayerProp;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckEntInLayerZero;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckEntityTypes;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckFileSize;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckLayersInUse;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckPaperTextFont;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckProjUnits;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckRevClouds;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainCheckXrefs;
using static TYPSA.SharedLib.Autocad.Main.cls_00_MainGetPlogTag;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using TYPSA.SharedLib.EndPoints;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class CadSessionInfo
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

    public class EntityNames
    {
        public string Polyline { get; set; } = "Polyline";

        public static EntityNames GetDefaultEntityNames()
        {
            return new EntityNames();
        }
    }

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

    public class ModelCheckerKeys
    {
        // -----------------------------
        // Public Properties
        // -----------------------------

        public string ProjectUnits { get; }
        public string LayersInUse { get; }
        public string LayerZero { get; }
        public string Version { get; }
        public string Xrefs { get; }
        public string CoordSystem { get; }
        public string PaperTextFont { get; }
        public string PurgeableStyles { get; }
        public string FileSize { get; }
        public string BlocksInUse { get; }
        public string BlockRefsInLayouts { get; }
        public string EntityTypes { get; }
        public string CivilStyles { get; }
        public string CivilObjects { get; }
        public string EntByLayer { get; }
        public string RevCloud { get; }
        public string AttrBlockRef { get; }
        public string PlotTag { get; }

        // -----------------------------
        // Constructor
        // -----------------------------

        public ModelCheckerKeys(bool isSpanish)
        {
            ProjectUnits = GetProjectUnits(isSpanish);
            LayersInUse = GetLayersInUse(isSpanish);
            LayerZero = GetLayerZero(isSpanish);
            Version = GetVersion(isSpanish);
            Xrefs = GetXrefs(isSpanish);
            CoordSystem = GetCoordSystem(isSpanish);
            PaperTextFont = GetPaperTextFont(isSpanish);
            PurgeableStyles = GetPurgeableStyles(isSpanish);
            FileSize = GetFileSize(isSpanish);
            BlocksInUse = GetBlocksInUse(isSpanish);
            BlockRefsInLayouts = GetBlockRefsInLayouts(isSpanish);
            EntityTypes = GetEntityTypes(isSpanish);
            CivilStyles = GetCivilStyles(isSpanish);
            CivilObjects = GetCivilObjects(isSpanish);
            EntByLayer = GetByLayerProperties(isSpanish);
            RevCloud = GetRevisionClouds(isSpanish);
            AttrBlockRef = GetBlockAttributes(isSpanish);
            PlotTag = GetPlotTag(isSpanish);
        }

        // -----------------------------
        // Helpers
        // -----------------------------

        private static string GetText(
            bool es,
            string esTitle,
            string esDesc,
            string enTitle,
            string enDesc,
            bool includeDescription = true
        )
        {
            if (es)
                return includeDescription
                    ? $"{esTitle}: {esDesc}"
                    : esTitle;

            return includeDescription
                ? $"{enTitle}: {enDesc}"
                : enTitle;
        }

        // -----------------------------
        // Keys
        // -----------------------------

        public static string GetProjectUnits(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Unidades del proyecto", "Comprueba las unidades del dibujo (longitud, ángulos, etc.)",
                "Project Units", "Checks drawing units (length, angles, etc.)",
                includeDescription
            );

        public static string GetLayersInUse(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Capas en uso", "Analiza qué capas están en uso y cuántas entidades contienen",
                "Layers in Use", "Analyzes which layers are used and entity count per layer",
                includeDescription
            );

        public static string GetLayerZero(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Entidades en capa 0", "Detecta entidades dibujadas en la capa 0",
                "Entities in Layer 0", "Detects entities drawn on layer 0",
                includeDescription
            );

        public static string GetVersion(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Versión del software", "Obtiene la versión de AutoCAD del archivo",
                "Software Version", "Retrieves AutoCAD file version",
                includeDescription
            );

        public static string GetXrefs(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Referencias externas", "Lista las referencias externas y su estado",
                "External References", "Lists external references and their status",
                includeDescription
            );

        public static string GetCoordSystem(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Sistema de coordenadas", "Obtiene el sistema de coordenadas del dibujo",
                "Coordinates System", "Retrieves drawing coordinate system",
                includeDescription
            );

        public static string GetPaperTextFont(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Fuente de texto en layout", "Analiza las fuentes de texto utilizadas en layouts",
                "Text Font in Layout", "Analyzes text fonts used in layouts",
                includeDescription
            );

        public static string GetByLayerProperties(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Propiedades por capa", "Verifica si color, tipo de línea y grosor están por capa",
                "ByLayer Properties", "Checks if color, linetype and lineweight are ByLayer",
                includeDescription
            );

        public static string GetRevisionClouds(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Nubes de revisión", "Detecta nubes de revisión en layouts",
                "Revision Clouds", "Detects revision clouds in layouts",
                includeDescription
            );

        public static string GetBlockAttributes(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Atributos de bloques", "Extrae atributos de bloques en layouts (title blocks, etc.)",
                "Block Attributes", "Extracts block attributes in layouts (title blocks, etc.)",
                includeDescription
            );

        public static string GetPlotTag(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Ruta de impresión", "Extrae información de Ritningsfil, Plottdatum y Plottad av",
                "Plot Information", "Extracts Ritningsfil, Plottdatum and Plottad av information",
                includeDescription
            );

        public static string GetAssemblies(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Ensamblajes", "Obtiene los ensamblajes existentes en el archivo Civil 3D",
                "Assemblies", "Retrieves existing assemblies in the Civil 3D file",
                includeDescription
            );

        public static string GetSubassemblies(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Subensamblajes", "Obtiene los subensamblajes existentes en el archivo Civil 3D",
                "Subassemblies", "Retrieves existing subassemblies in the Civil 3D file",
                includeDescription
            );

        public static string GetSurfaces(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Superficies", "Obtiene las superficies existentes en el archivo Civil 3D",
                "Surfaces", "Retrieves existing surfaces in the Civil 3D file",
                includeDescription
            );

        public static string GetSites(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Emplazamientos", "Obtiene los emplazamientos existentes en el archivo Civil 3D",
                "Sites", "Retrieves existing sites in the Civil 3D file",
                includeDescription
            );

        public static string GetFileSize(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Tamaño de archivo", "Comprueba que el archivo no exceda los 200 MB",
                "File Size", "Checks that the file does not exceed 200 MB",
                includeDescription
            );

        public static string GetBlocksInUse(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Bloques en uso", "Obtiene los bloques presentes en el dibujo indicando cuáles están en uso",
                "Blocks in Use", "Retrieves blocks present in the drawing indicating which ones are in use",
                includeDescription
            );

        public static string GetEntityTypes(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Tipos de elementos", "Identifica los tipos de elementos presentes en el modelo, indicando categoría, capa y handle",
                "Entity Types", "Identifies element types present in the model, including category, layer and handle",
                includeDescription
            );

        public static string GetPurgeableStyles(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Estilos purgables", "Identifica estilos de Civil 3D no utilizados que podrían ser purgados",
                "Purgeable Styles", "Identifies unused Civil 3D styles that could be purged",
                includeDescription
            );

        public static string GetBodies(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Bodies", "Detecta entidades Body presentes en el dibujo",
                "Bodies", "Detects Body entities present in the drawing",
                includeDescription
            );

        public static string GetStructures(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Structures", "Detecta estructuras de redes de tuberías presentes en el dibujo",
                "Structures", "Detects pipe network structures present in the drawing",
                includeDescription
            );

        public static string GetAlignmentsStyles(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Alineaciones", "Obtiene las alineaciones existentes en el archivo Civil 3D",
                "Alignments", "Retrieves existing alignments in the Civil 3D file",
                includeDescription
            );

        public static string GetCorridorsStyles(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Corredores", "Obtiene los corredores existentes en el archivo Civil 3D",
                "Corridors", "Retrieves existing corridors in the Civil 3D file",
                includeDescription
            );

        public static string GetFeatureLinesStyles(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Líneas características", "Obtiene las líneas características existentes en el archivo Civil 3D",
                "Feature Lines", "Retrieves existing feature lines in the Civil 3D file",
                includeDescription
            );

        public static string GetPipesStyles(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Tuberías", "Obtiene las tuberías existentes en el archivo Civil 3D",
                "Pipes", "Retrieves existing pipes in the Civil 3D file",
                includeDescription
            );

        public static string GetPressureNetworksStyles(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Redes a presión", "Obtiene las redes a presión existentes en el archivo Civil 3D",
                "Pressure Networks", "Retrieves existing pressure networks in the Civil 3D file",
                includeDescription
            );

        public static string GetCivilStyles(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Estilos de objetos Civil", "Analiza estilos utilizados por objetos Civil 3D como alineaciones, corredores, tuberías y redes",
                "Civil Object Styles", "Analyzes styles used by Civil 3D objects such as alignments, corridors, pipes and networks",
                includeDescription
            );

        public static string GetCivilObjects(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Objetos Civil", "Analiza objetos Civil 3D como ensamblajes, superficies, emplazamientos, bodies y structures",
                "Civil Objects", "Analyzes Civil 3D objects such as assemblies, surfaces, sites, bodies and structures",
                includeDescription
            );

        public static string GetBlockRefsInLayouts(bool es, bool includeDescription = true) =>
            GetText(
                es,
                "Bloques en layout", "Obtiene las referencias de bloque insertadas en layouts",
                "Block References in Layout", "Retrieves block references inserted in layouts",
                includeDescription
            );

        public static List<string> GetAllOptions(
            bool isSpanish,
            bool includeDescription = true
        )
        {
            // return
            return new List<string>
            {
                GetProjectUnits(isSpanish, includeDescription),
                GetLayersInUse(isSpanish, includeDescription),
                GetLayerZero(isSpanish, includeDescription),
                GetVersion(isSpanish, includeDescription),
                GetXrefs(isSpanish, includeDescription),
                GetPaperTextFont(isSpanish, includeDescription),
                GetByLayerProperties(isSpanish, includeDescription),
                GetRevisionClouds(isSpanish, includeDescription),
                GetBlockAttributes(isSpanish, includeDescription),
                GetPlotTag(isSpanish, includeDescription)
            };
        }

        public static List<string> GetDefaultSelectedOptions(
            bool isSpanish,
            bool includeDescription = true
        )
        {
            return GetAllOptions(isSpanish, includeDescription);
        }

        public static List<string> GetAllOptionsAteneaCivilCustom(
            bool isSpanish
        )
        {
            // return
            return new List<string>
            {
                GetProjectUnits(isSpanish, true),
                GetLayersInUse(isSpanish, true),
                GetLayerZero(isSpanish, true),
                GetVersion(isSpanish, true),
                GetXrefs(isSpanish, true),
                GetPaperTextFont(isSpanish, true),
                GetCoordSystem(isSpanish, true),
                GetFileSize(isSpanish, true),
                GetPurgeableStyles(isSpanish, true),
                GetBlockRefsInLayouts(isSpanish, true),
                GetBlocksInUse(isSpanish, true),
                GetEntityTypes(isSpanish, true),
                GetCivilStyles(isSpanish, true),
                GetCivilObjects(isSpanish, true)
            };
        }

        public static List<string> GetDefaultSelectedOptionsAteneaCivilCustom(
            bool isSpanish
        )
        {
            return GetAllOptionsAteneaCivilCustom(isSpanish);
        }
    }


    public class ModelCheckerResults
    {
        public List<ProjectUnitsResult> ProjectUnits { get; set; } = new List<ProjectUnitsResult>();
        public List<LayerUsageResult> LayersInUse { get; set; } = new List<LayerUsageResult>();
        public List<LayerZeroUsageResult> LayerZero { get; set; } = new List<LayerZeroUsageResult>();
        public List<AcadVersionResult> Version { get; set; } = new List<AcadVersionResult>();
        public List<XrefStatusResult> Xrefs { get; set; } = new List<XrefStatusResult>();
        public List<PaperTextFontResult> PaperTextFont { get; set; } = new List<PaperTextFontResult>();
        public List<ByLayerEntityResult> ByLayer { get; set; } = new List<ByLayerEntityResult>();
        public List<RevisionCloudResult> RevisionClouds { get; set; } = new List<RevisionCloudResult>();
        public List<BlockAttributesResult> BlockAttributes { get; set; } = new List<BlockAttributesResult>();
        public List<PlotInfoResult> PlotTags { get; set; } = new List<PlotInfoResult>();
        public List<FileSizeResult> FileSize { get; set; } = new List<FileSizeResult>();
        public List<BlockUsageResult> BlocksInUse { get; set; } = new List<BlockUsageResult>();
        public List<EntityTypeResult> EntityTypes { get; set; } = new List<EntityTypeResult>();
        public List<BlockRefLayoutResult> BlockRefsInLayouts { get; set; } = new List<BlockRefLayoutResult>();
    }

    

    public class cls_00_CadInfoHelper
    {
        public static CadSessionInfo GetCivilSessionInfo()
        {
            int year = 2000 + Application.Version.Major;

            return new CadSessionInfo
            {
                CivilVersion = $"Autodesk Civil 3D {year}",
                //CivilVersion = "Autodesk Civil 3D " + Application.Version.ToString(),
                UserName = Environment.UserName,
                DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CivilLanguage = System.Globalization.CultureInfo.InstalledUICulture.DisplayName,
                AteneaVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                SerapisMetrics = "69c59742e4dfd32366266eb8"
            };
        }

        public static Dictionary<string, object> GetParamSetDictionary(
            string projectCode,
            string softwareLanguage,
            int paramSetStatus
        )
        {
            CadSessionInfo sessionInfo = GetCivilSessionInfo();
            // return
            return new Dictionary<string, object>
            {
                { cls_00_AteneaJson.User, sessionInfo.UserName },
                { cls_00_AteneaJson.ProjectCode, projectCode },
                { cls_00_AteneaJson.CivilVersion, sessionInfo.CivilVersion },
                { cls_00_AteneaJson.CivilLanguage , softwareLanguage },
                { cls_00_AteneaJson.AteneaVersion, sessionInfo.AteneaVersion },
                { cls_00_AteneaJson.Status, paramSetStatus }
            };
        }

        public static Dictionary<string, object> GetProjectDataDictionary(
            string projectCode,
            string softwareLanguage
        )
        {
            CadSessionInfo sessionInfo = GetCivilSessionInfo();
            // return
            return new Dictionary<string, object>
            {
                { cls_00_AteneaJson.User, sessionInfo.UserName },
                { cls_00_AteneaJson.ProjectCode, projectCode },
                { cls_00_AteneaJson.CivilVersion, sessionInfo.CivilVersion },
                { cls_00_AteneaJson.CivilLanguage , softwareLanguage },
                { cls_00_AteneaJson.AteneaVersion, sessionInfo.AteneaVersion }
            };
        }

        public static Dictionary<string, object> GetFinalJsonDictionary(
            string projectCode,
            string softwareLanguage,
            List<Dictionary<string, object>> dataJsonByModel
        )
        {
            Dictionary<string, object> baseDict = GetProjectDataDictionary(projectCode, softwareLanguage);
            // Añadimos info
            baseDict[cls_00_AteneaJson.DataByFileName] = dataJsonByModel;
            // return
            return baseDict;
        }

        public static Dictionary<string, object> GetSetStatusDictionary(
            string projectCode
        )
        {
            CadSessionInfo sessionInfo = GetCivilSessionInfo();
            // return
            return new Dictionary<string, object>
            {
                { cls_00_AteneaJson.User, sessionInfo.UserName },
                { cls_00_AteneaJson.ProjectCode, projectCode },
                { cls_00_AteneaJson.AteneaVersion, sessionInfo.AteneaVersion }
            };
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
