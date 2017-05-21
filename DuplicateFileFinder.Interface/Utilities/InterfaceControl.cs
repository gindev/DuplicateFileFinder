using System.Collections.Generic;
using System.Windows.Forms;

namespace DuplicateFileFinder.Utilities
{
    public static class InterfaceControl
    {
        // Generates OpenFolderDialog and selects working folder
        public static string SelectFolder(string folder)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select forlder to scan for duplicates";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string sSelectedPath = fbd.SelectedPath;
                return sSelectedPath;
            }

            return null;
        }

        // Resets the checkbox controls given as collection
        public static void ResetControls(Control.ControlCollection controlsCollection)
        {
            foreach (var control in controlsCollection)
            {
                (control as CheckBox).Checked = false;
            }
        }

        // Disable controls given as collection
        public static void DisableControls(HashSet<Control> controls)
        {
            foreach (var control in controls)
            {
                control.Enabled = false;
            }
        }

        // Enable controls given as collection
        public static void EnableControls(HashSet<Control> controls)
        {
            foreach (var control in controls)
            {
                control.Enabled = true;
            }
        }
    }
}
