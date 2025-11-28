using System;
using System.Collections.Generic;

namespace TYPSA.SharedLib.Autocad.DbObjectsByType
{
    public class cls_00_ObjectsByTypeByLayerFilt
    {
        public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> infoByObjFiltByPset(
            Dictionary<string, Dictionary<string, Dictionary<string, object>>> fullData,
            string pSetName
        )
        {
            Dictionary<string, Dictionary<string, Dictionary<string, object>>> filteredResult =
                new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            // Iteramos por los archivos
            foreach (var fileEntry in fullData)
            {
                // Obtenemos el nombre del archivo
                string fileName = fileEntry.Key;

                // Definimos dict vacio
                Dictionary<string, Dictionary<string, object>> filteredObjects =
                    new Dictionary<string, Dictionary<string, object>>();

                // Iteramos por los handle
                foreach (var objEntry in fileEntry.Value)
                {
                    // Obtenemos el handle
                    string handle = objEntry.Key;
                    // Obtenemos la data del elemento
                    Dictionary<string, object> objectData = objEntry.Value;
                    // Validamos
                    if (objectData.ContainsKey("PropertySetInfo"))
                    {
                        // Obtenemos el dict de Psets
                        Dictionary<string, object> psets =
                            objectData["PropertySetInfo"] as Dictionary<string, object>;
                        // Validamos
                        if (psets != null && psets.ContainsKey(pSetName))
                        {
                            // Almacenamos 
                            filteredObjects[handle] = objectData;
                        }
                    }
                }

                // Solo añadimos si hay al menos un objeto válido en este archivo
                if (filteredObjects.Count > 0)
                {
                    // Almacenamos
                    filteredResult[fileName] = filteredObjects;
                }
            }
            // return
            return filteredResult;
        }

        public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> infoByObjFiltByPsetAndLayer(
            Dictionary<string, Dictionary<string, Dictionary<string, object>>> fullData,
            string layerNameFilter,
            string pSetName
        )
        {
            Dictionary<string, Dictionary<string, Dictionary<string, object>>> filteredResult =
                new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            // Iteramos por los archivos
            foreach (var fileEntry in fullData)
            {
                // Obtenemos el nombre del archivo
                string fileName = fileEntry.Key;

                // Definimos dict vacio
                Dictionary<string, Dictionary<string, object>> filteredObjects =
                    new Dictionary<string, Dictionary<string, object>>();

                // Iteramos por los handle
                foreach (var objEntry in fileEntry.Value)
                {
                    // Obtenemos el handle
                    string handle = objEntry.Key;
                    // Obtenemos la data del elemento
                    Dictionary<string, object> objectData = objEntry.Value;
                    // Validamos
                    if (objectData.ContainsKey("PropertySetInfo"))
                    {
                        // Obtenemos el dict de Psets
                        Dictionary<string, object> psets =
                            objectData["PropertySetInfo"] as Dictionary<string, object>;
                        // Validamos
                        if (psets != null && psets.ContainsKey(pSetName))
                        {
                            // Almacenamos 
                            filteredObjects[handle] = objectData;
                        }
                    }
                    // Validamos
                    if (objectData.ContainsKey("PropertySetInfo") &&
                        objectData.ContainsKey("Layer"))
                    {
                        var psets = objectData["PropertySetInfo"] as Dictionary<string, object>;
                        var layerName = objectData["Layer"] as string;

                        // Validar PropertySet y capa coincidente
                        if (psets != null && psets.ContainsKey(pSetName) &&
                            string.Equals(layerName, layerNameFilter, StringComparison.OrdinalIgnoreCase))
                        {
                            filteredObjects[handle] = objectData;
                        }
                    }
                }

                // Solo añadimos si hay al menos un objeto válido en este archivo
                if (filteredObjects.Count > 0)
                {
                    // Almacenamos
                    filteredResult[fileName] = filteredObjects;
                }
            }
            // return
            return filteredResult;
        }


    }
}
