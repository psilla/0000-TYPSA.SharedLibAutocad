using Autodesk.AutoCAD.DatabaseServices;
using TYPSA.SharedLib.Autocad.GetDocument;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckProjUnits
    {
        private static ProjectUnitsResult CheckUnits(
            string units,
            string fileName
        )
        {
            string u = units?.ToLower() ?? "";
            // Validamos
            bool isMeters = u.Contains("meter") || u.Contains("metro");
            // return
            return new ProjectUnitsResult
            {
                FileName = fileName,
                Units = units,
                IsMeters = isMeters
            };
        }

        public class ProjectUnitsResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }
            public string Units { get; set; }
            public bool IsMeters { get; set; }
        }

        public static ProjectUnitsResult AnalyzeUnits(
            Database db, 
            string fileName
        )
        {
            // Obtenemos unidades
            string units = cls_00_DocumentInfo.GetDrawingUnitsName();

            // Evaluamos
            ProjectUnitsResult unitsResult = CheckUnits(units, fileName);

            // return
            return unitsResult;
        }

        
    }
}
