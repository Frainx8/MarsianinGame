using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlgorithmLibrary;

namespace WindowsFormsUI
{
    public partial class GameForm : Form
    {
        private MainForm mainForm;
        private static string projectName = "MarsianinGame";
        private static string mapsFolder = @"maps";
        private static string logFolderName = "log";
        private static string movesName = @"moves.txt";
        private const int SLEEP_TIME = 300;
        private const int MAX_XP = 100;
        private int currentHP = MAX_XP;
        private Maps AlgorithmMap;
        private Maps myMap;
        private Algorithm algorithm;
        private string mapName;
        private PictureBox[,] pictureBoxes;
        private MyFlowLayoutPanel[] FlowLayoutPanels;
        private const int SIZE_OF_IMAGE_X = 40;
        private const int SIZE_OF_IMAGE_Y = 40;
        private Bitmap doomBoy = Properties.Resources.doomBoyDown;
        private Bitmap floor = Properties.Resources.floor;
        private event EventHandler<GameCompleteArgs> GameCompleteEvent;
        private int numberOfSteps = -1;
        private class GameCompleteArgs : EventArgs
        {
            public string Result { get; set; }
        }

        
        
        public GameForm()
        {
            InitializeComponent();
        }

        public GameForm(MainForm mainForm, string mapName)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.mapName = mapName;
        }

        private void GameForm_Load(object sender, EventArgs _event)
        {
            try
            {
                LoadMap();
            }
            catch (ArgumentException ex)
            {
                MyDebug.WriteExceptionInFile(ex, projectName, logFolderName);
                ShowMessageBox(ex.Message, "Error");
            }
            catch (Exception ex)
            {
                MyDebug.WriteExceptionInFile(ex, projectName, logFolderName);
                ShowMessageBox(ex.Message, "Error");
            }
        }
        private void GameComplete(object sender, GameCompleteArgs e)
        {
            if (!CheckForMovesTxt())
            {
                MessageBox.Show("File moves.txt wasn't created! " +
                    "Run the program as an administrator!", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            ShowMessageBox("Level complete!", "Result");
        }

        private static bool CheckForMovesTxt()
        {
            if (System.IO.File.Exists(movesName))
            {
                return true;
            }
            else
                return false;
        }

        private bool CheckForResult()
        {
            
            if (algorithm.Result == null)
            {
                if (algorithm.IsDead)
                {
                    ShowMessageBox("The character died in the way!", "Result");
                }
                else
                {
                    ShowMessageBox("There is no way to the exit!", "Result");
                }
                return false;
            }
            else
                return true;
        }

        private void ShowMessageBox(string text, string caption)
        {
            if(caption != "Error")
            {
                MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            DialogResult dialogResult = MessageBox.Show("Do you want change map?", "Change map", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                mainForm.ChangeToLoadingForm();
            }
            else
            {
                Application.Exit();
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

                char tempObject = myMap.ReturnObject(point.X, point.Y);
                if(tempObject != '.')
                {
                    if (Maps.DOORS.Contains(tempObject) ||
                    Maps.KEYS.Contains(tempObject) ||
                    Maps.MEDKIT == tempObject)
                    {
                        if (Maps.MEDKIT == tempObject)
                        {
                            UseMedkit();
                        }
                        PictureBox floorPicture = CreateNewPictureBox(floor, SIZE_OF_IMAGE_X, SIZE_OF_IMAGE_Y);
                        pictureBoxes[point.Y, point.X] = floorPicture;
                    }
                    else if (Maps.FIRE_POWER.Contains(tempObject))
                    {
                        int firePower = (int)Char.GetNumericValue(tempObject);
                        GetDamage(firePower);
                    }
                }

                tempPicture = CreateNewPictureBox(pictureBoxes[point.Y, point.X]);
                tempPoint = point;

                PictureBox doomBoyPicture = CreateNewPictureBox(doomBoy, SIZE_OF_IMAGE_X, SIZE_OF_IMAGE_Y);
                doomBoyPicture.Margin = new Padding(0);

                pictureBoxes[point.Y, point.X] = doomBoyPicture;

                foreach (Control c in FlowLayoutPanels[point.Y].Controls)
                {
                    if (c.Name == $"picture{point.X}")
                    {
                        PictureBox picture = (PictureBox)c;

                        picture.Image = (Image)doomBoyPicture.Image.Clone();
                        c.Refresh();
                        break;
                    }
                }

                numberOfSteps++;
                labelSteps.Text = numberOfSteps.ToString();
                labelHP.Text = currentHP.ToString();

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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void GetDamage(int firePower)
        {
            int damage = 20;
            currentHP -= damage * firePower;
        }
        private void UseMedkit()
        {
            currentHP = MAX_XP;
        }

        private void GameForm_Shown(object sender, EventArgs e)
        {
            if (CheckForResult())
            {
                algorithm.WriteResultToFile(movesName);

                GameCompleteEvent += GameComplete;

                ShowCharactersMovement();
            }
            
        }
    }
}
