using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Win32;
using DiffClass;

namespace Compare2pdf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var a = DateTime.Now.TimeOfDay.TotalSeconds;
                var c = DiffMatchClass.PdfAllowed(@"C:\Users\toky\Desktop\Source", @"C:\Users\toky\Desktop\Destination", @"C:\Users\toky\Desktop\Doublon"); //by diffMatchPatc
                var u = c;

                var b = DateTime.Now.TimeOfDay.TotalSeconds - a;
                MessageBox.Show(b.ToString(CultureInfo.InvariantCulture));

                //MovingdiffpdfC(@"C:\Users\toky\Desktop\Source", @"C:\Users\toky\Desktop\Destination", @"C:\Users\toky\Desktop\Doublon"); //by diffpdfC
                //var a = Diffpdfc.PdfAllowed(@"C:\Users\toky\Desktop\Source", @"C:\Users\toky\Desktop\Destination"); //by diffpdfC
                //var c = Diffpdfc.IsSamePdf(@"C:\Users\toky\Desktop\Source\HetNieuwsblad_HetNieuwsbladKempen_20180320_001.pdf", @"C:\Users\toky\Desktop\Destination\HetNieuwsblad_HetNieuwsbladKempen_20180320_001.pdf"); //by diffpdfC

                //MovingByByte(@"C:\Users\toky\Desktop\Source", @"C:\Users\toky\Desktop\Destination", @"C:\Users\toky\Desktop\Doublon");

                //CompareImages(@"C:\Users\toky\Desktop\Source\LeSoir_LeSoirBruxellesBrabant_20180202_001 - Copie.jpg",
                //    @"C:\Users\toky\Desktop\Source\LeSoir_LeSoirBruxellesBrabant_20180202_001.jpg");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private string GetText(string path)
        {
            string strText = "";
            PdfReader reader = new PdfReader(path);
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                ITextExtractionStrategy its = new LocationTextExtractionStrategy();
                string s = PdfTextExtractor.GetTextFromPage(reader, page, its);
                s = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));
                strText += s;
            }
            reader.Close();
            return strText;
        }

        private void Moving(string sourceDir, string destinationDir, string sameDir)
        {
            DirectoryInfo folderS = new DirectoryInfo(sourceDir);
            DirectoryInfo folderD = new DirectoryInfo(destinationDir);
            foreach (var files in folderS.GetFiles("*.pdf"))
            {
                string filesource = System.IO.Path.Combine(sourceDir, files.ToString());
                string filesout = System.IO.Path.Combine(destinationDir, files.ToString());

                if (File.Exists(filesource))
                {
                    var test = false;
                    foreach (var fs in folderD.GetFiles("*.pdf"))
                    {
                        string listout = System.IO.Path.Combine(destinationDir, fs.ToString());
                        if (TestDiff(filesource, listout) == 0)
                        {
                            test = true;
                            break;
                        }
                    }
                    if (!test)
                        File.Copy(filesource, filesout,true);
                    else
                    {
                        string filessame = System.IO.Path.Combine(sameDir, files.ToString());
                        File.Copy(filesource, filessame,true);
                    }
                }
            }

            //MessageBox.Show("Donne");
        }
        private void MovingdiffpdfC(string sourceDir, string destinationDir, string sameDir)
        {


            DirectoryInfo folderS = new DirectoryInfo(sourceDir);
            DirectoryInfo folderD = new DirectoryInfo(destinationDir);
            foreach (var files in folderS.GetFiles("*.pdf"))
            {
                string filesource = System.IO.Path.Combine(sourceDir, files.ToString());
                string filesout = System.IO.Path.Combine(destinationDir, files.ToString());

                if (File.Exists(filesource))
                {
                    var test = false;
                    foreach (var fs in folderD.GetFiles("*.pdf"))
                    {
                        string listout = System.IO.Path.Combine(destinationDir, fs.ToString());
                        if (TestDiffpdfc(filesource, listout).Contains("same"))
                        {
                            test = true;
                            break;
                        }
                    }
                    if (!test)
                        File.Copy(filesource, filesout,true);
                    else
                    {
                        string filessame = System.IO.Path.Combine(sameDir, files.ToString());
                        File.Copy(filesource, filessame,true);
                    }
                }
            }

            MessageBox.Show("Donne");

        }

        private string TestDiffpdfc(string filename, string filename1)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/C diffpdfc -w \""+filename +"\" \""+ filename1 + "\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

                process.Start();

                // Synchronously read the standard output of the spawned process.
                StreamReader reader = process.StandardOutput;
                string output = reader.ReadToEnd();


                process.WaitForExit();
                // Write the redirected output to this application's window.
               // MessageBox.Show(output);
                return output;
            }
        }

        private int TestDiff(string filename, string filename1)
        {

            string text1 = GetText(filename);
            string text2 = GetText(filename1);

            diff_match_patch dmp = new diff_match_patch();
            dmp.Diff_Timeout = 0;

            List<Diff> diff = dmp.diff_main(text1, text2);

            var count = diff.Where(d => d.operation == Operation.DELETE || d.operation == Operation.INSERT).Sum(d => d.text.Length);


           // MessageBox.Show($"{percent}% différent");
            return count;

        }

        private bool testByte(string filename, string filename1)
        {
            var t1 = File.ReadAllBytes(filename);
            var t2 = File.ReadAllBytes(filename1);
            return t1.SequenceEqual(t2);
        }


        private void CompareImages(string img1_path, string img2_path)
        {
            StringBuilder img1_ref = new StringBuilder();
            StringBuilder img2_ref = new StringBuilder();
            var flag = true;
            var img1 = new Bitmap(img1_path);
            var img2 = new Bitmap(img2_path);

            var e = ImageToByte(img1);
            var f = ImageToByte(img2);

            var newline = 0;

            foreach (var v in e)
            {
                newline++;
                if (newline%100 == 0)
                {
                    img1_ref.AppendFormat(Environment.NewLine);
                }
                img1_ref.Append(v);
            }
            newline = 0;
            foreach (var v in f)
            {
                newline++;
                if (newline % 100 == 0)
                {
                    img2_ref.AppendFormat(Environment.NewLine);
                }
                img2_ref.Append(v);
            }

            var st = img1_ref.ToString();
            var st2 = img2_ref.ToString();
            diff_match_patch dmp = new diff_match_patch();
            dmp.Diff_Timeout = 0;

            List<Diff> diff = dmp.diff_main(img1_ref.ToString(), img2_ref.ToString());

            var count = diff.Where(d => d.operation == Operation.DELETE || d.operation == Operation.INSERT).Sum(d => d.text.Length);

            var i = img1_ref.ToString().Length;

            var percent = count * 100 / img1_ref.ToString().Length;

            var c = percent;
        }
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void MovingByByte(string sourceDir, string destinationDir, string sameDir)
        {


            DirectoryInfo folderS = new DirectoryInfo(sourceDir);
            DirectoryInfo folderD = new DirectoryInfo(destinationDir);
            foreach (var files in folderS.GetFiles())
            {
                string filesource = System.IO.Path.Combine(sourceDir, files.ToString());
                string filesout = System.IO.Path.Combine(destinationDir, files.ToString());

                if (File.Exists(filesource))
                {
                    var test = false;
                    foreach (var fs in folderD.GetFiles())
                    {
                        string listout = System.IO.Path.Combine(destinationDir, fs.ToString());
                        if (testByte(filesource, listout))
                        {
                            test = true;
                            break;
                        }
                    }
                    if (!test)
                        File.Copy(filesource, filesout, true);
                    else
                    {
                        string filessame = System.IO.Path.Combine(sameDir, files.ToString());
                        File.Copy(filesource, filessame, true);
                    }
                }
            }

            MessageBox.Show("Donne");

        }
    }
}
