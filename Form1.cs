using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using QiyuesuoSDK.Bean;
using QiyuesuoSDK;
using QiyuesuoSDK.Api;
using QiyuesuoSDK.Sign;
using QiyuesuoSDK.Template;
using QiyuesuoSDK.Impl;

namespace SDKTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            url.Text = "http://openapi.qiyuesuo.net";
            token.Text = "upwwMOaavg";
            secret.Text = "XBy3aEQVwAhrojauLcg7ncRIxl1oJ6";
           // url.Text = "https://openapi.qiyuesuo.me";
           // token.Text = "STyiuYxKd6";
           // secret.Text = "6PUUqBz2RuydV8wHw6Ryg93CuxRNpu";

              sealId.Text = "2208938212208934912";
        }

        private void platform_CheckedChanged(object sender, EventArgs e)
        {
            UserInfo.Enabled = false;
            noView.Enabled = true;
            coordinate.Enabled = true;
            keyword.Enabled = true;
            setAppearanceStatus();
        }

        private void user_CheckedChanged(object sender, EventArgs e)
        {
            UserInfo.Enabled = true;
            noView.Enabled = true;
            coordinate.Enabled = true;
            keyword.Enabled = true;
            sealId.Enabled = false;
            setAppearanceStatus();
        }

        private void qiyuesuo_CheckedChanged(object sender, EventArgs e)
        {
            UserInfo.Enabled = false;
            noView.Checked = true;
            coordinate.Enabled = false;
            keyword.Enabled = false;
            sealId.Enabled = false;
            setAppearanceStatus();
        }

        private void noView_CheckedChanged(object sender, EventArgs e)
        {
            if (platform.Checked)
                sealId.Enabled = true;
            keywordTxt.Enabled = false;
            pageNo.Enabled = false;
            offsetX.Enabled = false;
            offsetY.Enabled = false;
        }

        private void coordinate_CheckedChanged(object sender, EventArgs e)
        {
            if(platform.Checked)
             sealId.Enabled = true;
            keywordTxt.Enabled = false;
            pageNo.Enabled = true;
            offsetX.Enabled = true;
            offsetY.Enabled = true;
        }

        private void keyword_CheckedChanged(object sender, EventArgs e)
        {
            if (platform.Checked)
                sealId.Enabled = true;
            keywordTxt.Enabled = true;
            pageNo.Enabled = false;
            offsetX.Enabled = true;
            offsetY.Enabled = true;
        }

        private void setAppearanceStatus() {
            if (noView.Checked) {
                if (platform.Checked)
                    sealId.Enabled = true;
                keywordTxt.Enabled = false;
                pageNo.Enabled = false;
                offsetX.Enabled = false;
                offsetY.Enabled = false;
            }

            if (coordinate.Checked) {
                if (platform.Checked)
                    sealId.Enabled = true;
                keywordTxt.Enabled = false;
                pageNo.Enabled = true;
                offsetX.Enabled = true;
                offsetY.Enabled = true;
            }

            if (keyword.Checked) {
                if (platform.Checked)
                    sealId.Enabled = true;
                keywordTxt.Enabled = true;
                pageNo.Enabled = false;
                offsetX.Enabled = true;
                offsetY.Enabled = true;
            }
        }

        private void button_Local_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                localfile.Text = file;
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = "D:\\";
            sfd.Filter = "PDF文件(*.pdf)|*.pdf";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SavePath.Text = sfd.FileName;
            }
        }

        private void localSign_btn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(localfile.Text)) {
                MessageBox.Show("请指定需要签名的PDF文件");
                return;
            }

            if (String.IsNullOrEmpty(SavePath.Text)) {
                MessageBox.Show("请指定文件的输出路径");
                return;
            }

            if (!checkBeforeSign())
                return;

            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);

            Stream inputStream = new FileStream(localfile.Text, FileMode.Open);
            Stream outputStream = new FileStream(SavePath.Text, FileMode.Create);
            try
            {
                if (platform.Checked)
                {
                    LocalSignByPlatform(inputStream, ref outputStream, client);
                }
                else if (user.Checked)
                {
                    LocalSignByUser(inputStream, ref outputStream, client);
                }
                else if (qiyuesuo.Checked)
                {
                    LocalSignByQiyuesuo(inputStream, ref outputStream, client);
                }
            }
            catch (Exception er)
            {
                showMessage(er.Message);
            }
            finally {
                inputStream.Close();
                outputStream.Close();
            }
        }

        private void LocalSignByPlatform(Stream input,ref Stream output,SDKClient client) {
            showMessage("开始本地平台签署。");

            ILocalSignService localSign = new QiyuesuoSDK.Impl.LocalSignServiceImpl(client);
            Stamper stamper = null;

            if (noView.Checked)
            {
                localSign.Sign(input, ref output);
                return;
            }
            else if (coordinate.Checked)
            {
                stamper = new Stamper(int.Parse(pageNo.Text), float.Parse(offsetX.Text), float.Parse(offsetY.Text));

            }
            else {
                stamper = new Stamper(keywordTxt.Text);
               
                if (!String.IsNullOrEmpty(offsetX.Text)) {
                    stamper.offsetX = float.Parse(offsetX.Text);
                }

                if (!String.IsNullOrEmpty(offsetY.Text)) {
                    stamper.offsetY = float.Parse(offsetY.Text);
                }

            }

            showMessage("签名外观配置完成。");

            localSign.Sign(input, ref output, sealId.Text,stamper);
            showMessage("签名成功！");
        }

        private Person getPersonInfo() {

            if (String.IsNullOrEmpty(PName.Text))
            {
                MessageBox.Show("请输入个人名称！");
                return null;
            }

            if (String.IsNullOrEmpty(mobile.Text) && String.IsNullOrEmpty(email.Text))
            {
                MessageBox.Show("请指定个人的一项联系方式");
                return null;
            }
            Person person = new Person(PName.Text);
            person.idcard = IdCard.Text;
            person.mobile = mobile.Text;
            person.email = email.Text;
            person.gender = SerializeGender(gender.Text);
            person.paperType = SerializePaperType(paperType.Text);
            return person;
        }

        private Company getCompanyInfo() {


            if (String.IsNullOrEmpty(CName.Text))
            {
                MessageBox.Show("请输入企业名称！");
                return null;
            }

            if (String.IsNullOrEmpty(registerNo.Text))
            {
                MessageBox.Show("请输入工商注册号");
                return null;
            }
            Company company = new Company(CName.Text);
            company.registerNo = registerNo.Text;
            company.address = Caddress.Text;
            company.email = Cemail.Text;
            company.contact = contact.Text;
            company.telephone = Cmobile.Text;
            company.legalPerson = legalPerson.Text;
            company.legalPersonId = legalPersonId.Text;
            company.paperType = SerializePaperType(CpaperType.Text);
            return company;
        }

        private void LocalSignByUser(Stream input, ref Stream output, SDKClient client) {

            Person person = null;
            Company company = null;

            TabPage selectedPage =  UserInfo.SelectedTab;
            if (selectedPage.Text.Equals("个人"))
            {
                person = getPersonInfo();
                if (person == null)
                    return;
            }
            else {
                company = getCompanyInfo();
                if (company == null)
                    return;
            }
            showMessage("用户信息采集完成");
            ILocalSignService localSign = new LocalSignServiceImpl(client);
            ISealService sealService = new SealServiceImpl(client);

            if (noView.Checked)
            {
                if (person != null)
                    localSign.Sign(input, ref output, person);
                else
                    localSign.Sign(input, ref output, company);
            }
            else
            {
                Stamper stamper = null;
                string seal = null;
                if (person != null)
                {
                    seal = sealService.GenerateSeal(person);
                }
                else {
                    seal = sealService.GenerateSeal(company);
                }

                showMessage("成功生成电子印章");
                if (coordinate.Checked)
                {
                    stamper = new Stamper(int.Parse(pageNo.Text), float.Parse(offsetX.Text), float.Parse(offsetY.Text));
                }
                else {
                    stamper = new Stamper(keywordTxt.Text);

                    if (!String.IsNullOrEmpty(offsetX.Text))
                    {
                        stamper.offsetX = float.Parse(offsetX.Text);
                    }

                    if (!String.IsNullOrEmpty(offsetY.Text))
                    {
                        stamper.offsetY = float.Parse(offsetY.Text);
                    }
                }
                showMessage("签名外观配置完成");

                if (person != null)
                    localSign.Sign(input, ref output, person, seal, stamper);
                else
                    localSign.Sign(input, ref output, company,seal,stamper);
            }




            showMessage("签名成功！");
        }

        private void LocalSignByQiyuesuo(Stream input, ref Stream output, SDKClient client) {

            ILocalSignService localSign = new LocalSignServiceImpl(client);
            localSign.Complete(input,ref output);
            showMessage("完成合同封存！");

        }

        private void LocalVerify_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(localfile.Text))
            {
                MessageBox.Show("请指定需要验证的PDF文件");
                return;
            }

            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            Stream inputStream = new FileStream(localfile.Text, FileMode.Open);

            ILocalSignService localSignService = new LocalSignServiceImpl(client);
            try
            {
                localSignService.Verify(inputStream);
            }
            catch (Exception e1)
            {
                showMessage(e1.Message);
                return;
            }
            finally {
                inputStream.Close();
            }

            showMessage("合同有效！");
        }

        private Gender SerializeGender(string txt) {
            if (String.IsNullOrEmpty(txt))
                return Gender.UNKNOWN;

            if (txt.Equals("男"))
                return Gender.MALE;
            else if (txt.Equals("女"))
                return Gender.FEMALE;
            else
                return Gender.UNKNOWN;
        }

        private PaperType SerializePaperType(string txt) {
            if (String.IsNullOrEmpty(txt))
                return PaperType.OTHER;

            if (txt.Equals("身份证"))
            {
                return PaperType.IDCARD;
            }
            else if (txt.Equals("护照"))
            {
                return PaperType.PASSPORT;
            }
            else
                return PaperType.OTHER;
        }

        private bool checkBeforeSign() {
            if (platform.Checked || !noView.Checked)
            {
                if (String.IsNullOrEmpty(sealId.Text))
                {
                    MessageBox.Show("在进行平台签署非不可见签名时，需要指定印章ID");
                    return false;
                }
            }

            if (keyword.Checked)
            {
                if (String.IsNullOrEmpty(keywordTxt.Text))
                {
                    MessageBox.Show("在以关键字为签署位置时，需要指定关键字");
                    return false;
                }
            }

            if (coordinate.Checked)
            {
                if (String.IsNullOrEmpty(pageNo.Text) || String.IsNullOrEmpty(offsetX.Text) || String.IsNullOrEmpty(offsetY.Text))
                {
                    MessageBox.Show("在以坐标为签署位置时，需要指定页码和坐标位置");
                    return false;
                }
            }

            return true;
        }


        private void showMessage(string message) {
            System.DateTime currentTime = DateTime.Now;
            string myMsg = currentTime.Hour.ToString()+":"+currentTime.Minute.ToString() +":"+currentTime.Second.ToString()+"."+ currentTime.Millisecond.ToString()+" " + message;
            ResultList.Items.Add(myMsg);
            ResultList.TopIndex = ResultList.Items.Count - (int)(ResultList.Height / ResultList.ItemHeight);
        }

        private void clearResult_Click(object sender, EventArgs e)
        {
            if (ResultList.Items.Count > 0)
            {
                ResultList.Items.Clear();
            }
        }

        private void addParamer_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                rLocalFile.Text = file;
            }
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
        }

        private void RCreate_Click(object sender, EventArgs e)
        {
            string result = null;
            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            IRemoteSignService remoteSignService = new RemoteSignServiceImpl(client);

            if (radio_rFile.Checked)
            {
                if (String.IsNullOrEmpty(rLocalFile.Text))
                {
                    MessageBox.Show("请输入本地合同文件的路径");
                    return;
                }

                if (!checkBeforeSign())
                    return;

                FileStream inputStream = new FileStream(rLocalFile.Text, FileMode.Open);

                try
                {
                    result = remoteSignService.Create(inputStream, "测试合同", DateTime.Now.AddDays(7));
                }
                catch (Exception e1)
                {
                    showMessage("创建合同失败，" + e1.Message);
                    return;
                }
                finally
                {
                    inputStream.Close();
                }
            }
            else {
                if (String.IsNullOrEmpty(TemplateId.Text)) {
                    MessageBox.Show("请输入模板ID");
                    return;
                }

                if (!checkBeforeSign())
                    return;

                TemplateInfo templateinfo = new TemplateInfo(TemplateId.Text);
                Dictionary<string, string> tplParamers = new Dictionary<string, string>();
                foreach (ListViewItem item in paramers.Items) {
                    if(!String.IsNullOrEmpty(item.SubItems[0].Text)&& !String.IsNullOrEmpty(item.SubItems[0].Text))
                    tplParamers.Add(item.SubItems[0].Text, item.SubItems[1].Text);
                }
                templateinfo.parameters = tplParamers;
                try
                {
                    result = remoteSignService.Create(templateinfo, "模板测试合同", System.DateTime.Now.AddDays(7));
                }
                catch (Exception e2) {
                    showMessage("创建合同失败，" + e2.Message);
                    return;
                }
            }
 

            showMessage("合同创建成功，文档ID：" + result);
            Documents.Items.Clear();
            Documents.Items.Add(result);
            Documents.Text = result;
        }

        private void radio_rFile_CheckedChanged(object sender, EventArgs e)
        {
            rLocalFile.Enabled = true;
            TemplateId.Enabled = false;
            paramers.Enabled = false;
            AddParamer.Enabled = false;
            DelParamer.Enabled = false;
        }

        private void radio_rTemplate_CheckedChanged(object sender, EventArgs e)
        {
            rLocalFile.Enabled = false;
            TemplateId.Enabled = true;
            paramers.Enabled = true;
            AddParamer.Enabled = true;
            DelParamer.Enabled = true;

            paramers.Items.Clear();
            string[] nameitem = { "name", "张三" };
            string[] mobileitem = {"mobile","13262588888" };
            string[] moneyitem = { "money", "8000" };

            paramers.Items.Add(new ListViewItem(nameitem));
            paramers.Items.Add(new ListViewItem(mobileitem));
            paramers.Items.Add(new ListViewItem(moneyitem));
        }

        private void AddParamer_Click_1(object sender, EventArgs e)
        {
            ParamerDlg dlg = new ParamerDlg(paramers);
            dlg.Show();

        }

        private void DelParamer_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in paramers.SelectedItems) {
                paramers.Items.Remove(item);
            }
            
        }


        private void RemoteSign_btn_Click(object sender, EventArgs e)
        {
            if (!checkBeforeSign())
                return;

            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            IRemoteSignService remoteSignService = new RemoteSignServiceImpl(client);

            try
            {
                if (platform.Checked)
                {
                    RemoteSignByPlatform(remoteSignService);
                }
                else if (user.Checked)
                {
                    RemoteSignByUser();
                }
                else if (qiyuesuo.Checked)
                {
                    RemoteSignQiyuesuo();
                }
            }
            catch (Exception er)
            {
                showMessage(er.Message);
                return;
            }


        }

        private void RemoteSignByPlatform(IRemoteSignService remoteSign) {

            if (String.IsNullOrEmpty(Documents.Text))
            {
                throw new Exception("请指定文档ID");
            }
            if (noView.Checked)
            {
                remoteSign.Sign(Documents.Text);
            }
            else {
                Stamper stamper = null;
                if (keyword.Checked) {
                    stamper = new Stamper(keywordTxt.Text, float.Parse(offsetX.Text), float.Parse(offsetY.Text));
       
                }
                else {
                    stamper = new Stamper(int.Parse(pageNo.Text), float.Parse(offsetX.Text), float.Parse(offsetY.Text));
                }

                remoteSign.Sign(Documents.Text,sealId.Text, stamper);
            }
      
            showMessage("平台签署成功");
        }

        private void RemoteSignByUser() {
            Person person = null;
            Company company = null;
            string seal = null;

            TabPage selectedPage = UserInfo.SelectedTab;
            if (selectedPage.Text.Equals("个人"))
            {
                person = getPersonInfo();
                if (person == null)
                    return;
            }
            else {
                company = getCompanyInfo();
                if (company == null)
                    return;
            }
            showMessage("用户信息采集完成");

            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            IRemoteSignService remoteSign = new RemoteSignServiceImpl(client);
            ISealService sealService = new SealServiceImpl(client);
            if (noView.Checked)
            {
                if (person != null)
                    remoteSign.Sign(Documents.Text, person);
                else
                    remoteSign.Sign(Documents.Text, company);
            }
            else
            {
                if (person != null)
                {
                    seal = sealService.GenerateSeal(person);
                }
                else {
                    seal = sealService.GenerateSeal(company);
                }

                showMessage("成功生成电子印章");

                Stamper stamper =null;
                if (coordinate.Checked)
                {
                    stamper = new Stamper(int.Parse(pageNo.Text), float.Parse(offsetX.Text), float.Parse(offsetY.Text));
                }
                else {
                    stamper = new Stamper(keywordTxt.Text, float.Parse(offsetX.Text), float.Parse(offsetY.Text));
                }


                if (person != null)
                    remoteSign.Sign(Documents.Text, person, seal, stamper);
                else
                    remoteSign.Sign(Documents.Text, company, seal, stamper);
            }


        }

        private void RemoteSignQiyuesuo() {

            if (String.IsNullOrEmpty(Documents.Text))
            {
                showMessage("请指定文档ID");
            }

            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            IRemoteSignService remoteSign = new RemoteSignServiceImpl(client);

            try
            {
                remoteSign.Complete(Documents.Text);
                showMessage("合同签署成功！");
            }
            catch (Exception e) {
                showMessage("合同签署失败" + e.Message);
            }
        }

        private void DownLoadDoc_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Documents.Text)) {
                MessageBox.Show("请输入合同文档ID");
                return;
            }

            Stream stream = null;
            string filename;
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = "D:\\";
                sfd.Filter = "PDF文件(*.pdf)|*.pdf";
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                filename = sfd.FileName;

                stream = new FileStream(filename, FileMode.Create, FileAccess.Write);

                SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
                IRemoteSignService remoteSignService = new RemoteSignServiceImpl(client);

                remoteSignService.DownloadDoc(Documents.Text, ref stream);
            }
            catch (Exception e1)
            {
                showMessage("下载失败，" + e1.Message);
                return;
            }
            finally {
                if (stream != null)
                    stream.Close();
            }

            showMessage("合同文档下载完成，存放位置：" + filename);
            
        }

        private void SChooseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                SLocalFile.Text = file;
            }
        }

        private void SFile_CheckedChanged(object sender, EventArgs e)
        {
            SChooseFile.Enabled = true;
            SLocalFile.Enabled = true;
            STemplateId.Enabled = false;
            SParamers.Enabled = false;
            SAddParamer.Enabled = false;
            SDelParamer.Enabled = false;
        }

        private void STemplate_CheckedChanged(object sender, EventArgs e)
        {
            SChooseFile.Enabled = false;
            SLocalFile.Enabled = false;
            STemplateId.Enabled = true;
            SParamers.Enabled = true;
            SAddParamer.Enabled = true;
            SDelParamer.Enabled = true;

            SParamers.Items.Clear();
            string[] nameitem = { "name", "张三" };
            string[] mobileitem = { "mobile", "13262588888" };
            string[] moneyitem = { "money", "8000" };

            SParamers.Items.Add(new ListViewItem(nameitem));
            SParamers.Items.Add(new ListViewItem(mobileitem));
            SParamers.Items.Add(new ListViewItem(moneyitem));
        }

        private void SCreate_Click(object sender, EventArgs e)
        {
            string  documentId = null;
            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            IStandardSignService standardSign = new StandardSignServiceImpl(client);

            List<Receiver> receiveries = new List<Receiver>();
            Receiver receiver = new Receiver();
            receiver.name = "丁祥春";
            receiver.mobile = "18601324477";
            receiver.type = AccountType.PERSONAL;
            receiver.authLevel = AuthenticationLevel.FULL;
            receiver.ordinal = 1;

            receiveries.Add(receiver);


            if (SFile.Checked)
            {
                if (String.IsNullOrEmpty(SLocalFile.Text))
                {
                    MessageBox.Show("请输入本地合同文件的路径");
                    return;
                }

                if (!checkBeforeSign())
                    return;

                FileStream inputStream = new FileStream(SLocalFile.Text, FileMode.Open);

                try
                {
                    Dictionary<string, Stream> files = new Dictionary<string, Stream>();
                    files.Add(inputStream.Name, inputStream);
                    showMessage("开始创建合同");
                    documentId = standardSign.Create(inputStream,"C#标准签",receiveries);
                }
                catch (Exception e1)
                {
                    showMessage("创建合同失败，" + e1.Message);
                    return;
                }
                finally
                {
                    inputStream.Close();
                }
            }
            else {
                if (String.IsNullOrEmpty(STemplateId.Text))
                {
                    MessageBox.Show("请输入模板ID");
                    return;
                }

                if (!checkBeforeSign())
                    return;


                Dictionary<string, string> tplParamers = new Dictionary<string, string>();
                foreach (ListViewItem item in SParamers.Items)
                {
                    if (!String.IsNullOrEmpty(item.SubItems[0].Text) && !String.IsNullOrEmpty(item.SubItems[0].Text))
                        tplParamers.Add(item.SubItems[0].Text, item.SubItems[1].Text);
                }

                try
                {
                    showMessage("开始创建合同");
                    documentId = standardSign.Create(STemplateId.Text, tplParamers, "标准模板C#", receiveries, null);
                }
                catch (Exception e2)
                {
                    showMessage("创建合同失败，" + e2.Message);
                    return;
                }
            }


            string strmsg =" 文档ID："+documentId;

            SDocuments.Text = documentId;

            showMessage(strmsg);

        }

        private void SPlatformSign_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(SDocuments.Text)) {
                MessageBox.Show("请指定文档ID");
                return;
            }

            if (String.IsNullOrEmpty(sealId.Text)) {
                MessageBox.Show("请输入印章iD");
                return;
            }

            if (!checkBeforeSign())
                return;

            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);

            IStandardSignService standardSign = new StandardSignServiceImpl(client);

            Stamper stamper = null;
 
            if (keyword.Checked)
            {
                stamper=new Stamper(keywordTxt.Text, float.Parse(offsetX.Text), float.Parse(offsetY.Text));

            }
            else {
                stamper = new Stamper(int.Parse(pageNo.Text), float.Parse(offsetX.Text), float.Parse(offsetY.Text));
            }

            showMessage("开始签署合同");
            standardSign.Sign(SDocuments.Text,sealId.Text, stamper);
            showMessage("平台签署成功");
        }

        private void SDownDoc_Click(object sender, EventArgs e)
        {
            {
                if (String.IsNullOrEmpty(SDocuments.Text))
                {
                    MessageBox.Show("请输入合同文档ID");
                    return;
                }

                Stream stream = null;
                string filename;
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.InitialDirectory = "D:\\";
                    sfd.Filter = "PDF文件(*.pdf)|*.pdf";
                    if (sfd.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    filename = sfd.FileName;

                    stream = new FileStream(filename, FileMode.Create, FileAccess.Write);

                    SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
                    IStandardSignService standardSign = new StandardSignServiceImpl(client);

                    standardSign.DownloadDoc(SDocuments.Text, ref stream);
                }
                catch (Exception e1)
                {
                    showMessage("下载失败，" + e1.Message);
                    return;
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }

                showMessage("合同文档下载完成，存放位置：" + filename);
            }
        }

        private void SQueryStatus_Click(object sender, EventArgs e)
        {

            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            IStandardSignService standardSign = new StandardSignServiceImpl(client);

            Contract contract = standardSign.Detail(SDocuments.Text);

             showMessage(contract.subject);
        }

        private void SView_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(SDocuments.Text))
            {
                MessageBox.Show("请指定文档ID");
                return;
            }

            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);

            IStandardSignService standardSign = new StandardSignServiceImpl(client);

            string Url = standardSign.View(SDocuments.Text);

            System.Diagnostics.Process.Start(Url);
        }

        private void RDetail_Click(object sender, EventArgs e)
        {
            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            IRemoteSignService signService = new RemoteSignServiceImpl(client);

            if (String.IsNullOrEmpty(Documents.Text))
            {
                MessageBox.Show("请指定合同文档ID");
                return;
            }

           Contract contract =  signService.Detail(Documents.Text);

            showMessage("合同名称：" + contract.subject + "  合同状态：" + contract.status.ToString());
        }

        private void RSignUrl_Click(object sender, EventArgs e)
        {
            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            IRemoteSignService signService = new RemoteSignServiceImpl(client);

            if (String.IsNullOrEmpty(Documents.Text))
            {
                MessageBox.Show("请指定合同文档ID");
                return;
            }

            if (!user.Checked)
            {
                MessageBox.Show("签署页面仅支持用户签署！");
                return;
            }

            string strUrl = null;
            TabPage selectedPage = UserInfo.SelectedTab;
            if (selectedPage.Text.Equals("个人"))
            {
                Person person = getPersonInfo();
                if (person == null)
                    return;
                strUrl =  signService.SignUrl(Documents.Text, SignType.SIGNWITHPIN, person, "http://www.baidu.com");
            }
            else {
                Company company = getCompanyInfo();
                if (company == null)
                    return;

                strUrl =  signService.SignUrl(Documents.Text, SignType.SIGNWITHPIN, company, "http://www.baidu.com");
            }
            showMessage(strUrl);
            System.Diagnostics.Process.Start(strUrl);
        }

        private void RViewURL_Click(object sender, EventArgs e)
        {
            SDKClient client = new SDKClient(token.Text, secret.Text, url.Text);
            IRemoteSignService signService = new RemoteSignServiceImpl(client);

            if (String.IsNullOrEmpty(Documents.Text))
            {
                MessageBox.Show("请指定合同文档ID");
                return;
            }


            string strUrl = signService.ViewUrl(Documents.Text);
            showMessage(strUrl);
            System.Diagnostics.Process.Start(strUrl);
        }
    }
}
