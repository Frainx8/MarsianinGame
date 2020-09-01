using System;
using System.Windows.Forms;

namespace WindowsFormsUI
{
    public partial class LoadingForm : Form
    {
        private string fileLocation;
        private MainForm mainForm;
        public LoadingForm()
        {
            InitializeComponent();
        }

        public LoadingForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            Positioning.Center(groupBox1);
        }



        private void uploadButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) 
            {
                if(!openFileDialog1.FileName.Contains(".txt"))
                {
                    MessageBox.Show("Wrong file type!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    labelMapName.Text = openFileDialog1.SafeFileName;
                    labelMapName.Visible = true;
                    fileLocation = openFileDialog1.FileName;
                    buttonStart.Enabled = true;
                }
            }
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            labelMapName.Visible = false;
        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            mainForm.ChangeToGameForm(fileLocation);
        }
    }
}
