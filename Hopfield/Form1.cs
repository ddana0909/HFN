using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hopfield
{
    public partial class Form1 : Form
    {
        String path = "../../Numbers/";
        private int[,] matrix;
        private int _noOfNeurons;
        private int _inputMatrixSize;
        private bool[] _visitedNeurons;
        private bool load;
        private bool train;
        private Random _randomNumber;
        private double[,] WeightMatrix;
        private int[,] InputMatrix;
        private int[,] OutputMatrix;
        public void initHopfieldNetwork(int noOfNeurons, int inputMatrixSize)
        {
            _noOfNeurons = noOfNeurons;
            _inputMatrixSize = inputMatrixSize;
            matrix = new int[_inputMatrixSize, _inputMatrixSize];
            WeightMatrix = new double[_noOfNeurons, _noOfNeurons];
            InputMatrix = new int[_inputMatrixSize,_inputMatrixSize];
            _visitedNeurons = new bool[_noOfNeurons];
            _randomNumber = new Random();
        }

        public void TrainNetwork(List<int[,]> patternsToLearn)
        {
            var noOfPaterns = patternsToLearn.Count;
            foreach (var pattern in patternsToLearn)
            {
                for (int i = 0; i < _noOfNeurons; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (i == j)
                            WeightMatrix[i, j] = 0;
                        else
                        {
                            double wij = 1.0 / noOfPaterns * pattern[i / _inputMatrixSize, i % _inputMatrixSize] * pattern[j / _inputMatrixSize, j % _inputMatrixSize];
                            WeightMatrix[i, j] += wij;
                            WeightMatrix[j, i] += wij;
                        }

                    }
                }
            }
        }

        private void SetOutputAsInput()
        {
            OutputMatrix = new int[_inputMatrixSize, _inputMatrixSize];
            for (int i = 0; i < _inputMatrixSize; i++)
            {
                for (int j = 0; j < _inputMatrixSize; j++)
                {
                    OutputMatrix[i, j] = InputMatrix[i, j];
                }
            }
        }

        private void CleanVisitedNeuronMatrix()
        {
            for (int i = 0; i < _noOfNeurons; i++)
            {
                _visitedNeurons[i] = false;
            }
        }

        private int GetRandomUnVisitedNeuron()
        {
            int index;
            do
            {
                index = _randomNumber.Next(0, _noOfNeurons);
            } while (_visitedNeurons[index]);
            return index;
        }

        private bool allVisited()
        {
            for (int i = 0; i < _noOfNeurons; i++)
            {
                if (!_visitedNeurons[i])
                    return false;
            }
            return true;
        }


        public void RunRecognition()
        {
            SetOutputAsInput();
            int noOfChanges = 1000;
            while (noOfChanges > 0)
            {
                noOfChanges = 0;
                CleanVisitedNeuronMatrix();
                while (!allVisited())
                {
                    var neuron = GetRandomUnVisitedNeuron();
                    _visitedNeurons[neuron] = true;
                        double neuronOutput = (double)InputMatrix[neuron / _inputMatrixSize, neuron % _inputMatrixSize];

                        for (int i = 0; i < _noOfNeurons; i++)
                        {
                            neuronOutput += WeightMatrix[neuron, i] * OutputMatrix[i / _inputMatrixSize, i % _inputMatrixSize];
                        }
                        var discreetNeuronOutput = (neuronOutput < 0) ? -1 : 1;
                        if (OutputMatrix[neuron / _inputMatrixSize, neuron % _inputMatrixSize] != discreetNeuronOutput)
                        {
                            noOfChanges++;
                            OutputMatrix[neuron / _inputMatrixSize, neuron % _inputMatrixSize] = discreetNeuronOutput;
                            DrawMatrix(OutputMatrix);
                            Thread.Sleep(200);
                        }
                    
                }



            }
            MessageBox.Show("Recognition is finished!");

        }

        public Form1()
        {
            InitializeComponent();
            initHopfieldNetwork(400, 20);

            DrawBoard(_inputMatrixSize);
        }
        private void DrawBoard(int n)
        {
            int k = 0, l = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var x = new NeuronButton(-1, i, j);
                    x.Location = new Point(k, l += 20);
                    x.Height = 20;
                    x.Width = 20;
                    x.Click += new EventHandler(GridClick);
                    Controls.Add(x);
                }
                k += 20;
                l = 0;
            }
        }
        public void WriteMatrixToFile(int[,] a)
        {
            using (System.IO.TextWriter tw = new System.IO.StreamWriter(path + textBox1.Text + ".txt"))
            {
                for (int i = 0; i < _inputMatrixSize; i++)
                {
                    for (int j = 0; j < _inputMatrixSize; j++)
                    {
                        if (a[i, j] == 1)
                        {
                            tw.Write(a[i, j]);
                        }
                        else {
                            tw.Write(-1);
                        }
                        tw.Write(" ");
                    }
                    tw.WriteLine();
                }
            }
        }
        void GridClick(object sender, EventArgs e)
        {
            var x = (NeuronButton)sender;
            x.State = -x.State;
            x.ChangeColor();
            matrix[x.J, x.I] = x.State;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (  comboBox1.SelectedIndex != -1)
            {
                DrawNumber(comboBox1.SelectedIndex.ToString());
                load = true;
            }
            else
            {
                MessageBox.Show("Choose a number to load!");
            }
        }

        private int[,] GetmatrixFromFile(string path)
        {
            int[,] m = new int[_inputMatrixSize, _inputMatrixSize];
            using (Stream stream = File.Open("../../Numbers/" + path + ".txt", FileMode.Open))
            using (TextReader sr = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] arr = line.Split(' ');
                    int k = 0;
                    for (int j = 0; j < arr.Length; j++)
                    {
                        if (arr[j] != "")
                        {
                            m[i, k++] = (Int32.Parse(arr[j]));
                        }
                    }
                    i++;
                }
            }
            return m;
        }
        private void DrawMatrix(int[,]matrixToDraw)
        {
            for (int i = 0; i < _inputMatrixSize; i++)
            {
                for (int j = 0; j < _inputMatrixSize; j++)
                {
                    int uu = Controls.OfType<NeuronButton>().Where(a => a.I == j && a.J == i).First().State;
                    Controls.OfType<NeuronButton>().Where(a => a.I == j && a.J == i).First().State = matrixToDraw[i, j];
                    Controls.OfType<NeuronButton>().Where(a => a.I == j && a.J == i).First().ChangeColor();
                    if (uu != matrixToDraw[i, j])
                    {
                        Refresh();
                    }
                }
            }
        }
        public void DrawNumber(string fileName)
        {
            matrix = GetmatrixFromFile(fileName);
            DrawMatrix(matrix);
        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            var patterns = new List<int[,]>();
            if (checkedListBox1.CheckedItems.Count != 0)
            {
                foreach (object itemChecked in checkedListBox1.CheckedItems)
                {
                    patterns.Add(GetmatrixFromFile((string)itemChecked));
                }
                TrainNetwork(patterns);
                train = true;
                MessageBox.Show("Weight matrix is ready!");
            }
            else
            {
                MessageBox.Show("Choose patern(s) to learn!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (load && train)
            {
                for (int i = 0; i < _inputMatrixSize; i++)
                {
                    for (int j = 0; j < _inputMatrixSize; j++)
                    {
                        InputMatrix[i, j] = matrix[i, j];
                    }
                }

                RunRecognition();
            }
            else
            {
                if (load)
                {
                    MessageBox.Show("Train the network!");
                }
                else
                {
                    MessageBox.Show("Load number!");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == " ")
            {
                WriteMatrixToFile(matrix);
            }
            {
                MessageBox.Show("Give a filename to the pattern!");
            }
        }
    }
}
