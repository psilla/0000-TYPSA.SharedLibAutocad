using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using AcBr = Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using TYPSA.SharedLib.Autocad.UserForms;

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





    }
}
