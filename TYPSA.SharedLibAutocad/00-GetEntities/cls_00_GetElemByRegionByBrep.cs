using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using AcBr = Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using TYPSA.SharedLib.UserForms;
using System;

namespace TYPSA.SharedLib.Autocad.GetEntities
{
    public class cls_00_GetElemByRegionByBrep
    {
        public static bool PointByRegionByBrep(
            Point3d punto,
            Region region
        )
        {
            // Validamos
            if (region == null) return false;

            // try
            try
            {
                // Usamos Brep para verificar si el punto está dentro de la región
                using (Brep brep = new Brep(region))
                {
                    // Validamos
                    if (brep == null)
                        // Mensaje
                        new AutoCloseMessageForm(
                            "❌ BREP generation failed: " +
                            "the resulting object is null.", 1000
                        ).ShowDialog();

                    // Definimos resultado por defecto
                    PointContainment result = PointContainment.Outside;

                    using (BrepEntity ent = brep.GetPointContainment(punto, out result))
                    {
                        // Validamos
                        if (ent is AcBr.Face) result = PointContainment.Inside;
                    }

                    // Si el punto ya está dentro, retornamos true
                    if (result == PointContainment.Inside) return true;
                }
            }
            // catch
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                // Mensaje
                new AutoCloseMessageForm(
                    $"❌ Error checking if the point " +
                    $"is inside the region: {ex.Message}", 1000
                ).ShowDialog();
            }

            // Si llegamos aquí, significa que el punto no está dentro o hubo un error
            return false;
        }

        public static bool PointsByRegionByBrep(
            List<Point3d> puntos,
            Region region
        )
        {
            // Validamos
            if (region == null || puntos == null || puntos.Count == 0)
                return false;

            try
            {
                using (Brep brep = new Brep(region))
                {
                    if (brep == null)
                    {
                        new AutoCloseMessageForm(
                            "❌ BREP generation failed: the resulting object is null.",
                            1000
                        ).ShowDialog();
                        return false;
                    }

                    // Recorremos todos los puntos
                    foreach (Point3d punto in puntos)
                    {
                        PointContainment result = PointContainment.Outside;

                        using (BrepEntity ent = brep.GetPointContainment(punto, out result))
                        {
                            if (ent is AcBr.Face)
                                result = PointContainment.Inside;
                        }

                        // Si algún punto no está dentro → retornamos false directamente
                        if (result != PointContainment.Inside)
                            return false;
                    }

                    // Si todos los puntos están dentro
                    return true;
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                new AutoCloseMessageForm(
                    $"❌ Error checking if points are inside the region: {ex.Message}",
                    1500
                ).ShowDialog();
                return false;
            }
        }


        public static bool PolyByRegionByBrep(
            Polyline polilinea,
            Region region
        )
        {
            // Validamos
            if (polilinea == null || region == null) return false;

            // Obtener los vértices de la polilínea
            List<Point3d> puntosPolilinea = new List<Point3d>();
            for (int i = 0; i < polilinea.NumberOfVertices; i++)
            {
                puntosPolilinea.Add(polilinea.GetPoint3dAt(i));
            }

            bool polilineaDentro = true;

            // Verificar si todos los puntos están dentro de la región
            using (Brep brep = new Brep(region))
            {
                // Validamos
                if (brep == null) return false;

                // Iteramos
                foreach (Point3d punto in puntosPolilinea)
                {
                    // Definimos por defecto
                    PointContainment result = PointContainment.Outside;

                    using (BrepEntity ent = brep.GetPointContainment(punto, out result))
                    {
                        // Validamos
                        if (ent == null)
                        {
                            // Probar con un punto ligeramente desplazado
                            Point3d puntoAlternativo =
                                new Point3d(punto.X + 0.01, punto.Y + 0.01, punto.Z);
                            using (BrepEntity entAlt =
                                brep.GetPointContainment(puntoAlternativo, out result)) { }
                        }

                        if (result != PointContainment.Inside && result != PointContainment.OnBoundary)
                        {
                            polilineaDentro = false;
                        }
                    }
                }
            }

            // return
            return polilineaDentro;
        }
        public static bool PolyCentroidByRegionByBrep(
            Point3d centroid,
            Region region
        )
        {
            // Evaluar si el centroide está dentro o en el límite de la región
            using (Brep brep = new Brep(region))
            {
                if (brep == null)
                    return false;

                PointContainment result = PointContainment.Outside;
                using (BrepEntity ent = brep.GetPointContainment(centroid, out result))
                {
                    if (ent == null)
                    {
                        // Si falla, probamos con un ligero desplazamiento
                        Point3d alt = new Point3d(centroid.X + 0.01, centroid.Y + 0.01, centroid.Z);
                        using (BrepEntity entAlt = brep.GetPointContainment(alt, out result)) { }
                    }
                }

                return result == PointContainment.Inside || result == PointContainment.OnBoundary;
            }
        }

        public static Point3d GetPolylineCentroid(Polyline poly)
        {
            if (poly == null || poly.NumberOfVertices < 3)
                return Point3d.Origin;

            double area = 0.0;
            double cx = 0.0;
            double cy = 0.0;

            for (int i = 0; i < poly.NumberOfVertices; i++)
            {
                Point2d p1 = poly.GetPoint2dAt(i);
                Point2d p2 = poly.GetPoint2dAt((i + 1) % poly.NumberOfVertices); // siguiente (cierra el bucle)

                double cross = (p1.X * p2.Y) - (p2.X * p1.Y);
                area += cross;
                cx += (p1.X + p2.X) * cross;
                cy += (p1.Y + p2.Y) * cross;
            }

            area *= 0.5;

            if (Math.Abs(area) < 1e-9)
                return Point3d.Origin; // evita división por cero

            cx /= (6.0 * area);
            cy /= (6.0 * area);

            // Z del primer vértice
            double z = poly.GetPoint3dAt(0).Z;

            return new Point3d(cx, cy, z);
        }

        public static Point3d GetBlockReferenceCentroid(BlockReference blockRef)
        {
            if (blockRef == null)
                return Point3d.Origin;

            try
            {
                // 1️⃣ Si tiene BoundingBox, usamos su punto medio
                Extents3d? ext = blockRef.Bounds;
                if (ext.HasValue)
                {
                    Point3d min = ext.Value.MinPoint;
                    Point3d max = ext.Value.MaxPoint;

                    return new Point3d(
                        (min.X + max.X) / 2.0,
                        (min.Y + max.Y) / 2.0,
                        (min.Z + max.Z) / 2.0
                    );
                }

                // 2️⃣ Si no tiene bounding box (raro), usamos directamente su posición
                return blockRef.Position;
            }
            catch
            {
                // 3️⃣ Si algo falla (por ejemplo, el bloque no tiene definición cargada)
                return blockRef.Position;
            }
        }









    }
}
