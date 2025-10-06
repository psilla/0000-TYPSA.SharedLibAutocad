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

































    }
}
