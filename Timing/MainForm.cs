using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Net;
using System.Windows.Forms;
using System.IO;


namespace Timing
{
    public partial class MainForm : Form
    {
        private int seconds = 0;
        private string XMLconfig = "./config.xml";
        System.Timers.Timer timer = null;
        List<TimingEntity> list = new List<TimingEntity>();

        public MainForm()
        {
            InitializeComponent();
            try{
                //自动加载配置文件
                loadConfig();
            }
            catch (Exception e){
                MessageBox.Show("自动加载config.xml失败，请点击\"加载列表\"手动选择配置文件", "温馨提醒");
                this.btnLoad.Visible = true;
            }
        }

        //开始执行
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (list.Count == 0) return;
            //实例化Timer类，设置间隔时间；
            if (timer == null) {
                timer = new System.Timers.Timer();
                timer.Interval = 1000;
                //到达时间的时候执行事件；
                timer.Elapsed += new System.Timers.ElapsedEventHandler(doRequest);
                //设置是执行一次（false）还是一直执行(true)；
                timer.AutoReset = true;
                //是否执行System.Timers.Timer.Elapsed事件；
                timer.Enabled = true;
            }
            timer.Start();
            this.btnStart.Enabled = false;
            this.btnStop.Enabled = true;
        }


        //停止执行
        private void btnStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Close();
            timer = null;

            this.btnStart.Enabled = true;
            this.btnStop.Enabled = false;
        }

        //执行请求
        public void doRequest(object sender,System.Timers.ElapsedEventArgs e) {
            //MessageBox.Show(time.ToString(),"温馨提醒");
            seconds++;
            foreach (var entity in list){
                if (seconds % entity.Interval == 0 && entity.Status){
                    var request = (HttpWebRequest)WebRequest.Create(entity.Url);
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    entity.Lasttime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    this.dgvTiming.Refresh();
                    //list.Add(entity);
                    //MessageBox.Show(responseString, "温馨提醒");
                }
            }
        }


        /**
         *  加载自定义配置文件
         *  
         **/
        private void btnLoad_Click(object sender, EventArgs e)
        {
            list.Clear();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XML文件|*.xml|配置文件|*.config|所有文件|*.*";
            dialog.InitialDirectory = "./";
            dialog.RestoreDirectory = true;
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() == DialogResult.OK) {
                XMLconfig = dialog.FileName;
                //MessageBox.Show(XMLconfig);

                XmlTextReader reader = new XmlTextReader(@XMLconfig);

                TimingEntity entity = new TimingEntity();
                //读取xml
                while (reader.Read()){
                    if (reader.NodeType == XmlNodeType.Element) {
                        if (reader.Name == "url") {
                            entity.Url = reader.ReadElementString().Trim();
                            MessageBox.Show(entity.Url);
                        }
                        if (reader.Name == "interval") {
                            entity.Interval = Convert.ToInt32(reader.ReadElementString().Trim());
                        }
                        if (reader.Name == "status") {
                            entity.Status = Convert.ToBoolean(reader.ReadElementString().Trim());
                        }
                        if (reader.Name == "lasttime") {
                            entity.Lasttime = reader.ReadElementString().Trim();
                        }
                    }
                    if (reader.NodeType == XmlNodeType.EndElement) {
                        list.Add(entity);
                        entity = new TimingEntity();
                    }
                }
                list.RemoveAt(list.Count - 1);
                BindingSource bindsource = new BindingSource();
                bindsource.DataSource = list;
                this.dgvTiming.DataSource = bindsource;
                this.dgvTiming.AllowUserToAddRows = true;
                this.dgvTiming.AllowUserToDeleteRows = true;

                reader.Close();

            }

            if (list.Count > 0) {
                this.btnSave.Enabled = true;
                this.btnStart.Enabled = true;
            }
        }

        //加载配置文件
        private void loadConfig() {
            list.Clear();
            XmlTextReader reader = new XmlTextReader(@XMLconfig);

            TimingEntity entity = new TimingEntity();
            //读取xml
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "url")
                    {
                        //entity.Url = reader.ReadElementString().Trim();
                        entity.Url = reader.ReadElementContentAsString();
                    }
                    if (reader.Name == "interval")
                    {
                        entity.Interval = Convert.ToInt32(reader.ReadElementString().Trim());
                    }
                    if (reader.Name == "status")
                    {
                        entity.Status = Convert.ToBoolean(reader.ReadElementString().Trim());
                    }
                    if (reader.Name == "lasttime")
                    {
                        entity.Lasttime = reader.ReadElementString().Trim();
                    }
                }
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    list.Add(entity);
                    entity = new TimingEntity();
                }
            }
            list.RemoveAt(list.Count - 1);
            BindingSource bindsource = new BindingSource();
            bindsource.DataSource = list;
            this.dgvTiming.DataSource = bindsource;
            this.dgvTiming.AllowUserToAddRows = true;
            this.dgvTiming.AllowUserToDeleteRows = true;

            reader.Close();

            if (list.Count > 0){
                this.btnSave.Enabled = true;
                this.btnStart.Enabled = true;
            }

        }

        //保存修改后的配置文件
        private void btnSave_Click(object sender, EventArgs e){

            if (list.Count == 0) {
                MessageBox.Show("对不起，保存失败，列表为空!", "温馨提醒");
                return;
            }
            XmlTextWriter writer = new XmlTextWriter(@XMLconfig, null);
            //设置缩进
            writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument(false);

            //创建root元素
            writer.WriteStartElement("links");

            for (int i = 0; i < list.Count; i++) {
                TimingEntity entity = list[i];
                //创建子元素
                writer.WriteStartElement("link");
                writer.WriteElementString("url", entity.Url);
                writer.WriteElementString("interval", entity.Interval.ToString());
                writer.WriteElementString("status", entity.Status.ToString());
                writer.WriteElementString("lasttime", entity.Lasttime);
                //子元素结束
                writer.WriteEndElement();
            }

            //root元素结束
            writer.WriteEndElement();

            writer.Flush();
            writer.Close();

            MessageBox.Show("恭喜你，保存成功！","温馨提醒");


        }

    }
}
