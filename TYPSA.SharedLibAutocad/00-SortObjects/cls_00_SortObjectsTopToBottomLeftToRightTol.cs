using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System;
using TYPSA.SharedLib.Autocad.GetEntityCoordinates;

namespace TYPSA.SharedLib.Autocad.SortObjects
{
    public class cls_00_SortObjectsTopToBottomLeftToRightTol
    {
        public static List<DBObject> SortObjectsTopToBottomLeftToRightTol(
            List<DBObject> objectList,
            double toleranciaY // Se usa directamente para la agrupacion en filas
        )
        {
            try
            {
                if (objectList == null || objectList.Count == 0)
                {
                    return new List<DBObject>();
                }

                // Lista de filas de objetos (BlockReference o Polyline)
                List<List<DBObject>> filas = new List<List<DBObject>>();

                // Diccionario de polilíneas fallidas con el Handle y el motivo del fallo
                Dictionary<Handle, string> polilineasFallidas = new Dictionary<Handle, string>();

                // Diccionario para almacenar regiones generadas y evitar duplicidad
                Dictionary<Handle, Autodesk.AutoCAD.DatabaseServices.Region> diccRegiones =
                    new Dictionary<Handle, Autodesk.AutoCAD.DatabaseServices.Region>();

                // Ordenar objetos de mayor a menor coordenada Y
                var objetosOrdenados = objectList
                    .Where(o => o != null) // Evitar objetos nulos
                    .OrderByDescending(o =>
                    {
                        try
                        {
                            return cls_00_GetCoordY.GetPositionY(o);
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            polilineasFallidas[o.Handle] = $"Error getting Y: {ex.Message}";
                            return double.MinValue;
                        }
                    })
                    .ToList();

                // 🔹 Agrupar objetos en filas iniciales 🔹
                foreach (var obj in objetosOrdenados)
                {
                    bool agregado = false;
                    double posYObj = cls_00_GetCoordY.GetPositionY(obj);

                    foreach (var fila in filas)
                    {
                        // 🔹 Verificar si el objeto pertenece a una fila existente (comparando con todos los elementos)
                        if (fila.Any(f => Math.Abs(cls_00_GetCoordY.GetPositionY(f) - posYObj) < toleranciaY))
                        {
                            fila.Add(obj);
                            agregado = true;
                            break;
                        }
                    }

                    if (!agregado)
                    {
                        filas.Add(new List<DBObject> { obj });
                    }
                }

                // Ordenar las filas después de la agrupación
                foreach (var fila in filas)
                {
                    // Ordenar cada fila de izquierda a derecha (menor a mayor X)
                    fila.Sort((a, b) =>
                    {
                        try
                        {
                            return cls_00_GetCoordX.GetPositionX(a)
                                .CompareTo(cls_00_GetCoordX.GetPositionX(b));
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            polilineasFallidas[a.Handle] = $"Error getting X: {ex.Message}";
                            return 0;
                        }
                    });
                }

                // Reasignar la numeración después del orden final
                int contador = 1;
                foreach (var fila in filas)
                {
                    foreach (var obj in fila)
                    {
                        // Aquí debes aplicar la numeración correcta en función de `contador`
                        contador++;
                    }
                }

                // Mostrar mensaje si hay polilíneas fallidas
                if (polilineasFallidas.Count > 0)
                {
                    // Mensaje
                    StringBuilder message = new StringBuilder("⚠ The following polylines could not be converted into regions:\n");
                    foreach (var poly in polilineasFallidas)
                    {
                        message.AppendLine($"- Handle: {poly.Key} | Reason: {poly.Value}");
                    }
                    // Mensaje
                    MessageBox.Show(message.ToString(), "Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Unir todas las filas en un solo listado ordenado
                return filas.SelectMany(fila => fila).ToList();
            }
            // catch
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                // Mensaje
                MessageBox.Show($"❌ Fatal error in SortObjectsTopToBottomLeftToRightTol: {ex.Message}",
                    "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Finalizamos
                return new List<DBObject>();
            }
        }


    }
}
