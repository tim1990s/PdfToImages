using GemBox.Pdf;
using System;
using System.IO;
using System.Linq;

namespace PdfToImagesConsoleApp
{
    class Program
    {
        private static void PdfToImage(string filePath, int outputType, string outputDirectory)
        {
            // For free user, only allow to page which has only 2 pages.
            // More than 2 pages, we must purchase the license.
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            using (PdfDocument document = PdfDocument.Load(filePath))
            {
                int i = 1;
                foreach (var page in document.Pages)
                {
                    var docObj = new PdfDocument();
                    docObj.Pages.AddClone(page);

                    var img = new ImageSaveOptions();
                    img.PixelFormat = PixelFormat.Default;
                    /* Please presses F12 to see the description
                        img.Height = 400;
                        img.Width = 400;
                        img.DpiX = 100;
                        img.DpiY = 100;
                    */
                    docObj.Save(outputDirectory + Path.GetFileNameWithoutExtension(filePath).ToString()+ "_" + i.ToString() + ((outputType == 1) ? ".jpg" : ".png"), img);
                    i++;
                }
            }
        }
        private static void TifToImage(string filePath, int outputType, string outputDirectory)
        {
            Console.WriteLine("To be developing.............................");
            //Console.WriteLine("TIF To Images is in progessing..........");
        }
        private static void ShowAllResults(string outputPath)
        {
            DirectoryInfo d = new DirectoryInfo(outputPath);
            FileInfo[] Files = d.GetFiles("*.*");
            Console.WriteLine("Your result: ");
            foreach (FileInfo file in Files)
            {
                Console.WriteLine($"\t{file.Name}");
            }
        }
        public static void ConvertToImages(int outputType)
        {
            string inputDirectory = System.IO.Directory.GetCurrentDirectory().Replace(@"\bin\Debug", @"\input");
            string outputDirectory = System.IO.Directory.GetCurrentDirectory().Replace(@"\bin\Debug", @"\output\");
            var inputFiles = Directory.GetFiles(inputDirectory).ToList();

            if (inputFiles != null && inputFiles.Any())
            {
                foreach (var file in inputFiles)
                {
                    var fileType = Path.GetExtension(file);
                    switch (fileType)
                    {
                        case ".pdf":
                            PdfToImage(file, outputType, outputDirectory);
                            break;
                        case ".tif":
                            TifToImage(file, outputType, outputDirectory);
                            break;
                        default:
                            break;
                    }
                }
            }
            ShowAllResults(outputDirectory);
            Console.WriteLine("=> Finish          ");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("\n");
        }
        public static void Start()
        {
            do
            {
                Console.WriteLine("  ----------------------------------------------------------------   ");
                Console.WriteLine(" | => Please to carefully to choose an OUTPUT type ! <==           | ");
                Console.WriteLine(" |          ********************************                       | ");
                Console.WriteLine(" |          *       => 1. JPG Type         *                       | ");
                Console.WriteLine(" |          *       => 2. PNG Type         *                       | ");
                Console.WriteLine(" |          *       => 0. To EXIT          *                       | ");
                Console.WriteLine(" |          *******************************                        | ");
                Console.WriteLine("  -----------------------------------------------------------------  ");
                
                int outputType = Int32.TryParse(Console.ReadLine(), out outputType) ? outputType : -1;
                if (outputType != 0 && (outputType == 1 || outputType == 2))
                {
                    Console.Clear();
                    Console.WriteLine("-----------------------------------------------------------------------");
                    Console.WriteLine($"Your output: {outputType}");
                    Console.WriteLine("=> Progessing...   ");
                    switch (outputType)
                    {
                        case 1:
                            ConvertToImages(outputType);
                            break;
                        case 2:
                            ConvertToImages(outputType);
                            break;
                        default:
                            Console.WriteLine("Invalid Option! Are you kidding me? ");
                            Console.WriteLine("\n");
                            break;
                    }
                }
                else if (outputType == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Please enter to EXIT");
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("  ==> Hey! Invalid Option! Are you kidding me? <==     ");
                    Console.WriteLine("\n");
                }

            } while (true);
        }
        static void Main(string[] args)
        {
            Start();
        }
    }
}

