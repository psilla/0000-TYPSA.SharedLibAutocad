using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using System.Reflection;

namespace TYPSA.SharedLib.Autocad.DbObjectsByType
{
    public class cls_00_GetAllDbObjectTypes
    {
        public static List<string> GetAllDbObjectTypes()
        {
            // Lista de tuplas: (FullName, Name)
            var result = new List<(string FullName, string Name)>();
            Type baseType = typeof(DBObject);

            Assembly[] assembliesToScan = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(asm => !asm.IsDynamic)
                .ToArray();

            foreach (var asm in assembliesToScan)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract);

                    foreach (var t in types)
                    {
                        string fullName = t.FullName;
                        string name = t.Name;

                        if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(name))
                            result.Add((fullName, name));
                    }
                }
                catch
                {
                    continue;
                }
            }

            // Ordenar por el nombre corto (Name), pero devolver el FullName
            return result
                .OrderBy(x => x.Name)
                .Select(x => x.FullName)
                .ToList();
        }




    }
}
