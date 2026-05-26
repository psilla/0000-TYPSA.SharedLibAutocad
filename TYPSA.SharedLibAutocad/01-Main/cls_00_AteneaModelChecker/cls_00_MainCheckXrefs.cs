using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckXrefs
    {
        private static XrefStatusResult CheckXref(
            string fileName,
            string xRefName,
            bool isLoaded
        )
        {
            return new XrefStatusResult
            {
                FileName = fileName,
                XrefName = xRefName,
                IsLoaded = isLoaded
            };
        }

        public class XrefStatusResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }
            public string XrefName { get; set; }
            public bool IsLoaded { get; set; }
        }

        public static List<XrefStatusResult> AnalyzeXrefs(
            BlockTable bt,
            Transaction tr,
            string fileName
        )
        {
            List<XrefStatusResult> results = new List<XrefStatusResult>();
            // Iteramos
            foreach (ObjectId id in bt)
            {
                // Abrimos cada BlockTableRecord en modo lectura
                BlockTableRecord btr = tr.GetObject(id, OpenMode.ForRead) as BlockTableRecord;
                // Obviamos si no son xRef
                if (!btr.IsFromExternalReference) continue;

                // Vemos si esta cargado
                bool isLoaded = !btr.IsUnloaded;

                // Almacenamos
                results.Add(CheckXref(
                    fileName, btr.Name, isLoaded
                ));
            }

            // return
            return results;
        }

    }
}
