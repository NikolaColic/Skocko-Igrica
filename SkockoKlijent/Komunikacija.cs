using Domen;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace SkockoKlijent
{
    public class Komunikacija
    {
        private Socket klijentskiSocket;
        private NetworkStream tok;
        private BinaryFormatter formatter = new BinaryFormatter();
        private static Komunikacija instance;
        public frmKlijent1 formaKlijent;

        private Komunikacija()
        {
            
        }
        public static Komunikacija Instance
        {
            get
            {
                if (instance is null) instance = new Komunikacija();
                return instance;
            }
        }
        public bool PoveziSe()
        {
            try
            {
                klijentskiSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                klijentskiSocket.Connect("localhost", 9090);
                tok = new NetworkStream(klijentskiSocket);
                return true;
            }
            catch (SocketException)
            {
                return false;
               
            }

        }
        public void PosaljiPoruku(string kombinacija, string tekst, int broj)
        {
            try
            {
                Zahtev z = KreirajZahtev(kombinacija, tekst, broj);
                formatter.Serialize(tok, z);
            }
            catch (Exception)
            {
                klijentskiSocket.Close();
                
                Environment.Exit(0);
            }
            
        }
        private Zahtev KreirajZahtev(string kombinacija, string tekst, int broj)
        {

            Zahtev z = new Zahtev();
            z.brojReci = broj;
            z.rec = tekst;
            z.kombinacija = kombinacija;
            return z;
        }

        public void PosaljiString (string tekst)
        {
            try
            {
                formatter.Serialize(tok, tekst);
            }
            catch (Exception)
            {
                klijentskiSocket.Close();
                Environment.Exit(0);
            }

        }
        public void PokreniNit()
        {
            new Thread(PrihvatiPoruku).Start();
        }

        private void PrihvatiPoruku()
        {
            bool kraj = false;
            
            while (!kraj)
            {
                try
                {
                    Odgovor odgovor = (Odgovor)formatter.Deserialize(tok);
                    if (odgovor.Poruka == "Protivnik je na potezu")
                    {
                        formaKlijent.VisibleFalse();
                        PozivFunkcija();
                    }
                    if (odgovor.Poruka == "Vi ste na potezu") 
                    {
                        formaKlijent.VisibleTrue();
                        PozivFunkcija();
                    }
                    switch (odgovor.Igrac)
                    {
                       case Igrac.Ja: formaKlijent.lblDodaj(odgovor.Poruka); break;
                       case Igrac.Protivnik: formaKlijent.DodajProtivnik(odgovor.Poruka); break;
                       case Igrac.Kombinacija: formaKlijent.PopuniTxt(odgovor.Poruka, odgovor.broj); break;
                       case Igrac.Trazenje: formaKlijent.DodajTrazenje(odgovor.Poruka); break;
                        case Igrac.Rezultat: formaKlijent.lblRezu(odgovor.Poruka); break; 
                    }
                }
                catch (Exception)
                {
                    klijentskiSocket.Close();
                    Environment.Exit(0);
                    kraj = true;
                }

            }
        }

        private void PozivFunkcija()
        {
            formaKlijent.DodajProtivnik("");
            formaKlijent.OcistiTxt();
            //formaKlijent.VisibleTxt();
            formaKlijent.VisiblePocetni();
        }
    }
}
