using Accord.MachineLearning;
using Accord.Neuro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace NeuralNetwork1
{
    public class StudentNetwork : BaseNetwork
    {
        class Layer
        {
            public Func<double, double> activationFunction = (sum) => 1.0 / (1.0 + Math.Exp(-sum));
            public Func<double, double> activationFunctionDerivative = (sum) => sum * (1 - sum);
            private Neuron[] neurons;

            public Layer(int[] structure, int layer)
            {
                neurons = new Neuron[structure[layer]];
                for (int i = 0; i < structure[layer]; ++i)
                    if (layer == 0)
                        neurons[i] = new Neuron(layer, -1);
                    else
                        neurons[i] = new Neuron(layer, structure[layer - 1]);
            }

            public Neuron this[int key]
            {
                get => neurons[key];
                set => neurons[key] = value;
            }

            public int Length { get => neurons.Length; }

            public double[] output()
            {
                var res = new double[neurons.Length];
                for (int i = 0; i < neurons.Length; ++i)
                    res[i] = neurons[i].output;
                return res;
            }

            public double CalcLoss(Func<double[], double[], double> lossFunction, double[] expectedOutput)
            {
                return lossFunction(output(), expectedOutput);
            }
        }

        class Neuron
        {
            static Random random = new Random();

            public double output;
            public double error = 0;
            public double[] prevLayerWeights = null;

            // biadNeuron Constructor
            public Neuron()
            {
                this.output = 1;
            }

            public Neuron(int layer, int prevLayerSize)
            {
                if (layer >= 1) 
                { 
                    prevLayerWeights = new double[prevLayerSize + 1];
                    for (int i = 0; i < prevLayerWeights.Length; i++)
                        prevLayerWeights[i] = random.NextDouble() * 2 - 1;
                }
            }
        }

        public Stopwatch stopWatch = new Stopwatch();

        private const double learningRate = 0.15;

        private Neuron biasNeuron = new Neuron();
        private List<Layer> layers = new List<Layer>();

        private Func<double[], double[], double> lossFunction = (output, expectedResult) =>
        {
            double res = 0;
            for (int i = 0; i < expectedResult.Length; i++)
                res += Math.Pow(expectedResult[i] - output[i], 2);
            return res / 2;
        };

        private Func<double, double, double> lossFunctionDerivative = (output, expectedResult) => expectedResult - output;

        public StudentNetwork(int[] structure)
        {
            for (int layer = 0; layer < structure.Length; ++layer)
                layers.Add(new Layer(structure, layer));
        }

        private double CalcLoss(double[] expectedOutput)
        {
            return layers.Last().CalcLoss(lossFunction, expectedOutput);
        }

        private double NeuronsAndWeightsDotProduct(int layer, int neuron)
        {
            double sum = biasNeuron.output * layers[layer][neuron].prevLayerWeights[0];
            for (int i = 1; i < layers[layer][neuron].prevLayerWeights.Length; ++i)
                sum += layers[layer - 1][i - 1].output * layers[layer][neuron].prevLayerWeights[i];
            return sum;
        }

        private void ForwardPropagation(double[] input)
        {
            // Filling network input layer
            Parallel.For(0, layers[0].Length, neuron =>
            {
                layers[0][neuron].output = input[neuron];
            });
            //for (int neuron = 0; neuron < layers[0].Length; ++neuron)
            //layers[0][neuron].output = input[neuron];

            // Filling other layers from left to right
            for (int layer = 1; layer < layers.Count; ++layer)
                Parallel.For(0, layers[layer].Length, neuron =>
                {
                    layers[layer][neuron].output = layers[layer].activationFunction(NeuronsAndWeightsDotProduct(layer, neuron));
                });
                //for (int neuron = 0; neuron < layers[layer].Length; ++neuron)
                    //layers[layer][neuron].output = layers[layer].activationFunction(NeuronsAndWeightsDotProduct(layer, neuron));
        }

        private void UpdateWeightsByErros(int layer, int neuron_ind)
        {
            Neuron neuron = layers[layer][neuron_ind];
            neuron.error *= layers[layer].activationFunctionDerivative(neuron.output);

            biasNeuron.error += neuron.error * neuron.prevLayerWeights[0];
            neuron.prevLayerWeights[0] += learningRate * neuron.error * biasNeuron.output;

            for (int i = 1; i < neuron.prevLayerWeights.Length; ++i)
            {
                layers[layer - 1][i - 1].error += neuron.error * neuron.prevLayerWeights[i];
                neuron.prevLayerWeights[i] += learningRate * neuron.error * layers[layer - 1][i - 1].output;
            }

            neuron.error = 0;
        }

        private void BackwardPropagation(double[] expected_output)
        {
            // Filling network output layer error
            Parallel.For(0, layers[layers.Count - 1].Length, i =>
            {
                layers[layers.Count - 1][i].error = lossFunctionDerivative(layers[layers.Count - 1][i].output, expected_output[i]);
            });
            //for (int i = 0; i < layers[layers.Count - 1].Length; ++i)
            //layers[layers.Count - 1][i].error = lossFunctionDerivative(layers[layers.Count - 1][i].output, expected_output[i]);

            // Change other layers weights from right to left
            for (int layer = layers.Count - 1; layer > 0; --layer)
                Parallel.For(0, layers[layer].Length, neuron =>
                {
                    UpdateWeightsByErros(layer, neuron);
                });
                //for (int neuron = 0; neuron < layers[layer].Length; ++neuron)
                    //UpdateWeightsByErros(layer, neuron);
        }

        public override int Train(Sample sample, double acceptableError, bool parallel)
        {
            int iters = 0;

            while (true)
            {
                iters++;

                ForwardPropagation(sample.input);

                if (CalcLoss(sample.Output) <= acceptableError || iters > 30)
                    break;

                BackwardPropagation(sample.Output);
            }

            return iters;
        }

        double OneSampleRun(Sample sample)
        {
            ForwardPropagation(sample.input);
            double loss = CalcLoss(sample.Output);
            BackwardPropagation(sample.Output);
            return loss;
        }

        private double RunEpoch(SamplesSet samplesSet)
        {
            double error_sum = 0;
            for (int index = 0; index < samplesSet.samples.Count; index++)
            {
                var sample = samplesSet.samples[index];
                error_sum += OneSampleRun(sample);
            }
            return error_sum / samplesSet.samples.Count;
        } 

        public override double TrainOnDataSet(SamplesSet samplesSet, int epochsCount, double acceptableError, bool parallel)
        {
            stopWatch.Restart();
            int epoch_to_run = 0;
            double error = double.PositiveInfinity;
            OnTrainProgress(error, 0, stopWatch.Elapsed);
            while (epoch_to_run < epochsCount && error > acceptableError)
            {
                ++epoch_to_run;
                error = RunEpoch(samplesSet);

                OnTrainProgress((epoch_to_run * 1.0) / epochsCount, error, stopWatch.Elapsed);
            }
            OnTrainProgress(1.0, error, stopWatch.Elapsed);
            stopWatch.Stop();
            return error;
        }

        protected override double[] Compute(double[] input)
        {
            ForwardPropagation(input);
            return layers.Last().output();
        }
    }
}