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
    public delegate void ServerStartFehlerEventHandler();
    class Server
    {
        int Port { get; set; }     
        TcpListener Listener { get; set; }
        
        List<Verbindung> Verbindungen = new List<Verbindung>();
        bool serverActive = false;
        bool serverSend = false;

        public event ServerStartFehlerEventHandler ServerStartFehler;
        public void OnServerStartFehler()
        {
            if(ServerStartFehler != null)
            {
                ServerStartFehler();
            }
        }

        public Server(int _Port)
        {       
            Port = _Port;
            Listener = new TcpListener(IPAddress.Any,Port);
            serverActive = true;
            Listener.Start();
            new Thread(new ThreadStart(this.Start)).Start();           
        }


        void Start()
        {
            Console.WriteLine("Verbinde mit Server...");

            while (serverActive)
            {
                if (Listener.Pending())
                {                   
                    while (serverSend)
                    { }
                    MyTcpClient Client = new MyTcpClient();
                    Client.Client = Listener.AcceptTcpClient().Client;
                    Client.StartListen();
                    Verbindung verbindung = new Verbindung(Client);
                    verbindung.Nachricht += StartLesen;
                    verbindung.VerbindungsAbbruch += StartClientVerbindungsTrenung;                                                     
                    Verbindungen.Add(verbindung);
                }
            }

            Listener.Stop();
        }

        void StartClientVerbindungsTrenung(Verbindung verbindung)
        {
            ParameterizedThreadStart ps = new ParameterizedThreadStart(TrenneClientVerbindung);
            new Thread(ps).Start(verbindung);
        }
        void TrenneClientVerbindung(object verbindungData)
        {
            Verbindung verbindung = (Verbindung)verbindungData;
            Verbindungen.Remove(verbindung);            
            StartSendServerNachricht("Nutzer Hat Chat Verlassen");           
        }

        void StartLesen(ServerNachrichtData data)
        {
            ParameterizedThreadStart ps = new ParameterizedThreadStart(Lese);
            new Thread(ps).Start(data);
        }

        void Lese(object verbindungData)
        {
            ServerNachrichtData serverData = (ServerNachrichtData)verbindungData;
            Verbindung verbindung = serverData.Verbindung;
            Nachricht nachricht = serverData.Nachricht;
            
            
                      
            verbindung.blockEventa2 = false;

            if (nachricht.nachricht.Length > 0  && nachricht.nachricht[0] == '/')
            {
                ParameterizedThreadStart ps = new ParameterizedThreadStart(ServerComandReader);
                new Thread(ps).Start(serverData);
            }
            else
            {
                ParameterizedThreadStart ps = new ParameterizedThreadStart(SendChatNachricht);
                new Thread(ps).Start(serverData);
            }
        }

        void SendChatNachricht(object dataClint)
        {                  
            ServerNachrichtData serverNachricht =  (ServerNachrichtData)dataClint;
            Verbindung verbindung = serverNachricht.Verbindung;
            Nachricht nachricht = serverNachricht.Nachricht;

            serverSend = true;

            foreach (var item in Verbindungen)
            {               
                if (item != verbindung)
                {
                    ServerNachrichtData serverNachrichtData = new ServerNachrichtData(nachricht,item);
                    StartSend(serverNachrichtData);
                }
            }

            serverSend = false;
        }
       
        void StartSend(ServerNachrichtData serverNachrichtData)
        {
            ParameterizedThreadStart ps = new ParameterizedThreadStart(Send);
            new Thread(ps).Start(serverNachrichtData);
        }
        
        void Send(object nachrichtData)
        {            
            ServerNachrichtData data = (ServerNachrichtData)nachrichtData;

            MyTcpClient empfänger = data.Verbindung.MyTcpClient;
            Nachricht nachricht = data.Nachricht;

            if (empfänger.Verbunden)
            {
                byte[] buffer = new byte[1024];
                buffer = NachrichtFormater.WandelNachrichtInBytArray(nachricht);
                empfänger.GetStream().Write(buffer);
            }                     
        }

        void ServerComandReader(object serverNachrichtData)
        {
            ServerNachrichtData data = (ServerNachrichtData)serverNachrichtData;

            Verbindung verbindung = data.Verbindung;
            Nachricht nachricht = data.Nachricht;

            string comand = nachricht.nachricht;
            

            Nachricht nachrichtServer = new Nachricht("Server","");

            switch (comand)
            {
                case "/End":
                    verbindung.MyTcpClient.TrenneVerbindung();
                    break;

                default:
                    StartSendServerNachricht("Unbekanterbefehl");
                    break;
            }
        }

        void StartSendServerNachricht(string nachricht)
        {       
            ParameterizedThreadStart ps = new ParameterizedThreadStart(SendServerNachrichtAll);
            new Thread(ps).Start(new Nachricht("Server", nachricht));
        }

        void StartSendServerNachricht(string nachricht,Verbindung verbindung)
        {
            ParameterizedThreadStart ps = new ParameterizedThreadStart(SendServerNachricht);
            new Thread(ps).Start(new ServerNachrichtData(new Nachricht("Server", nachricht),verbindung));
        }

        void SendServerNachrichtAll(object nachrichtData)
        {
            Nachricht nachricht = (Nachricht)nachrichtData;

            serverSend = true;

            foreach (var item in Verbindungen)
            {
                ServerNachrichtData serverNachrichtData = new ServerNachrichtData(nachricht,item);
                StartSend(serverNachrichtData);
            }

            serverSend = false;
        }
        void SendServerNachricht(object nachrichtData)
        {
            ServerNachrichtData serverNachricht = (ServerNachrichtData)nachrichtData;             
            StartSend(serverNachricht);          
        }

        public void Close()
        {
            serverActive = false;       
            
            foreach (var item in Verbindungen)
            {
                item.MyTcpClient.TrenneVerbindung();
            }           
        }
    }


    public struct ServerNachrichtData
    {
        public Nachricht Nachricht;
        public Verbindung Verbindung;

        public ServerNachrichtData(Nachricht nachricht,Verbindung verbindung)
        {
            Nachricht = nachricht;
            Verbindung = verbindung;
        }
    }
        
}
