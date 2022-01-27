using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOL_jacobburgo
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        #region Fields
        // The universe array
        bool[,] universe = new bool[30, 30];
        bool[,] scratchPadUniverse = new bool[30, 30];
        int universeWidth, universeHeight;
        
        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.LawnGreen;

        // The Timer class
        readonly Timer timer = new Timer();

        // Generation count
        int generations = 0;
        int seed = 0;

        // Used to toggle paint events
        bool displayGrid = true;
        bool displayNeighborCount = true;
        bool displayHUD = true;

        // Controls which boundary type is being used:
        // true: Finite
        // false: Torrodial
        bool neighborCountType = true;

        #endregion

        /** Handles initialization and the application of settings */
        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running

            gridColor = Properties.Settings.Default.gridColor;
            cellColor = Properties.Settings.Default.cellColor;
            graphicsPanel.BackColor = Properties.Settings.Default.BackColor;
            timer.Interval = Properties.Settings.Default.timerInterval;
            universeWidth = Properties.Settings.Default.universeX;
            universeHeight = Properties.Settings.Default.universeY;
            universe = new bool[universeWidth, universeHeight];
            scratchPadUniverse = new bool[universeWidth, universeHeight];
        }

        #region Progression Functions
        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            int count;
            // Iterate through the universe in the y, top to bottom & in the x, left to right.
            // It then assigns true/false to universe[x,y] based on conditionals for later painting
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (neighborCountType == true)
                    {
                        count = CountNeighborsFinite(x, y);
                    }
                    else
                    {
                        count = CountNeighborsTorroidal(x, y);
                    }

                    if (count < 2 && universe[x, y] == true)
                    {
                        scratchPadUniverse[x, y] = false;
                    }
                    else if (count > 3 && universe[x, y] == true)
                    {
                        scratchPadUniverse[x, y] = false;
                    }
                    else if ((count == 2 || count == 3) && universe[x, y] == true)
                    {
                        scratchPadUniverse[x, y] = true;
                    }
                    else if (count == 3 && universe[x, y] == false)
                    {
                        scratchPadUniverse[x, y] = true;
                    }
                }
            }

            bool[,] temp = scratchPadUniverse;
            scratchPadUniverse = universe;
            universe = temp;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    scratchPadUniverse[x, y] = false;
                }
            }

            // Increment generation count
            generations++;

            // Update status strip generations
            generationsToolStripStatusLabel.Text = "Generations: " + generations.ToString();
            IntervalToolStripStatusLabel.Text = "Interval: " + timer.Interval;
            seedCountToolStripStatusLabel.Text = "Seed: " + seed;


            graphicsPanel.Invalidate();
            statusStrip.Invalidate();

        }

        #endregion

        #region NeighborCount
        /** Handles boundary type when set to Finite */
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /** Handles the Finite click event. Toggles CountNeighborsFinite(x, y) */
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            neighborCountType = true;
            torrodialToolStripMenuItem.Checked = false;

            graphicsPanel.Invalidate();
        }

        /** Handles boundary type when set to Torroidal */
        private int CountNeighborsTorroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        /** Handles the torrodial click event. Toggles CountNeighborsTorroidal(x, y) */
        private void torrodialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            neighborCountType = false;
            finiteToolStripMenuItem.Checked = false;

            graphicsPanel.Invalidate();
        }

        #endregion

        #region Randomize
        /** Randomizes the universe based on random class with seed*/
        private void RandomizeBySeed()
        {
            Random randSeed = new Random(seed);
            //Random randTime = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int seedCheck = randSeed.Next(0, 2);
                    if (seedCheck == 0)
                    {
                        universe[x, y] = true;
                    }
                }
            }
        }

        /** Handles the Randomize by Seed click event. Randomizes from seed. */
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomizeBySeed();
            graphicsPanel.Invalidate();
        }

        /** Randomizes the universe based on random class*/
        private void RandomizeByTime()
        {
            Random randTime = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int timeCheck = randTime.Next(0, 2);
                    if (timeCheck == 0)
                    {
                        universe[x, y] = true;
                    }
                }
            }
        }

        /** Handles the Randomize from time click event. Randomizes from the Random class which uses local machine time to seed. */
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomizeByTime();
            graphicsPanel.Invalidate();
        }

        #endregion

        #region Painting
        /** Hanldes painting to the graphics panel*/
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("Arial", 20f);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines
            Pen gridPen = new Pen(gridColor, 1);
            // A pen for drawing the 10x10 grid lines
            Pen gridPen10 = new Pen(gridColor, 2);

            // A Brush for filling living cell's interiors
            Brush cellBrush = new SolidBrush(cellColor);
            Brush HUDBrush = new SolidBrush(Color.HotPink);

            // A rectangle to represent each cell in pixels
            RectangleF cellRect = RectangleF.Empty;

            string generationsString = "Generations: " + generations.ToString();
            string intervalString = "Interval: " + timer.Interval;
            string seedString = "Seed: " + seed;
            if (displayHUD)
            {
                e.Graphics.DrawString(generationsString, font, HUDBrush, new Point(0, graphicsPanel.ClientSize.Height - 30));
                e.Graphics.DrawString(intervalString, font, HUDBrush, new Point(0, graphicsPanel.ClientSize.Height - 60));
                e.Graphics.DrawString(seedString, font, HUDBrush, new Point(0, graphicsPanel.ClientSize.Height - 90));
            }

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Setting cellRect dimensions
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    int count;

                    if (neighborCountType == true)
                    {
                        count = CountNeighborsFinite(x, y);
                    }
                    else
                    {
                        count = CountNeighborsTorroidal(x, y);
                    }


                    // Fill the cell with a brush if alive
                    if (universe[x, y])
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }


                    // Draws a 1x1 and 10x10 grid
                    if (displayGrid)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                        e.Graphics.DrawRectangle(gridPen10, (cellRect.X * 10), (cellRect.Y * 10), (cellRect.Width * 10), (cellRect.Height * 10));
                    }

                    if (displayNeighborCount)
                    {
                        if (count != 0 || universe[x, y])
                        {
                            if ((count == 2 || count == 3) && universe[x, y] == true)
                            {
                                e.Graphics.DrawString(count.ToString(), graphicsPanel.Font, Brushes.DarkGreen, cellRect, stringFormat);
                            }
                            else if (count == 3 && universe[x, y] == false)
                            {
                                e.Graphics.DrawString(count.ToString(), graphicsPanel.Font, Brushes.Green, cellRect, stringFormat);
                            }
                            else
                            {
                                e.Graphics.DrawString(count.ToString(), graphicsPanel.Font, Brushes.Red, cellRect, stringFormat);
                            }
                        }
                    }
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        /** Handles mouse click event setting universe point to true */
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = (float)graphicsPanel.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                graphicsPanel.Invalidate();
                statusStrip.Invalidate();
            }
        }

        #endregion

        #region Start, Pause, Next
        /** Handles the pause toolstrip button click event */
        private void pauseToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        /** Handles the start toolstrip button click event */
        private void startToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        /** Handles the next generation toolstrip button click event */
        private void nextGenToolStripButton_Click(object sender, EventArgs e)
        {
            NextGeneration();

            graphicsPanel.Invalidate();
        }

        #endregion

        #region New, Save, Open
        /** Handles the new toolstrip button click event */
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }

            graphicsPanel.Invalidate();
        }

        /** Handles the save click event. Writes universe to a plaintext file */
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                DateTime dateTime = DateTime.Now;
                StreamWriter writer = new StreamWriter(dlg.FileName);

                writer.WriteLine("!" + dateTime);
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    String currentRowString = string.Empty;
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == true)
                        {
                            currentRowString += 'O';
                        }
                        else
                        {
                            currentRowString += '.';
                        }
                    }
                    writer.WriteLine(currentRowString);
                }
                writer.Close();
            }
        }

        /** Handles the open click event. Opens plain text file and rendering to the application */
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                int maxWidth = 0;
                int maxHeight = 0;

                while (!reader.EndOfStream)
                {
                    string row = reader.ReadLine();

                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    else
                    {
                        maxHeight++;
                    }
                    maxWidth = row.Length;
                }

                universe = new bool[maxWidth, maxHeight];
                scratchPadUniverse = new bool[maxWidth, maxHeight];

                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                int y = 0;
                while (!reader.EndOfStream)
                {
                    string row = reader.ReadLine();
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    else
                    {
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            universe[xPos, y] = row[xPos] == 'O';
                        }
                        y++;
                    }
                }
                reader.Close();
            }
            graphicsPanel.Invalidate();
        }

        #endregion

        #region Toggles
        /** Handles the HUD click event. Toggles HUD on/off. */
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayHUD = !displayHUD;
            graphicsPanel.Invalidate();
        }

        /** Handles the grid toggle click event */
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayGrid = !displayGrid;
            graphicsPanel.Invalidate();
        }

        /** Handles the neighborCount toggle click event */
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayNeighborCount = !displayNeighborCount;
            graphicsPanel.Invalidate();
        }

        #endregion

        #region Dialog Boxes
        /** Handles the grid color click event */
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog gridColorDialog = new ColorDialog();
            gridColorDialog.Color = gridColor;
            if (DialogResult.OK == gridColorDialog.ShowDialog())
            {
                gridColor = gridColorDialog.Color;

                graphicsPanel.Invalidate();
            }
        }

        /** Handles the cell color click event */
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cellColorDialog = new ColorDialog();
            cellColorDialog.Color = cellColor;
            if (DialogResult.OK == cellColorDialog.ShowDialog())
            {
                cellColor = cellColorDialog.Color;

                graphicsPanel.Invalidate();
            }
        }

        /** Handles the background color click event */
        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog backgroundColorDialog = new ColorDialog();
            backgroundColorDialog.Color = graphicsPanel.BackColor;
            if (DialogResult.OK == backgroundColorDialog.ShowDialog())
            {
                graphicsPanel.BackColor = backgroundColorDialog.Color;

                graphicsPanel.Invalidate();
            }
        }

        /** Handles the universe dimensions click event */
        private void universeDimensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UniverseDimensionsDialog udd = new UniverseDimensionsDialog();
            udd.SetUniverseWidth(universe.GetLength(0));
            udd.SetUniverseHeight(universe.GetLength(1));
            universeWidth = udd.GetUniverseWidth();
            universeHeight = udd.GetUniverseHeight();

            if (DialogResult.OK == udd.ShowDialog()) 
            {
                universe = new bool[udd.GetUniverseWidth(), udd.GetUniverseHeight()];
                scratchPadUniverse = new bool[udd.GetUniverseWidth(), udd.GetUniverseHeight()];
                universeWidth = udd.GetUniverseWidth();
                universeHeight = udd.GetUniverseHeight();

                graphicsPanel.Invalidate();
            }
        }

        /** Handles the seed/Interval click event */
        private void seedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedDialog sd = new SeedDialog();
            sd.SetTimerInterval(timer.Interval);
            Random randSeed = new Random();
            if (seed == 0)
            {
                seed = randSeed.Next(0, 499);
            }
            sd.SetSeed(seed);

            if (DialogResult.OK == sd.ShowDialog())
            {
                timer.Interval = sd.GetTimerInterval();
                seed = sd.GetSeed();
            }
        }

        #endregion

        #region Form Closing Events
        /** Handles when form closes. Saves vars to settings */
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.gridColor = gridColor;
            Properties.Settings.Default.cellColor = cellColor;
            Properties.Settings.Default.universeX = universeWidth;
            Properties.Settings.Default.universeY = universeHeight;
            Properties.Settings.Default.BackColor = graphicsPanel.BackColor;
            Properties.Settings.Default.timerInterval = timer.Interval;

             Properties.Settings.Default.Save();
        }

        /** Handles the exit toolstrip menu click event */
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Reset & Reload
        /** Handles the reset feature. Resets vars to initial state */
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            gridColor = Properties.Settings.Default.gridColor;
            cellColor = Properties.Settings.Default.cellColor;
            universeWidth = Properties.Settings.Default.universeX;
            universeHeight = Properties.Settings.Default.universeY;
            graphicsPanel.BackColor = Properties.Settings.Default.BackColor;
            timer.Interval = Properties.Settings.Default.timerInterval;
        }

        /** Handles the reload feature. Reloads settings that were implemented at start of application */
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();

            gridColor = Properties.Settings.Default.gridColor;
            cellColor = Properties.Settings.Default.cellColor;
            universeHeight = Properties.Settings.Default.universeY;
            universeWidth = Properties.Settings.Default.universeX;
            graphicsPanel.BackColor = Properties.Settings.Default.BackColor;
            timer.Interval = Properties.Settings.Default.timerInterval;
        }

        #endregion
    }
}
