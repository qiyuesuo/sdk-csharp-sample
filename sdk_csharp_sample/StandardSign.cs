using QiyuesuoSDK.Api;
using QiyuesuoSDK.Bean;
using QiyuesuoSDK.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace sdk_csharp_sample
{
    public class StandardSign
    {
        private IStandardSignService signService;
        private ISealService sealService;
        public StandardSign(SDKClient client)
        {
            signService = new StandardSignServiceImpl(client);
            sealService = new SealServiceImpl(client);
        }

        public void Process()
        {
            try
            {
                //根据模板创建合同
                string documentId = CreateByTemplate();

                //根据html创建合同
                documentId = CreateByHtml();

                //根据文件创建合同
                documentId = CreateByFile();

                //对接平台签署
                SignByPlatform(documentId);

                //查询合同详情
                Detail(documentId);

                //下载合同文档
                DownloadDoc(documentId);

                //下载合同
                Download(documentId);
            }
            catch (Exception e) {
                Console.WriteLine("标准签署模式异常，错误信息{0}", e.Message);
            }
            Console.ReadKey();//防止闪退 
        }

        private string CreateByTemplate() {
            //设置模板参数，模板在契约锁云平台维护
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("param1", "参数1");
            parameters.Add("param2", "参数2");
            parameters.Add("param3", "参数3");

            //设置接收人，名称、联系方式必填
            Receiver receiver = new Receiver();
            receiver.name = "丁武";
            receiver.mobile = "13262898649";
            receiver.type = AccountType.PERSONAL;
            receiver.authLevel = AuthenticationLevel.BASIC;
            receiver.ordinal = 1;

            List<Receiver> receivers = new List<Receiver>();
            receivers.Add(receiver);

            //根据模板创建合同,合同存放于默认合同分类中，合同分类在云平台上维护
            string documentId = signService.Create("2287912848399175726", parameters, "标准模板测试合同", receivers);
            //documentId = signService.Create("2287912848399175726", parameters, "标准模板测试合同", receivers, "2287912843499439826");//文件存放于指定合同分类，合同分类在云平台进行维护
            Console.WriteLine("根据模板创建合同成功，文档ID:{0}", documentId);
            return documentId;
        }

        private string CreateByFile() {
            FileStream fileInput = new FileStream("D://authorization.pdf", FileMode.Open);

            //设置接收人，名称、联系方式必填
            Receiver receiver = new Receiver();
            receiver.name = "丁武";
            receiver.mobile = "13262898649";
            receiver.type = AccountType.PERSONAL;
            receiver.authLevel = AuthenticationLevel.BASIC;
            receiver.ordinal = 1;

            List<Receiver> receivers = new List<Receiver>();
            receivers.Add(receiver);

            string documentid = signService.Create(fileInput, "远程签授权协议书", receivers);//合同存放于默认合同分类中，合同分类在云平台上【参数模板】维护
            // documentid = signService.Create(fileInput, "远程签授权协议书",receivers,"2287912843499439826");/文件存放于指定合同分类，合同分类在云平台进行维护
            fileInput.Close();
            Console.WriteLine("根据文件创建合同成功，文档ID:{0}", documentid);
            return documentid;
        }

        private string CreateByHtml()
        {
            string documentid = "";
            //根据html创建合同,不带有效时间
            string html = "<html><body><p>title</p><p>在线第三方电子合同平台。企业及个人用户可通过本平台与签约方快速完成合同签署，安全、合法、有效。</p></body></html>";

            //设置接收人，名称、联系方式必填
            Receiver receiver = new Receiver();
            receiver.name = "丁武";
            receiver.mobile = "13262898649";
            receiver.type = AccountType.PERSONAL;
            receiver.authLevel = AuthenticationLevel.BASIC;
            receiver.ordinal = 1;

            List<Receiver> receivers = new List<Receiver>();
            receivers.Add(receiver);

            //根据html创建合同,带有效时间
            documentid = signService.Create(html, receivers, "html测试合同");
            // documentid = signService.Create(html, receivers, "html测试合同","2287912843499439826");/文件存放于指定合同分类，合同分类在云平台进行维护
            Console.WriteLine("根据html创建合同成功，文档ID:{0}", documentid);

            return documentid;
        }

        private void SignByPlatform(string documentId) {
            Stamper stamper = new Stamper(1,0.5f,0.5f);//根据坐标比确定位置
            //Stamper stamper = new Stamper("公章：", 0.1f, 0f);//根据关键字确定位置
            //签署对接方公章，公章在云平台上进行维护
            signService.Sign(documentId, "2208938212208934912", stamper);
            Console.WriteLine("平台签署成功！");
        }

        private void Detail(string documentId) {
            Contract result = signService.Detail(documentId);
            Console.WriteLine("合同状态：{0}", result.status);
        }

        private void DownloadDoc(string documentId) {
            Stream stream = new FileStream("D://Download.pdf", FileMode.Create, FileAccess.Write);
            signService.DownloadDoc(documentId, ref stream);
            stream.Close();
            Console.WriteLine("下载合同文件成功！");
        }

        private void Download(string documentId) {
            Stream stream = new FileStream("D://Download.zip", FileMode.Create, FileAccess.Write);
            signService.Download(documentId, ref stream);
            stream.Close();
            Console.WriteLine("下载合同成功！");
        }
    }
}
