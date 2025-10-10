using System.Collections.Generic;
using System.Linq;
using System;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TYPSA.SharedLib.Autocad.ObjectsByTypeByLayer
{
    public class cls_00_ExportLabelsToExcel_EPPlus
    {
        public static void ExportLabelsToExcel_EPPlus(
            List<List<string>> etiquetas,
            List<string> headers5
        )
        {
            try
            {
                // Necesario en .NET Core/.NET 5+ para EPPlus
                System.Text.Encoding.RegisterProvider(
                    System.Text.CodePagesEncodingProvider.Instance
                );

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Labels");

                    if (etiquetas == null || etiquetas.Count == 0)
                        throw new ArgumentException("No hay datos para exportar.");

                    // Determinar número de columnas reales en los datos
                    int columnCount = etiquetas.Max(e => e.Count);

                    // Ajustar headers según el número de columnas
                    List<string> headersToUse;
                    if (columnCount == 4)
                    {
                        // Usamos Proyecto, Inversor, String, Comentario
                        headersToUse = new List<string>
                {
                    headers5[0], // Proyecto
                    headers5[1], // Inversor
                    headers5[3], // String
                    headers5[4]  // Comentario
                };
                    }
                    else if (columnCount == 5)
                    {
                        headersToUse = headers5;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Número de columnas inesperado: {columnCount}. Solo se admite 4 o 5."
                        );
                    }

                    // Escribir encabezados
                    for (int col = 0; col < headersToUse.Count; col++)
                    {
                        worksheet.Cells[1, col + 1].Value = headersToUse[col];
                        worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                    }

                    // Escribir datos
                    int currentRow = 2;
                    foreach (var sublist in etiquetas)
                    {
                        for (int col = 0; col < sublist.Count; col++)
                        {
                            // Si son 4 columnas → se escriben tal cual
                            // Si son 5 columnas → también
                            worksheet.Cells[currentRow, col + 1].Value = sublist[col];
                        }
                        currentRow++;
                    }

                    // Dar formato de tabla
                    string tableRange =
                        $"A1:{ExcelCellBase.GetAddress(etiquetas.Count + 1, headersToUse.Count)}";
                    ExcelTable table =
                        worksheet.Tables.Add(worksheet.Cells[tableRange], "LabelsTable");
                    table.ShowFilter = true;
                    table.TableStyle = TableStyles.Medium2;

                    // Guardar en archivo temporal
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        package.SaveAs(memoryStream);
                        memoryStream.Flush();

                        string tempFilePath =
                            Path.Combine(Path.GetTempPath(), $"Labels_{Guid.NewGuid()}.xlsx");

                        File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                        if (File.Exists(tempFilePath))
                        {
                            Process.Start(
                                new ProcessStartInfo(tempFilePath) { UseShellExecute = true }
                            );
                        }
                        else
                        {
                            MessageBox.Show(
                                $"Could not create the Excel file.\nPath: {tempFilePath}",
                                "File Creation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error exporting labels with EPPlus: {ex.Message}\n{ex.StackTrace}",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        public static void ExportLabelsToExcel_EPPlus(
            List<List<string>> etiquetas
        )
        {
            try
            {
                // Necesario en .NET Core/.NET 5+ para EPPlus
                System.Text.Encoding.RegisterProvider(
                    System.Text.CodePagesEncodingProvider.Instance
                );
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Labels");

                    if (etiquetas == null || etiquetas.Count == 0)
                        throw new ArgumentException("No hay datos para exportar.");

                    // Determinar el número máximo de columnas reales en todas las filas
                    int columnCount = etiquetas.Max(e => e.Count);

                    // Generar encabezados dinámicos
                    for (int col = 0; col < columnCount; col++)
                    {
                        worksheet.Cells[1, col + 1].Value = $"Columna {col + 1}";
                        worksheet.Cells[1, col + 1].Style.Font.Bold = true;
                    }

                    // Escribir datos dinámicamente
                    int currentRow = 2;
                    foreach (var sublist in etiquetas)
                    {
                        for (int col = 0; col < sublist.Count; col++)
                        {
                            worksheet.Cells[currentRow, col + 1].Value = sublist[col];
                        }
                        currentRow++;
                    }

                    // Ajustar ancho de columnas automáticamente
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Crear tabla con estilo
                    string tableRange =
                        $"A1:{ExcelCellBase.GetAddress(etiquetas.Count + 1, columnCount)}";
                    ExcelTable table =
                        worksheet.Tables.Add(worksheet.Cells[tableRange], "LabelsTable");
                    table.ShowFilter = true;
                    table.TableStyle = TableStyles.Medium2;

                    // Guardar en archivo temporal
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        package.SaveAs(memoryStream);
                        memoryStream.Flush();

                        string tempFilePath =
                            Path.Combine(Path.GetTempPath(), $"Labels_{Guid.NewGuid()}.xlsx");

                        File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                        if (File.Exists(tempFilePath))
                        {
                            Process.Start(
                                new ProcessStartInfo(tempFilePath) { UseShellExecute = true }
                            );
                        }
                        else
                        {
                            MessageBox.Show(
                                $"No se pudo crear el archivo Excel.\nRuta: {tempFilePath}",
                                "Error de creación de archivo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al exportar etiquetas con EPPlus: {ex.Message}\n{ex.StackTrace}",
                    "Error de Exportación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

    }
}
