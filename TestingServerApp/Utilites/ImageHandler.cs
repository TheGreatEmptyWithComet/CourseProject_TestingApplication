using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp.Utilites
{
    public static class ImageHandler
    {
        public static bool LoadImage(string destinationFileName, out string errorMessage)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (op.ShowDialog() == true && op.FileName != destinationFileName)
            {
                long fileSize = new FileInfo(op.FileName).Length;
                if (fileSize > AppConfig.DbExternalFiles.ImageMaxSize)
                {
                    errorMessage = "Error: image size exeeds 1 MB";
                    return false;
                }

                File.Copy(op.FileName, destinationFileName, true);

                errorMessage = string.Empty;
                return true;
            }
            errorMessage = string.Empty;
            return false;
        }

        public static async Task DeleteImageAsync(string fileName)
        {
            await Task.Run(() =>
            {
                try
                {
                    // wait for the file will be released from using
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    LogWriter.Write($"{DateTime.Now}: File {fileName} wasn't deleted. Reason: {ex.Message}" + Environment.NewLine);
                }
            });
        }
    }
}
