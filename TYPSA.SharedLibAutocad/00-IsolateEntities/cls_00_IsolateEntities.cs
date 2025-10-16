using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace TYPSA.SharedLib.Autocad.IsolateEntities
{
    public class cls_00_IsolateEntities
    {

        public static void CommandIsolate(
            Editor ed,
            IEnumerable<ObjectId> objectIds
        )
        {
            // Seleccionar los objetos
            ed.SetImpliedSelection(objectIds.ToArray());

            // Ejecutar el comando ISOLATEOBJECTS
            Autodesk.AutoCAD.ApplicationServices.Application
                .DocumentManager
                .MdiActiveDocument
                .SendStringToExecute("ISOLATEOBJECTS ", true, false, false);
        }

        public static void IsolateObjects(
            Editor ed,
            HashSet<ObjectId> objetosInvalidos
        )
        {
            // No hacer nada si la lista está vacía
            if (objetosInvalidos.Count == 0) return;

            // Mensaje
            MessageBox.Show(
                $"{objetosInvalidos.Count} invalid objects were found. They will be isolated in AutoCAD.",
                "Isolated Invalid objects"
            );

            // Comando Aislar
            CommandIsolate(ed, objetosInvalidos);
        }

        public static void IsolateObjectsOutsideRegions(
            Editor ed,
            HashSet<ObjectId> todosLosObjetos,
            HashSet<ObjectId> objetosEnRegiones,
            List<Autodesk.AutoCAD.DatabaseServices.Region> validRegion // Agregado para aislar regiones
        )
        {
            // Obtener objetos fuera de regiones
            List<ObjectId> objetosFueraDeRegiones = todosLosObjetos.Where(id => !objetosEnRegiones.Contains(id)).ToList();

            // Lista de ObjectId de regiones a aislar
            List<ObjectId> regionesAislar = new List<ObjectId>();

            // Obtener los ObjectId de las regiones contenidas en validRegion
            foreach (var region in validRegion)
            {
                if (region != null)
                {
                    regionesAislar.Add(region.ObjectId);
                }
            }

            // Si hay objetos fuera de las regiones o regiones a aislar
            if (objetosFueraDeRegiones.Count > 0 || regionesAislar.Count > 0)
            {
                int totalAislar = objetosFueraDeRegiones.Count + regionesAislar.Count;

                MessageBox.Show(
                    $"{totalAislar} objects (including {objetosFueraDeRegiones.Count} blocks and {regionesAislar.Count} regions) will be isolated in AutoCAD.",
                    "Isolated Objects and Regions"
                );

                // Combinar listas de objetos a aislar
                List<ObjectId> objetosAislar = objetosFueraDeRegiones.Concat(regionesAislar).ToList();

                // Comando Aislar
                CommandIsolate(ed, objetosAislar);
            }
        }
































    }
}
