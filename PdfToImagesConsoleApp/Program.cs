using GemBox.Pdf;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PdfToImagesConsoleApp
{
    class Program
    {
        public enum FileType : int
        {
            [Description(".jpg")]
            JPG = 1,
            [Description(".png")]
            PNG = 2,
            [Description(".gif")]
            GIF = 3,
            [Description(".tiff")]
            TIFF = 4,
            [Description(".bmp")]
            BMP = 5,
            [Description(".wmp")]
            WMP =6
        }
        
        private static void SplittingPdfFile(string originFilePath, string outputFilePath)
        {
            var inputFiles = Directory.GetFiles(originFilePath).ToList();
            foreach (var fileItem in inputFiles)
            {
                int interval = 1;
                int pageNameSuffix = 0;
                PdfReader reader = new PdfReader(fileItem);
                FileInfo file = new FileInfo(fileItem);
                string pdfFileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + "-";

                for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber += interval)
                {
                    pageNameSuffix++;
                    string newPdfFileName = string.Format(pdfFileName + "{0}", pageNameSuffix);
                    SplitAndSaveInterval(fileItem, outputFilePath, pageNumber, interval, newPdfFileName);
                }
            }
            
        }
        private static void SplitAndSaveInterval(string pdfFilePath, string outputPath, int startPage, int interval, string pdfFileName)
        {
            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                Document document = new Document();
                PdfCopy copy = new PdfCopy(document, new FileStream(outputPath + "\\" + pdfFileName + ".pdf", FileMode.Create));
                document.Open();

                for (int pagenumber = startPage; pagenumber < (startPage + interval); pagenumber++)
                {
                    if (reader.NumberOfPages >= pagenumber)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, pagenumber));
                    }
                    else
                    {
                        break;
                    }
                }
                document.Close();
            }
        }
        private static void PdfToImageByItextSharp(string filePath, int outputType, string outputDirectory)
        {
            var pdf = new PdfReader(filePath);
            int n = pdf.NumberOfPages;
            for (int i = 1; i <= n; i++)
            {
                var pg = pdf.GetPageN(i);
                var res = PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES)) as PdfDictionary;
                var xobj = PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT)) as PdfDictionary;
                if (xobj == null) continue;

                var keys = xobj.Keys;
                if (keys.Count == 0) continue;

                var obj = xobj.Get(keys.ElementAt(0));
                if (!obj.IsIndirect()) continue;

                var tg = PdfReader.GetPdfObject(obj) as PdfDictionary;
                var type = PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE)) as PdfName;
                if (!PdfName.IMAGE.Equals(type)) continue;

                int XrefIndex = (obj as PRIndirectReference).Number;
                var pdfStream = pdf.GetPdfObject(XrefIndex) as PRStream;
                var data = PdfReader.GetStreamBytesRaw(pdfStream);
                var jpeg = Path.Combine(outputDirectory, string.Format("{0:0000}.jpg", i));
                File.WriteAllBytes(jpeg, data);
            }
        }
        private static void PdfToImage(string filePath, int outputType, string outputDirectory)
        {
            // For free user, only allow to page which has only 2 pages.
            // More than 2 pages, we must purchase the license.
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            using (GemBox.Pdf.PdfDocument document = GemBox.Pdf.PdfDocument.Load(filePath))
            {
                int i = 1;
                foreach (var page in document.Pages)
                {
                    var docObj = new GemBox.Pdf.PdfDocument();
                    docObj.Pages.AddClone(page);
                    string extType = ".jpg";

                    var imgOption = new ImageSaveOptions();
                    imgOption.PixelFormat = PixelFormat.Default;
                    /* Please presses F12 to see the description
                        img.Height = 400;
                        img.Width = 400;
                        img.DpiX = 100;
                        img.DpiY = 100;
                    */
                    switch (outputType)
                    {
                        case (int)FileType.JPG:
                            extType = ".jpg";
                            break;
                        case (int)FileType.PNG:
                            extType = ".png";
                            break;
                        case (int)FileType.GIF:
                            extType = ".gif";
                            break;
                        case (int)FileType.TIFF:
                            extType = ".tiff";
                            break;
                        case (int)FileType.BMP:
                            extType = ".bmp";
                            break;
                        case (int)FileType.WMP:
                            extType = ".wmp";
                            break;
                        default:
                            break;
                    }
                    var img = outputDirectory + Path.GetFileNameWithoutExtension(filePath).ToString() + "_" + i.ToString() + extType;
                    docObj.Save(img, imgOption);
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
                            //PdfToImageByItextSharp(file, outputType, outputDirectory);
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
                Console.WriteLine(" |          *       => 3. GIF Type         *                       | ");
                Console.WriteLine(" |          *       => 4. TIFF Type        *                       | ");
                Console.WriteLine(" |          *       => 5. BMP Type         *                       | ");
                Console.WriteLine(" |          *       => 6. WMP Type         *                       | ");
                Console.WriteLine(" |          *       => 7. Splitting PDF    *                       | ");
                Console.WriteLine(" |          *       => 0. To EXIT          *                       | ");
                Console.WriteLine(" |          *******************************                        | ");
                Console.WriteLine("  -----------------------------------------------------------------  ");
                Console.Write("Your Choice:  ");
                int outputType = int.TryParse(Console.ReadLine(), out outputType) ? outputType : -1;
                if (outputType != 0 && (outputType == 1 || outputType == 2 || outputType == 3 || outputType == 4 || outputType == 5 || outputType == 6 || outputType == 7))
                {
                    Console.Clear();
                    Console.WriteLine("-----------------------------------------------------------------------");
                    Console.WriteLine($"Your output: {outputType}");
                    Console.WriteLine("=> Progessing...   ");
                    switch (outputType)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            ConvertToImages(outputType);
                            break;
                        case 7:
                            string inputDirectory = System.IO.Directory.GetCurrentDirectory().Replace(@"\bin\Debug", @"\input");
                            string outputDirectory = System.IO.Directory.GetCurrentDirectory().Replace(@"\bin\Debug", @"\output\");
                            SplittingPdfFile(inputDirectory, outputDirectory);
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

