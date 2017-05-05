using DuplicateFileFinder.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace DuplicateFileFinder.Interface
{
    public partial class MainForm : Form
    {
        public Dictionary<string,HashSet<FileSystemEntity>> Duplicates { get; set; }

        public MainForm()
        {
            this.Duplicates = new Dictionary<string, HashSet<FileSystemEntity>>();

            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnFolder1_Click(object sender, EventArgs e)
        {
            string folder = Actions.SelectFolder();
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
            Actions.ResetControls(this.groupBox2.Controls);
            this.tbFolder1.Text = "";
        }

        private void newSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnReset_Click(sender,e);
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            this.Duplicates.Clear();
            this.lvDuplicates.Items.Clear();

        }
    }
}
