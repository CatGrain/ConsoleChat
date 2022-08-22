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
  
    public class MyTcpClient : TcpClient
    {
        public delegate void NachrichtEventHandler2(byte[] byt);
        public delegate void VerbindungGetrentEventHandler();

        public bool frageDatenAb = false;
        public bool Verbunden = true;
        public event VerbindungGetrentEventHandler VerbindungGetrent;

        Thread thread;

        public void OnVerbindungGetrent()
        {
            if(VerbindungGetrent != null)
            {
                VerbindungGetrent();
            }
        }

        public event NachrichtEventHandler2 Nachricht;
        public void OnNachricht(byte[] byt)
        {
            if (Nachricht != null)
            {
                Nachricht(byt);
            }
        }

        public void StartListen()
        {
            new Thread(new System.Threading.ThreadStart(Listen)).Start(); 
        }

        void Listen()
        {
            NetworkStream networkStream = GetStream();

            while (ÜberprüfeVerbindung())
            {
                if (networkStream.DataAvailable)
                {
                    LeseNachricht();
                }
            }

            OnVerbindungGetrent();
            Verbunden = false;
        }

        public bool ÜberprüfeVerbindung()
        {
            try
            {
                byte[] buffer = new byte[0];
                GetStream().Write(buffer);
                return true;
            }
            catch (Exception ex) {return false; }           
        }

        void LeseNachricht()
        {
            byte[] buffer = new byte[1024];
            NetworkStream networkStream = GetStream();
            int bytesReceived = networkStream.Read(buffer);
            Nachricht(buffer);
        }

        public void TrenneVerbindung()
        {           
            Close();
        }
    }
}
