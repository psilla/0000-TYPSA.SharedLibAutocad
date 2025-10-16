using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace TYPSA.SharedLib.Autocad.GetEntityCoordinates
{
    public class cls_00_GetCoordX
    {
        public static double GetPositionX(
            DBObject obj
        )
        {
            // BlockReference
            if (obj is BlockReference bloque)
                // Para los bloques usa el punto base
                return bloque.Position.X;
            // Polyline
            else if (obj is Polyline poly)
            {
                // Obtener la coordenada X minima de la polilínea
                double xMin = double.MaxValue;
                // Recorrer los vertices de la polilínea
                for (int i = 0; i < poly.NumberOfVertices; i++)
                {
                    Point2d vertice = poly.GetPoint2dAt(i);
                    if (vertice.X < xMin)
                    {
                        xMin = vertice.X;
                    }
                }
                // return
                return xMin;
            }
            // Valor por defecto
            return 0;
        }




    }
}
