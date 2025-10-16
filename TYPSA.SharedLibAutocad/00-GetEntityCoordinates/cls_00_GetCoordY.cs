using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace TYPSA.SharedLib.Autocad.GetEntityCoordinates
{
    public class cls_00_GetCoordY
    {
        public static double GetPositionY(
            DBObject obj
        )
        {
            // BlockReference
            if (obj is BlockReference bloque)
                // Para los bloques usa el punto base
                return bloque.Position.Y;
            // Polyline
            else if (obj is Polyline poly)
            {
                // Obtener la coordenada Y minima de la polilínea
                double maxY = double.MinValue;
                // Recorrer los vertices de la polilínea
                for (int i = 0; i < poly.NumberOfVertices; i++)
                {
                    Point2d vertice = poly.GetPoint2dAt(i);
                    if (vertice.Y > maxY)
                    {
                        maxY = vertice.Y;
                    }
                }
                // return
                return maxY;
            }
            // Valor por defecto
            return double.MinValue;
        }




    }
}
