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
using System.IO;


namespace ChatProgram
{
    public delegate void ClientNachrichtEventEventHandler(ServerNachrichtData serverNachrichtData);
    public delegate void ClientVerbindungsAbbruchEventHandler(Verbindung verbindung);

    public class Verbindung
    {               
        public MyTcpClient MyTcpClient;
        public bool blockEvent = false;
        public bool blockEventa2 = false;       

        public Verbindung(MyTcpClient Client)
        {         
            MyTcpClient = Client;          
            MyTcpClient.VerbindungGetrent += OnVerbindungAbbruch;
            MyTcpClient.Nachricht += OnNachricht; 
        }      

       
        public event ClientNachrichtEventEventHandler Nachricht;
        void OnNachricht(byte[] byt)
        {
            if (Nachricht != null)
            {
                ServerNachrichtData serverNachrichtData = new ServerNachrichtData(NachrichtFormater.WandelBytArrayUm(byt),this);
                Nachricht(serverNachrichtData);
            }
        }

        public event ClientVerbindungsAbbruchEventHandler VerbindungsAbbruch;
        void OnVerbindungAbbruch()
        {
            if (VerbindungsAbbruch != null && !blockEvent)
            {
                blockEvent = true;
                VerbindungsAbbruch(this);
            }
        }
    }
}
