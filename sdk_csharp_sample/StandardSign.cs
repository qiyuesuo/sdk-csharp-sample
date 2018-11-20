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
                long contractId;
                //根据文件创建合同
                //  long contractId = CreateByFile();

                //根据模板创建合同
                //InitResponse initResponse = CreateByTemplate();

                //根据html创建合同
                InitResponse initResponse = CreateByHtml();

                //根据文件增加文档
                // long documentId1 = AddDocumentByFile(contractId);

                //根据模板增加文档
                // long documentId2 = AddDocumentByTemplate(contractId);

                //根据html增加文档
               //  long documentId3 = AddDocumentByHtml(contractId);

                //合同发起
                List<long> documents = new List<long>();
               contractId = initResponse.contractId.Value;
               documents.Add(initResponse.documentId.Value);
               // documents.Add(documentId1);
               // documents.Add(documentId2);
               // documents.Add(documentId3);

                Send(contractId, documents);

                //如需要法人签署
                //SignByLegalPerson(contractId, documents);

                //对接平台签署
                SignByPlatform(contractId, documents);

                //查询合同详情
                Detail(contractId);

                //下载合同文档
                DownloadDoc(documents[0]);

                //下载合同
                Download(contractId);
            }
            catch (Exception e) {
                Console.WriteLine("标准签署模式异常，错误信息{0}", e.Message);
            }
            Console.ReadKey();//防止闪退 
        }

        private InitResponse CreateByFile()
        {
            string filePath = "D://create.pdf";

            InitResponse initResponse;
            string docName = Path.GetFileNameWithoutExtension(filePath);
            InitByFileRequest request = new InitByFileRequest();
            request.subject = "C#标准签测试"+ DateTime.Now.ToString();
            request.docName = docName;

            FileStream inputStream = new FileStream(filePath, FileMode.Open);
            request.file = inputStream;

            try
            {
                initResponse= signService.Create(request);
            }
            catch (Exception e)
            {
                inputStream.Close();
                throw e;
            }
            inputStream.Close();
            Console.WriteLine("根据文件创建合同成功，合同ID:{0},文档ID:{1}", initResponse.contractId, initResponse.documentId);
            return initResponse;
        }

        private InitResponse CreateByTemplate() {

            InitResponse initResponse;
            //设置模板参数，模板在契约锁云平台维护
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("name", "参数1");
            parameters.Add("content", "参数2");

            InitByTemplateRequest request = new InitByTemplateRequest();
            request.docName = "测试模板";
            request.subject = "C#标准签测试" + DateTime.Now.ToString(); 
            request.templateId = 2388901589532369448;
            request.templateParams = parameters;

            try
            {
                initResponse = signService.Create(request);
            }
            catch (Exception e)
            {
                throw e;
            }
            Console.WriteLine("根据模板创建合同成功，合同ID:{0},文档ID:{1}", initResponse.contractId, initResponse.documentId);

            return initResponse;
        }

        private InitResponse CreateByHtml()
        {
            InitResponse initResponse;
            //根据html创建合同
            string html = "<html><body><p>title</p><p>在线第三方电子合同平台。企业及个人用户可通过本平台与签约方快速完成合同签署，安全、合法、有效。</p></body></html>";

            InitByHtmlRequest request = new InitByHtmlRequest();
            request.subject = "C#标准签测试" + DateTime.Now.ToString();
            request.docName = "html文件";
            request.html = html;

            try
            {
                initResponse = signService.Create(request);
            }
            catch (Exception e)
            {
                throw e;
            }

            Console.WriteLine("根据html创建合同成功，合同ID:{0},文档ID:{1}", initResponse.contractId, initResponse.documentId);

            return initResponse;
        }

        private long AddDocumentByFile(long contractId)
        {
            long documentId;
            string filePath = "D://20180108.pdf";
            AddDocumentByFileRequest addRequest = new AddDocumentByFileRequest();
            string docName = Path.GetFileNameWithoutExtension(filePath);

            FileStream inputStream = new FileStream(filePath, FileMode.Open);
            addRequest.file = inputStream;
            addRequest.contractId = contractId;//合同ID，需要调用Create获取
            addRequest.title = docName;

            try
            {
                documentId = signService.AddDocument(addRequest);
                Console.WriteLine("根据文件增加文档成功，文档ID:{0}", documentId);
            }
            catch (Exception e)
            {
                inputStream.Close();
                throw e;
            }
            inputStream.Close();
            return documentId;
        }

        private long AddDocumentByTemplate(long contractId)
        {
            long documentId;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("userName", "参数1");
            parameters.Add("mobile", "参数2");

            AddDocumentByTemplateRequest addRequest = new AddDocumentByTemplateRequest();
            addRequest.contractId = contractId;
            addRequest.templateId = 2378126196387532900;
            addRequest.title = "模板文件";
            addRequest.templateParams = parameters;

            try
            {
                documentId = signService.AddDocument(addRequest);
                Console.WriteLine("根据模板增加文档成功，文档ID:{0}", documentId);
            }
            catch (Exception e)
            {
                throw e;
            }

            return documentId;
        }

        private long AddDocumentByHtml(long contractId)
        {
            long documentId;
            string html = "<html><body><p>title</p><p>在线第三方电子合同平台。企业及个人用户可通过本平台与签约方快速完成合同签署，安全、合法、有效。</p></body></html>";

            AddDocumentByHtmlRequest addRequest = new AddDocumentByHtmlRequest();
            addRequest.contractId = contractId;
            addRequest.title = "html";
            addRequest.html = html;

            try
            {
                documentId = signService.AddDocument(addRequest);
                Console.WriteLine("根据html增加文档成功，文档ID:{0}", documentId);
            }
            catch (Exception e)
            {
                throw e;
            }

            return documentId;
        }

        private void Send(long contractId,List<long> documentIds)
        {
            //设置接收人，名称、联系方式必填
            List<Receiver> receivers = new List<Receiver>();

            List<StandardStamper> platestampers = new List<StandardStamper>();
            //指定本方签署位置
            StandardStamper sealStamper = new StandardStamper();
            sealStamper.documentId = documentIds[0];
            sealStamper.page = 1;
            sealStamper.offsetX = 0.5;
            sealStamper.offsetY = 0.5;
            sealStamper.type = StandardSignType.SEAL;

            StandardStamper stStamper = new StandardStamper();
            stStamper.documentId = documentIds[0];
            stStamper.page = 1;
            stStamper.offsetX = 0.5;
            stStamper.offsetY = 0.45;
            stStamper.type = StandardSignType.SEAL_TIMESTAMP;

           // StandardStamper lpStamper = new StandardStamper();
           // lpStamper.documentId = documentIds[1];
           // lpStamper.page = 1;
           // lpStamper.offsetX = 0.6;
           // lpStamper.offsetY = 0.2;
           // lpStamper.type = StandardSignType.LEGAL_PERSON;

            platestampers.Add(sealStamper);
            platestampers.Add(stStamper);
            //platestampers.Add(lpStamper);

            //需要本方签署，则将本方加入接收者列表
            Receiver platform = new Receiver();
            platform.type = AccountType.PLATFORM;
            platform.ordinal = 0;
            platform.stampers = platestampers; //指定签署位置
            platform.legalPersonRequired = false; //指定法人签署
            receivers.Add(platform);

            StandardStamper pStamper = new StandardStamper();
            pStamper.documentId = documentIds[0];
            pStamper.page = 1;
            pStamper.keyword = null;
            pStamper.offsetX = 0.64;
            pStamper.offsetY = 0.105; 
            pStamper.type = StandardSignType.PERSONAL;

            StandardStamper ptStamper = new StandardStamper();
            ptStamper.documentId = documentIds[0];
            ptStamper.page = 1;
            ptStamper.keyword = null;
            ptStamper.offsetX = 0.655;
            ptStamper.offsetY = 0.058;
            ptStamper.type = StandardSignType.PERSONAL_TIMESTAMP;

            StandardStamper pStamper_d = new StandardStamper();
            pStamper_d.documentId = documentIds[0];
            pStamper_d.page = 2;
            pStamper_d.keyword = null;
            pStamper_d.offsetX = 0.658;
            pStamper_d.offsetY = 0.09;
            pStamper_d.type = StandardSignType.PERSONAL;

            StandardStamper ptStamper_d = new StandardStamper();
            ptStamper_d.documentId = documentIds[0];
            ptStamper_d.page = 2;
            ptStamper_d.keyword = null;
            ptStamper_d.offsetX = 0.662;
            ptStamper_d.offsetY = 0.05;
            ptStamper_d.type = StandardSignType.PERSONAL_TIMESTAMP;

            List<StandardStamper> personalStampers = new List<StandardStamper>();
            personalStampers.Add(pStamper);
            personalStampers.Add(ptStamper);
            personalStampers.Add(pStamper_d);
            personalStampers.Add(ptStamper_d);

            Receiver receiver = new Receiver();
            receiver.name = "丁祥春";
            receiver.mobile = "13262598398";
            receiver.type = AccountType.PERSONAL;
            receiver.authLevel = AuthenticationLevel.BASIC;
            receiver.ordinal = 1;
            receiver.stampers = personalStampers;//个人签署位置

            receivers.Add(receiver);

            SendRequest request = new SendRequest();

            request.contractId = contractId;//指定发起的合同ID
            request.categoryId = null;      //不指定业务分类

            request.receivers = receivers;
            request.receiveType = ReceiveType.SEQ;//设置签署人顺序签署
            try
            {
                signService.Send(request);
                Console.WriteLine("合同成功发起！");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

         private void SignByLegalPerson(long contractId,List<long> documents) {
            SignByLegalPersonRequest request = new SignByLegalPersonRequest();
            request.contractId = contractId;

            List<StandardStamper> stampers = new List<StandardStamper>();
            StandardStamper stamper = new StandardStamper();
            stamper.documentId = documents[0];
            stamper.page = 1;
            stamper.offsetX = 0.1;
            stamper.offsetY = 0.2;
            stamper.type = StandardSignType.LEGAL_PERSON;
            stampers.Add(stamper);

            request.stampers = stampers;

            try
            {
                signService.SignByLegalPerson(request);
                Console.WriteLine("平台法人签署成功！");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

         private void SignByPlatform(long contractId,List<long> documents) {
            SignRequest request = new SignRequest();
            request.acrossPage = true;
            request.contractId = contractId;
            request.sealId = 2372592386585493570;

            /* List<StandardStamper> stampers = new List<StandardStamper>();
             StandardStamper stamper = new StandardStamper();
             stamper.documentId = documents[0];
             stamper.page = 1;
             stamper.offsetX = 0.1;
             stamper.offsetY = 0.2;
             stamper.type = StandardSignType.SEAL;
             stampers.Add(stamper);*/

            request.stampers = new List<StandardStamper>();

            try
            {
                signService.Sign(request);
                Console.WriteLine("平台签署成功！");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

         private void Detail(long contractId) {
             Contract result = signService.Detail(contractId);
             Console.WriteLine("合同状态：{0}", result.status);
         }

         private void DownloadDoc(long documentId) {
             Stream stream = new FileStream("D://Download.pdf", FileMode.Create, FileAccess.Write);
             signService.DownloadDoc(documentId, ref stream);
             stream.Close();
             Console.WriteLine("下载合同文件成功！");
         }

        private void Download(long contractId)
        {
            Stream stream = new FileStream("D://Download.zip", FileMode.Create, FileAccess.Write);
            signService.Download(contractId, ref stream);
            stream.Close();
            Console.WriteLine("下载合同成功！");
        }
    }
}
