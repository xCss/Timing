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
            seconds++;
            foreach (var entity in list)
            {
                if (entity.Interval == -1) {
                    continue;
                }
                if (seconds % entity.Interval == 0 && entity.Status)
                {
                    try{
                        httpRequest(entity);
                        entity.Lasttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    }catch (WebException we){

                        entity.Status = false;
                        entity.Lasttime = we.Message;
                        continue;
                    }finally{
                        this.dgvTiming.Refresh();
                        var k = list.FindIndex(v1=>v1==entity);
                        list[k] = entity;
                    }
                }
            }
        }

        /// <summary>
        /// 封装请求
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static string httpRequest(TimingEntity entity) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(entity.Url);
            request.Method = entity.Method;
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=utf-8";
            if (entity.Method.ToUpper() == "POST"){
                byte[] buffer = Encoding.ASCII.GetBytes(entity.Postdata);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //判断编码格式
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8";
            }
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(encoding));
            var responseString = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            stream.Close();
            stream.Dispose();
            response.Close();
            return responseString;
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
                loadConfig();

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
                        entity.Url = reader.ReadElementString().Trim();
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
                    if (reader.Name == "method") {
                        entity.Method = reader.ReadElementString().Trim();
                    }
                    if (reader.Name == "postdata") {
                        entity.Postdata = reader.ReadElementString().Trim();
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
                writer.WriteElementString("method", entity.Method);
                writer.WriteElementString("postdata", entity.Postdata);
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
