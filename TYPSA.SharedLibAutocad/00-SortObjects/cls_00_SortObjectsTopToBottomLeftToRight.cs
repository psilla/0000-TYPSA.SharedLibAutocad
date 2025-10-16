using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System;
using TYPSA.SharedLib.Autocad.GetEntityCoordinates;

namespace TYPSA.SharedLib.Autocad.SortObjects
{
    public class cls_00_SortObjectsTopToBottomLeftToRight
    {
        public static List<DBObject> SortObjectsTopToBottomLeftToRight(
            List<DBObject> objectList
        )
        {
            // try
            try
            {
                // Lista de filas de objetos (BlockReference o Polyline)
                List<List<DBObject>> filas = new List<List<DBObject>>();

                // Diccionario de polilíneas fallidas con el Handle y el motivo del fallo
                Dictionary<Handle, string> polilineasFallidas = new Dictionary<Handle, string>();

                // Diccionario para almacenar regiones generadas y evitar duplicidad
                Dictionary<Handle, Autodesk.AutoCAD.DatabaseServices.Region> diccRegiones =
                    new Dictionary<Handle, Autodesk.AutoCAD.DatabaseServices.Region>();

                // Ordenar objetos de mayor a menor coordenada Y
                List<DBObject> objectListOrder = objectList
                    .Where(o => o != null) // Evitar objetos nulos
                    .OrderByDescending(o =>
                    {
                        // try
                        try
                        {
                            return cls_00_GetCoordY.GetPositionY(o);
                        }
                        // catch
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            polilineasFallidas[o.Handle] = $"Error getting Y: {ex.Message}";
                            return double.MinValue;
                        }
                    })
                    .ToList();
                // Iteramos
                foreach (var obj in objectListOrder)
                {
                    bool agregado = false;
                    double posYObj = cls_00_GetCoordY.GetPositionY(obj);

                    // Obtener altura del objeto
                    double alturaObj = 0;
                    try
                    {
                        // En caso de ser poly
                        if (obj is Polyline pl)
                        {
                            alturaObj = cls_00_GetPolylineHeight.GetPolylineHeight(pl);
                        }

                        // En caso de ser blockRef
                        else if (obj is BlockReference br)
                        {
                            alturaObj = br.GeometricExtents.MaxPoint.Y - br.GeometricExtents.MinPoint.Y;
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex)
                    {
                        polilineasFallidas[obj.Handle] = $"Error calculating height: {ex.Message}";
                    }

                    // Recorremos las filas
                    foreach (var fila in filas)
                    {
                        double posYFila = cls_00_GetCoordY.GetPositionY(fila[0]);

                        // Obtener altura del primer elemento de la fila
                        double alturaFila = 0;
                        try
                        {
                            // En caso de ser poly
                            if (fila[0] is Polyline plFila)
                            {
                                alturaFila = cls_00_GetPolylineHeight.GetPolylineHeight(plFila);
                            }

                            // En caso de ser blockRef
                            else if (fila[0] is BlockReference brFila)
                            {
                                alturaFila = brFila.GeometricExtents.MaxPoint.Y - brFila.GeometricExtents.MinPoint.Y;
                            }
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            polilineasFallidas[fila[0].Handle] = $"Error calculating row height: {ex.Message}";
                        }

                        // Nuevo criterio: si la diferencia en Y es menor que la mitad de la altura, agrupar
                        //double toleranciaDinamica = Math.Min(alturaObj, alturaFila) / 2.0;
                        double toleranciaDinamica = (alturaObj == 0 || alturaFila == 0)
                            ? 5.0 // Tolerancia por defecto
                            : Math.Min(alturaObj, alturaFila) / 2.0;

                        if (Math.Abs(posYFila - posYObj) < toleranciaDinamica)
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

                // Ordenar los objetos dentro de cada fila de izquierda a derecha
                foreach (var fila in filas)
                {
                    fila.Sort((a, b) =>
                    {
                        // try
                        try
                        {
                            return cls_00_GetCoordX.GetPositionX(a)
                                .CompareTo(cls_00_GetCoordX.GetPositionX(b));
                        }
                        // catch
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            polilineasFallidas[a.Handle] = $"Error getting X: {ex.Message}";
                            return 0;
                        }
                    });
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
                MessageBox.Show($"❌ Fatal error in SortObjectsTopToBottomLeftToRight: {ex.Message}",
                    "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Finalizamos
                return new List<DBObject>();
            }
        }


    }
}
