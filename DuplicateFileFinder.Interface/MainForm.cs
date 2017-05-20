﻿using DuplicateFileFinder.Enums;
using DuplicateFileFinder.Models;
using DuplicateFileFinder.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuplicateFileFinder
{
    public partial class MainForm : Form
    {
        public Dictionary<string, List<FileSystemEntity>> Duplicates { get; set; }
        private Dictionary<string, List<FileSystemEntity>> AllFiles { get; set; }

        public MainForm()
        {
            this.Duplicates = new Dictionary<string, List<FileSystemEntity>>();
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
        private async void btnAction_Click(object sender, EventArgs e)
        {
            this.Duplicates.Clear();
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
                    this.btnDelete,
                    this.btnReset,
                    this.btnExit,
                    this.btnFolder1
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
                    this.lvDuplicates.Items.Add(lvItem);
                }
                counter++;
                if(counter % 100 == 0)
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
                this.btnDelete.Enabled = true;
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItem = this.lvDuplicates.FocusedItem;
            string fileName = listViewItem.SubItems[0].Text;
            string filePath = listViewItem.SubItems[1].Text;
            Process openFile = new Process();
            openFile.StartInfo.FileName = $@"{filePath}\{fileName}";
            openFile.Start();
        }

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

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItem = this.lvDuplicates.FocusedItem;
            string fileName = listViewItem.SubItems[0].Text;
            string filePath = listViewItem.SubItems[1].Text;

            try
            {
                FileSystem.FileDelete($@"{filePath}\{fileName}");
            }
            catch (Exception ex)
            {
                // implement a message to the user
                MessageBox.Show($@"{ex.Message}");
            }
            finally
            {
                if (!File.Exists($@"{filePath}\{fileName}"))
                {
                    this.lvDuplicates.Items.Remove(listViewItem);
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.lvDuplicates.SelectedItems.Count < 1)
            {
                MessageBox.Show("You have to select atleast one file to delete!");
                return;
            }

            var itemsToDelete = this.lvDuplicates.SelectedItems;

            foreach (var item in itemsToDelete)
            {
                string fileName = (item as ListViewItem).SubItems[0].Text;
                string filePath = (item as ListViewItem).SubItems[1].Text;

                Task task = new Task(() => FileSystem.FileDelete($@"{filePath}\{fileName}"));
                task.Start();
                this.lvDuplicates.Items.Remove((item as ListViewItem));
                task.Wait();
            }
        }
    }
}
