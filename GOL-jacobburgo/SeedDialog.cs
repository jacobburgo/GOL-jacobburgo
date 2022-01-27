using System;
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
    public partial class SeedDialog : System.Windows.Forms.Form
    {
        public SeedDialog()
        {
            InitializeComponent();
        }

        public int GetSeed()
        {
            return (int)seedNumericUpDown.Value;
        }

        public int GetTimerInterval()
        {
            return (int)timerNumericUpDown.Value;
        }

        public void SetSeed(int seed)
        {
            seedNumericUpDown.Value = seed;
        }

        public void SetTimerInterval(int miliseconds)
        {
            timerNumericUpDown.Value = miliseconds;
        }
    }
}
