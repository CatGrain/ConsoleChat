using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProgram
{
    public static class BenutzerNachricht
    {
        public static void ZeigeNachricht(string nachricht)
        {
            Console.Clear();
            Console.Write(nachricht);
        }

        public static void HalteNachricht(string nachricht)
        {
            ZeigeNachricht(nachricht);
            Console.WriteLine("\n\nZum Fortfahrenbelibige Taste Drücken");
            Console.ReadKey();
        }
    }
}
