using EVS_SDK;
using EVS_SDK.Api;
using EVS_SDK.Bean;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace sdk_csharp_sample
{
    public class Storage
    {
        private IStorageService storageService;

        public Storage(SDKClient client) {
            storageService = new StorageServiceImpl(client);
        }

        public void Process() {
            //存证文件
            StorageFile();

            //下载存证文件
            DownloadFile("123123");
        }

        public void StorageFile() {
            Stream stream = null;
            try
            {
                stream = new FileStream("C:\\Users\\Administrator\\Desktop\\aa.pdf", FileMode.Open);
                string fid = storageService.Upload(stream, "劳动合同", "pdf", false);
                Console.WriteLine("文件存证成功！文件ID:" + fid);
            }
            catch (Exception e)
            {
                Console.WriteLine("文件存证失败！"+e.Message);
            }
            finally {
                stream.Close();
            }
            
        }

        public void DownloadFile(string fid)
        {
            Stream downloadFile = new FileStream("downlowd.pdf", FileMode.CreateNew);
            storageService.Download(fid, ref downloadFile);
            downloadFile.Close();
            Console.WriteLine("文件存证下载成功！文件ID:" + fid);
        }
    }
}
