using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetEntityListByRegion
    {
        public static List<Entity> GetEntityListByRegionByPoint(
            Transaction tr,
            Region entityRegion,
            HashSet<ObjectId> entitiesIds
        )
        {
            // Diccionario de Entidades por region
            Dictionary<string, List<DBObject>> dictEntByRegion =
                cls_00_GetEntityDictByRegion.GetEntityDictByRegionByPoint(
                    tr, entityRegion, entitiesIds
                );
            // Validamos
            if (dictEntByRegion == null || dictEntByRegion.Count == 0) return null;

            // Obtenemos lista de Objetos por region 
            List<DBObject> objList =
                dictEntByRegion.SelectMany(kv => kv.Value).ToList();
            // Validamos
            if (objList.Count == 0) return null;

            // Convertimos a entidades
            List<Entity> entList = objList.OfType<Entity>().ToList();
            // Validamos
            if (entList.Count == 0) return null;

            // return
            return entList;
        }

        public static List<Entity> GetEntityListByRegionByPoints(
            Transaction tr,
            Region entityRegion,
            HashSet<ObjectId> entitiesIds
        )
        {
            // Diccionario de Entidades por región
            Dictionary<string, List<DBObject>> dictEntByRegion =
                cls_00_GetEntityDictByRegion.GetEntityDictByRegionByPoints(
                    tr, entityRegion, entitiesIds
                );
            // Obtenemos lista de Objetos por region 
            List<DBObject> objList =
                dictEntByRegion.SelectMany(kv => kv.Value).ToList();
            // Convertimos a entidades
            List<Entity> entList = objList.OfType<Entity>().ToList();
            // Return
            return entList;
        }


    }
}
