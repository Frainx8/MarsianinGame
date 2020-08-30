using System;
using System.Windows.Forms;

namespace WindowsFormsUI
{
    public partial class MainForm : Form
    {
        private string mapName;
        private GameForm gameForm;
        private LoadingForm loadingForm;

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
            if(mapName == null)
            {
                loadingForm = new LoadingForm(this);
                loadingForm.WindowState = FormWindowState.Maximized;
                loadingForm.MdiParent = this;
                loadingForm.Show();
            }
            else
            {
                gameForm = new GameForm(this, mapName);
                gameForm.WindowState = FormWindowState.Maximized;
                gameForm.MdiParent = this;
                gameForm.Show();
            }


        }

        public void ChangeToGameForm(string mapPath)
        {
            if (loadingForm != null)
            {
                loadingForm.Close();
            }
            gameForm = new GameForm(this, mapPath);
            gameForm.WindowState = FormWindowState.Maximized;
            gameForm.MdiParent = this;
            gameForm.Show();
            
        }

        public void ChangeToLoadingForm()
        {
            if (gameForm != null)
            {
                gameForm.Close();
            }
            loadingForm = new LoadingForm(this);
            loadingForm.WindowState = FormWindowState.Maximized;
            loadingForm.MdiParent = this;
            loadingForm.Show();
        }

    }
}
