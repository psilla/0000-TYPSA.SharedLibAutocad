using System.Collections.Generic;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckXrefs
    {
        private static XrefStatusResult CheckXref(
            string fileName,
            string xRefName,
            bool isLoaded,
            bool isOverlay,
            string savedPath,
            string fullPath
        )
        {
            // return
            return new XrefStatusResult
            {
                FileName = fileName,
                XrefName = xRefName,
                IsLoaded = isLoaded,
                IsOverlay = isOverlay,
                SavedPath = savedPath,
                FullPath = fullPath
            };
        }

        public class XrefStatusResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }
            public string XrefName { get; set; }
            public bool IsLoaded { get; set; }
            public bool IsOverlay { get; set; }
            // Ruta almacenada en el DWG (puede ser relativa)
            public string SavedPath { get; set; }
            // Ruta absoluta resuelta
            public string FullPath { get; set; }
        }

        public static List<XrefStatusResult> AnalyzeXrefs(
            Database db,
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

                string savedPath = btr.PathName;
                string fullPath = savedPath;
                // Resolver rutas relativas
                if (!string.IsNullOrWhiteSpace(savedPath) &&
                    !Path.IsPathRooted(savedPath) &&
                    !string.IsNullOrWhiteSpace(db.Filename))
                {
                    string dwgFolder = Path.GetDirectoryName(db.Filename);
                    fullPath = Path.GetFullPath(
                        Path.Combine(dwgFolder, savedPath)
                    );
                }

                // Almacenamos
                results.Add(CheckXref(
                    fileName,
                    btr.Name,
                    !btr.IsUnloaded,
                    btr.IsFromOverlayReference,
                    savedPath,
                    fullPath
                ));
            }

            // return
            return results;
        }

    }
}
