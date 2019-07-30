using QiyuesuoClient.Http;
using QiyuesuoSDK.Bean;
using QiyuesuoSDK.Bean.Request;
using QiyuesuoSDK.Bean.Response;
using QiyuesuoSDK.Tools;
using System;
using System.IO;

namespace sdk_csharp_sample_v2
{
    class SendByCodeSetting
    {
        public void Process(SDKClient client)
        {
            //根据代码配置发起合同草稿，生成一个平台方公司发，个人签署的流程
            SdkResponse<Contract> draft = this.Draft(client);
            Console.WriteLine("创建合同草稿成功，合同ID：{0}", draft.Result.Id);
            //为合同添加签署的文件：（1）根据本地文件添加（2）根据在线模板添加，并维护参数
            SdkResponse<DocumentAddResult> fileAddResult = this.AddDocumentByFile(client, draft.Result.Id);
            Console.WriteLine("根据文件添加合同文档成功，文档ID：{0}", fileAddResult.Result.DocumentId);
            SdkResponse<DocumentAddResult> templateAddResult = this.AddDocumnetByTemplate(client, draft.Result.Id);
            Console.WriteLine("根据模板添加合同文件成功，文档ID：{0}", templateAddResult.Result.DocumentId);
            //发起合同，并指定签署位置（可选）
            SdkResponse<Object> sendResult = this.Send(client, draft.Result, fileAddResult.Result.DocumentId, templateAddResult.Result.DocumentId);
            Console.WriteLine("发起合同成功");
            //审批合同
            this.Audit(client, draft.Result.Id);
            Console.WriteLine("合同审批完成");
            //公司公章签署（若未指定位置，需指定位置签署）
            this.CompanySealSign(client, draft.Result.Id, fileAddResult.Result.DocumentId, templateAddResult.Result.DocumentId);
            Console.WriteLine("合同公章签署成功");
            // 法人章签署（若未指定位置，需指定位置签署）
            this.LpSign(client, draft.Result.Id, fileAddResult.Result.DocumentId, templateAddResult.Result.DocumentId);
            Console.WriteLine("法人章签署完成");
            /**
		      *  平台方签署完成，签署方签署可采用
		      * （1）接收短信的方式登录契约锁云平台进行签署
		      * （2）生成内嵌页面签署链接进行签署（下方生成的链接）
		      * （3）JS-SDK签署（仅支持个人）
		      */
            SdkResponse<PageUrlResult> pageUrl = this.GerenateSignUrl(client, draft.Result.Id);
            Console.WriteLine("个人签署方签署链接生成成功,签署链接：{0}", pageUrl.Result.PageUrl);
            Console.ReadKey();
        }

        /**
         *  创建草稿
         **/
        private SdkResponse<Contract> Draft(SDKClient client)
        {
            ContractDraftRequest request = new ContractDraftRequest();
            Contract contract = new Contract();
            contract.Subject = "合同主题名称";
            //添加平台方签署方
            Signatory platformSignatory = new Signatory("COMPANY", new User("张忱昊", "17621699044", "MOBILE"), 1);
            platformSignatory.TenantName = "大头橙橙汁公司";//平台方公司名称
            //添加平台方签署流程，可根据需要调整
            //目前平台方签署流程为：（1）员工审批 （2）法人章签署（3）公章签署
            // 审批流程，并设置审批操作人
            SignAction aduitAction = new SignAction("AUDIT", 1);
            aduitAction.AddOperators(new User("17621699044", "MOBILE"));
            platformSignatory.AddAction(aduitAction);
            // 公章签署流程，并设置签署公章ID
            SignAction sealAction = new SignAction("COMPANY", 2);
            sealAction.SealId = "2490828768980361630";
            platformSignatory.AddAction(sealAction);
            // 法人章签署流程
            platformSignatory.AddAction(new SignAction("LP", 3));
            contract.AddSignatory(platformSignatory);

            //添加个人签署方，并设置个人签署方需要上传的附件内容
            Signatory personalSignatory = new Signatory("PERSONAL", new User("邓茜茜", "15021504325", "MOBILE"), 2);
            personalSignatory.TenantName = "邓茜茜";//接收方名称
            personalSignatory.AddAttachment(new Attachment("附件", true, false));//添加上传附件要求，并设置为必须上传
            contract.AddSignatory(personalSignatory);

            //设置合同基本信息
            contract.ExpireTime = "2019-07-28 23:59:59";//设置合同过期时间，合同过期时间需要晚于发起时间
            contract.Send = false;  //合同不发起
            request.Contract = contract;

            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("创建合同草稿请求服务器失败,失败原因" + e.Message);
            }

            SdkResponse<Contract> sdkResponse = HttpJsonConvert.DeserializeResponse<Contract>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("创建合同草稿失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

        private SdkResponse<DocumentAddResult> AddDocumentByFile(SDKClient client, string contractId)
        {
            string path = "C:\\Users\\Richard Cheung\\Documents\\契约锁\\测试\\AA.pdf";   //待添加的文件路径
            Stream file = new FileStream(path, FileMode.Open);
            //指定文件名称，以及文件类型，比如此时的文件为pdf
            DocumentAddByFileRequest request = new DocumentAddByFileRequest(contractId, "根据文件添加文档", file, "pdf");

            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("根据文件添加合同文档请求服务器失败，失败原因：" + e.Message);
            }

            SdkResponse<DocumentAddResult> sdkResponse = HttpJsonConvert.DeserializeResponse<DocumentAddResult>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("根据文件添加合同文档失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

        private SdkResponse<DocumentAddResult> AddDocumnetByTemplate(SDKClient client, string contractId)
        {
            string templateId = "2492236993899110515";   //待添加的文件模板Id，合同模板请前往契约锁云平台维护
            DocumentAddByTemplateRequest request = new DocumentAddByTemplateRequest(contractId, "添加模板", templateId);
            // 如果文件模板为参数模板，添加参数内容
            request.AddTemplateParam(new TemplateParam("接收方1", "契约锁"));
            request.AddTemplateParam(new TemplateParam("接收方2", "电子合同"));

            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("根据模板添加合同文档请求服务器失败，失败原因：" + e.Message);
            }
            SdkResponse<DocumentAddResult> sdkResponse = HttpJsonConvert.DeserializeResponse<DocumentAddResult>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("根据模板添加合同文档添加失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }


        private SdkResponse<Object> Send(SDKClient client, Contract contract, string documentId1, string documentId2)
        {
            ContractSendRequest request = new ContractSendRequest();
            request.ContractId = contract.Id;
            string platformSignatoryId = null;
            string personalSignatoryId = null;
            string companyActionId = null;
            string lpActionId = null;
            // 获取对应的ActionId与SignatoryId
            foreach (Signatory signatory in contract.Signatories)
            {
                //获取平台方公司Signatory，确保TenantName替换为自己的公司名称
                if (signatory.TenantName.Equals("大头橙橙汁公司") && signatory.TenantType.Equals("COMPANY"))
                {
                    platformSignatoryId = signatory.Id;
                    foreach (SignAction action in signatory.Actions)
                    {
                        if (action.Type.Equals("COMPANY"))
                        {
                            companyActionId = action.Id;
                        }
                        if (action.Type.Equals("LP"))
                        {
                            lpActionId = action.Id;
                        }
                    }
                }
                //获取个人签署方,可通过TenantName确定精确的签署方
                if (signatory.TenantType.Equals("PERSONAL"))
                {
                    personalSignatoryId = signatory.Id;
                }
            }

            //指定签署位置 
            //发起方（公司）：印章签署位置、法人章签署位置、时间戳位置
            Stamper sealStamper = new Stamper();
            sealStamper.DocumentId = documentId1;
            sealStamper.ActionId = companyActionId;
            sealStamper.Type = "COMPANY";
            // 绝对位置定位：页数、X、Y定位
            sealStamper.Page = 1;
            sealStamper.OffsetX = 0.1;
            sealStamper.OffsetY = 0.2;
            // 关键字位置定位：keyword,keywordIndex,X,Y
            //sealStamper.Keyword = "甲方签字"; //确保文件中包含该关键字
            //sealStamper.KeywordIndex = 1;
            //sealStamper.OffsetX = 0.0;
            //sealStamper.OffsetY = 0.0;

            Stamper lpStamper = new Stamper();
            lpStamper.DocumentId = documentId1;
            lpStamper.ActionId = lpActionId;
            lpStamper.Type = "LP";
            lpStamper.Page = 1;
            lpStamper.OffsetX = 0.4;
            lpStamper.OffsetY = 0.6;

            Stamper timeStamper = new Stamper();
            timeStamper.DocumentId = documentId1;
            timeStamper.ActionId = companyActionId;
            timeStamper.Type = "TIMESTAMP";
            timeStamper.Page = 1;
            timeStamper.OffsetX = 0.3;
            timeStamper.OffsetY = 0.5;

            //个人签署方 签署位置
            Stamper personalStamper = new Stamper();
            personalStamper.DocumentId = documentId1;
            personalStamper.SignatoryId = personalSignatoryId;
            personalStamper.Type = "PERSONAL";
            personalStamper.Page = 1;
            personalStamper.OffsetX = 0.7;
            personalStamper.OffsetY = 0.5;

            request.AddStamper(sealStamper);
            request.AddStamper(lpStamper);
            request.AddStamper(timeStamper);
            request.AddStamper(personalStamper);

            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("发起合同请求服务器失败，失败原因：" + e.Message);
            }
            SdkResponse<Object> sdkResponse = HttpJsonConvert.DeserializeResponse<Object>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("发起合同失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }


        private SdkResponse<Object> Audit(SDKClient client, string contractId)
        {
            ContractAuditRequest request = new ContractAuditRequest();
            request.ContractId = contractId;
            request.Pass = true;    //审批结果，True:通过，False:不通过，若不通过合同无法继续签署
            request.Comment = "符合要求，审批通过";  //审批原因

            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("审批合同请求服务器失败，失败原因：" + e.Message);
            }
            SdkResponse<Object> sdkResponse = HttpJsonConvert.DeserializeResponse<Object>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("审批合同失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }


        private SdkResponse<Object> LpSign(SDKClient client, string contractId, string documentId1, string documentId2)
        {
            ContractSignLPRequest request = new ContractSignLPRequest();
            request.ContractId = contractId;

            //若法人章未指定签署位置，需要在签署时指定签署位置
            //若制定了签署位置，优先使用签署位置中的位置进行签署
            /*
            Stamper lpStamper = new Stamper();
            lpStamper.Type = "LP";
            lpStamper.OffsetX = 0.2;
            lpStamper.OffsetY = 0.3;
            lpStamper.Page = 1;
            lpStamper.DocumentId = documentId1;

            Stamper lpTimeStamp = new Stamper();
            lpTimeStamp.Type = "TIMESTAMP";
            lpTimeStamp.OffsetX = 0.4;
            lpTimeStamp.OffsetY = 0.2;
            lpTimeStamp.Page = 1;
            lpTimeStamp.DocumentId = documentId1;
            request.AddStampers(lpStamper);
            request.AddStampers(lpTimeStamp);
            */

            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("法人章签署请求服务器失败,失败原因：" + e.Message);
            }
            SdkResponse<Object> sdkResponse = HttpJsonConvert.DeserializeResponse<Object>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("法人章签署失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

        private SdkResponse<Object> CompanySealSign(SDKClient client, string contractId, string documentId1, string documentId2)
        {
            ContractSignCompanyRequest request = new ContractSignCompanyRequest();
            request.ContractId = contractId;

            //若公章未指定签署位置，需要在签署时指定签署位置
            //若制定了签署位置，优先使用签署位置中的位置进行签署
            /*
            Stamper companyStamper = new Stamper();
            companyStamper.Type = "COMPANY";
            companyStamper.OffsetX = 0.2;
            companyStamper.OffsetY = 0.3;
            companyStamper.Page = 1;
            companyStamper.DocumentId = documentId1;
            companyStamper.SealId = "2490828768980361630";

            Stamper companyTimeStamp = new Stamper();
            companyTimeStamp.Type = "TIMESTAMP";
            companyTimeStamp.OffsetX = 0.4;
            companyTimeStamp.OffsetY = 0.2;
            companyTimeStamp.Page = 1;
            companyTimeStamp.DocumentId = documentId1;
            companyTimeStamp.SealId = "2490828768980361630";

            //骑缝章签署需要文档页数在一页以上
            Stamper acrossPageStamper = new Stamper();
            acrossPageStamper.Type = "ACROSS_PAGE";
            acrossPageStamper.OffsetY = 0.2;
            acrossPageStamper.DocumentId = documentId1;
            acrossPageStamper.SealId = "2490828768980361630";

            request.AddStampers(companyStamper);
            request.AddStampers(companyTimeStamp);
            request.AddStampers(acrossPageStamper);
            */


            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("公章签署请求服务器失败，失败原因：" + e.Message);
            }
            SdkResponse<Object> sdkResponse = HttpJsonConvert.DeserializeResponse<Object>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("公章签署失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

        private SdkResponse<Contract> Detail(SDKClient client, string contractId)
        {
            ContractDetailRequest detailRequest = new ContractDetailRequest()
            {
                ContractId = contractId
            };
            string response = null;
            try
            {
                response = client.Service(detailRequest);
            }
            catch (Exception e)
            {
                throw new Exception("查询合同详情请求服务器失败，错误原因：" + e.Message);
            }
            SdkResponse<Contract> sdkResponse = HttpJsonConvert.DeserializeResponse<Contract>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("查询合同详情失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }

        private SdkResponse<PageUrlResult> GerenateSignUrl(SDKClient client, string contractId)
        {
            ContractPageRequest request = new ContractPageRequest();
            request.ContractId = contractId;                        //内嵌页面签署合同ID
            request.User = new User("15021504325", "MOBILE");       //内嵌页面签署人信息
            string response = null;
            try
            {
                response = client.Service(request);
            }
            catch (Exception e)
            {
                throw new Exception("生成签署链接请求服务器失败，失败原因：" + e.Message);
            }
            SdkResponse<PageUrlResult> sdkResponse = HttpJsonConvert.DeserializeResponse<PageUrlResult>(response);
            if (!sdkResponse.Code.Equals(0))
            {
                throw new Exception("生成签署链接失败，失败原因：" + sdkResponse.Message);
            }
            return sdkResponse;
        }
    }
}
