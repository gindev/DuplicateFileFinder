using DuplicateFileFinder.Enums;
using DuplicateFileFinder.Models;
using DuplicateFileFinder.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DuplicateFileFinder
{
    public partial class MainForm : Form
    {
        private Dictionary<string, List<FileSystemEntity>> AllFiles { get; set; }


        public MainForm()
        {
            this.AllFiles = new Dictionary<string, List<FileSystemEntity>>();

            InitializeComponent();

            var enums = Enum.GetValues(typeof(CompareMethods));
            for (int i = 0; i < enums.Length; i++)
            {
                CheckBox tmpCB = new CheckBox();
                tmpCB.Tag = i.ToString();
                tmpCB.Text = $"Compare by {enums.GetValue(i).ToString().ToLower()}";
                tmpCB.AutoSize = true;
                tmpCB.Location = new Point(10, 20 + i * 20);
                this.groupBox2.Controls.Add(tmpCB);
            }
        }

        // Initiates the scan for duplicates.
        private void btnAction_Click(object sender, EventArgs e)
        {
            this.AllFiles.Clear();
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
                    this.btnDeleteChecked,
                    this.btnDeleteSelected,
                    this.btnReset,
                    this.btnExit,
                    this.btnFolder1,
                    this.btnSelectAll
                });

            //// Traverse file structure from the selected folder.
            this.AllFiles = FileSystem.TraverseDirectories(this.tbFolder1.Text, methods);
            int counter = 0;

            foreach (var file in this.AllFiles.Where(x => x.Value.Count > 1))
            {
                foreach (var item in file.Value)
                {
                    ListViewItem lvItem = new ListViewItem(item.Name);
                    lvItem.SubItems.Add(item.Path);
                    lvItem.Tag = item;
                    this.lvDuplicates.Items.Add(lvItem);
                }
                counter++;
                if (counter % 100 == 0)
                {
                    this.lvDuplicates.Refresh();
                }
            }
            this.lvDuplicates.Sorting = SortOrder.Ascending;

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
            if (this.lvDuplicates.Items.Count > 0)
            {
                this.btnDeleteChecked.Enabled = true;
                this.btnDeleteSelected.Enabled = true;
                this.btnSelectAll.Enabled = true;
            }
        }

        // Select the folder to scan for duplicates.
        private void btnFolder1_Click(object sender, EventArgs e)
        {
            string folder = InterfaceControl.SelectFolder(this.tbFolder1.Text);
            if (!string.IsNullOrEmpty(folder))
            {
                this.tbFolder1.Text = folder;
            }
        }

        // Resets the interface to default
        private void btnReset_Click(object sender, EventArgs e)
        {
            InterfaceControl.ResetControls(this.groupBox2.Controls);
            InterfaceControl.DisableControls(new HashSet<Control> { this.btnDeleteSelected, btnDeleteChecked, btnSelectAll });
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

        // Exits the application from ToolStripMenu
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnExit_Click(sender, e);
        }

        // Exits the application from Exit button
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Resets all search options to the initial state
        private void newSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnReset_Click(sender, e);
        }

        // Opens context menu at the selected item position
        private void lvDuplicates_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this.lvDuplicates.FocusedItem != null && this.lvDuplicates.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    lvContextMenuStrip.Show(Cursor.Position);
                }
            }
        }

        // Opens the selected file from the context menu strip
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItem = this.lvDuplicates.FocusedItem;
            string fileName = listViewItem.SubItems[0].Text;
            string filePath = listViewItem.SubItems[1].Text;
            Process openFile = new Process();
            openFile.StartInfo.FileName = $@"{filePath}\{fileName}";
            openFile.Start();
        }

        // Opens the folder of the selected file from the context menu strip
        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItem = this.lvDuplicates.FocusedItem;
            string fileName = listViewItem.SubItems[0].Text;
            string filePath = listViewItem.SubItems[1].Text;
            Process openFileLocation = new Process();
            openFileLocation.StartInfo.FileName = "explorer.exe";
            openFileLocation.StartInfo.Arguments = $@"/e, /select, ""{filePath}\{fileName}""";
            openFileLocation.Start();
        }

        // Deletes the selected file from the context menu strip
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItem = this.lvDuplicates.FocusedItem;
            var fileSystemEntity = (listViewItem.Tag as FileSystemEntity);
            
            var result = fileSystemEntity.FileDelete();

            if (result.Key == false)
            {
                MessageBox.Show(result.Value, "Unable to delete!");
            }
            else
            {
                if (!File.Exists($@"{fileSystemEntity.Path}\{fileSystemEntity.Name}"))
                {
                    this.lvDuplicates.Items.Remove(listViewItem);
                }
            }
        }

        // Deletes all selected (checked) files from the ListView
        private void btnDelete_Click(object sender, EventArgs e)
        {
            HashSet<string> RemovedFiles = new HashSet<string>();
            HashSet<string> Errors = new HashSet<string>();

            if (this.lvDuplicates.CheckedItems.Count < 1)
            {
                MessageBox.Show("You have to select atleast one file to delete!");
                return;
            }

            this.lvDuplicates.Sorting = SortOrder.None;

            foreach (ListViewItem item in this.lvDuplicates.CheckedItems)
            {
                var fileSystemEntity = (item.Tag as FileSystemEntity);

                var result = fileSystemEntity.FileDelete();

                if (result.Key == false)
                {
                    if (!Errors.Contains(result.Value))
                    {
                        Errors.Add(result.Value);
                    }
                }
                else
                {
                    this.lvDuplicates.Items.Remove(item);
                    RemovedFiles.Add($@"{fileSystemEntity.Path}\{fileSystemEntity.Name}");
                }
                item.Checked = false;
            }

            string errorMessage = String.Empty;
            if (Errors.Count > 0)
            {
                errorMessage = $" {Errors.Count} error(s) occurred while removing file(s).";
            }

            MessageBox.Show($"{RemovedFiles.Count} file(s) removed successfuly.{errorMessage}", "Result");
            
            this.lvDuplicates.Sorting = SortOrder.Ascending;
            this.lvDuplicates.Refresh();
        }

        // Opens the about MessageBox from the Menu Strip
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Developed by:\n\nVasil Aleksandrov - 97-itsr\nPanayot Gindev - 96-itsr", "About Duplicate File Finder");
        }


        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            this.deleteToolStripMenuItem_Click(sender, e);
        }

        private void deleteCheckedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnDelete_Click(sender, e);
        }


        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (this.lvDuplicates.Items.Count > 0)
            {
                bool state;
                string label;
                if (this.btnSelectAll.Tag == null || this.btnSelectAll.Tag.Equals("Select All"))
                {
                    this.btnSelectAll.Tag = "Deselect All";
                    label = "Deselect All";
                    state = true;
                }
                else
                {
                    this.btnSelectAll.Tag = "Select All";
                    label = "Select All";
                    state = false;
                }

                foreach (var item in this.lvDuplicates.Items)
                {
                    (item as ListViewItem).Checked = state;
                }
                this.btnSelectAll.Text = label;
            }
            else
            {
                MessageBox.Show("There is nothing to select!", "Warning");
            }
        }
    }
}
