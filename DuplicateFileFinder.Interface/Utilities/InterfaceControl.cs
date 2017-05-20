using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DuplicateFileFinder.Utilities
{
    public static class InterfaceControl
    {
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

        public static void ResetControls(Control.ControlCollection controlsCollection)
        {
            foreach (var control in controlsCollection)
            {
                (control as CheckBox).Checked = false;
            }
        }

        public static void DisableControls(HashSet<Control> controls)
        {
            foreach (var control in controls)
            {
                control.Enabled = false;
            }
        }

        public static void EnableControls(HashSet<Control> controls)
        {
            foreach (var control in controls)
            {
                control.Enabled = true;
            }
        }
    }
}
