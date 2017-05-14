using DuplicateFileFinder.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DuplicateFileFinder.Interface
{
    public partial class MainForm : Form
    {
        public Dictionary<string, List<FileSystemEntity>> AllFiles { get; set; }
        public Dictionary<string, List<FileSystemEntity>> Duplicates { get; set; }

        public MainForm()
        {
            this.AllFiles = new Dictionary<string, List<FileSystemEntity>>();
            this.Duplicates = new Dictionary<string, List<FileSystemEntity>>();

            InitializeComponent();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnFolder1_Click(object sender, EventArgs e)
        {
            string folder = Utilities.SelectFolder();
            if (!string.IsNullOrEmpty(folder))
            {
                this.tbFolder1.Text = folder;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnExit_Click(sender,e);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Utilities.ResetControls(this.groupBox2.Controls);
            this.tbFolder1.Text = "";
            this.lvDuplicates.Items.Clear();
        }

        private void newSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnReset_Click(sender,e);
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            this.Duplicates.Clear();
            this.AllFiles.Clear();
            this.lvDuplicates.Items.Clear();
            this.lvDuplicates.Refresh();

            int method = 0;

            if (string.IsNullOrEmpty(this.tbFolder1.Text))
            {
                MessageBox.Show("You have to select a folder to compare its content!");
                return;
            }

            if (this.checkBox1.Checked)
            {
                method = 1;
            }
            else if(this.checkBox2.Checked)
            {
                method = 2;
            }
            else
            {
                method = 0;
                MessageBox.Show("You have to select atleast one comparison method!");
                return;
            }

            this.btnAction.Text = "Scanning...";
            Utilities.DisableControls(
                new HashSet<Control>()
                {
                    this.btnAction,
                    this.btnReset,
                    this.btnExit,
                    this.btnFolder1
                });

            Utilities.TraverseDirectories(this.tbFolder1.Text, this.AllFiles, method);

            foreach (var file in AllFiles)
            {
                if (file.Value.Count > 1)
                {
                    foreach (var item in file.Value)
                    {
                        this.lvDuplicates.Items.Add($"Name: \"{item.Name}\", Location: \"{item.Path}\"");
                    }
                }
            }
            this.btnAction.Text = "Search";
            Utilities.EnableControls(
                new HashSet<Control>()
                {
                    this.btnAction,
                    this.btnReset,
                    this.btnExit,
                    this.btnFolder1
                });
        }
    }
}
