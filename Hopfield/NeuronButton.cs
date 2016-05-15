using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hopfield
{
    public class NeuronButton : Button
    {
        private void InitializeComponent()
        {

        }
        public int State { get; set; }
        public int I { get; set; }
        public int J { get; set; }        

        public NeuronButton(int state, int i, int j)
        {
            State = state;
            I = i;
            J = j;
            
            TabStop = false;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 1;
            FlatAppearance.BorderColor = Color.Black;
        }
        
        public void ChangeColor()
        {
            if (State == 1)
            {
                BackColor = Color.Green;
                FlatAppearance.BorderColor = Color.Green;
            }
            else
            {
                BackColor = DefaultBackColor;
                FlatAppearance.BorderColor = Color.Black;
            }
                
        }

  
    }
}
