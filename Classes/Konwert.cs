using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer
{
    static class KonvertierungÜberprüfung
    {
        public static bool LästStringSichInIntKonvertieren(string text)
        {
            if (text.Length == 0)
            {
                return false;
            }

            int stringLänge = text.Length;

            for (int i = 0; i < stringLänge; i++)
            {
                char charAmIndex = text[i];

                if (!(charAmIndex >= 48 && charAmIndex <= 57))
                {
                    return false;
                }
            }

            return true;
        }        
    }
}
