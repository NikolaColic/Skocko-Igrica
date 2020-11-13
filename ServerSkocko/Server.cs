using Domen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSkocko
{
    public class Server
    {

        Socket osluskujuciSoket;
        List<Socket> listaSoketa = new List<Socket>();
        List<IgracSkocko> listaIgraca = new List<IgracSkocko>();

        public object Dispacher { get; private set; }

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
                Task.Run(() => Osluskuj());
                //new Thread(Osluskuj).Start();

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
                    //new Thread(() => ObradiIme(klijentskiSoket)).Start();
                    Task.Run(() => ObradiIme(klijentskiSoket));

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
                    ime =  (string)formatter.Deserialize(tok);
                    if (ime is null || ime.Length <= 4) continue;
                    kraj = true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            IgracSkocko igrac =  (IgracSkocko) KreirajIgraca(ime, klijentskiSoket);
            listaIgraca.Add(igrac);
            Odgovor o = (Odgovor)KreirajOdgovor("Trazimo protivnika", 1, Igrac.Trazenje);
            formatter.Serialize(tok, o);

            if (listaIgraca.Count == 2)
            {
                Obrada obrada = new Obrada(listaIgraca[0], listaIgraca[1]);
                
                //Thread klijentNit = new Thread(obrada.ObradiZahtev);
                listaIgraca.Clear();
                var rezultat = Task.Run(async() => await obrada.ObradiZahtev());
                rezultat.ContinueWith((t) =>
                {
                    var rez = t.Result.ToString();
                    formatter.Serialize(tok,KreirajOdgovor(rez, 1, Igrac.Trazenje));
                });

                //klijentNit.Start();
            }

        }

        private object KreirajIgraca(string ime, Socket soket)
        {
            return new IgracSkocko()
            {
                Ime = ime,
                Soket = soket
            };
        }
        private object KreirajOdgovor(string poruka, int broj, Igrac igrac)
        {
            return new Odgovor()
            {
                Poruka = poruka,
                broj = broj,
                Igrac = igrac
            };
        }

    }
 }
