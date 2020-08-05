using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlgorithmLibrary;

namespace WindowsFormsUI
{
    public partial class GameForm : Form
    {
        private const int SLEEP_TIME = 300;
        private Maps AlgorithmMap;
        private Maps myMap;
        private Algorithm algorithm;
        private string mapName;
        private PictureBox[,] pictureBoxes;
        private MyFlowLayoutPanel[] FlowLayoutPanels;
        private const int SIZE_OF_IMAGE_X = 40;
        private const int SIZE_OF_IMAGE_Y = 40;
        private Bitmap doomBoy = Properties.Resources.doomBoyDown;
        private event EventHandler<GameCompleteArgs> GameCompleteEvent;
        private class GameCompleteArgs : EventArgs
        {
            public string Result { get; set; }
        }

        
        
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
            LoadMap();

            GameCompleteEvent += GameComplete;

            ShowCharactersMovement();
        }
        private void GameComplete(object sender, GameCompleteArgs e)
        {
            if(e.Result == "Complete")
            {
                MessageBox.Show("Level complete!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void ShowCharactersMovement()
        {
            PictureBox tempPicture = pictureBoxes[algorithm.Result[0].Y, algorithm.Result[0].X];
            AlgorithmLibrary.Point tempPoint = algorithm.Result[0];
            foreach (AlgorithmLibrary.Point point in algorithm.Result)
            {
                foreach (Control c in FlowLayoutPanels[tempPoint.Y].Controls)
                {
                    
                    if (c.Name == $"picture{tempPoint.X}")
                    {
                        PictureBox picture = (PictureBox)c;
                        picture.Image = (Image)tempPicture.Image.Clone();
                        c.Refresh();
                        break;
                    }
                }

                pictureBoxes[tempPoint.Y, tempPoint.X] = tempPicture;

                tempPicture = CreateNewPictureBox(pictureBoxes[point.Y, point.X]);
                tempPoint = point;

                PictureBox newPicture = CreateNewPictureBox(doomBoy, SIZE_OF_IMAGE_X, SIZE_OF_IMAGE_Y);
                newPicture.Margin = new Padding(0);

                pictureBoxes[point.Y, point.X] = newPicture;

                foreach (Control c in FlowLayoutPanels[point.Y].Controls)
                {
                    if (c.Name == $"picture{point.X}")
                    {
                        PictureBox picture = (PictureBox)c;

                        picture.Image = (Image)newPicture.Image.Clone();
                        c.Refresh();
                        break;
                    }
                }

                await Task.Delay(SLEEP_TIME);
                
            }

            GameCompleteEvent?.Invoke(this, new GameCompleteArgs() { Result = "Complete" });
        }

        private void LoadMap()
        {
            AlgorithmMap = new Maps(mapName);
            algorithm = new Algorithm(AlgorithmMap);
            myMap = new Maps(mapName);

            pictureBoxes = new PictureBox[myMap.Map.GetLength(0), myMap.Map.GetLength(1)];
            FlowLayoutPanels = new MyFlowLayoutPanel[myMap.Map.GetLength(0)];

            for (int y = 0; y < myMap.Map.GetLength(0); y++)
            {
                MyFlowLayoutPanel flowLayoutPanel = new MyFlowLayoutPanel();
                
                FlowLayoutPanels[y] = flowLayoutPanel;
                flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
                flowLayoutPanel.Margin = new Padding(0);
                flowLayoutPanel.AutoSize = true;
                for (int x = 0; x < myMap.Map.GetLength(1); x++)
                {
                    char tempObject = myMap.ReturnObject(x, y);
                    PictureBox pictureBox = CreateNewPictureBox(ReturnImage(tempObject), SIZE_OF_IMAGE_X, SIZE_OF_IMAGE_Y);
                    pictureBox.Name = $"picture{x}";
                    pictureBox.Margin = new Padding(0);
                    flowLayoutPanel.Controls.Add(pictureBox);
                    pictureBoxes[y, x] = pictureBox;
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

        private PictureBox CreateNewPictureBox(PictureBox pictureBox)
        {
            PictureBox newPictureBox = new PictureBox();
            newPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Bitmap image = new Bitmap(pictureBox.Image);
            newPictureBox.ClientSize = new Size(pictureBox.Size.Width, pictureBox.Size.Height);
            newPictureBox.Image = image;
            return newPictureBox;
        }

        private Bitmap ReturnImage(char _object)
        {
            switch(_object)
            {
                case '.':
                    return Properties.Resources.floor;
                case 'X':
                    return Properties.Resources.wall;
                case 'a':
                    return Properties.Resources.cardA;
                case 'b':
                    return Properties.Resources.cardB;
                case 'c':
                    return Properties.Resources.cardC;
                case 'd':
                    return Properties.Resources.cardD;
                case 'e':
                    return Properties.Resources.cardE;
                case 'A':
                    return Properties.Resources.doorA;
                case 'B':
                    return Properties.Resources.DoorB;
                case 'C':
                    return Properties.Resources.DoorC;
                case 'D':
                    return Properties.Resources.DoorD;
                case 'E':
                    return Properties.Resources.DoorE;
                case 'S':
                    return Properties.Resources.spawn;
                case 'Q':
                    return Properties.Resources.quit;
                case 'H':
                    return Properties.Resources.medkit;
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                    return Properties.Resources.fire;
                default:
                    return Properties.Resources.unknown;
            }
        }
    }
}
