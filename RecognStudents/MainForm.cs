using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Diagnostics;
using System.IO.Ports;
using System.IO;
using System.Security.Cryptography;
using NeuralNetwork1;

namespace AForge.WindowsForms
{
    delegate void FormUpdateDelegate();

    public partial class MainForm : Form
    {
        /// <summary>
        /// Класс, реализующий всю логику работы
        /// </summary>
        private Controller controller = null;

        /// <summary>
        /// Событие для синхронизации таймера
        /// </summary>
        private AutoResetEvent evnt = new AutoResetEvent(false);
                
        /// <summary>
        /// Список устройств для снятия видео (веб-камер)
        /// </summary>
        private FilterInfoCollection videoDevicesList;
        
        /// <summary>
        /// Выбранное устройство для видео
        /// </summary>
        private IVideoSource videoSource;
        
        /// <summary>
        /// Таймер для измерения производительности (времени на обработку кадра)
        /// </summary>
        private Stopwatch sw = new Stopwatch();
        
        /// <summary>
        /// Таймер для обновления объектов интерфейса
        /// </summary>
        System.Threading.Timer updateTmr;

        /// <summary>
        /// Функция обновления формы, тут же происходит анализ текущего этапа, и при необходимости переключение на следующий
        /// Вызывается автоматически - это плохо, надо по делегатам вообще-то
        /// </summary>
        private void UpdateFormFields()
        {
            //  Проверяем, вызвана ли функция из потока главной формы. Если нет - вызов через Invoke
            //  для синхронизации, и выход
            if (statusLabel.InvokeRequired)
            {
                this.Invoke(new FormUpdateDelegate(UpdateFormFields));
                return;
            }

            sw.Stop();
            ticksLabel.Text = "Тики : " + sw.Elapsed.ToString();
            originalImageBox.Image = controller.GetOriginalImage();
            processedImgBox.Image = controller.GetProcessedImage();
        }

        /// <summary>
        /// Обёртка для обновления формы - перерисовки картинок, изменения состояния и прочего
        /// </summary>
        /// <param name="StateInfo"></param>
        public void Tick(object StateInfo)
        {
            UpdateFormFields();
            return;
        }

        NeuralNetworksStand MotherForm;

        public MainForm(NeuralNetworksStand mother)
        {
            MotherForm = mother;
            InitializeComponent();
            // Список камер получаем
            videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoDevice in videoDevicesList)
            {
                cmbVideoSource.Items.Add(videoDevice.Name);
            }
            if (cmbVideoSource.Items.Count > 0)
            {
                cmbVideoSource.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("А нет у вас камеры!", "Ошибочка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            controller = new Controller(new FormUpdateDelegate(UpdateFormFields));
//            updateTmr = new System.Threading.Timer(Tick, evnt, 500, 100);
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //  Время засекаем
            sw.Restart();

            //  Отправляем изображение на обработку, и выводим оригинал (с раскраской) и разрезанные изображения
            if(controller.Ready)
                
                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                controller.ProcessImage((Bitmap)eventArgs.Frame.Clone());
                #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                //  Это выкинуть в отдельный поток!
                //  И отдать делегат? Или просто проверять значение переменной?
                //  Тут хрень какая-то

                //currentState = Stage.Thinking;
                //sage.solveState(processor.currentDeskState, 16, 7);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (videoSource == null)
            {
                var vcd = new VideoCaptureDevice(videoDevicesList[cmbVideoSource.SelectedIndex].MonikerString);
                vcd.VideoResolution = vcd.VideoCapabilities[resolutionsBox.SelectedIndex];
                Debug.WriteLine(vcd.VideoCapabilities[1].FrameSize.ToString());
                Debug.WriteLine(resolutionsBox.SelectedIndex);
                videoSource = vcd;
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                videoSource.Start();
                StartButton.Text = "Стоп";
                cmbVideoSource.Enabled = false;
            }
            else
            {
                videoSource.SignalToStop();
                if (videoSource != null && videoSource.IsRunning && originalImageBox.Image != null)
                {
                    originalImageBox.Image.Dispose();
                }
                videoSource = null;
                StartButton.Text = "Старт";
                cmbVideoSource.Enabled = true;
            }
            controlPanel.Enabled = false;
        }

        private void tresholdTrackBar_ValueChanged(object sender, EventArgs e)
        {
            controller.settings.threshold = (byte)tresholdTrackBar.Value;
            controller.settings.differenceLim = (float)tresholdTrackBar.Value/tresholdTrackBar.Maximum;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (updateTmr != null)
                updateTmr.Dispose();

            //  Как-то надо ещё робота подождать, если он работает

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.W: controller.settings.decTop(); Debug.WriteLine("Up!"); break;
                case Keys.S: controller.settings.incTop(); Debug.WriteLine("Down!"); break;
                case Keys.A: controller.settings.decLeft(); Debug.WriteLine("Left!"); break;
                case Keys.D: controller.settings.incLeft(); Debug.WriteLine("Right!"); break;
            }
        }

        private void cmbVideoSource_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var vcd = new VideoCaptureDevice(videoDevicesList[cmbVideoSource.SelectedIndex].MonikerString);
            resolutionsBox.Items.Clear();
            for (int i = 0; i < vcd.VideoCapabilities.Length; i++)
                resolutionsBox.Items.Add(vcd.VideoCapabilities[i].FrameSize.ToString());
            resolutionsBox.SelectedIndex = 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            controller.settings.processImg = checkBox1.Checked;
            controlPanel.Enabled = checkBox1.Checked;
        }

        private void originalImageBox_Click(object sender, EventArgs e)
        {

        }

        private void processedImgBox_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        // Путь к папке DATASETS
        private string path = Path.Combine(Application.StartupPath, @"..\..\DATASET");
        // Путь к папке ORIGIN
        private string pathOrigin = Path.Combine(Application.StartupPath, @"..\..\ORIGIN");

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(path))
            {
                MessageBox.Show($"Папка DATASET не найдена: {path}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string targetFolderPath = Path.Combine(path, comboBox1.SelectedItem.ToString());
            string targetOriginPath = Path.Combine(pathOrigin, comboBox1.SelectedItem.ToString());

            if (!Directory.Exists(targetFolderPath))
            {
                MessageBox.Show($"Папка \"{comboBox1.SelectedItem.ToString()}\" не найдена в DATASET.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap new_sample = controller.processor.processed;
            Bitmap origin = controller.processor.originImage;

            string fileName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}";
            string filePath = Path.Combine(targetFolderPath, fileName+".bmp");
            string originPath = Path.Combine(targetOriginPath, fileName+".png");

            try
            {
                // Сохраняем изображение
                new_sample.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                origin.Save(originPath, System.Drawing.Imaging.ImageFormat.Png);
                MessageBox.Show($"Изображение успешно сохранено в папке: {targetFolderPath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                new_sample.Dispose();
                origin.Dispose();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap new_sample = controller.processor.processed;
            label3.Text = DatasetManager.Names[MotherForm.Net.Predict(DatasetManager.CreateOneSample(new_sample))];
            new_sample.Dispose();
        }
    }
}
