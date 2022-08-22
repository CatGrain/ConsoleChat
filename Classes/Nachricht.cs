using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatProgram
{
    [Serializable]
    public class Nachricht
    {
        public string Absender { get; set; }
        public string nachricht { get; set; }

        public Nachricht(string _Abseneder,string _Nachricht)
        {
            Absender = _Abseneder;
            nachricht = _Nachricht;
        }      

        public Nachricht()
        {

        }
    }  

    public static class NachrichtFormater
    {
        public static byte[] WandelNachrichtInBytArray(Nachricht nachricht)
        {         
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write(nachricht.nachricht);
            bw.Write(nachricht.Absender);

            return ms.ToArray();
        }

        public static Nachricht WandelBytArrayUm(byte[] bytArray)
        {
            MemoryStream ms = new MemoryStream(bytArray);
            BinaryReader br = new BinaryReader(ms);
            string absender = br.ReadString();
            string nachricht = br.ReadString();

            return new Nachricht(nachricht,absender);
        }

    }

    public class NachrichtArgs : EventArgs
    {
        public byte[] NachrichtInByte;

        public NachrichtArgs(byte[] _nachrichtInByte)
        {
            NachrichtInByte = _nachrichtInByte;
        }
    }

}
