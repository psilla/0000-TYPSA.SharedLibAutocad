using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace TYPSA.SharedLib.Autocad.GetDocument
{
    public class cls_00_DocumentInfo
    {
        // Info Document
        public static Document GetActiveDocument()
        {
            // Retorna el documento activo en AutoCAD
            return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        }

        public static string GetActiveDocumentName(Document doc)
        {

            if (doc != null)
            {
                // Obtener la ruta completa del archivo
                string fullPath = doc.Name;

                // Devolver solo el nombre del archivo (sin la ruta completa)
                return System.IO.Path.GetFileNameWithoutExtension(fullPath);
            }
            else
            {
                return "Sin nombre o documento no disponible";
            }
        }

        public static string GetActiveDocumentNameWithExt(Document doc)
        {

            if (doc != null)
            {
                // Obtener la ruta completa del archivo
                string fullPath = doc.Name;

                // Devolver solo el nombre del archivo (sin la ruta completa)
                return System.IO.Path.GetFileName(fullPath);
            }
            else
            {
                return "Sin nombre o documento no disponible";
            }
        }

        public static string GetDocumentFullPath(Document doc)
        {

            if (doc != null)
            {
                // Obtener la ruta completa del archivo
                return doc.Name;

            }
            else
            {
                return "Sin nombre o documento no disponible";
            }
        }

        public static string GetFileNameFromPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;

            // return
            return Path.GetFileName(fullPath);
        }

        // Info Units
        public static double ConvertUnitsFromDrawing(
            double value, string targetUnit, int power = 1
        )
        {
            try
            {
                // Obtener la unidad base del dibujo (INSUNITS)
                object rawInsUnits = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("INSUNITS");
                int insUnits = Convert.ToInt32(rawInsUnits);

                // Factores de conversión de INSUNITS a metros
                Dictionary<int, double> insUnitsToMeters = new Dictionary<int, double>
                {
                    { 0, 1.0 },       // Unitless
                    { 1, 0.0254 },    // Inches
                    { 2, 0.3048 },    // Feet
                    { 4, 0.001 },     // Millimeters
                    { 5, 0.01 },      // Centimeters
                    { 6, 1.0 },       // Meters
                    { 7, 1000.0 },    // Kilometers
                    { 8, 25.4 },      // Microinches
                    { 9, 2.54 },      // Mils
                    {10, 1609.34 },   // Miles
                    {11, 0.9144 },    // Yards
                    {12, 0.0000254 }, // Nanometers
                    {13, 0.000001 },  // Micrometers
                    {14, 0.1 },       // Decimeters
                    {15, 10.0 },      // Decameters
                    {16, 100.0 },     // Hectometers
                    {17, 100000.0 }   // Gigameters
                };

                if (!insUnitsToMeters.TryGetValue(insUnits, out double toMeters))
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog($"❌ INSUNITS no reconocido: {insUnits}");
                    return value;
                }

                // Determinar el factor de conversión desde metros a la unidad destino
                string unit = targetUnit.ToLower();
                double fromMeters;

                if (unit == "m" || unit == "meters")
                    fromMeters = 1.0;
                else if (unit == "mm" || unit == "millimeters")
                    fromMeters = 1000.0;
                else if (unit == "cm" || unit == "centimeters")
                    fromMeters = 100.0;
                else if (unit == "dm" || unit == "decimeters")
                    fromMeters = 10.0;
                else if (unit == "dam" || unit == "decameters")
                    fromMeters = 0.1;
                else if (unit == "hm" || unit == "hectometers")
                    fromMeters = 0.01;
                else if (unit == "km" || unit == "kilometers")
                    fromMeters = 0.001;
                else if (unit == "ft" || unit == "feet")
                    fromMeters = 3.28084;
                else if (unit == "in" || unit == "inches")
                    fromMeters = 39.3701;
                else if (unit == "yd" || unit == "yards")
                    fromMeters = 1.09361;
                else
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(
                        $"❌ Unidad de destino no reconocida: {targetUnit}"
                    );

                    // Finalizamos
                    return value;
                }

                // Aplicar conversión con el exponente correspondiente (1=lineal, 2=área, 3=volumen)
                return value * Math.Pow(toMeters, power) * Math.Pow(fromMeters, power);
            }
            catch (System.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog($"❌ Error al convertir unidades:\n{ex.Message}");
                return value;
            }

        }

        public static string GetDrawingUnitsName()
        {
            // Obtener la unidad base del dibujo (INSUNITS)
            object rawInsUnits = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("INSUNITS");
            int insUnits = Convert.ToInt32(rawInsUnits);

            switch (insUnits)
            {
                case 0: return "Unitless";
                case 1: return "Inches";
                case 2: return "Feet";
                case 3: return "Miles";
                case 4: return "Millimeters";
                case 5: return "Centimeters";
                case 6: return "Meters";
                case 7: return "Kilometers";
                case 8: return "Microinches";
                case 9: return "Mils";
                case 10: return "Yards";
                case 11: return "Angstroms";
                case 12: return "Nanometers";
                case 13: return "Microns";
                case 14: return "Decimeters";
                case 15: return "Decameters";
                case 16: return "Hectometers";
                case 17: return "Gigameters";
                case 18: return "Astronomical units";
                case 19: return "Light years";
                case 20: return "Parsecs";
                default: return "Unknown";
            }
        }

        public static Dictionary<string, int> GetUnitsDictionary()
        {
            return new Dictionary<string, int>
            {
                { "Unitless", 0 },
                { "Inches", 1 },
                { "Feet", 2 },
                { "Miles", 3 },
                { "Millimeters", 4 },
                { "Centimeters", 5 },
                { "Meters", 6 },
                { "Kilometers", 7 },
                { "Microinches", 8 },
                { "Mils", 9 },
                { "Yards", 10 },
                { "Angstroms", 11 },
                { "Nanometers", 12 },
                { "Microns", 13 },
                { "Decimeters", 14 },
                { "Decameters", 15 },
                { "Hectometers", 16 },
                { "Gigameters", 17 },
                { "Astronomical units", 18 },
                { "Light years", 19 },
                { "Parsecs", 20 }
            };
        }

        // Info Data Base
        public static Database GetDatabaseFromDocument(Document doc)
        {
            return doc.Database;
        }

        public static Database GetDatabaseFromBlockTableRecord(BlockTableRecord xrefBtr)
        {
            return xrefBtr.Database;
        }

        // Info Editor
        public static Editor GetEditor(Document doc)
        {
            return doc.Editor;
        }

        // Info BlockTable
        public static BlockTable GetBlockTableForRead(Transaction tr, Database db)
        {
            return tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        }

        // Info BlockTableRecord
        public static void AddEntityToBlockTableRecord(
            Entity ent, BlockTableRecord btr, Transaction tr
        )
        {
            // Añadir la entidad al BlockTableRecord (puede ser ModelSpace, PaperSpace, etc.)
            btr.AppendEntity(ent);
            tr.AddNewlyCreatedDBObject(ent, true);
        }

        public static BlockTableRecord GetBlockTableRecordForWrite(
            Transaction tr, BlockTable bt
        )
        {
            return tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
        }

        public static BlockTableRecord GetBlockTableRecordForRead(
            Transaction tr, BlockTable bt
        )
        {
            return tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
        }

        // Info LayerTable
        public static LayerTable GetLayerTableForRead(
            Transaction tr, Database db
        )
        {
            return (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
        }

        // Info LayerTableRecord
        public static LayerTableRecord GetLayerFromEntityForRead(
            Transaction tr, LayerTable lt, Entity ent
        )
        {
            if (tr == null || lt == null || ent == null)
                throw new ArgumentNullException("Transaction, LayerTable, or Entity is null.");

            if (!lt.Has(ent.Layer))
                throw new System.Exception($"La capa '{ent.Layer}' no existe.");

            return (LayerTableRecord)tr.GetObject(lt[ent.Layer], OpenMode.ForRead);
        }

        // Info Object from Handle
        public static ObjectId GetObjectIdFromHandle(Database db, Handle handle)
        {
            ObjectId id = ObjectId.Null;
            db.TryGetObjectId(handle, out id);
            return id;
        }

        // Close Xref
        public static void SaveAndCloseXref(
            Document xrefDoc, string xrefFilePath
        )
        {
            xrefDoc.CloseAndSave(xrefFilePath);
        }

        public static BlockTableRecord GetLoadedXrefBlockTableRecordByPath(
            BlockTable bt, Transaction tr, string xrefFilePath
        )
        {
            // Extraemos el nombre del archivo sin la ruta
            string fileName = Path.GetFileName(xrefFilePath);

            // Recorremos todas las entradas del BlockTable
            foreach (ObjectId id in bt)
            {
                // Obtenemos BlockTableRecord
                BlockTableRecord btr = tr.GetObject(id, OpenMode.ForRead) as BlockTableRecord;
                // Validamos
                if (btr != null && btr.IsFromExternalReference)
                {
                    // Obtenemos el archivo
                    string loadedFile = Path.GetFileName(btr.PathName);
                    // Validamos
                    if (loadedFile.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        // return
                        return btr;
                    }
                }
            }

            // Finalizamos
            return null;
        }

        public static DocumentCollection GetAutoCADDocumentCollection()
        {
            return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
        }

        public static Document OpenAutoCADDocument(
            DocumentCollection docs,
            string filePath
        )
        {
            // Abrimos no solo forReadOnly
            return docs.Open(filePath, false);
        }

        public static List<string> GetAllTextStylesFromDrawing(Database db)
        {
            List<string> styles = new List<string>();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable tst = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                foreach (ObjectId id in tst)
                {
                    TextStyleTableRecord tsr = (TextStyleTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    styles.Add(tsr.Name);
                }
                tr.Commit();
            }
            // return
            return styles;
        }

















    }
}


