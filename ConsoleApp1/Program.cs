using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Timers;

class UdpMessenger
{
    private static UdpClient udpClient;
    private static int listenPort;
    private static int sendPort;
    private static string sendAddress;

    //public static string usercount;

    static void Main(string[] args)
    {
        Console.Write("Введите свой порт для прослушивания: ");
        listenPort = int.Parse(Console.ReadLine());

        Console.Write("Введите порт для отправки сообщений: ");
        sendPort = int.Parse(Console.ReadLine());

        Console.Write("Введите IP-адрес получателя: ");
        sendAddress = Console.ReadLine();

        udpClient = new UdpClient(listenPort);
        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        Console.WriteLine("Введите сообщение для отправки (введите 'exit' для выхода):");

        while (true)
        {
            string message = Console.ReadLine();
            if (message.ToLower() == "exit")
                break;

            SendMessage(message);
            
        }

        udpClient.Close();
    }



    private static void WriteToLogFile(string message)
    {
        using (FileStream fstream = new FileStream($"logs.txt", FileMode.Append, FileAccess.Write))
        {
            byte[] buffer = Encoding.Default.GetBytes(message);
            fstream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
    private static void SendMessage(string message)
    {
        try
        {
            udpClient.Connect(sendAddress, sendPort);
            byte[] sendBytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(sendBytes, sendBytes.Length);
            Console.WriteLine($"Сообщение отправлено на {sendAddress}:{sendPort}: {message}");
            //WriteToLogFile($"Сообщение отправлено на {sendAddress}:{sendPort}:  Содержание сообщения: {message}  Дата отправки: {DateTime.Now.ToString()} \n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
            WriteToLogFile($"Ошибка при отправке сообщения: {ex.Message}  Дата ошибки: {DateTime.Now.ToString()} \n");
        }
    }

    private static void ReceiveMessages()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Console.WriteLine("Ожидание сообщений...");

        while (true)
        {
            try
            {
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
                Console.WriteLine($"Получено сообщение от {remoteEndPoint}: {receivedMessage}");
                WriteToLogFile($"Получено сообщение от {remoteEndPoint}: {receivedMessage}  Дата получения: {DateTime.Now.ToString()} \n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении сообщения: {ex.Message}");
                WriteToLogFile($"Ошибка при отправке сообщения: {ex.Message}  Дата ошибки: {DateTime.Now.ToString()} \n");
            }
        }
    }
}
