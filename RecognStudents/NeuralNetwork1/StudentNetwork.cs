using Accord;
using Accord.Statistics.Kernels;
using System;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;

namespace NeuralNetwork1
{
    public class StudentNetwork : BaseNetwork
    {
        private readonly Stopwatch watch = new Stopwatch(); // часы
        static Random rand = new Random();

        // функция активации - сигмоида
        public static double Sigmoid(double x) => 1 / (1 + Math.Exp(-x));
        public static double DerivativeSigmoid(double outx) => outx * (1 - outx); // функция производной для сигмоиды. используем сразу выходной сигнал в качетве f(x)
        public static double learningRate = 0.1;
        private class Neuron
        {
            public Neuron[] inputs;
            //выходной сигнал
            public double output;
            //веса
            public double[] weights;
            //значение ошибки
            public double error;
            // биас
            public double bias;

            public Neuron(Neuron[] prevLayer = null)
            {

                if (prevLayer == null || prevLayer.Length == 0)
                    return;
                inputs = prevLayer;
                weights = new double[inputs.Length];
                InitRandomWeights(); // инициализация весов случайными значениями
            }
            /// <summary>
            /// Запускает работу нейрона. Присваивает выходу значение функции активации
            /// </summary>
            public void Work()
            {
                double Sum = 0;
                Sum += bias;
                for (int i = 0; i < inputs.Length; ++i)
                {
                    Sum += inputs[i].output * weights[i]; // взвешенная сумма сигналов
                }
                output = Sigmoid(Sum);
            }
            // Инициализация весов случайными величинами
            private void InitRandomWeights()
            {
                double stdDev = 1.0 / Math.Sqrt(weights.Length);
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = rand.NextDouble() * 2 * stdDev - stdDev;
                }
                bias = rand.NextDouble() * 2 * stdDev - stdDev;
            }

            internal void AdjWeights()
            {
                for (int i = 0;  i < weights.Length; ++i)
                {
                    weights[i] -= learningRate * error * inputs[i].output;
                }
                bias -= learningRate * error;
            }
        }
        // ссылка на сенсорный слой
        Neuron[] sensors;
        //ссылка на выходной слой
        Neuron[] outputs;
        //массив всех слоев сети
        Neuron[][] layers;

        public StudentNetwork(int[] structure)
        {
            InitNetwork(structure);
        }

        private void InitNetwork(int[] structure)
        {
            if (structure.Length < 2)
            {
                throw new ArgumentException("invalid structure network");
            }
            layers = new Neuron[structure.Length][];
            // выделяем память под сенсорный слой и его нейроны
            layers[0] = new Neuron[structure[0]];
            for (int i = 0;i < structure[0]; i++)
            {
                layers[0][i] = new Neuron(); 
            }
            //выделяем память под остальные слои
            for(int i = 1; i < structure.Length; i++)
            {
                layers[i] = new Neuron[structure[i]];
                for (int j = 0; j < structure[i]; j++)
                {
                    layers[i][j] = new Neuron(layers[i - 1]); // каждый нейрон связан с нейроном предыдущего слоя
                }
            }
            // присваиваем ссылки 
            sensors = layers[0];
            outputs = layers[layers.Length-1];
        }
        // Запуск сети на переданном образе. 
        double[] Run(Sample image)
        {
            var res = Compute(image.input);
            image.ProcessPrediction(res);
            return res;
        }
        //тренировка на одном образце
        public override int Train(Sample sample, double acceptableError, bool parallel=false)
        {
            var iter = 0;
            Run(sample);
            var error = sample.EstimatedError(); // вычисляем суммарную ошибку
            while(error > acceptableError)
            {
                iter++;
                Run(sample);
                error = sample.EstimatedError();
                BackProp(sample);
            }
            return iter;
        }
        //Обучение на датасете
        public override double TrainOnDataSet(SamplesSet samplesSet, int epochsCount, double acceptableError, bool parallel=false)
        {
            watch.Restart();
            double error = 0;
            for (int epoch = 0; epoch < epochsCount; epoch++)
            {
                double errorSum = 0;
                foreach(var sample in samplesSet.samples)
                {
                    int TrainResult = Train(sample, acceptableError);
                    if (TrainResult == 0) 
                        errorSum += sample.EstimatedError();
                }
                error = errorSum;
                OnTrainProgress(((epoch + 1) * 1.0) / epochsCount, error, watch.Elapsed);
            }
            watch.Stop();
            return error;
        }

        // вычисление выхода сети
        protected override double[] Compute(double[] input)
        {
            //кладем в выход сенсора наши входные данные
            for (int i = 0; i < sensors.Length; i++)
                sensors[i].output = input[i];
            //каждый нейрон работает над входными данными
            for (int layer = 1; layer < layers.Length; layer++)
                for (int j = 0; j < layers[layer].Length; j++)
                    layers[layer][j].Work();
            //берем выход нейронов
            return outputs.Select(o => o.output).ToArray();
        }

        // обратное распространение ошибки
        private void BackProp(Sample image)
        {
            //ошибка выходного слоя
            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i].error = image.error[i];
            }

            // ошибки скрытых слоев
            // перебор слоев
            for (int layer = layers.Length - 2; layer > 0; layer--)
            {
                // перебор нейронов слоя layer
                for (int j = 0; j < layers[layer].Length; j++)
                {
                    double sum = 0;
                    // перебор нейронов следующего слоя  за layer
                    for (int o = 0; o < layers[layer + 1].Length; o++)
                    {
                        sum += layers[layer + 1][o].error * layers[layer + 1][o].weights[j];
                    }
                    layers[layer][j].error = sum * DerivativeSigmoid(layers[layer][j].output);
                }
            }
            // обновление весов выходного слоя
            for (int i = 0; i < outputs.Length; ++i)
            {
                outputs[i].AdjWeights();
            }
            // обновление весов нейронов скрытых слове
            for (int layer = layers.Length - 2; layer > 0; layer--)
            {
                for (int j = 0; j < layers[layer].Length; ++j)
                {
                    layers[layer][j].AdjWeights();
                }
            }
        }
    }
}