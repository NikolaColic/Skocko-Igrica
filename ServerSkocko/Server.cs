using Domen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace ServerSkocko
{
    public class Server
    {

        Socket osluskujuciSoket;
        List<Socket> listaSoketa = new List<Socket>();
        List<IgracSkocko> listaIgraca = new List<IgracSkocko>();


        public bool ZaustaviServer()
        {
            try
            {
                listaSoketa.ForEach((sok) => sok.Close());
                osluskujuciSoket.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool PokreniServer()
        {
            try
            {
                osluskujuciSoket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //listaSoketa.Add(osluskujuciSoket);
                IPEndPoint parametri = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9090);
                
                osluskujuciSoket.Bind(parametri);
                osluskujuciSoket.Listen(5);
                new Thread(Osluskuj).Start();

                return true;
            }
            catch (SocketException)
            {

                return false;
            }
        }

        public void Osluskuj()
        {
            bool kraj = false;
            while (!kraj)
            {
                try
                {
                    Socket klijentskiSoket = osluskujuciSoket.Accept();
                    new Thread(() => ObradiIme(klijentskiSoket)).Start();
                    
                }
                catch (SocketException)
                {
                    kraj = true;

                }
            }


        }
        public void ObradiIme(Socket klijentskiSoket)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            NetworkStream tok = new NetworkStream(klijentskiSoket);
            bool kraj = false;
            string ime = "";
            while (!kraj)
            {
                try
                {
                    ime = (string)formatter.Deserialize(tok);
                    if (ime is null || ime.Length <= 4) continue;
                    kraj = true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            IgracSkocko igrac = new IgracSkocko();
            igrac.Ime = ime;
            igrac.Soket = klijentskiSoket;
            listaIgraca.Add(igrac);
            Odgovor o = new Odgovor();
            o.broj = 1;
            o.Igrac = Igrac.Trazenje;
            o.Poruka = "Trazimo protivnika!";
            formatter.Serialize(tok, o);
            if (listaIgraca.Count == 2)
            {
                Obrada obrada = new Obrada(listaIgraca[0], listaIgraca[1]);
                Thread klijentNit = new Thread(obrada.ObradiZahtev);
                listaIgraca.Clear();
                klijentNit.Start();
            }

        }

        
    }
}
