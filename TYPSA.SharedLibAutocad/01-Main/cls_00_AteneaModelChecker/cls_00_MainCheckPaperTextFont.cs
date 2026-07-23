using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;

namespace TYPSA.SharedLib.Autocad.Main
{
    public class cls_00_MainCheckPaperTextFont
    {
        private static PaperTextFontResult CheckFont(
            string fileName,
            string layoutName,
            string textType,
            string textValue,
            string styleName,
            string fontName,
            string expectedFont
        )
        {
            bool isExpected = fontName != null &&
                fontName.ToLower().Contains(expectedFont.ToLower());

            // return
            return new PaperTextFontResult
            {
                FileName = fileName,
                LayoutName = layoutName,
                TextType = textType,
                TextValue = textValue,
                StyleName = styleName,
                FontName = fontName,
                IsExpected = isExpected
            };
        }

        private static (string styleName, string fontName) GetFontInfo(
            Transaction tr,
            ObjectId textStyleId
        )
        {
            TextStyleTableRecord style = tr.GetObject(textStyleId, OpenMode.ForRead) as TextStyleTableRecord;
            // Validamos
            if (style == null) return ("", "");

            // return
            return (style.Name, style.FileName);
        }

        public class PaperTextFontResult
        {
            [JsonIgnore] // ignoramos en JSON
            public string FileName { get; set; }
            public string LayoutName { get; set; }
            public string TextType { get; set; }
            public string TextValue { get; set; }
            // Nombre del estilo
            public string StyleName { get; set; }
            // Fuente
            public string FontName { get; set; }
            public bool IsExpected { get; set; }
        }

        public static List<PaperTextFontResult> AnalyzeTextFont(
            Transaction tr,
            Database db,
            string fileName,
            string expectedFont
        )
        {
            List<PaperTextFontResult> results = new List<PaperTextFontResult>();

            // Obtenemos dict de layouts
            DBDictionary layoutDict = 
                tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
            // Iteramos
            foreach (DBDictionaryEntry entry in layoutDict)
            {
                // Obtenemos el layout
                Layout layout = tr.GetObject(entry.Value, OpenMode.ForRead) as Layout;
                // Validamos
                if (layout.ModelType) continue;

                // Obtenemos nombre
                string layoutName = layout.LayoutName;

                // Obtenemos btr
                BlockTableRecord btr = tr.GetObject(
                    layout.BlockTableRecordId, OpenMode.ForRead
                ) as BlockTableRecord;
                // Iteramos
                foreach (ObjectId id in btr)
                {
                    // Obtenemos entidad
                    Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                    // Casos
                    if (ent is DBText dbText)
                    {
                        // Obtenemos info
                        var info = GetFontInfo(tr, dbText.TextStyleId);
                        // Añadimos
                        results.Add(CheckFont(
                            fileName, layoutName, "DBText", dbText.TextString, 
                            info.styleName, info.fontName, expectedFont
                        ));
                    }
                    else if (ent is MText mText)
                    {
                        // Obtenemos fuente
                        var info = GetFontInfo(tr, mText.TextStyleId);
                        // Añadimos
                        results.Add(CheckFont( 
                            fileName, layoutName, "MText", mText.Contents, 
                            info.styleName, info.fontName, expectedFont
                        ));
                    }
                    else if (ent is BlockReference br)
                    {
                        // Iteramos
                        foreach (ObjectId attId in br.AttributeCollection)
                        {
                            // Obtenemos attr
                            AttributeReference att = tr.GetObject(
                                attId, OpenMode.ForRead
                            ) as AttributeReference;
                            // Validamos
                            if (att == null) continue;

                            // Obtenemos fuente
                            var info = GetFontInfo(tr, att.TextStyleId);
                            // Añadimos
                            results.Add(CheckFont(
                                fileName, layoutName, "Attribute", att.TextString, 
                                info.styleName, info.fontName, expectedFont
                            ));
                        }
                    }
                }
            }

            // return
            return results;
        }

        
    }
}
