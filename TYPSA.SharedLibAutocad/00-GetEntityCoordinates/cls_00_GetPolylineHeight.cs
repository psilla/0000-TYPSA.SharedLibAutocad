using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetEntityCoordinates
{
    public class cls_00_GetPolylineHeight
    {
        public static double GetPolylineHeight(Polyline pl)
        {
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                double y = pl.GetPoint2dAt(i).Y;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }
            // Devuelve la altura total de la polilínea
            return maxY - minY;
        }



    }
}
