﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOL_jacobburgo
{
    public partial class Form1 : Form
    {

        // The universe array
        bool[,] universe = new bool[25 , 25];
        bool[,] scratchPadUniverse = new bool[25, 25];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.LawnGreen;

        // The Timer class
        readonly Timer timer = new Timer();

        // Generation count
        int generations = 0;

        bool displayGrid = true;
        bool displayNeighborCount = true;

        public Form1()
        {
            InitializeComponent();

                // Setup the timer
                timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {

            // Iterate through the universe in the y, top to bottom & in the x, left to right.
            // It then assigns true/false to universe[x,y] based on conditionals for later painting
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count = CountNeighborsFinite(x, y);

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
            /* This will be changed into a more appealing UI and include addition information */
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            graphicsPanel.Invalidate();

        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
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



            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Setting cellRect dimensions
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    int count = CountNeighborsFinite(x, y); 

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

            if (DialogResult.OK == udd.ShowDialog()) 
            {
                universe = new bool[udd.GetUniverseWidth(), udd.GetUniverseHeight()];

                graphicsPanel.Invalidate();
            }
        }

        private void seedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedDialog sd = new SeedDialog();


            if (DialogResult.OK == sd.ShowDialog())
            {

            }
        }
    }
}