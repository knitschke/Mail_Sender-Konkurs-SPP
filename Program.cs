using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Mail_Sender_Konkurs
{
    class Program
    {
        //
        protected static void Write_to_file()
        {
            string email, pw, port, smtpserv, emailto;
            Console.WriteLine("SmtpServer:");
            smtpserv = Console.ReadLine();
            Console.WriteLine("Port:");
            port = Console.ReadLine();
            Console.WriteLine("Email wychodzący:");
            email = Console.ReadLine();
            Console.WriteLine("Hasło");
            pw = Console.ReadLine();
            Console.WriteLine("Email docelowy:");
            emailto = Console.ReadLine();
            //Console.WriteLine("Ścieżka do załączników:");
            //attach_path = Console.ReadLine();
            File.WriteAllText("konfiguracja.txt", $"{smtpserv};{port};{email};{pw};{emailto}");
        }
        private static void Read_file(ref List<string> file)
        {
            string temp = File.ReadAllText("konfiguracja.txt");
            string[] temp_devided = temp.Split(';');
            foreach (var item in temp_devided)
            {
                file.Add(item);
            }
        }

        static void send_and_delete(ref List<string> readfile)
        {
            int mail_number = 0;
            string[] fileEntries = Directory.GetFiles("../");
            
            foreach (string fileName in fileEntries)
            {
                MailMessage mail = new MailMessage();
                if (fileName.Split('_').Count() > 2)
                {
                    readfile[5] = $"{fileName}";
                    SmtpClient SmtpServer = new SmtpClient(readfile[0]);
                    mail.From = new MailAddress(readfile[2]);
                    mail.To.Add(readfile[4]);
                    mail.Subject = $"{fileName.Split('_')[1]} - Konkurs - \"Czujka tlenku węgla może uratować Twoje życie\"";
                    mail.Body = "Pdf z ankietą w załączniku.";

                    Attachment attachment;
                    attachment = new Attachment(readfile[5]);
                    mail.Attachments.Add(attachment);

                    SmtpServer.Port = int.Parse(readfile[1]);
                    SmtpServer.Credentials = new NetworkCredential(readfile[2], readfile[3]);
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                    mail_number++;
                    Console.WriteLine("Mail wysłany - " + fileName.Split('_')[1]);
                    mail.Attachments[0].Dispose();
                    mail.Attachments.Clear();
                    mail.Dispose();
                    mail = null;
                    File.Delete(fileName);
                    Console.WriteLine($"Plik usunięty - {fileName}");
                }
                
            }
            Console.WriteLine($"Wysłano {mail_number} maili");
            
        }

        static void Main(string[] args)
        {

            while (true)
            {
                List<string> readfile = new List<string>();
                while (true)
                {
                    Console.WriteLine("Konfiguracja:(c) -- Wyslanie maili:(s) -- Wyjście:(x)");

                    //0smtpserver;1port;2email-from;3password;4email-to;5path-to-attachments
                    var key = Console.ReadKey();
                    Console.WriteLine("");
                    Read_file(ref readfile);


                    switch (key.Key)
                    {
                        case ConsoleKey.X:
                            return;
                        case ConsoleKey.S:
                            try
                            {
                                send_and_delete(ref readfile);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            break;

                        case ConsoleKey.C:
                            Write_to_file();
                            break;
                    }
                }
            }

        }
    }
}
