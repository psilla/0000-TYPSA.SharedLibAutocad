using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Text;

namespace TYPSA.SharedLib.Autocad.ProcessPoly
{
    public class cls_00_ProcessPoly
    {
        public static void ProcessPoly(
            Polyline contorno,
            HashSet<ObjectId> polyToIsolate,
            StringBuilder infoPoly,
            List<Polyline> validPoly,
            ref int allPolyCount,
            ref int nullPolyCount,
            ref int validPolyCount,
            string entityTag,
            string projectUnits,
            double toleranciaCierre = 5
        )
        {
            // Contamos las polylines analizadas
            allPolyCount++;

            // Validamos la polilinea
            Polyline contornoValido =
                cls_00_ValidatePoly.ValidateAndClosePoly(contorno, toleranciaCierre);
            // Validamos
            if (contornoValido == null || !contornoValido.Closed)
            {
                // Almacenamos
                polyToIsolate.Add(contorno.ObjectId);

                // Contamos como poly no válida
                nullPolyCount++;

                // Especificar motivo
                string motivo = contornoValido == null
                    ? "could not be validated or closed (invalid geometry)"
                    : "is not closed";

                // Agregar info con motivo
                infoPoly.AppendLine(
                    $"⚠ Discarded {entityTag} Polyline - " +
                    $"ObjectId: {contorno.ObjectId} → Reason: {motivo}"
                );

                // Finalizamos
                return;
            }

            // Registrar la polilínea como válida
            int numVertices = contornoValido.NumberOfVertices;
            double length = contornoValido.Length;
            string layer = contornoValido.Layer;

            // Mostramos
            infoPoly.AppendLine(
                $"✅ Successfully processed {entityTag} Polyline - " +
                $"Handle: {contorno.Handle} | " +
                $"Vertices: {numVertices} | " +
                $"Closed: {contornoValido.Closed} | " +
                $"Length: {length:F2} {projectUnits} | " +
                $"Layer: {layer}"
            );

            // Contamos como poly válida
            validPolyCount++;

            // Almacenamos la poly válida
            validPoly.Add(contorno);
        }




    }
}
