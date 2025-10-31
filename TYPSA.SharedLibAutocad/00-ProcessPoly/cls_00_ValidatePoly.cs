using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace TYPSA.SharedLib.Autocad.ProcessPoly
{
    public class cls_00_ValidatePoly
    {
        public static Polyline ValidateAndClosePoly(
            Polyline poly,
            double toleranciaCierre
        )
        {
            // Validamos
            if (poly == null) return null;

            // return
            if (poly.Closed) return poly;

            // Obtener puntos inicial y final
            Point3d startPoint = poly.GetPoint3dAt(0);
            Point3d endPoint = poly.GetPoint3dAt(poly.NumberOfVertices - 1);

            // Medir la distancia entre ellos
            double distance = startPoint.DistanceTo(endPoint);

            // Comprobar distancia
            if (distance <= toleranciaCierre)
            {
                // Cerrar automáticamente si la distancia es pequeña
                poly.UpgradeOpen();
                poly.Closed = true;

                // return
                return poly;
            }

            // Por defecto
            return null;
        }



    }
}
