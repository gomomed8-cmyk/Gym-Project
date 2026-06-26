using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize=5*1024*1024; //5MB
        private readonly ILogger<IAttachmentService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".png", ".jpeg",".jpg" };

        public AttachmentService(ILogger<IAttachmentService> logger,IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public bool Delete(string fileName, string folderName)
        {
            var filePath= Path.Combine(_env.ContentRootPath,folderName, fileName);
            try
            {
            if (!File.Exists(filePath)) return false;
            File.Delete(filePath);
            return true;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex,$"Failed To Delete Attachment {fileName}");
                return false;
            }
        }

        public (Stream stream, string contentType)? GetFile(string fileName, string folderName)
        {
           if(string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileName)) return null;
            var filePath = Path.Combine(_env.ContentRootPath, folderName, fileName);
            if (!File.Exists(filePath)) return null;
            var stream=new FileStream(filePath,FileMode.Open,FileAccess.Read);
            var extension = Path.GetExtension(filePath).ToLower();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" =>"image/jpeh",
               _ =>"application/octet-stream" //Binary Data

            };
            return (stream, contentType);

        }

        public async Task<string?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream == null || !fileStream.CanRead) return null;
            if (fileStream.Length ==0) return null;
            if(fileStream.Length>_maxFileSize)
            {
                _logger.LogError($"File Rejected : File To Large {fileStream.Length} Bytes");
                return null;
            }
            var extension = Path.GetExtension(fileName);
            if(string.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension))
            {
                _logger.LogError($"File Rejected : Extension {extension} Not Allowed");
                return null;
            }
            var uploadsFolder = Path.Combine(_env.ContentRootPath,folderName);
            Directory.CreateDirectory(uploadsFolder);

            var storedFileName = $"{Guid.NewGuid()}{fileName}";
            var filePath= Path.Combine(uploadsFolder,storedFileName);
            try
            {
                   using var fs= new FileStream(filePath, FileMode.Create,FileAccess.Write);
                   await fileStream.CopyToAsync(fs,ct);
                   return storedFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Failed To Upload File{fileName}");
                return null;
            }
        }
    }
}
