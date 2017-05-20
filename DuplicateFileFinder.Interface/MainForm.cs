using DuplicateFileFinder.Enums;
using DuplicateFileFinder.Models;
using DuplicateFileFinder.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DuplicateFileFinder
{
    public partial class MainForm : Form
    {
        public Dictionary<string, List<FileSystemEntity>> Duplicates { get; set; }

        public MainForm()
        {
            this.Duplicates = new Dictionary<string, List<FileSystemEntity>>();

            InitializeComponent();

            var enums = Enum.GetValues(typeof(CompareMethods));
            for (int i = 0; i < enums.Length; i++)
            {
                CheckBox tmpCB = new CheckBox();
                tmpCB.Tag = i.ToString();
                tmpCB.Text = $"Compare by {enums.GetValue(i).ToString()}";
                tmpCB.AutoSize = true;
                tmpCB.Location = new Point(10, 20 + i * 20);
                this.groupBox2.Controls.Add(tmpCB);
            }
        }
        
        // Initiates the scan for duplicates.
        private void btnAction_Click(object sender, EventArgs e)
        {
            this.Duplicates.Clear();
            this.lvDuplicates.Items.Clear();
            this.lvDuplicates.Refresh();

            //// Check to see if the user has selected a folder to scan for duplicates
            if (string.IsNullOrEmpty(this.tbFolder1.Text))
            {
                MessageBox.Show("You have to select a folder to compare its content!");
                return;
            }

            //// Collect all of the selected comparison methods.
            HashSet<int> methods = new HashSet<int>();
            foreach (CheckBox control in this.groupBox2.Controls)
            {
                if (control.Checked == true)
                {
                    int enumID;
                    int.TryParse(control.Tag.ToString(), out enumID);
                    methods.Add(enumID);
                }
            }

            //// Check to see if there are any comparison methods selected.
            if (methods.Count == 0)
            {
                MessageBox.Show("You have to select atleast one comparison method!");
                return;
            }
            
            //// Modify the interface for processing data.
            this.btnAction.Text = "Scanning...";
            InterfaceControl.DisableControls(
                new HashSet<Control>()
                {
                    this.btnAction,
                    this.btnReset,
                    this.btnExit,
                    this.btnFolder1
                });

            //// Traverse file structure from the selected folder.
            FileSystem.TraverseDirectories(this.tbFolder1.Text, methods);

            foreach (var file in FileSystem.AllFiles)
            {
                if (file.Value.Count > 1)
                {
                    foreach (var item in file.Value)
                    {
                        ListViewItem lvItem = new ListViewItem(item.Name);
                        lvItem.SubItems.Add(item.Path);
                        this.lvDuplicates.Items.Add(lvItem);
                    }
                }
            }

            //// Modify the interface (reset to default) after processing data is complete.
            this.btnAction.Text = "Search";
            InterfaceControl.EnableControls(
                new HashSet<Control>()
                {
                    this.btnAction,
                    this.btnReset,
                    this.btnExit,
                    this.btnFolder1
                });
        }

        // Select the folder to scan for duplicates.
        private void btnFolder1_Click(object sender, EventArgs e)
        {
            string folder = InterfaceControl.SelectFolder();
            if (!string.IsNullOrEmpty(folder))
            {
                this.tbFolder1.Text = folder;
            }
        }

        // Resets the interface to default
        private void btnReset_Click(object sender, EventArgs e)
        {
            InterfaceControl.ResetControls(this.groupBox2.Controls);
            this.tbFolder1.Text = "";
            this.lvDuplicates.Items.Clear();
        }

        //
        // Other interface events
        //
        private void tbFolder1_MouseClick(object sender, MouseEventArgs e)
        {
            this.btnFolder1_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnExit_Click(sender, e);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnReset_Click(sender, e);
        }
    }
}
