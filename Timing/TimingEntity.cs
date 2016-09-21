using System;
using System.Collections.Generic;
using System.Text;

namespace Timing
{
    class TimingEntity
    {
        //链接地址
        private string url;
        //执行间隔
        private int interval;
        //执行状态(默认true)
        private bool status = true;
        //最后执行时间
        private string lasttime = "0000-00-00 00:00:00";

        public TimingEntity() { }
        public TimingEntity(string url,int interval){
            this.url = url;
            this.interval = interval;
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public bool Status
        {
            get { return status; }
            set { status = value; }
        }

        public string Lasttime
        {
            get { return lasttime; }
            set { lasttime = value; }
        }

    }
}
