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
    public partial class Form1 : Form
    {

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

        bool displayGrid = true;
        bool displayNeighborCount = true;
        bool displayHUD = true;

        // Controls which boundary type is being used:
        // true: Finite
        // false: Torrodial
        bool neighborCountType = true;

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
                        count = CountNeighborsToroidal(x, y);
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

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

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

            // A rectangle to represent each cell in pixels
            RectangleF cellRect = RectangleF.Empty;

            string generationsString = "Generations: " + generations.ToString();
            string intervalString = "Interval: " + timer.Interval;
            string seedString = "Seed: " + seed;
            if (displayHUD)
            {
                e.Graphics.DrawString(generationsString, font, cellBrush, new Point(0, graphicsPanel.ClientSize.Height - 30));
                e.Graphics.DrawString(intervalString, font, cellBrush, new Point(0, graphicsPanel.ClientSize.Height - 60));
                e.Graphics.DrawString(seedString, font, cellBrush, new Point(0, graphicsPanel.ClientSize.Height - 90));
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
                        count = CountNeighborsToroidal(x, y);
                    }


                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }


                    // Draws a 1x1 and 10x10 grid
                    if (displayGrid == true)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                        e.Graphics.DrawRectangle(gridPen10, (cellRect.X * 10), (cellRect.Y * 10), (cellRect.Width * 10), (cellRect.Height * 10));
                    }

                    if (displayNeighborCount == true)
                    {
                        if (count != 0 || universe[x, y] == true)
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

        private int CountNeighborsToroidal(int x, int y)
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

        // Handles the new toolstrip button click event
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

        // Handles the exit toolstrip menu click event
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Handles the pause toolstrip button click event
        private void pauseToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        // Handles the start toolstrip button click event
        private void startToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        // Handles the next generation toolstrip button click event
        private void nextGenToolStripButton_Click(object sender, EventArgs e)
        {
            NextGeneration();

            graphicsPanel.Invalidate();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayGrid = !displayGrid;
            graphicsPanel.Invalidate();
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayNeighborCount = !displayNeighborCount;
            graphicsPanel.Invalidate();
        }

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

        private void torrodialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            neighborCountType = false;

            graphicsPanel.Invalidate();
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            neighborCountType = true;

            graphicsPanel.Invalidate();
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomizeBySeed();
            graphicsPanel.Invalidate();
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomizeByTime();
            graphicsPanel.Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                DateTime dateTime = DateTime.Now;
                StreamWriter writer = new StreamWriter(dlg.FileName);

                writer.WriteLine(dateTime);
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    String currentRowString = string.Empty;
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == true)
                        {
                            currentRowString += "0";
                        }
                        else
                        {
                            currentRowString += ".";
                        }
                    }
                    writer.WriteLine(currentRowString);
                }
                
                    // Create a string to represent the current row.
                    //String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
          
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.

                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                    

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayHUD = !displayHUD;
            graphicsPanel.Invalidate();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
