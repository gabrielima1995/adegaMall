using Domain.DTO;
using System.Web;

namespace Presentation.Helpers
{
    public static class FileUploadHelper
    {
        public static FileUpload GetFileUpload(this HttpPostedFileBase postedFile)
        {
            if (postedFile == null || postedFile.ContentLength == 0) return null;
            return new FileUpload(postedFile.InputStream, postedFile.FileName, postedFile.ContentType);
        }
    }
}