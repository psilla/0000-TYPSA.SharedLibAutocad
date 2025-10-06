using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TYPSA.SharedLib.Autocad.UserForms;

namespace TYPSA.SharedLib.Autocad.Buttons
{
    public class cls_00_GetUserData
    {
        public static bool GetUserData(
            out string projectCode,
            out List<string> selectedFiles,
            out string selectedFolderPath,
            out DateTime startTime
        )
        {
            projectCode = null;
            selectedFiles = null;
            selectedFolderPath = string.Empty;
            startTime = DateTime.Now;

            // Obtener código de proyecto
            projectCode = GetProjectCodeFromDialog();
            // Validamos
            if (projectCode == null) return false;

            // Ejecutar directamente la opción B: formulario PathEntry
            using (PathEntry pathEntryForm = new PathEntry(projectCode))
            {
                DialogResult pathEntryResult = pathEntryForm.ShowDialog();
                // Validamos
                if (pathEntryResult == DialogResult.OK && pathEntryForm.SelectedFiles.Length > 0)
                {
                    selectedFiles = new List<string>(pathEntryForm.SelectedFiles);
                    selectedFolderPath = System.IO.Path.GetDirectoryName(selectedFiles[0]);
                    // return
                    return true;
                }
                else
                {
                    // Mensaje
                    MessageBox.Show(
                        "No files were selected. Operation cancelled.",
                        "Cancelled",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    // Finalizamos
                    return false;
                }
            }
        }

        public static string GetProjectCodeFromDialog()
        {
            using (ProjectCodeDialogTask projectCodeDialog = new ProjectCodeDialogTask())
            {
                DialogResult result = projectCodeDialog.ShowDialog();
                // Validamos
                if (result != DialogResult.OK)
                {
                    // Mensaje
                    MessageBox.Show(
                        "Project code input was cancelled.", "Input Cancelled",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    // Finalizamos
                    return null;
                }

                string projectCode = projectCodeDialog.ProjectCode?.Trim().ToUpper();
                // Validamos
                if (string.IsNullOrWhiteSpace(projectCode))
                {
                    // Mensaje
                    MessageBox.Show(
                        "Project code cannot be empty or whitespace.", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                    // Finalizamos
                    return null;
                }

                // return
                return projectCode;
            }
        }





    }
}
