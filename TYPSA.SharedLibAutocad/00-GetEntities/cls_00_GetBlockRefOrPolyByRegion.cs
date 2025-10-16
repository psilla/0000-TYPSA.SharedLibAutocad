using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.EntitiesInsertionPoint;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetBlockRefOrPolyByRegion
    {
        public static Dictionary<string, List<DBObject>> GetBlockRefOrPolyByRegion(
            Transaction tr,
            Region region,
            HashSet<ObjectId> dcBlockIds,
            bool boolDcBlocksBool
        )
        {
            Dictionary<string, List<DBObject>> objetosPorRegion =
                new Dictionary<string, List<DBObject>>();

            // try
            try
            {
                // Validamos
                if (region == null) return objetosPorRegion;

                // Obtenemos el handle
                string handleRegion = region.Handle.ToString();
                // Almacenamos el handle en el dict
                objetosPorRegion[handleRegion] = new List<DBObject>();

                // Iteramos
                foreach (ObjectId objId in dcBlockIds)
                {
                    // try
                    try
                    {
                        // Obtenemos el objeto
                        DBObject obj = tr.GetObject(objId, OpenMode.ForWrite);
                        // Validamos
                        if (obj == null) continue;

                        // En caso de ser BlockRef
                        if (boolDcBlocksBool && obj is BlockReference bloque)
                        {
                            // Validamos
                            if (bloque == null) continue;

                            // Obtener el punto de inserción del bloque
                            Point3d basePoint = cls_07_GetEntityInsertionPoint.
                                GetEntityInsertionPoint(bloque);

                            if (cls_00_GetElemByRegionByBrep.PointByRegionByBrep(basePoint, region))
                            {
                                objetosPorRegion[handleRegion].Add(bloque);
                            }
                        }
                        // En caso de ser Poly
                        else if (obj is Polyline poly)
                        {
                            if (cls_00_GetElemByRegionByBrep.PolyByRegionByBrep(poly, region))
                            {
                                objetosPorRegion[handleRegion].Add(poly);
                            }
                        }
                    }
                    // catch
                    catch (System.Exception exObj)
                    {
                        // Mensaje
                        MessageBox.Show(
                            $"⚠ Error processing object {objId}:\n{exObj.Message}",
                            "Object Processing Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                // return
                return objetosPorRegion;
            }
            // catch
            catch (System.Exception ex)
            {
                // Mensaje
                MessageBox.Show(
                    $"❌ Fatal error in GetBlockRefOrPolyByRegion:\n{ex.Message}",
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                // Finalizamos
                return new Dictionary<string, List<DBObject>>();
            }
        }





    }
}
