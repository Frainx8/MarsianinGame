using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsUI
{
    public partial class MainForm : Form
    {
        private string mapName;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string mapName)
        {
            InitializeComponent();
            this.mapName = mapName;
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
            
            GameForm gameForm = new GameForm(mapName);
            gameForm.WindowState = FormWindowState.Maximized;
            gameForm.MdiParent = this;
            gameForm.Show();
        }

    }
}
