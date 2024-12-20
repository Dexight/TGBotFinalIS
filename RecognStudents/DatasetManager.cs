using Accord.Math;
using NeuralNetwork1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForge.WindowsForms
{
    //Класс для работы с датасетом


    internal class DatasetManager
    {
        public static SamplesSet TrainSet = new SamplesSet(); // обучающая выборка
        public static SamplesSet TestSet = new SamplesSet(); // тестовая выборка

        public static int imgW = 75;
        public static int imgH = 150;
        public static int threshold = 50; // порог для цвета пикселя 
        public static bool[] imgBits = new bool[imgW * imgH]; // логические метки для пикселей изображения

        static string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\DATASET";

        //класс фигуры по ключу
        private static Dictionary<string, FigureType> Classes = new Dictionary<string, FigureType>() {
            {"0", FigureType.Zero },
            {"1", FigureType.One},
            {"2", FigureType.Two},
            {"3", FigureType.Three},
            {"4", FigureType.Four},
            {"5", FigureType.Five},
            {"6", FigureType.Six},
            {"7", FigureType.Seven },
            {"8", FigureType.Eight },
            {"9", FigureType.Nine },
            {"-", FigureType.Undef }
        };
        
        //класс фигуры по ключу
        public static Dictionary<FigureType, string> Names = new Dictionary<FigureType, string>() {
            {FigureType.Zero  , "0"},
            {FigureType.One   , "1"},
            {FigureType.Two   , "2"},
            {FigureType.Three , "3"},
            {FigureType.Four  , "4"},
            {FigureType.Five  , "5"},
            {FigureType.Six   , "6"},
            {FigureType.Seven , "7"},
            {FigureType.Eight , "8"},
            {FigureType.Nine  , "9"},
            {FigureType.Undef , "-"}
        };

        public static Sample CreateOneSample(Bitmap bmp, FigureType figureType = FigureType.Undef)
        {
            imgBits.Clear(); // очищаем массив
            FillImgBits(bmp);
            var transitions = CountBlackToWhiteTransitions(); // Считаем количество переходов
            return new Sample(transitions, 10, figureType);
        }

        //Создание датасета
        public static void CreateDataset()
        {
            var samples = new SamplesSet();
            var dirs = Directory.GetDirectories(path); // получаем пути к папкам датасета
            foreach (var dir in dirs)
            {
                var key = dir.Substring(dir.Length - 1); // ключ 
                var figureType = Classes[key]; // класс по ключу
                string[] fnames = Directory.GetFiles(dir);
                foreach (string fname in fnames)
                {
                    using (Bitmap bmp = new Bitmap(fname))
                    {
                        samples.AddSample(CreateOneSample(bmp, figureType)); // добавляем sample
                    }
                }
            }
            samples.Shuffle(); // перемешиваем выборку
            TrainSet = samples;
        }
        //Заполняет массив с информацие про пиксели
        private static void FillImgBits(Bitmap bmp)
        {
            for (int x = 0; x < imgW; ++x)
            {
                for (int y = 0; y < imgH; ++y)
                {
                    var pix = bmp.GetPixel(x, y);
                    imgBits[y*imgW + x] = pix.R < threshold || pix.G < threshold || pix.B < threshold;
                }
            }
        }

        //Считаем число переходов
        static double[] CountBlackToWhiteTransitions()
        {
            double[] transitions = new double[imgW + imgH]; // Итоговый массив

            // Подсчет переходов по строкам
            for (int y = 0; y < imgH; y++)
            {
                int rowTransitions = 0;
                for (int x = 0; x < imgW - 1; x++)
                {
                    int currentIndex = y * imgW + x;
                    int nextIndex = y * imgW + x + 1;

                    // Проверяем переход от черного к белому
                    if (imgBits[currentIndex] != imgBits[nextIndex])
                        rowTransitions++;
                }
                transitions[y] = rowTransitions;
            }

            // Подсчет переходов по столбцам
            for (int x = 0; x < imgW; x++)
            {
                int colTransitions = 0;
                for (int y = 0; y < imgH - 1; y++)
                {
                    int currentIndex = y * imgW + x;
                    int nextIndex = (y + 1) * imgW + x;

                    // Проверяем переход от черного к белому
                    if (imgBits[currentIndex] != imgBits[nextIndex])
                        colTransitions++;
                }
                transitions[imgH + x] = colTransitions;
            }
            return transitions;
        }


    }
}
