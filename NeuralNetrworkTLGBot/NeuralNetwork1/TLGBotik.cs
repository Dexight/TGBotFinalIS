using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Windows.Forms;
using AForge.WindowsForms;
using System.Reflection.Emit;
using System.Reflection;
using System.Drawing;

namespace NeuralNetwork1
{
    class TLGBotik
    {
        public Telegram.Bot.TelegramBotClient botik = null;

        private UpdateTLGMessages formUpdater;

        private BaseNetwork perseptron = null;
        Form1 motherForm;

        AForge.WindowsForms.MagicEye eye = new AForge.WindowsForms.MagicEye();
        // Путь к папке DATASETS
        private string processedInputsPath = Path.Combine(Application.StartupPath, @"..\..\INPUTS");
        private int pictureCounter = 0;
        
        // CancellationToken - инструмент для отмены задач, запущенных в отдельном потоке
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        public TLGBotik(Form1 motherForm, BaseNetwork net,  UpdateTLGMessages updater)
        {
            this.motherForm = motherForm;
            var botKey = System.IO.File.ReadAllText("botkey.txt");
            botik = new Telegram.Bot.TelegramBotClient(botKey);
            formUpdater = updater;
            perseptron = net;

            clearInputs();
        }

        private void clearInputs()
        {
            if (!Directory.Exists(processedInputsPath))
            {
                Console.WriteLine($"Папка {processedInputsPath} не существует.");
                return;
            }

            string[] files = Directory.GetFiles(processedInputsPath);
            foreach (var file in files)
                System.IO.File.Delete(file);
        }

        private void AddToInputs(System.Drawing.Bitmap b)
        {
            string filePath = Path.Combine(processedInputsPath, pictureCounter++ + ".bmp");
            b.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public void SetNet(BaseNetwork net)
        {
            perseptron = net;
            formUpdater("Net updated!");
        }

        private async Task HandleUpdateMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //  Тут очень простое дело - банально отправляем назад сообщения
            var message = update.Message;
            formUpdater("Тип сообщения : " + message.Type.ToString());

            //  Получение файла (картинки)
            if (message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
            {
                formUpdater("Picture loadining started");
                var photoId = message.Photo.Last().FileId;
                Telegram.Bot.Types.File fl = botik.GetFileAsync(photoId).Result;
                var imageStream = new MemoryStream();
                await botik.DownloadFileAsync(fl.FilePath, imageStream, cancellationToken: cancellationToken);
                var img = System.Drawing.Image.FromStream(imageStream);
                
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(img);

                //  Масштабируем aforge
                //AForge.Imaging.Filters.ResizeBilinear scaleFilter = new AForge.Imaging.Filters.ResizeBilinear(75, 125);
                //var uProcessed = scaleFilter.Apply(AForge.Imaging.UnmanagedImage.FromManagedImage(bm));

                //AForge.Imaging.Filters.ResizeBilinear scaleFilter = new AForge.Imaging.Filters.ResizeBilinear(75, 125);
                //bm = scaleFilter.Apply(bm);
                
                bm = ProcessImage(bm);

                AddToInputs(bm);

                // меняет изображение в форме, но падает для нескольких запросов
                //motherForm.UpdatePicture(bm);



                //Sample sample = GenerateImage.GenerateFigure(uProcessed);
                Sample sample = DatasetManager.CreateOneSample(bm);
                string res = "";

                switch(perseptron.Predict(sample))
                {
                    case FigureType.Zero: res = "0"; break;
                    case FigureType.One: res = "1"; break;
                    case FigureType.Two: res = "2"; break;
                    case FigureType.Three: res = "3"; break;
                    case FigureType.Four: res = "4"; break;
                    case FigureType.Five: res = "5"; break;
                    case FigureType.Six: res = "6"; break;
                    case FigureType.Seven: res = "7"; break;
                    case FigureType.Eight: res = "8"; break;
                    case FigureType.Nine: res = "9"; break;
                }
                botik.SendTextMessageAsync(message.Chat.Id, "Я думаю это цифра " + res);
                formUpdater("Picture recognized!");
                return;
            }

            if (message == null || message.Type != MessageType.Text) return;
            if(message.Text == "Authors")
            {
                string authors = "Мовчан Егор, Тупикин Олег, Панихидин Дима";
                botik.SendTextMessageAsync(message.Chat.Id, "Авторы проекта : " + authors);
            }
            botik.SendTextMessageAsync(message.Chat.Id, "Bot reply : " + message.Text);
            formUpdater(message.Text);
            return;
        }
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var apiRequestException = exception as ApiRequestException;
            if (apiRequestException != null)
                Console.WriteLine($"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}");
            else
                Console.WriteLine(exception.ToString());
            return Task.CompletedTask;
        }

        public bool Act()
        {
            try
            {
                botik.StartReceiving(HandleUpdateMessageAsync, HandleErrorAsync, new ReceiverOptions
                {   // Подписываемся только на сообщения
                    AllowedUpdates = new[] { UpdateType.Message }
                },
                cancellationToken: cts.Token);
                // Пробуем получить логин бота - тестируем соединение и токен
                Console.WriteLine($"Connected as {botik.GetMeAsync().Result}");
            }
            catch(Exception e) { 
                return false;
            }
            return true;
        }

        public void Stop()
        {
            cts.Cancel();
        }

        public Bitmap ProcessImage(Bitmap original)
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

        private Bitmap processSample(AForge.Imaging.UnmanagedImage unmanaged)
        {
            ///  Инвертируем изображение
            AForge.Imaging.Filters.Invert InvertFilter = new AForge.Imaging.Filters.Invert();
            InvertFilter.ApplyInPlace(unmanaged);

            CutAndScalePicture(ref unmanaged);

            return unmanaged.ToManagedImage();
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
