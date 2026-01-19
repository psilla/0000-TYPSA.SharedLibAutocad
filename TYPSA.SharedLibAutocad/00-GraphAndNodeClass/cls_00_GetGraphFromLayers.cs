using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using TYPSA.SharedLib.Autocad.GetLayersInfo;
using TYPSA.SharedLib.Autocad.ProcessPoly;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetGraphFromLayers
    {
        public static cls_00_GraphClass.Graph GetGraphFromLayers(
            Database db,
            Transaction tr,
            BlockTableRecord btr,
            HashSet<string> layersByDefault,
            string layersFormMess,
            out List<Line> clonedEntities,
            out Dictionary<ObjectId, Entity> originalEntities,
            out HashSet<string> layersByUserSet,
            out Dictionary<ObjectId, List<Line>> dictPolyIdExplodedLines
        )
        {
            // Lista para almacenar las entidades clonadas
            clonedEntities = new List<Line>();
            // Creamos un diccionario de las entidades originales
            originalEntities = new Dictionary<ObjectId, Entity>();
            // HashSet de capas obtenidas
            layersByUserSet = new HashSet<string>();
            // Dict para relacionar poly original con lineas explotadas
            dictPolyIdExplodedLines = new Dictionary<ObjectId, List<Line>>();

            // Obtenemos las capas deseadas
            layersByUserSet = cls_00_GetSelLayersByDefaultFromDoc.
                GetSelLayersByDefaultFromDoc(layersByDefault, layersFormMess);
            // Validamos
            if (layersByUserSet == null) return null;

            int lineCount, polylineCount, arcCount;
            HashSet<string> capasUsadas;
            // Creamos enumerable de entidades a partir lineas/poly de las capas anteriores
            IEnumerable<Entity> entitiesFromLinesAndPoly =
                cls_00_GetPolyAndLinesByLayerFilterAsEnu.GetPolyAndLinesByLayerFilterAsEnu(
                    db, tr, btr, layersByUserSet,
                    out lineCount, out polylineCount, out arcCount,
                    out capasUsadas
                );
            // Validamos
            if (entitiesFromLinesAndPoly == null) return null;

            // Iteramos
            foreach (Entity ent in entitiesFromLinesAndPoly)
            {
                // Almacenamos
                originalEntities[ent.ObjectId] = ent;
            }

            // Iteramos por las entidades originales
            foreach (Entity ent in entitiesFromLinesAndPoly)
            {
                // Hacemos una copia de las entidades y las explotamos
                clonedEntities.AddRange(
                    cls_00_CloneAndExpPoly.CloneAndExplodePoly(ent, dictPolyIdExplodedLines)
                );
            }

            // Creamos el grafo con esas entidades clonadas y/o explotadas
            cls_00_GraphClass.Graph graph =
                cls_00_GraphTools.BuildGraphFromEntities(clonedEntities);

            // return
            return graph;
        }


    }
}
