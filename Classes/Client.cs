using System;
using System.Collections.Generic;
using System.Text;
// Allgemeine Funktionalität für Netzwerk-Programmierung
using System.Net;
// Funktionalitäten speziell für die Verbindungsverwaltung
using System.Net.Sockets;
// Funktionalitäten für die Prozess-Erstellung und -Verwaltung
using System.Threading;

namespace ChatProgram
{
    public delegate void ClientChatNachrichtEventHandler(Nachricht nachricht);

    class Client
    {
        public MyTcpClient MyClient { get; set; }                
        public event ClientChatNachrichtEventHandler Nachricht;
        

        public void OnNachricht(Nachricht nachricht)
        {
            if(Nachricht != null)
            {
                Nachricht(nachricht);
            }
        }

        public Client()
        {            
            MyClient = new MyTcpClient();
            MyClient.Nachricht += StartEmpfang;
        }
        
        public void VerbindeMitServer(IPAddress ipAddress, int Port,Benutzer benutzer)
        {           
            BenutzerNachricht.ZeigeNachricht("Verbinde mit Servrer...");
            MyClient.Connect(ipAddress, Port);
            MeldeNutzerBeitritAn(benutzer.Name);
            MyClient.StartListen();         
        }

        public void VerbindeMitServer(string ipAddress, int Port,Benutzer benutzer)
        {            
            BenutzerNachricht.ZeigeNachricht("Verbinde mit Servrer...");
            MyClient.Connect(ipAddress, Port);
            MeldeNutzerBeitritAn(benutzer.Name);
            MyClient.StartListen();           
        }
              
        void StartEmpfang(byte[] byt)
        {
            ParameterizedThreadStart ps = new ParameterizedThreadStart(EmpfangNachricht);
            new Thread(ps).Start(byt);
        }

        void MeldeNutzerBeitritAn(string Name)
        {
            Send(NachrichtFormater.WandelNachrichtInBytArray(new Nachricht(Name,"Ist Chat Beigetreten")));
        }

        void EmpfangNachricht(object data)
        {
            byte[] byt = data as byte[];
            OnNachricht(NachrichtFormater.WandelBytArrayUm(byt));
        }
      
        public void Send(byte[] nachricht)
        {
            if (MyClient.Verbunden)
            {
                byte[] buffer = nachricht;
                if (buffer != null)
                {
                    MyClient.GetStream().Write(buffer);
                }
            }
        }
    }
}
