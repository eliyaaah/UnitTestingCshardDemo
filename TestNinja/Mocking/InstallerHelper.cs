using System.Net;

namespace TestNinja.Mocking
{
    public class InstallerHelper
    {
        public InstallerHelper(IFileDownloader fileDownloader)
        {
            _fileDownloader = _fileDownloader ?? new FileDownloader();
        }

        private string _setupDestinationFile = "path";
        private readonly IFileDownloader _fileDownloader;

        public bool DownloadInstaller(string customerName, string installerName)
        {

            try
            {
                _fileDownloader.DownloadFile(
                    string.Format("http://example.com/{0}/{1}",
                        customerName,
                        installerName),
                    _setupDestinationFile);

                return true;
            }
            catch (WebException)
            {
                return false; 
            }
        }
    }
}