using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network
{
    class Neuron
    {
        //the neuron output line 
        public double[] DendritesWeights;
        //gradient descent learning rule for updating the weights
        public double Delta;

        public double Value;

        public Neuron(int NumberOfNeuronOutputLine)
        {
            DendritesWeights = new double[NumberOfNeuronOutputLine];

            Random random = new Random();

            for (int i = 0 ; i < NumberOfNeuronOutputLine; i++ )
            {
                random.NextDouble();
                DendritesWeights[i] = random.NextDouble();
            }

            Delta = 0;
            Value = 0;
        }
            
    }





    class Layer
    {

        //a bias value allows to shift the activation function 
        public Neuron Bias;

        public Neuron[] Neurons;

        public Layer(int NumberOfNeuron,int NumberofNeuroninTheNextLayer)
        {
            Neurons = new Neuron[NumberOfNeuron];

            for(int i=0;i< NumberOfNeuron;i++)
            {
                Neurons[i] = new Neuron(NumberofNeuroninTheNextLayer);
            }

            //Check if this layer not the output layer
            if(NumberofNeuroninTheNextLayer != 0)
            {
                Bias = new Neuron(NumberofNeuroninTheNextLayer);
            }
            

        }
    }





    class NeuralNetwork
    {
        public Layer[] Layers;
        //The length of the leap you take in the current direction of the slope
        public double LearningRate;

        public NeuralNetwork(double learningRate, int[] layers)
        {
            if (layers.Length < 2)
            {
                return;
            }

            LearningRate = learningRate;

            Layers = new Layer[layers.Length];

            for (int i=0;i<layers.Length;i++)
            {
                //Check if this layer is output layer
                if (i==layers.Length-1)
                {
                    Layers[i] = new Layer(layers[i],0);
                }
                else
                {
                    Layers[i] = new Layer(layers[i], layers[i+1]);
                }
 
            }
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public double[] Predict(double[] inputLayer)
        {

            if (inputLayer.Length != Layers[0].Neurons.Length)
            {
                return null;
            }



            for (int i=0;i<Layers.Length;i++)
            {

                 for(int j=0;j<Layers[i].Neurons.Length;j++)
                 {

                    if (i == 0)
                    {
                        Layers[i].Neurons[j].Value = inputLayer[i];
                    }
                    else
                    {
                        for (int h = 0; h < Layers[i - 1].Neurons.Length;h++)
                        {
                            // Calclute the value of this neurons
                            Layers[i].Neurons[j].Value += Layers[i - 1].Neurons[h].Value * Layers[i - 1].Neurons[h].DendritesWeights[j];
                        }
                        // Add bias value to neuran value and apply sigmoid func to value
                        Layers[i].Neurons[j].Value = Sigmoid(Layers[i].Neurons[j].Value + Layers[i - 1].Bias.DendritesWeights[j]);
                    }

                 }

            }

            double[] OutputLayer = new double[Layers[Layers.Length - 1].Neurons.Length];

            for (int i=0;i<Layers[Layers.Length-1].Neurons.Length;i++)
            {
                //take the output layer to return it
                OutputLayer[i] = Layers[Layers.Length - 1].Neurons[i].Value;
            }
         
            return OutputLayer;

        }

        public bool Train(double[] input, double[] output)
        {
           
            if (input.Length != Layers[0].Neurons.Length || output.Length != Layers[Layers.Length - 1].Neurons.Length)
            {
                return false;
            }


            // Run Forward
            Predict(input);


            // Calclute Output Error   
            for(int i=0;i<Layers[Layers.Length-1].Neurons.Length;i++)
            {

                double OutputValue = Layers[Layers.Length - 1].Neurons[i].Value;
                Layers[Layers.Length - 1].Neurons[i].Delta = OutputValue * (1 - OutputValue) * (OutputValue - output[i]);

            }

            // Calclute Hidden Error
            for (int i = Layers.Length-2;i>0;i--)
            {

                for(int j=0;j<Layers[i].Neurons.Length;j++)
                {

                    for(int h=0;h<Layers[i+1].Neurons.Length;h++)
                    {

                        Layers[i].Neurons[j].Delta +=   Layers[i].Neurons[j].DendritesWeights[h] * Layers[i + 1].Neurons[h].Delta;

                    }

                    Layers[i].Neurons[j].Delta *= Layers[i].Neurons[j].Value * (1 - Layers[i].Neurons[j].Value);

                }

            }

            
            
            for (int i=0;i<Layers.Length-1;i++)
            {
                // Update Bias
                for (int h = 0; h < Layers[i].Bias.DendritesWeights.Length; h++)
                {

                    Layers[i].Bias.DendritesWeights[h] += - LearningRate * Layers[i + 1].Neurons[h].Delta;

                }

                // Update Weight
                for (int j=0;j<Layers[i].Neurons.Length;j++)
                {
                    for(int h=0;h<Layers[i].Neurons[j].DendritesWeights.Length;h++)
                    {
                      
                        Layers[i].Neurons[j].DendritesWeights[h] += - LearningRate * Layers[i].Neurons[j].Value * Layers[i+1].Neurons[h].Delta;
                  
                    }
                }
            }           

            return true;

        }

    }




    class Program
    {
        static void Main(string[] args)
        {
            int[] Layers = new int[2] { 2,  1 };

            NeuralNetwork NN = new NeuralNetwork(0.01f, Layers);

            double[] Inputlayer1 = new double[2] {1,1};
            double[] Inputlayer2 = new double[2] {0, 0 };
            
            double[] OutputLayer1 = new double[1] { 1};
            double[] OutputLayer2 = new double[1] { 0 };


            for (int i=0;i<1000000;i++)
            {
                NN.Train(Inputlayer1, OutputLayer1);
                NN.Train(Inputlayer2, OutputLayer2);
            }
  
            Console.WriteLine("EndTraining");

            double[] Predictions1 = NN.Predict(Inputlayer1);
            Console.WriteLine(Predictions1[0]);

            double[] Predictions2 = NN.Predict(Inputlayer2);
            Console.WriteLine(Predictions2[0]);

            Console.ReadKey();
        }
    }
}
