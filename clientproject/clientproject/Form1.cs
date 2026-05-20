using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;

namespace clientproject
{
    public partial class Form1 : Form
    {
        Socket client;

        byte[] buffer = new byte[65536];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void connect_Click(object sender, EventArgs e)
        {
            try
            {
                client = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                client.Connect("127.0.0.1", 5000);

                richTextBox1.AppendText(
                    "Connected To Server\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog op =
                new OpenFileDialog();

            if (op.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = op.FileName;
            }
        }

        private void ReceiveAll(byte[] data, int size)
        {
            int total = 0;

            while (total < size)
            {
                int r = client.Receive(
                    data,
                    total,
                    size - total,
                    SocketFlags.None);

                total += r;
            }
        }

        private void send_Click(object sender, EventArgs e)
        {
            try
            {
                string path = textBox1.Text;

                if (!File.Exists(path))
                {
                    MessageBox.Show(
                        "Choose File First");

                    return;
                }

                byte[] fileData =
                    File.ReadAllBytes(path);

                byte[] size =
                    BitConverter.GetBytes(
                        fileData.Length);

                client.Send(size);

                client.Send(fileData);

                richTextBox1.AppendText(
                    "File Sent ✔\n");

                byte[] compressedSize =
                    new byte[4];

                ReceiveAll(compressedSize, 4);

                int sizeCompressed =
                    BitConverter.ToInt32(
                        compressedSize, 0);

                richTextBox1.AppendText(
                    "Compressed Size: " +
                    sizeCompressed + "\n");

                byte[] compressedData =
                    new byte[sizeCompressed];

                ReceiveAll(
                    compressedData,
                    sizeCompressed);

                string savePath =
                    "compressed_file.gz";

                File.WriteAllBytes(
                    savePath,
                    compressedData);

                richTextBox1.AppendText(
                    "Compressed File Saved ✔\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            try
            {
                client?.Close();
            }
            catch { }

            richTextBox1.AppendText(
                "Client Closed\n");
        }
    }
}