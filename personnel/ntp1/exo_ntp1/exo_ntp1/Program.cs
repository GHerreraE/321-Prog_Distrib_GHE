using System;
using System.Net;
using System.Net.Sockets;

namespace exo_ntp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (UdpClient client = new UdpClient())
            {

                string ntpServer = "0.ch.pool.ntp.org";

                byte[] timeMessage = new byte[48];
                timeMessage[0] = 0x1B; // type de message pour ntp (requete)

                // GetHostAddresses = retourne l'adresses IP lié au serveur (dans ce cas host)
                IPEndPoint ntpReference = new IPEndPoint(Dns.GetHostAddresses(ntpServer)[0], 123);

                // connection au serveur NTP
                client.Connect(ntpReference);

                // envoi de demande ntp au serveur
                client.Send(timeMessage, timeMessage.Length);

                // reponse ntp du serveur
                timeMessage = client.Receive(ref ntpReference);

                // convertion des données ntp to ToDateTime
                DateTime ntpTime = NtpPacket.ToDateTime(timeMessage);

                Console.WriteLine($"Heure actuelle : {ntpTime}");
                Console.WriteLine($"Heure actuelle (format personnalisé) : {ntpTime.ToString("dd/MM/yyyy HH:mm:ss")}");
                Console.WriteLine($"Heure actuelle (ISO 8601) : {ntpTime.ToString("yyyy-MM-ddTHH:mm:ssZ")}");

                // Calculate time difference (both in UTC)
                DateTime ntpTimeUtc = ntpTime; // Already UTC from NTP
                DateTime systemTimeUtc = DateTime.UtcNow;
                TimeSpan timeDiff = systemTimeUtc - ntpTimeUtc;
                Console.WriteLine($"Différence de temps : {timeDiff.TotalSeconds:F2} secondes");

                // Convert to local time zone properly
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(ntpTimeUtc, TimeZoneInfo.Local);
                Console.WriteLine($"Heure locale : {localTime}");

                // Convert to specific time zones
                TimeZoneInfo swissTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
                DateTime swissTime = TimeZoneInfo.ConvertTimeFromUtc(ntpTimeUtc, swissTimeZone);
                Console.WriteLine($"Heure suisse : {swissTime}");

                // Convert to time zone Suisse
                TimeZoneInfo utcTimeZone = TimeZoneInfo.Utc;
                DateTime backToUtc = TimeZoneInfo.ConvertTime(localTime, TimeZoneInfo.Local, utcTimeZone);
                Console.WriteLine($"Retour vers UTC : {backToUtc}");


           
            }

            

            
        }
    }
}
