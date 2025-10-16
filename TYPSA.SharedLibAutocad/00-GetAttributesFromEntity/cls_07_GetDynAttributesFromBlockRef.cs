using Autodesk.AutoCAD.DatabaseServices;

namespace TYPSA.SharedLib.Autocad.GetAttribute
{
    public class cls_07_GetDynAttributesFromBlockRef
    {
        public static string GetDynAttributeFromBlockRef(
            BlockReference blockRef,
            string attributeName
        )
        {
            // Validamos
            if (blockRef == null || !blockRef.IsDynamicBlock) return null;

            // Iteramos
            foreach (DynamicBlockReferenceProperty prop in blockRef.DynamicBlockReferencePropertyCollection)
            {
                // Validamos propiedad
                if (prop.PropertyName == attributeName)
                {
                    // return
                    return prop.Value?.ToString();
                }
            }

            // return
            return null;
        }














    }
}
