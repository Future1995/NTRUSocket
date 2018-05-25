using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NTRUControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Socket clientSocket;
        private IPAddress ipAddress;
        private IPEndPoint endPoint;
        private NTRUEncryption encryption = new NTRUEncryption();
        private void btnConnect_Click(object sender, EventArgs e)
        {
            ipAddress = IPAddress.Parse(txtIP.Text);
            endPoint = new IPEndPoint(ipAddress, int.Parse(txtPort.Text));
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(endPoint);
            }
            catch (Exception)
            {
                MessageBox.Show("连接服务器失败");
            }

            btnConnect.Enabled = false;
            txtIP.Enabled = false;
            txtPort.Enabled = false;

            Task.Run(() =>
            {
                while (true)
                {
                    string recStr = "";
                    byte[] recBytes = new byte[4096];
                    int bytes = clientSocket.Receive(recBytes, recBytes.Length, 0);
                    recStr += Encoding.Default.GetString(recBytes, 0, bytes);
                    txtMsg.Text += recStr + "\r\n";
                }
            });
        }
    }
}
