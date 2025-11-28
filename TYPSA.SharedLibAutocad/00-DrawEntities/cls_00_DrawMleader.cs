using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using TYPSA.SharedLib.Autocad.GetDocument;

namespace TYPSA.SharedLib.Autocad.DrawEntities
{
    public class cls_00_DrawMleader
    {
        public static ObjectId DrawMLeaderOnPoint(
            Point3d basePoint,
            string textValue,
            Transaction tr,
            BlockTableRecord btr,
            double textHeight = 7.2,
            int colorIndex = 1,
            string layer = "0",
            string textStyle = null,
            double offsetX = 2.0,
            double offsetY = 2.0,
            AttachmentPoint? justification = null
        )
        {
            // Crear el MLeader
            MLeader mLeader = new MLeader();
            mLeader.SetDatabaseDefaults();
            mLeader.Layer = layer;
            mLeader.ColorIndex = colorIndex;
            mLeader.ContentType = ContentType.MTextContent;

            // Crear el texto (MText)
            Point3d textPosition =
                new Point3d(basePoint.X + offsetX, basePoint.Y + offsetY, basePoint.Z);
            // Configuramos el texto
            MText mText = new MText
            {
                Contents = textValue,
                TextHeight = textHeight,
                Location = textPosition,
                Attachment = justification ?? AttachmentPoint.MiddleLeft
            };

            // Activar el fondo opaco (background mask)
            mText.BackgroundFill = true;
            mText.UseBackgroundColor = true;
            mText.BackgroundScaleFactor = 1.0;

            // Aplicar estilo de texto si se especifica
            if (!string.IsNullOrEmpty(textStyle))
            {
                TextStyleTable tst =
                    (TextStyleTable)tr.GetObject(btr.Database.TextStyleTableId, OpenMode.ForRead);
                // Validamos
                if (tst.Has(textStyle))
                    // Aplicamos
                    mText.TextStyleId = tst[textStyle];
            }

            // Asignar el MText al MLeader
            mLeader.MText = mText;

            // Crear el líder (flecha)
            int leaderIndex = mLeader.AddLeader();
            int leaderLineIndex = mLeader.AddLeaderLine(leaderIndex);

            // Añadir el punto de inicio del líder
            mLeader.AddFirstVertex(leaderLineIndex, basePoint);

            // Añadir el punto de anclaje del texto (para que no apunte a 0,0,0)
            mLeader.AddLastVertex(leaderLineIndex, textPosition);

            // Asegurar que la dirección del texto se vincule correctamente
            mLeader.SetDoglegLength(leaderIndex, 5.0); // Ladding distance
            mLeader.LandingGap = 3.0;
            mLeader.EnableFrameText = true; // activa el recuadro

            // Insertar en el dibujo
            cls_00_DocumentInfo.AddEntityToBlockTableRecord(mLeader, btr, tr);
            // return
            return mLeader.ObjectId;
        }
    }
}
