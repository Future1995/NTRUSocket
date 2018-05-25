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

namespace NTRUServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private NTRUEncryption encryption = new NTRUEncryption();
        private Socket serverSocket;
        private List<Socket> proxSocketList;
        private Task acceptTask;
        private void btnListen_Click(object sender, EventArgs e)
        {
            //置为不可用
            btnListen.Enabled = false;
            lbStatus.Text = "Listening......";
            txtIP.Enabled = false;
            txtPort.Enabled = false;
            btnStop.Enabled = true;
            try
            {
                proxSocketList = new List<Socket>();
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(txtIP.Text.Trim());
                int port = int.Parse(txtPort.Text.Trim());
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现异常{ex.Message}");
                return;
            }
            acceptTask = Task.Run(() =>
             {
                 while (true)
                 {
                     Socket proxSocket = serverSocket.Accept();
                     proxSocketList.Add(proxSocket);
                     this.Invoke(new Action(() =>
                     {
                         listboxLog.Items.Add("新的连接:" + proxSocket.RemoteEndPoint.ToString() + " \r\n");
                     }));

                     Task.Run(() =>
                     {
                         while (true)
                         {
                             try
                             {
                                 byte[] buffer = new byte[1024 * 1024];
                                 int realLenth = proxSocket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                                 string strResult = Encoding.Default.GetString(buffer, 0, realLenth);
                                 var message = encryption.Decryption(strResult);
                                 this.Invoke(new Action(() =>
                                 {

                                     listboxLog.Items.Add(proxSocket.RemoteEndPoint + ":" + message + Environment.NewLine);
                                 }));
                                 byte[] sendBuffer = Encoding.Default.GetBytes(strResult);
                                 foreach (var item in proxSocketList)
                                 {
                                     if (item != proxSocket)
                                     {
                                         item.Send(sendBuffer, 0, sendBuffer.Length, SocketFlags.None);
                                     }
                                 }
                             }
                             catch (SocketException)
                             {
                                 this.Invoke(new Action(() =>
                                 {
                                     listboxLog.Items.Add(proxSocket.RemoteEndPoint + "断开了连接\r\n");
                                 }));
                                 proxSocketList.Remove(proxSocket);
                                 proxSocket.Close();
                             }
                         }
                     });
                 }
             });
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            serverSocket.Close();
            acceptTask.Dispose();
            serverSocket = null;
            proxSocketList = new List<Socket>();

            btnListen.Enabled = true;
            lbStatus.Text = "Stop......";
            txtIP.Enabled = true;
            txtPort.Enabled = true;
            btnStop.Enabled = false;
        }
    }
}
