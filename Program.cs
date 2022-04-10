using System;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Renci.SshNet.Common;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace SSH_easy_install
{
    class Program
    {
        static void Main(string[] args)
        {
            // Данные для входа
            string host = args[0];
            string username = "bsk";

            // Данные авторизации (Ключ)
            var keyFile = new PrivateKeyFile(@"1", "11111");
            var auntMethods = new AuthenticationMethod[] { new PrivateKeyAuthenticationMethod(username, keyFile) };

            Console.WriteLine($"---Настройка {host}---");

            // Общие данные авторизации
            var connectInfo = new ConnectionInfo(host, 22, username, auntMethods);

            // Создание клиента SFTP
            var sftpClient = new SftpClient(connectInfo);
            // Создание клиента SSH
            var sshClient = new SshClient(connectInfo);
            // Подключение sftp
            sftpClient.Connect();
            // Подключение ssh
            sshClient.Connect();
            Console.WriteLine("SSH и SFTP подключены");

            // Закидываем майнер
            using (Stream fileStream = File.OpenRead("11.7z"))
            {
                sftpClient.UploadFile(fileStream, "11.7z");
            }
            Console.WriteLine("Майнер перекинут");

            // Создание терминала
            var shell = sshClient.CreateShellStream(terminalName: "xterm-256color", rows: 150, columns: 150, width: 100, height: 100, bufferSize: 1000);
            Console.WriteLine("Терминал создан");
            // Установка 7z
            sshClient.RunCommand("sudo apt install p7zip-full --assume-yes");
            Console.WriteLine("7z установлен");
            // Установка screen
            sshClient.RunCommand("sudo apt install screen --assume-yes");
            Console.WriteLine("screen установлен");
            // Распаковка майнера
            sshClient.RunCommand("sudo 7z x 11.7z");
            Console.WriteLine("Майнер распакован");
            // Выдача прав майнеру
            sshClient.RunCommand("sudo chmod -R 777 11");
            Console.WriteLine("Выданы права майнеру");
            // Переход в папку с майнером
            shell.WriteLine("cd 11/");
            // Запуск майнера
            shell.WriteLine(@"screen -S main sudo ./start.sh");
            //Console.ReadKey();

            while (true)
            {
                Thread.Sleep(50);
                if (shell.ReadLine().Contains("cpuminer"))
                {
                    break;
                }
            }
            Console.WriteLine("Создан screen и запущен майнер");


            // Отключение sftp
            sftpClient.Disconnect();
            // Отключение ssh
            sshClient.Disconnect();
        }
    }
}
