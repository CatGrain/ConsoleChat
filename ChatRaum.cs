using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Allgemeine Funktionalität für Netzwerk-Programmierung
using System.Net;
// Funktionalitäten speziell für die Verbindungsverwaltung
using System.Net.Sockets;
// Funktionalitäten für die Prozess-Erstellung und -Verwaltung
using System.Threading;

namespace ChatProgram
{
    public class ChatRaum
    {       
        Client Client { get; set; }
        Server Server { get; set; }
        Benutzer Benutzer { get; set; }
        IPAddress ChatIP { get; set; }              
        int Port { get; set; }
        bool chatBeigetreten = true;
        

        public ChatRaum(Benutzer benutzer)
        {
            Benutzer = benutzer;
            Client = new Client();
            Client.Nachricht += ZeigeNachricht;
            Client.MyClient.VerbindungGetrent += SchließeChat;                     
        }

        public void ErsteleNeuenChatRaum()
        {
            int port = SetPort();
            Port = port;

            IPAddress iPAddress = RufeLokaleIPAb();
            ChatIP = iPAddress;

            chatBeigetreten = false;

            try
            {
                Server = new Server(port);
            }
            catch (Exception ex)
            {
                BenutzerNachricht.HalteNachricht("Server Start Fehlgeschlagen\n\n Fehler: " + ex);
                ErsteleNeuenChatRaum();
            }

            try
            {
                Client.VerbindeMitServer(iPAddress, port, Benutzer);
            }
            catch (Exception ex)
            {
                BenutzerNachricht.HalteNachricht("Chatraum erstelung Fehlgeschlagen\n\n Fehler: " + ex);
                ErsteleNeuenChatRaum();
            }           
        }

        public void VerbindeMitChatRaum()
        {
            int port = SetPort();
            IPAddress IP = SetIp();

            try
            {
                Client.VerbindeMitServer(IP, port, Benutzer);
            }
            catch (Exception ex)
            {
                BenutzerNachricht.HalteNachricht("Chatraum beitrit Fehlgeschlagen\n\n Fehler: " + ex);
                VerbindeMitChatRaum();
            }   
        }


        void ZeigeNachricht(Nachricht nachricht)
        {                      
            Console.WriteLine(nachricht.Absender + ": " + nachricht.nachricht);
        }

        void SendeNachricht(string nachricht)
        {
            Nachricht nueNachricht = new Nachricht(Benutzer.Name,nachricht);
            Client.Send(NachrichtFormater.WandelNachrichtInBytArray(nueNachricht));
        }

        public void Chat()
        {            
            if (chatBeigetreten)
            {
                BenutzerNachricht.ZeigeNachricht("Chat beigetreten\n\n\n");
            }
            else
            {
                BenutzerNachricht.ZeigeNachricht("Chat von " + Benutzer.Name + " Der Chat Kann beigetreten werden Über\n\nIPAdresse: " + ChatIP + "\nPort: " + Port + "\n\n\n");
            }

            Console.WriteLine("Chat kann Mit /Ende verlassen werden\n\n\n");

            while (Client.MyClient.Verbunden)
            {
                string nachricht = Console.ReadLine();
                SendeNachricht(nachricht);
            }
        }         
     
        
        void SchließeChat()
        {          
            if (Server != null)
            {
                Server.Close();
            }

            BenutzerNachricht.ZeigeNachricht("Verbindung zum Server Getrent Zum Fortfahren Enter Drüken");
        }

        IPAddress RufeLokaleIPAb()
        {
            string host = Dns.GetHostName();
            Console.WriteLine("Host: " + host);
            IPHostEntry iPHostEntry = Dns.GetHostByName(host);


            return iPHostEntry.AddressList[3];
        }

        int SetPort()
        {
            int port = 0;
            try
            {
                BenutzerNachricht.ZeigeNachricht("Geben Sie Bitte eine Gültige Portnumer ein\nPort: ");              
                port = Convert.ToInt32(Console.ReadLine());                
            }
            catch (Exception ex)
            {
                BenutzerNachricht.HalteNachricht("Es Können nur zahlen alls Portnumer eingegebn werden: \n" + "Fehler: " + ex.Message);
                Console.ReadKey();
                SetPort();
            }

            return port;
        }

        IPAddress SetIp()
        {
            IPAddress iP = null;
            try
            {
                BenutzerNachricht.ZeigeNachricht("Geben Sie Bitte Die Eine Gültige IPAdresse ein\nIP: ");
                iP = IPAddress.Parse(Console.ReadLine());
            }
            catch (Exception ex)
            {
                BenutzerNachricht.HalteNachricht("Ungültige IP Adresse: \n" + "Fehler: " + ex.Message);
                Console.ReadKey();
                SetIp();
            }

            return iP;
        }
    }
}
