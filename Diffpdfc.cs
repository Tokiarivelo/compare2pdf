using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Compare2pdf
{
    public class Diffpdfc
    {
        //compare 2 pdf : sourcePdf, destinationPdf : chemin + nom complet du pdf
        public static bool IsSamePdf(string sourcePdf, string destinationPdf)
        {
            return TestDiffpdfc(sourcePdf, destinationPdf).Contains("same");
        }

        //Compare all pdf : sourcePdf, destinationPdf : chemin complet du dossier source et du dossier de destination
        public static List<string> PdfAllowed(string sourceDir, string destinationDir)
        {
            if (Directory.Exists(sourceDir) && Directory.Exists(destinationDir))
            {
                List<string> listPdf = new List<string>();

                //get directory
                DirectoryInfo folderS = new DirectoryInfo(sourceDir);
                DirectoryInfo folderD = new DirectoryInfo(destinationDir);

                //compare all pdf files
                foreach (var files in folderS.GetFiles("*.pdf"))
                {
                    string filesource = Path.Combine(sourceDir, files.ToString());
                    bool test = false;

                    foreach (var fs in folderD.GetFiles())
                    {
                        string listout = Path.Combine(destinationDir, fs.ToString());
                        if (IsSamePdf(filesource, listout))
                        {
                            test = true;
                            break;
                        }
                    }

                    if (!test)
                        listPdf.Add(filesource);
                }

                return listPdf;
            }
            return null;
        }

        //excecute command diffpdfc
        private static string TestDiffpdfc(string filename, string filename1)
        {
            if (File.Exists(filename) && File.Exists(filename1))
            {
                using (Process process = new Process())
                {
                    //start process cmd
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = "/C diffpdfc -w \"" + filename + "\" \"" + filename1 + "\"";
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
                    return output;
                }
            }
            return null;
        }
    }
}
