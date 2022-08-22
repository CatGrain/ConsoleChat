using System;
// Allgemeine Funktionalität für Netzwerk-Programmierung
using System.Net;
// Funktionalitäten speziell für die Verbindungsverwaltung
using System.Net.Sockets;
// Funktionalitäten für die Prozess-Erstellung und -Verwaltung
using System.Threading;

namespace ChatProgram
{
    class Program
    {
        static void Main(string[] args)
        {            
            while (true)
            {              
                ChatRaum ChatRaum;

                BenutzerNachricht.ZeigeNachricht("Geben Sie einen Namen ein wie Sie im Chat genant werden möchten\nName:");
                string benutzerName = Console.ReadLine();                              
                ChatRaum = new ChatRaum(new Benutzer(benutzerName));

                while (true)
                {
                    BenutzerNachricht.ZeigeNachricht("Möchten Sie einen Chatraum beitreten oder einen neuen Chatraum erstelen ?\n\nZum erstellen eines Chats drücken Sie: N\n\nZum Beitreten eines Chats drücken Sie: J");
                    ConsoleKey benutzerEingabe = Console.ReadKey().Key;

                    if (benutzerEingabe == ConsoleKey.J)
                    {
                        ChatRaum.VerbindeMitChatRaum();
                        break;
                    }
                    else if (benutzerEingabe == ConsoleKey.N)
                    {
                        ChatRaum.ErsteleNeuenChatRaum();
                        break;
                    }
                }

                ChatRaum.Chat();                               
            }
        }
    }
}

