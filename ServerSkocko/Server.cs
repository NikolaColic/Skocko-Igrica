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
        List<IgracSkocko> listaIgraca = new List<IgracSkocko>();
        CancellationTokenSource token = null;
        public bool ZaustaviServer()
        {
            try
            {
                listaIgraca.ForEach((sok) => sok.Soket.Close());
                token.Cancel();
                token = null;
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
            IgracSkocko igrac = null;
            if (ime == "RESETUJ")
            {
                listaIgraca.ForEach((igrac2) =>
                {
                    if(igrac2.Soket == klijentskiSoket)
                    {
                        igrac = igrac2;
                    }
                });
            }
            igrac = igrac is null ? (IgracSkocko)KreirajIgraca(ime, klijentskiSoket) : igrac;
            listaIgraca.Add(igrac);
            Odgovor o = (Odgovor)KreirajOdgovor("Trazimo protivnika", 1, Igrac.Trazenje);
            formatter.Serialize(tok, o);

            if (listaIgraca.Count % 2 == 0)
            {
                token = new CancellationTokenSource();
                IgracSkocko prviIgrac = listaIgraca[0];
                IgracSkocko drugiIgrac = listaIgraca[1];
                Obrada obrada = new Obrada(listaIgraca[0], listaIgraca[1]);
                var rezultat = Task.Run(()=>obrada.ObradiZahtev(token.Token), token.Token);

                //Thread klijentNit = new Thread(obrada.ObradiZahtev);
                //listaIgraca.Clear();
                //var vreme = Task.Delay(5000);

                //var rez = await Task.WhenAny(rezultat, vreme);
                //if(rez ==vreme)
                //{
                //    formatter.Serialize(tok, KreirajOdgovor("Isteklo vreme", 1, Igrac.Ja));
                //}
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
