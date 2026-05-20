using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clientPractical
{
    public partial class Form1 : Form
    {
        Socket client;
        byte[] data = new byte[65536];
        public Form1()
        {
            InitializeComponent();
        }
        private void Log(string text)
        {
            if (richTextBox1.InvokeRequired)
                richTextBox1.Invoke(new Action(() =>
                    richTextBox1.AppendText(text + "\n")));
            else
                richTextBox1.AppendText(text + "\n");
        }
        private static int ReceiveAll(Socket socket, byte[] buffer, int length)
        {
            int total = 0;
            while (total < length)
            {
                int received = socket.Receive(
                    buffer, total, length - total, SocketFlags.None);

                if (received == 0)
                    throw new SocketException(
                        (int)SocketError.ConnectionReset);

                total += received;
            }
            return total;
        }
        private string ReceiveString()
        {
            int n = client.Receive(data);
            return Encoding.ASCII.GetString(data, 0, n);
        }
        private async void connect_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
                client = new Socket( AddressFamily.InterNetwork ,SocketType.Stream,ProtocolType.Tcp);
                await Task.Run(() => client.Connect(ipep));
                Log("Connected To Server");
                string welcome = await Task.Run(() => ReceiveString());
                Log(welcome);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connect error: " + ex.Message);
            }
        }
        private async void sendInfo_Click(object sender, EventArgs e)
        {
            try
            {
                string msg =
                    textBox1.Text + "|" +
                    textBox2.Text + "|" +
                    textBox3.Text + "|" +
                    textBox4.Text;

                byte[] info = Encoding.ASCII.GetBytes(msg);

                await Task.Run(() => client.Send(info));
                Log("Info Sent");
                string echo = await Task.Run(() => ReceiveString());
                Log("Server : " + echo);
                string done = await Task.Run(() => ReceiveString());
                Log(done);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Job1 error: " + ex.Message);
            }
        }
        private async void ready2_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() => client.Send(Encoding.ASCII.GetBytes("#ready2")));
                Log("Ready2 Sent");
                byte[] sizeBuffer = new byte[4];
                await Task.Run(() => ReceiveAll(client, sizeBuffer, 4));
                int imageSize = BitConverter.ToInt32(sizeBuffer, 0);
                Log("Image Size = " + imageSize);
                if (imageSize <= 0 || imageSize > 50 * 1024 * 1024)
                {
                    MessageBox.Show("Invalid image size: " + imageSize);
                    return;
                }
                byte[] imageData = new byte[imageSize];
                await Task.Run(() => ReceiveAll(client, imageData, imageSize));
                string path = Application.StartupPath + "\\received.jpg";
                await Task.Run(() => File.WriteAllBytes(path, imageData));
                Log("Image Received");
                await Task.Run(() =>
                    client.Send(Encoding.ASCII.GetBytes(path)));
                string done = await Task.Run(() => ReceiveString());
                Log(done);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Job2 error: " + ex.Message);
            }
        }
        private async void ready3_Click_1(object sender, EventArgs e)
        {
            try
            {
                client.Send(Encoding.ASCII.GetBytes("#ready3"));
                Log("Ready3 Sent");
                string msg = await Task.Run(() => ReceiveString());
                string[] parts = msg.Split(new char[] { '|' }, 2);
                string path = Application.StartupPath + parts[0];
                File.WriteAllText(path, parts[1]);
                Log("File Saved: " + path);
                client.Send(Encoding.ASCII.GetBytes(path));
                string done = await Task.Run(() => ReceiveString());
                Log(done);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Job3 error: " + ex.Message);
            }
        }
    }
}
