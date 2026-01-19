using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetPolyAndLinesByLayerFilter
    {
        public static DBObjectCollection GetPolyAndLinesByLayerFilter(
            Database db,
            Transaction tr,
            BlockTableRecord btr,
            HashSet<string> capasValidas,
            out int lineCount,
            out int polylineCount,
            out int arcCount,
            out HashSet<string> capasUsadas
        )
        {
            // Collection de lines/poly a incluir
            DBObjectCollection result = new DBObjectCollection();

            // Contadores
            lineCount = 0;
            polylineCount = 0;
            arcCount = 0;

            // Capas usadas
            capasUsadas = new HashSet<string>();

            // Obtenemos la tabla de capas
            LayerTable lt = cls_00_DocumentInfo.GetLayerTableForRead(tr, db);

            // Recorremos entidades del espacio modelo
            foreach (ObjectId id in btr)
            {
                // Obtenemos la entidad
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;

                // Obviamos entidades nulas
                if (ent == null) continue;

                // Validamos tipo
                if (ent is Line || ent is Polyline || ent is Arc)
                {
                    // Obtenemos la capa
                    LayerTableRecord layer = cls_00_DocumentInfo.GetLayerFromEntityForRead(tr, lt, ent);

                    // Obviamos capas apagadas o congeladas
                    bool capaVisible = !layer.IsOff && !layer.IsFrozen;
                    bool entidadVisible = ent.Visible; // esta propiedad es adicional

                    // Filtramos por visibilidad y capa válida
                    if (capaVisible && entidadVisible && capasValidas.Contains(ent.Layer))
                    {
                        // Almacenamos
                        result.Add(ent);
                        capasUsadas.Add(ent.Layer);

                        // Contamos
                        if (ent is Line) lineCount++;
                        else if (ent is Polyline) polylineCount++;
                        else if (ent is Arc) arcCount++;
                    }
                }
            }
            // return
            return result;
        }



    }
}
