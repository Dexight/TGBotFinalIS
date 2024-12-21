using Accord.IO;
using AForge.WindowsForms;
using AForge.WindowsForms.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace NeuralNetwork1
{
    public partial class NeuralNetworksStand : Form
    {
        /// <summary>
        /// Генератор изображений (образов)
        /// </summary>
        GenerateImage generator = new GenerateImage();

        MainForm CameraDialog;
        
        /// <summary>
        /// Текущая выбранная через селектор нейросеть
        /// </summary>
        public BaseNetwork Net
        {
            get
            {
                var selectedItem = (string) netTypeBox.SelectedItem;
                if (!networksCache.ContainsKey(selectedItem))
                    networksCache.Add(selectedItem, CreateNetwork(selectedItem));

                return networksCache[selectedItem];
            }
        }

        private readonly Dictionary<string, Func<int[], BaseNetwork>> networksFabric;
        private Dictionary<string, BaseNetwork> networksCache = new Dictionary<string, BaseNetwork>();

        /// <summary>
        /// Конструктор формы стенда для работы с сетями
        /// </summary>
        /// <param name="networksFabric">Словарь функций, создающих сети с заданной структурой</param>
        public NeuralNetworksStand(Dictionary<string, Func<int[], BaseNetwork>> networksFabric)
        {
            InitializeComponent();
            this.networksFabric = networksFabric;
            netTypeBox.Items.AddRange(this.networksFabric.Keys.Select(s => (object) s).ToArray());
            netTypeBox.SelectedIndex = 0;
            generator.FigureCount = (int) classCounter.Value;
            button3_Click(this, null);
            //pictureBox1.Image = Properties.Resources.Title;
        }

        public void UpdateLearningInfo(double progress, double error, TimeSpan elapsedTime)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new TrainProgressHandler(UpdateLearningInfo), progress, error, elapsedTime);
                return;
            }

            StatusLabel.Text = "Ошибка: " + error;
            int progressPercent = (int) Math.Round(progress * 100);
            progressPercent = Math.Min(100, Math.Max(0, progressPercent));
            elapsedTimeLabel.Text = "Затраченное время : " + elapsedTime.Duration().ToString(@"hh\:mm\:ss\:ff");
            progressBar1.Value = progressPercent;
        }


        private void set_result(Sample figure)
        {
            label1.ForeColor = figure.Correct() ? Color.Green : Color.Red;

            label1.Text = "Распознано : " + figure.recognizedClass;

            label8.Text = string.Join("\n", figure.Output.Select(d => d.ToString(CultureInfo.InvariantCulture)));
            pictureBox1.Image = generator.GenBitmap();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Sample fig = generator.GenerateFigure();

            Net.Predict(fig);

            set_result(fig);
        }

        private async Task<double> train_networkAsync(int training_size, int epoches, double acceptable_error,
            bool parallel = true)
        {
            //  Выключаем всё ненужное
            label1.Text = "Выполняется обучение...";
            label1.ForeColor = Color.Red;
            groupBox1.Enabled = false;
            pictureBox1.Enabled = false;
            trainOneButton.Enabled = false;

            //  Создаём новую обучающую выборку
            //SamplesSet samples = new SamplesSet();

            //for (int i = 0; i < training_size; i++)
            //samples.AddSample(generator.GenerateFigure());

            //Обучающая выборка
            DatasetManager.CreateDataset();

            SamplesSet samples = DatasetManager.TrainSet;
            //Console.WriteLine(samples.samples[0].input.Count());
            try
            {
                //  Обучение запускаем асинхронно, чтобы не блокировать форму
                var curNet = Net;
                double f = await Task.Run(() => curNet.TrainOnDataSet(samples, epoches, acceptable_error, parallel));

                label1.Text = "Щелкните на картинку для теста нового образа";
                label1.ForeColor = Color.Green;
                groupBox1.Enabled = true;
                pictureBox1.Enabled = true;
                trainOneButton.Enabled = true;
                StatusLabel.Text = "Ошибка: " + f;
                StatusLabel.ForeColor = Color.Green;
                return f;
            }
            catch (Exception e)
            {
                label1.Text = $"Исключение: {e.Message}";
            }

            return 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            train_networkAsync((int) TrainingSizeCounter.Value, (int) EpochesCounter.Value,
                (100 - AccuracyCounter.Value) / 100.0, parallelCheckBox.Checked);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        // Создание тестовой выборки
        // TODO: Добавить тестирование через менеджер
        private void button2_Click(object sender, EventArgs e)
        {
            Enabled = false;
            //  Тут просто тестирование новой выборки
            //  Создаём новую обучающую выборку
            SamplesSet samples = DatasetManager.TrainSet;
            //SamplesSet =
            //for (int i = 0; i < (int) TrainingSizeCounter.Value; i++)
                //samples.AddSample(generator.GenerateFigure());

            double accuracy = samples.TestNeuralNetwork(Net);

            StatusLabel.Text = $"Точность на тестовой выборке : {accuracy * 100,5:F2}%";
            StatusLabel.ForeColor = accuracy * 100 >= AccuracyCounter.Value ? Color.Green : Color.Red;

            Enabled = true;
        }

        string pathOrigin = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\ORIGIN";
        string pathDataset = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\DATASET";

        private void button3_Click(object sender, EventArgs e)
        {
            // Проверяем корректность задания структуры сети
            int[] structure = CurrentNetworkStructure();

            // Пересоздаём датасет

            oldDatasetClear();

            createNewDataset();

            DatasetManager.CreateDataset();

            // Чистим старые подписки сетей
            foreach (var network in networksCache.Values)
                network.TrainProgress -= UpdateLearningInfo;
            // Пересоздаём все сети с новой структурой
            networksCache = networksCache.ToDictionary(oldNet => oldNet.Key, oldNet => CreateNetwork(oldNet.Key));
        }

        private void oldDatasetClear()
        {
            Console.WriteLine("Очистка старого датасета");
            // Очищаем все папки в DATASET
            var dirsDataset = Directory.GetDirectories(pathDataset); // получаем пути к папкам датасета
            foreach (string dir in dirsDataset)
            {
                string[] files = Directory.GetFiles(dir);
                foreach (var file in files)
                    File.Delete(file);
            }
        }

        private void createNewDataset()
        {
            Console.WriteLine("Формирование нового датасета");
            //Создаём новые образцы
            var dirsOrigin = Directory.GetDirectories(pathOrigin); // получаем пути к папкам датасета
            foreach (var dir in dirsOrigin)
            {
                var key = dir.Substring(dir.Length - 1);
                string[] fnames = Directory.GetFiles(dir);

                // папка датасета (куда сохраняем)
                string targetPath = Path.Combine(pathDataset, key);

                if (!Directory.Exists(targetPath))
                {
                    Console.WriteLine($"Папка {targetPath} не существует.");
                    continue;
                }

                int index = 0;
                //для каждого оригинала
                foreach (string fname in fnames)
                {
                    // добавить в датасет
                    try
                    {
                        using (Bitmap bmp = new Bitmap(fname))
                        {
                            List<Bitmap> processedImages = ProcessImage(bmp);
                            foreach(Bitmap img in processedImages)
                            {
                                img.Save(Path.Combine(targetPath, key + "_" + index++ + ".bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке файла {fname}: {ex.Message}");
                    }
                }
            }
            Console.WriteLine("Датасет успешно сформирован");
        }

        private int[] CurrentNetworkStructure()
        {
            return netStructureBox.Text.Split(';').Select(int.Parse).ToArray();
        }

        private void classCounter_ValueChanged(object sender, EventArgs e)
        {
            generator.FigureCount = (int) classCounter.Value;
            var vals = netStructureBox.Text.Split(';');
            if (!int.TryParse(vals.Last(), out _)) return;
            vals[vals.Length - 1] = classCounter.Value.ToString();
            netStructureBox.Text = vals.Aggregate((partialPhrase, word) => $"{partialPhrase};{word}");
        }

        private void btnTrainOne_Click(object sender, EventArgs e)
        {
            CameraDialog = new MainForm(this);
            CameraDialog.ShowDialog();
            //if (Net == null) return;
            //Sample fig = generator.GenerateFigure();
            //pictureBox1.Image = generator.GenBitmap();
            //pictureBox1.Invalidate();
            //Net.Train(fig, 0.00005, parallelCheckBox.Checked);
            //set_result(fig);
        }

        private BaseNetwork CreateNetwork(string networkName)
        {
            var network = networksFabric[networkName](CurrentNetworkStructure());
            network.TrainProgress += UpdateLearningInfo;
            return network;
        }

        private void recreateNetButton_MouseEnter(object sender, EventArgs e)
        {
            infoStatusLabel.Text = "Заново пересоздаёт сеть с указанными параметрами";
        }

        private void netTrainButton_MouseEnter(object sender, EventArgs e)
        {
            infoStatusLabel.Text = "Обучить нейросеть с указанными параметрами";
        }

        private void testNetButton_MouseEnter(object sender, EventArgs e)
        {
            infoStatusLabel.Text = "Тестировать нейросеть на тестовой выборке такого же размера";
        }

        public List<Bitmap> ProcessImage(Bitmap original)
        {
            // На вход поступает необработанное изображение из ORIGIN

            //  Теперь всю эту муть пилим в обработанное изображение
            var orig = AForge.Imaging.UnmanagedImage.FromManagedImage(original);

            AForge.Imaging.Filters.Grayscale grayFilter = new AForge.Imaging.Filters.Grayscale(0.2125, 0.7154, 0.0721);
            var uProcessed = grayFilter.Apply(orig);

            //  Пороговый фильтр
            AForge.Imaging.Filters.BradleyLocalThresholding threshldFilter = new AForge.Imaging.Filters.BradleyLocalThresholding();
            threshldFilter.PixelBrightnessDifferenceLimit = 0.15f;
            threshldFilter.ApplyInPlace(uProcessed);

            return processSample(uProcessed);
        }

        private List<Bitmap> processSample(AForge.Imaging.UnmanagedImage orig_unmanaged)
        {
            List<Bitmap> samples = new List<Bitmap>();
            // Добавляем аугментацию — повороты
            float[] angles = { -15f, 10f, -5f, 0f, 5f, 10f, 15f }; // Углы поворота
            foreach (float angle in angles)
            {
                var unmanaged = orig_unmanaged.Clone();

                ///  Инвертируем изображение
                AForge.Imaging.Filters.Invert InvertFilter = new AForge.Imaging.Filters.Invert();
                InvertFilter.ApplyInPlace(unmanaged);

                CutAndScalePicture(ref unmanaged);

                //поворот
                AForge.Imaging.Filters.RotateBilinear rotateFilter = new AForge.Imaging.Filters.RotateBilinear(angle);
                rotateFilter.FillColor = Color.Black; // Задаем цвет фона для пустых областей
                unmanaged = rotateFilter.Apply(unmanaged);

                CutAndScalePicture(ref unmanaged);

                samples.Add(unmanaged.ToManagedImage());
            }

            return samples;
        }

        private int _sampleSizeX = 75;
        private int _sampleSizeY = 150;

        void CutAndScalePicture(ref AForge.Imaging.UnmanagedImage unmanaged)
        {
            ///    Создаём BlobCounter, выдёргиваем самый большой кусок, масштабируем, пересечение и сохраняем
            ///    изображение в эксклюзивном использовании
            AForge.Imaging.BlobCounterBase bc = new AForge.Imaging.BlobCounter();

            bc.FilterBlobs = true;
            bc.MinWidth = 3;
            bc.MinHeight = 3;
            // Упорядочиваем по размеру
            bc.ObjectsOrder = AForge.Imaging.ObjectsOrder.Size;

            // Обрабатываем картинку
            bc.ProcessImage(unmanaged);

            Rectangle[] rects = bc.GetObjectsRectangles();

            // К сожалению, код с использованием подсчёта blob'ов не работает, поэтому просто высчитываем максимальное покрытие
            // для всех блобов - для нескольких цифр, к примеру, 16, можем получить две области - отдельно для 1, и отдельно для 6.
            // Строим оболочку, включающую все блоки. Решение плохое, требуется доработка
            int lx = unmanaged.Width;
            int ly = unmanaged.Height;
            int rx = 0;
            int ry = 0;
            for (int i = 0; i < rects.Length; ++i)
            {
                if (lx > rects[i].X) lx = rects[i].X;
                if (ly > rects[i].Y) ly = rects[i].Y;
                if (rx < rects[i].X + rects[i].Width) rx = rects[i].X + rects[i].Width;
                if (ry < rects[i].Y + rects[i].Height) ry = rects[i].Y + rects[i].Height;
            }

            // Обрезаем края, оставляя только центральные блобчики
            AForge.Imaging.Filters.Crop cropFilter = new AForge.Imaging.Filters.Crop(new Rectangle(lx, ly, rx - lx, ry - ly));
            try
            {
                unmanaged = cropFilter.Apply(unmanaged);
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка чтения символа с картинки");
            }

            //  Масштабируем до нужного размера
            AForge.Imaging.Filters.ResizeBilinear scaleFilter = new AForge.Imaging.Filters.ResizeBilinear(_sampleSizeX, _sampleSizeY);
            unmanaged = scaleFilter.Apply(unmanaged);
        }
    }
}