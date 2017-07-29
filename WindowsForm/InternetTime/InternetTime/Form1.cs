using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InternetTime
{
    public partial class internet_time : Form
    {
        public object LblLocalTime { get; private set; }

        public internet_time()
        {
            InitializeComponent();
            timer1.Start();
        }

        public static string get_time()
        {
                //  const string ntpServer = "pool.ntp.org";
                var ntp_incoming_data = new byte[48];
                ntp_incoming_data[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

                var addresses = System.Net.Dns.GetHostEntry("194.146.251.101").AddressList;
                var ip_point = new IPEndPoint(addresses[0], 123);
                var socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                socket.Connect(ip_point);
                socket.Send(ntp_incoming_data);
                socket.Receive(ntp_incoming_data);
                socket.Close();

                ulong int_part = (ulong)ntp_incoming_data[40] << 24 | (ulong)ntp_incoming_data[41] << 16 | (ulong)ntp_incoming_data[42] << 8 | (ulong)ntp_incoming_data[43];
                ulong fractal_part = (ulong)ntp_incoming_data[44] << 24 | (ulong)ntp_incoming_data[45] << 16 | (ulong)ntp_incoming_data[46] << 8 | (ulong)ntp_incoming_data[47];

                var milliseconds = (int_part * 1000) + ((fractal_part * 1000) / 0x100000000L);
                var npt_time = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

                TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(npt_time);
                npt_time = npt_time.AddHours(offset.Hours);
           
            return npt_time.ToString("HH:mm:ss");
        }

        private void internet_time_Load(object sender, EventArgs e)
        {
            
        }

        private void tick(object sender, EventArgs e)
        {
            time.Text = get_time();
        }
    }
}
