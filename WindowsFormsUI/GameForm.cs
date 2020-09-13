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
        private int currentHP = CommonStuff.MAX_HP;
        private Maps AlgorithmMap;
        private Maps myMap;
        private Algorithm algorithm;
        private string mapName;
        //Array for all images on a map.
        private PictureBox[,] pictureBoxes;
        //Represent rows of a map.
        private MyFlowLayoutPanel[] FlowLayoutPanels;
        private const int SIZE_OF_IMAGE_X = 40;
        private const int SIZE_OF_IMAGE_Y = 40;
        private Bitmap doomBoy = Properties.Resources.doomBoyDown;
        private Bitmap floor = Properties.Resources.floor;
        private event EventHandler GameCompleteEvent;
        private int numberOfSteps = -1;

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
            PleaseWaitForm pleaseWaitForm;
            try
            {
                pleaseWaitForm = new PleaseWaitForm();

                pleaseWaitForm.Show();
                pleaseWaitForm.Refresh();

                LoadMap();

                pleaseWaitForm.Dispose();
            }
            catch (ArgumentException ex)
            {
                MyDebug.WriteExceptionInFile(ex, CommonStuff.projectName, CommonStuff.logFolderName);
                ShowMessageBox(ex.Message, "Error");
            }
            catch (Exception ex)
            {
                MyDebug.WriteExceptionInFile(ex, CommonStuff.projectName, CommonStuff.logFolderName);
                ShowMessageBox(ex.Message, "Error");
            }
        }
        private void GameComplete(object sender, EventArgs e)
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
            if (System.IO.File.Exists(CommonStuff.movesDefaultName))
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
                #region Changing previous image
                //Change image of previous point with image it had.
                foreach (Control c in FlowLayoutPanels[tempPoint.Y].Controls)
                {
                    if (c.Name == $"picture{tempPoint.X}")
                    {
                        PictureBox picture = (PictureBox)c;
                        //When trying to call dispose before uploading new image - get error.
                        picture.Image = (Image)tempPicture.Image.Clone();
                        c.Refresh();
                        break;
                    }
                }

                pictureBoxes[tempPoint.Y, tempPoint.X] = tempPicture;

                #endregion

                # region Deleting current image if not floor
                char tempObject = myMap.ReturnObject(point);
                if(tempObject != '.')
                {
                    //Replace image of object to image of floor.
                    if (myMap.Doors.Contains(tempObject) ||
                    myMap.Keys.Contains(tempObject) ||
                    Maps.MEDKIT == tempObject)
                    {
                        if (Maps.MEDKIT == tempObject)
                        {
                            UseMedkit();
                        }
                        PictureBox floorPicture = CreateNewPictureBox(floor, SIZE_OF_IMAGE_X, SIZE_OF_IMAGE_Y);
                        pictureBoxes[point.Y, point.X] = floorPicture;
                    }
                    else if (myMap.FirePower.Contains(tempObject))
                    {
                        int firePower = (int)Char.GetNumericValue(tempObject);
                        GetDamage(firePower);
                    }
                }

                #endregion

                //Save current image for recovering it in the next step.
                //Coping image, not making a link to it!
                tempPicture = CreateNewPictureBox(pictureBoxes[point.Y, point.X]);
                tempPoint = point;


                #region Changing current picture with doomBoy picture.

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
                #endregion

                numberOfSteps++;
                labelSteps.Text = numberOfSteps.ToString();
                labelHP.Text = currentHP.ToString();

                await Task.Delay(CommonStuff.SLEEP_TIME);
                
            }

            GameCompleteEvent?.Invoke(this, new EventArgs());
        }

        private void LoadMap()
        {
            AlgorithmMap = new Maps(mapName);
            myMap = new Maps(AlgorithmMap);
            algorithm = new Algorithm(AlgorithmMap);



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
            pictureBox.Image.Dispose();
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
            currentHP = CommonStuff.MAX_HP;
        }

        private void GameForm_Shown(object sender, EventArgs e)
        {
            if (CheckForResult())
            {
                algorithm.WriteResultToFile(CommonStuff.movesDefaultName);

                GameCompleteEvent += GameComplete;

                ShowCharactersMovement();
            }
            
        }
    }
}
