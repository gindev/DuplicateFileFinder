using System.Windows.Forms;

namespace DuplicateFileFinder.Interface
{
    public static class Actions
    {
        public static string SelectFolder()
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

        public static string DirectoryTraverse(string directory)
        {

            return null;
        }
    }
}
