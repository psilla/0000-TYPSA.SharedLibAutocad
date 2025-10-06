using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Text;
using System;
using TYPSA.SharedLib.Autocad.ShowInfoBox;
using static TYPSA.SharedLib.Autocad.ProcessPoly.cls_00_ProcessPolyResult;

namespace TYPSA.SharedLib.Autocad.ProcessPoly
{
    public class cls_00_ProcessAllPoly
    {
        public static ProcessPolyResult ProcessAllPoly(
            SelectionSet analyzePoly,
            Transaction tr,
            string entityTag,
            string projectUnits
        )
        {
            // Inicializar contadores
            int allPolyCount = 0;
            int nullPolyCount = 0;
            int validPolyCount = 0;

            // Conjunto para almacenar las polilíneas no válidas que serán aisladas antes de continuar con el proceso
            HashSet<ObjectId> polyToIsolate = new HashSet<ObjectId>();

            // Lista para almacenar polilíneas válidas para su posterior procesamiento
            List<Polyline> validPoly = new List<Polyline>();

            // StringBuilder para almacenar la información de polylines no válidas
            StringBuilder infoPoly = new StringBuilder();

            // Iterar sobre cada contorno exterior
            foreach (SelectedObject selObjContorno in analyzePoly)
            {
                // Validamos el ObjectId
                if (selObjContorno?.ObjectId == null) continue;

                // Obtenemos el entityTag
                DBObject dbObj = tr.GetObject(selObjContorno.ObjectId, OpenMode.ForWrite, false);

                // En caso de ser polyline
                if (dbObj is Polyline poly)
                {
                    // Procesamos la poly
                    cls_00_ProcessPoly.ProcessPoly(
                        poly, polyToIsolate, infoPoly, validPoly,
                        ref allPolyCount, ref nullPolyCount, ref validPolyCount,
                        entityTag, projectUnits
                    );
                }
            }

            // Agregar el conteo total al inicio del `StringBuilder` de infoPoly
            infoPoly.Insert(0,
                $"📌 Summary of {entityTag} Polylines:{Environment.NewLine}" +
                $"🔹 Total {entityTag} Polylines analyzed: {allPolyCount}{Environment.NewLine}" +
                $"⚠ Null or invalid {entityTag} Polylines: {nullPolyCount}{Environment.NewLine}" +
                $"✅ Successfully processed {entityTag} Polylines: {validPolyCount}{Environment.NewLine}{Environment.NewLine}"
            );

            // Mostrar info
            if (nullPolyCount > 0)
            {
                cls_00_ShowInfoBox.ShowStringBuilder(
                    $"⚠ {entityTag} Issues Found:",
                    infoPoly.ToString()
                );
            }

            // return
            return new ProcessPolyResult
            {
                ValidPolylines = validPoly,
                PolylinesToIsolate = polyToIsolate,
                InfoSummary = infoPoly,
                Total = allPolyCount,
                NullCount = nullPolyCount,
                ValidCount = validPolyCount
            };
        }

        public static ProcessPolyResult ProcessAllPoly(
            List<DBObject> polyObjects,
            Transaction tr,
            string entityTag,
            string projectUnits
        )
        {
            // Inicializar contadores y colecciones
            int allPolyCount = 0;
            int nullPolyCount = 0;
            int validPolyCount = 0;

            // Conjunto para almacenar las polilíneas no válidas que serán aisladas antes de continuar con el proceso
            HashSet<ObjectId> polyToIsolate = new HashSet<ObjectId>();

            // Lista para almacenar polilíneas válidas para su posterior procesamiento
            List<Polyline> validPoly = new List<Polyline>();

            // StringBuilder para almacenar la información de polylines no válidas
            StringBuilder infoPoly = new StringBuilder();

            // Iterar sobre cada contorno exterior
            foreach (DBObject obj in polyObjects)
            {
                // En caso de ser polyline
                if (obj is Polyline poly)
                {
                    // Procesamos la poly
                    cls_00_ProcessPoly.ProcessPoly(
                        poly, polyToIsolate, infoPoly, validPoly,
                        ref allPolyCount, ref nullPolyCount, ref validPolyCount,
                        entityTag, projectUnits
                    );
                }

                // En caso de no ser polyline
                else
                {
                    // Contamos como nula
                    nullPolyCount++;
                    // Aislamos
                    polyToIsolate.Add(obj.ObjectId);
                }

                // Contamos como poly analizada
                allPolyCount++;
            }

            // Agregar el conteo total al inicio del `StringBuilder` de infoPoly
            infoPoly.Insert(0,
                $"📌 Summary of {entityTag} Polylines:{Environment.NewLine}" +
                $"🔹 Total {entityTag} Polylines analyzed: {allPolyCount}{Environment.NewLine}" +
                $"⚠ Null or invalid {entityTag} Polylines: {nullPolyCount}{Environment.NewLine}" +
                $"✅ Successfully processed {entityTag} Polylines: {validPolyCount}{Environment.NewLine}{Environment.NewLine}"
            );

            //// Mostrar información
            //cls_00_ShowInfoBox.ShowStringBuilder(
            //    $"📌 {entityTag} Summary:",
            //    infoPoly.ToString()
            //);

            // return
            return new ProcessPolyResult
            {
                ValidPolylines = validPoly,
                PolylinesToIsolate = polyToIsolate,
                InfoSummary = infoPoly,
                Total = allPolyCount,
                NullCount = nullPolyCount,
                ValidCount = validPolyCount
            };
        }






    }
}
