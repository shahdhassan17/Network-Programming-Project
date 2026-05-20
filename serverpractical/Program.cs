using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        Socket server = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);

        IPEndPoint ip = new IPEndPoint(IPAddress.Any, 5000);

        server.Bind(ip);
        server.Listen(1);

        Console.WriteLine("Waiting for client...");

        Socket client = server.Accept();

        Console.WriteLine("Client Connected");

        // =========================
        // JOB 1
        // =========================

        string msg =
            "Send SeatID|Name|Department|Section";

        client.Send(Encoding.ASCII.GetBytes(msg));

        byte[] data = new byte[4096];

        int recv = client.Receive(data);

        string student =
            Encoding.ASCII.GetString(data, 0, recv);

        Console.WriteLine(student);

        string[] parts = student.Split('|');

        string confirm =
            "Student Data OK, Seat Id is "
            + parts[0]
            + ", Name is "
            + parts[1]
            + ", dept is "
            + parts[2]
            + ", section is "
            + parts[3];

        client.Send(Encoding.ASCII.GetBytes(confirm));

        client.Send(
            Encoding.ASCII.GetBytes("job1 done"));

        // =========================
        // JOB 2
        // =========================

        recv = client.Receive(data);

        string ready2 =
            Encoding.ASCII.GetString(data, 0, recv);

        if (ready2 == "#ready2")
        {
            Console.WriteLine("Client ready for Job2");

            // send image
            string imagePath = "test.jpg";

            byte[] imageBytes =
                File.ReadAllBytes(imagePath);

            // send image size
            byte[] size =
                BitConverter.GetBytes(imageBytes.Length);

            client.Send(size);

            // send image
            client.Send(imageBytes);

            Console.WriteLine("Image sent");

            // receive saved path
            recv = client.Receive(data);

            string savedPath =
                Encoding.ASCII.GetString(data, 0, recv);

            Console.WriteLine(savedPath);

            client.Send(
                Encoding.ASCII.GetBytes("job2 done"));
        }

        // =========================
        // JOB 3
        // =========================

        recv = client.Receive(data);

        string ready3 =
            Encoding.ASCII.GetString(data, 0, recv);

        if (ready3 == "#ready3")
        {
            Console.WriteLine("Client ready for Job3");

            string fileName = "note.txt";

            string content =
                "Welcome Shahd Hassan";

            string packet =
                fileName
                + "|"
                + content.Length
                + "|"
                + content;

            client.Send(
                Encoding.ASCII.GetBytes(packet));

            Console.WriteLine("Text file sent");
        }

        client.Close();
        server.Close();
    }
}