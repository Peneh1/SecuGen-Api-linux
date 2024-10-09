using WebAPI1toN.Interfaces;

// MVC ASP.NET Core 6 Web API -> 9/16/22
// Image containter doesn't need to be service.  This is mostly due to WebAPI1toN
// needs to be able to reference the individual user's FP images, not just a collection
// of images that can be used with a single DB.

namespace WebAPI1toN.Services
{
    public class ImageContainer : IImageContainer
    {
        private readonly string IMAGE_FILE_NAME = "Image_";
        private readonly string IMAGE_FILENAME_BASE = "Image_{0}_{1}";
        private const string IMAGE_FILE_EXTENSION = ".bmp";

        public ImageContainer()
        {
        }

        public void storeBMPBase64Image(string strPath, string strSessionID, string strbmpbase64Image)
        {
            List<string> strPathFileNames = new List<string>();
            strPathFileNames = FindImageFileNames(strPath, strSessionID);
            FileDownImage(strPath, strSessionID, strPathFileNames.Count.ToString(),strbmpbase64Image);
        }

        public List<string> getBMPBase64Image(string strPath, string strSessionID)
        {
            List<string> strPathFileNames = new List<string>();
            List<string> strImageData = new List<string>();
            strPathFileNames = FindImageFileNames(strPath, strSessionID);
            strImageData = LoadImageData(strPathFileNames);
            return strImageData;
        }

        private List<string> FindImageFileNames(string strPath, string strSessionID)
        {
            List<string> FileNames = new List<string>();
            FileNames.Clear();
            string[] fileEntries = Directory.GetFiles(strPath);
            foreach (string fileEntry in fileEntries)
            {
                if (fileEntry.Contains(strSessionID) && fileEntry.Contains(IMAGE_FILE_NAME))
                    FileNames.Add(Path.Combine(strPath, fileEntry));
            }
            return FileNames;
        }

        private List<string> LoadImageData(List<string> strPathFileNames)
        {
            List<string> strImageData = new List<string>();
            foreach (string fileEntry in strPathFileNames)
            {
                using (FileStream fs = System.IO.File.OpenRead(fileEntry))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    strImageData.Add(Convert.ToBase64String(bytes));
                } // end using FileStream
            } // end foreach
            return strImageData;
        }

        // Use this to file down image data; go get a image viewer from somewhere to look at image.
        private void FileDownImage(string strPath, string strSessionID, string strImageNumber, string strbmpbase64Image)
        {
            string strImageFileName = string.Format(IMAGE_FILENAME_BASE, strSessionID, strImageNumber) + IMAGE_FILE_EXTENSION;
            string strTemplateName = Path.Combine(strPath, strImageFileName);
            using (FileStream fs = System.IO.File.Create(strTemplateName))
            {
                Int32 length = strbmpbase64Image.Length;
                byte[] data;
                //data = Encoding.ASCII.GetBytes(bmpBase64Image);
                data = Convert.FromBase64String(strbmpbase64Image);
                fs.Write(data, 0, data.Length);
            }
        } // end private void FileDownImage
    }
}
