using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetLayersInfo
{
    public class cls_00_GetVisibPrintUnlockLayerNamesFromXref
    {
        public static List<string> GetVisibleAndPlottableLayerNamesFromXref(
            LayerTable layerTable, Transaction tr
)
        {
            return layerTable.Cast<ObjectId>()
                .Select(id => (LayerTableRecord)tr.GetObject(id, OpenMode.ForRead))
                .Where(ltr =>
                    !ltr.IsOff &&
                    !ltr.IsLocked &&
                    ltr.IsPlottable &&
                    ltr.Name.Contains("|") // solo capas de Xrefs
                )
                .Select(ltr => ltr.Name.Split('|').Last())
                .Distinct()
                .ToList();
        }



    }
}
