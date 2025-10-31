using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace TYPSA.SharedLib.Autocad.GetEntityCoordinates
{
    public class cls_00_GetBottomPoint
    {
        public static Point3d GetBottomEdgeMidPoint(Polyline poly, BlockReference tracker)
        {
            double minY = double.MaxValue;
            Point3d midPoint = Point3d.Origin;

            int n = poly.NumberOfVertices;

            for (int i = 0; i < n; i++)
            {
                Point3d p1 = poly.GetPoint3dAt(i).TransformBy(tracker.BlockTransform);
                Point3d p2 = poly.GetPoint3dAt((i + 1) % n).TransformBy(tracker.BlockTransform);

                // Tomamos el segmento con el Y medio más bajo en global
                double avgY = (p1.Y + p2.Y) / 2.0;
                if (avgY < minY)
                {
                    minY = avgY;
                    midPoint = new Point3d(
                        (p1.X + p2.X) / 2.0,
                        (p1.Y + p2.Y) / 2.0,
                        (p1.Z + p2.Z) / 2.0
                    );
                }
            }
            // return
            return midPoint;
        }

        public static Point3d GetBottomEdgeLeftPoint(Polyline poly, BlockReference tracker)
        {
            double minY = double.MaxValue;
            int n = poly.NumberOfVertices;
            LineSegment3d bottomEdge = null;

            // 1️⃣ Encontrar la arista inferior (la de menor Y promedio)
            for (int i = 0; i < n; i++)
            {
                Point3d p1 = poly.GetPoint3dAt(i).TransformBy(tracker.BlockTransform);
                Point3d p2 = poly.GetPoint3dAt((i + 1) % n).TransformBy(tracker.BlockTransform);

                double avgY = (p1.Y + p2.Y) / 2.0;
                if (avgY < minY)
                {
                    minY = avgY;
                    bottomEdge = new LineSegment3d(p1, p2);
                }
            }

            // 2️⃣ Determinar el punto más a la izquierda (menor X) de esa arista
            if (bottomEdge != null)
            {
                Point3d p1 = bottomEdge.StartPoint;
                Point3d p2 = bottomEdge.EndPoint;

                return (p1.X <= p2.X) ? p1 : p2;
            }

            // 3️⃣ Si algo falla, devolver el origen
            return Point3d.Origin;
        }


        public static Point3d GetBottomMidPoint(Polyline poly, BlockReference tracker)
        {
            // Calcular punto medio inferior en coordenadas locales del bloque
            Extents3d ext = poly.GeometricExtents;
            double midX = (ext.MinPoint.X + ext.MaxPoint.X) / 2.0;
            double minY = ext.MinPoint.Y;

            Point3d basePoint = new Point3d(midX, minY, 0);

            // Transformar al sistema del modelo usando el BlockReference
            return basePoint.TransformBy(tracker.BlockTransform);
        }


    }
}
