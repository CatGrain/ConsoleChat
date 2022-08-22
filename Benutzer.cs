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
    public class Benutzer
    { 
        public string Name;

        public Benutzer(string _Name)
        {
            Name = _Name;
            
        }
    }
}
