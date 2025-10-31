using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.ProcessPoly
{
    public class cls_00_CloneAndExpPoly
    {
        public static List<Line> CloneAndExplodePoly(
            Entity ent,
            Dictionary<ObjectId, List<Line>> dictPolyIdExplodedLines
        )
        {
            // Declaramos lista de lineas vacía
            var explodedLines = new List<Line>();

            // En caso de ser una poly
            if (ent is Polyline poly)
            {
                // Obtenemos el ID original
                ObjectId originalId = poly.ObjectId;

                // Clonamos la polyline antes de explotar
                Polyline clone = poly.Clone() as Polyline;

                // Explotamos el clon
                DBObjectCollection exploded = new DBObjectCollection();
                clone.Explode(exploded);

                // Declaramos lista de lineas explotadas vacía
                List<Line> lines = new List<Line>();
                // Por cada segmento obtenido al explotar
                foreach (Entity subEnt in exploded)
                {
                    // En caso de ser linea
                    if (subEnt is Line line)
                    {
                        // Añadimos a la lista
                        lines.Add(line);
                        explodedLines.Add(line);
                    }
                    else
                    {
                        subEnt.Dispose();
                    }
                }
                // En caso de obtener líneas explotadas
                if (lines.Count > 0)
                {
                    // Almacenamos en el dicc
                    dictPolyIdExplodedLines[originalId] = lines;
                }
                // Eliminamos el clon
                clone.Dispose();
            }
            // En caso de ser una linea
            else if (ent is Line line)
            {
                // Clonamos la linea
                Line clone = line.Clone() as Line;
                // Validamos
                if (clone != null)
                {
                    // Añadimos el clon
                    explodedLines.Add(clone);
                    // Almacenamos en el dicc
                    dictPolyIdExplodedLines[line.ObjectId] = new List<Line> { clone };
                }
            }

            // return
            return explodedLines;
        }



    }
}
