using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.EntitiesInsertionPoint;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetEntityDictByRegion
    {
        private static bool BelongsToRegionByEntity(
            DBObject obj, 
            Region region
        )
        {
            // Casos
            switch (obj)
            {
                case BlockReference bloque:
                    return BelongsBlockToRegion(bloque, region);

                case Polyline poly:
                    return BelongsPolylineToRegion(poly, region);

                case MText mText:
                    return BelongsMTextToRegion(mText, region);

                case DBText dbText:
                    return BelongsDBTextToRegion(dbText, region);

                default:
                    return false;
            }
        }

        private static bool BelongsBlockToRegion(
            BlockReference bloque, 
            Region region
        )
        {
            // Intento 1: Pto base
            Point3d point = cls_00_GetEntityInsertionPoint.GetEntityInsertionPoint(bloque);
            // Validamos
            if (cls_00_GetElemByRegionByBrep.PointByRegionByBrep(point, region)) return true;

            // Intento 2: Centroide
            point = cls_00_GetElemByRegionByBrep.GetBlockReferenceCentroid(bloque);
            // return
            return cls_00_GetElemByRegionByBrep.PointByRegionByBrep(point, region);
        }

        private static bool BelongsPolylineToRegion(
            Polyline poly, 
            Region region
        )
        {
            // Obtener el centroide de la Polyline
            Point3d point = cls_00_GetElemByRegionByBrep.GetPolylineCentroid(poly);
            // return
            return cls_00_GetElemByRegionByBrep.PolyCentroidByRegionByBrep(point, region);
        }

        private static bool BelongsMTextToRegion(
            MText mText, 
            Region region
        )
        {
            // Intento 1: Pto base
            Point3d point = cls_00_GetEntityInsertionPoint.GetEntityInsertionPoint(mText);
            // Validamos
            if (cls_00_GetElemByRegionByBrep.PointByRegionByBrep(point, region)) return true;

            // Intento 2: GeometricExtents
            // try
            try
            {
                Extents3d ext = mText.GeometricExtents;
                // Obtenemos pto
                point = new Point3d(
                    (ext.MinPoint.X + ext.MaxPoint.X) * 0.5,
                    (ext.MinPoint.Y + ext.MaxPoint.Y) * 0.5,
                    (ext.MinPoint.Z + ext.MaxPoint.Z) * 0.5
                );
                // return
                return cls_00_GetElemByRegionByBrep.PointByRegionByBrep(point, region);
            }
            // catch
            catch
            {
                // Por defecto
                return false;
            }
        }

        private static bool BelongsDBTextToRegion(
            DBText dbText,
            Region region
        )
        {
            // Intento 1: Pto base
            Point3d point = cls_00_GetEntityInsertionPoint.GetEntityInsertionPoint(dbText);
            // Validamos
            if (cls_00_GetElemByRegionByBrep.PointByRegionByBrep(point, region)) return true;

            // Intento 2: GeometricExtents
            // try
            try
            {
                Extents3d ext = dbText.GeometricExtents;
                // Obtenemos pto
                point = new Point3d(
                    (ext.MinPoint.X + ext.MaxPoint.X) * 0.5,
                    (ext.MinPoint.Y + ext.MaxPoint.Y) * 0.5,
                    (ext.MinPoint.Z + ext.MaxPoint.Z) * 0.5
                );
                // return
                return cls_00_GetElemByRegionByBrep.PointByRegionByBrep(point, region);
            }
            // catch
            catch
            {
                // Por defecto
                return false;
            }
        }

        public static Dictionary<string, List<DBObject>> GetEntityDictByRegionByPoint(
            Transaction tr,
            Region region,
            HashSet<ObjectId> elemIds
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
                foreach (ObjectId objId in elemIds)
                {
                    // try
                    try
                    {
                        // Obtenemos el objeto
                        DBObject obj = tr.GetObject(objId, OpenMode.ForWrite);
                        // Validamos
                        if (obj == null) continue;

                        // Validamos en region
                        if (BelongsToRegionByEntity(obj, region))
                        {
                            objetosPorRegion[handleRegion].Add(obj);
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

        public static Dictionary<string, List<DBObject>> GetEntityDictByRegionByPoints(
            Transaction tr,
            Region region,
            HashSet<ObjectId> elemIds
        )
        {
            Dictionary<string, List<DBObject>> objetosPorRegion =
                new Dictionary<string, List<DBObject>>();

            try
            {
                if (region == null) return objetosPorRegion;

                string handleRegion = region.Handle.ToString();
                objetosPorRegion[handleRegion] = new List<DBObject>();

                foreach (ObjectId objId in elemIds)
                {
                    try
                    {
                        DBObject obj = tr.GetObject(objId, OpenMode.ForRead);
                        if (obj == null || !(obj is BlockReference bloque))
                            continue;

                        // Obtenemos el BlockTableRecord (definición del bloque)
                        BlockTableRecord btr =
                            (BlockTableRecord)tr.GetObject(bloque.BlockTableRecord, OpenMode.ForRead);

                        // Lista de puntos de todas las entidades del bloque
                        List<Point3d> puntos = new List<Point3d>();

                        // Recorremos cada entidad dentro del bloque
                        foreach (ObjectId entId in btr)
                        {
                            Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                            if (ent == null) continue;

                            // Extraer puntos según tipo de entidad
                            if (ent is Line line)
                            {
                                puntos.Add(line.StartPoint.TransformBy(bloque.BlockTransform));
                                puntos.Add(line.EndPoint.TransformBy(bloque.BlockTransform));
                            }
                            else if (ent is Polyline pline)
                            {
                                for (int i = 0; i < pline.NumberOfVertices; i++)
                                {
                                    Point3d pt = pline.GetPoint3dAt(i).TransformBy(bloque.BlockTransform);
                                    puntos.Add(pt);
                                }
                            }
                            else if (ent is Arc arc)
                            {
                                puntos.Add(arc.StartPoint.TransformBy(bloque.BlockTransform));
                                puntos.Add(arc.EndPoint.TransformBy(bloque.BlockTransform));
                                puntos.Add(arc.Center.TransformBy(bloque.BlockTransform));
                            }
                            else if (ent is Circle circle)
                            {
                                // Usamos el centro y 4 puntos cardinales para aproximar el perímetro
                                Point3d c = circle.Center.TransformBy(bloque.BlockTransform);
                                double r = circle.Radius;
                                puntos.Add(c);
                                puntos.Add(c + new Vector3d(r, 0, 0));
                                puntos.Add(c + new Vector3d(-r, 0, 0));
                                puntos.Add(c + new Vector3d(0, r, 0));
                                puntos.Add(c + new Vector3d(0, -r, 0));
                            }
                        }

                        // Si no hay puntos, continuar
                        if (puntos.Count == 0) continue;

                        // Verificar si TODOS los puntos están dentro de la región
                        bool allInside = cls_00_GetElemByRegionByBrep.PointsByRegionByBrep(puntos, region);

                        if (allInside)
                        {
                            objetosPorRegion[handleRegion].Add(bloque);
                        }
                    }
                    catch (System.Exception exObj)
                    {
                        MessageBox.Show(
                            $"⚠ Error processing BlockReference {objId}:\n{exObj.Message}",
                            "Object Processing Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }

                return objetosPorRegion;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"❌ Fatal error in GetBlockRefsByRegionByPoints:\n{ex.Message}",
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return new Dictionary<string, List<DBObject>>();
            }
        }






    }
}
