using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AlgorithmLibrary;

namespace WindowsFormsUI
{
    public partial class GameForm : Form
    {
        private const int SLEEP_TIME = 200;
        private Maps AlgorithmMap;
        private Maps myMap;
        private Algorithm algorithm;
        private string mapName;
        private PictureBox[,] pictureBoxes;
        #region Images
        private Bitmap imageWall = Properties.Resources.wall;
        private Bitmap imageCardA = Properties.Resources.cardA;
        private Bitmap imageCardB = Properties.Resources.cardB;
        private Bitmap imageCardC = Properties.Resources.cardC;
        private Bitmap imageCardD = Properties.Resources.cardD;
        private Bitmap imageCardE = Properties.Resources.cardE;
        private Bitmap imageDoorA = Properties.Resources.doorA;
        private Bitmap imageDoorB = Properties.Resources.DoorB;
        private Bitmap imageDoorC = Properties.Resources.DoorC;
        private Bitmap imageDoorD = Properties.Resources.DoorD;
        private Bitmap imageDoorE = Properties.Resources.DoorE;
        private Bitmap imageFire = Properties.Resources.fire;
        private Bitmap imageFloor = Properties.Resources.floor;
        private Bitmap imageMedkit = Properties.Resources.medkit;
        private Bitmap imageQuit = Properties.Resources.quit;
        private Bitmap imageSpawn = Properties.Resources.spawn;
        #endregion
        public GameForm()
        {
            InitializeComponent();
        }

        public GameForm(string mapName)
        {
            InitializeComponent();
            this.mapName = mapName;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            AlgorithmMap = new Maps(mapName);
            algorithm = new Algorithm(AlgorithmMap);
            myMap = new Maps(mapName);

            pictureBoxes = new PictureBox[myMap.Map.GetLength(0), myMap.Map.GetLength(1)];

            for (int y = 0; y < myMap.Map.GetLength(0); y++)
            {
                FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
                flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
                flowLayoutPanel.Margin = new Padding(0);
                flowLayoutPanel.AutoSize = true;
                for (int x = 0; x < myMap.Map.GetLength(1); x++)
                {
                    char tempObject = myMap.ReturnObject(x, y);
                    PictureBox pictureBox = CreateNewPictureBox(ReturnImage(tempObject), 40, 40);
                    pictureBox.Margin = new Padding(0);
                    flowLayoutPanel.Controls.Add(pictureBox);
                    pictureBoxes[y,x] = pictureBox;
                }
                mainFlowPanel.Controls.Add(flowLayoutPanel);
            }
        }

        private PictureBox CreateNewPictureBox(Bitmap imageToDisplay, int xSize, int ySize)
        {
            PictureBox pictureBox = new PictureBox();
            // Stretches the image to fit the pictureBox.
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            imageToDisplay = new Bitmap(imageToDisplay);
            pictureBox.ClientSize = new Size(xSize, ySize);
            pictureBox.Image = imageToDisplay;
            return pictureBox;
        }

        private Bitmap ReturnImage(char _object)
        {
            switch(_object)
            {
                case '.':
                    return imageFloor;
                case 'X':
                    return imageWall;
                case 'a':
                    return imageCardA;
                case 'b':
                    return imageCardB;
                case 'c':
                    return imageCardC;
                case 'd':
                    return imageCardD;
                case 'e':
                    return imageCardE;
                case 'A':
                    return imageDoorA;
                case 'B':
                    return imageDoorB;
                case 'C':
                    return imageDoorC;
                case 'D':
                    return imageDoorD;
                case 'E':
                    return imageDoorE;
                case 'S':
                    return imageSpawn;
                case 'Q':
                    return imageQuit;
                case 'H':
                    return imageMedkit;  
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                    return imageFire;
                default:
                    return null;
            }
        }
    }
}
