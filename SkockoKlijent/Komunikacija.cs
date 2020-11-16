using Domen;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            Task.Run(() => PrihvatiPoruku());
        }

        private void PrihvatiPoruku()
        {
            bool kraj = false;
            while (!kraj)
            {
                try
                {
                    Odgovor odgovor = (Odgovor)formatter.Deserialize(tok);
                    ProtivnikPotez(odgovor.Poruka);
                    JaPotez(odgovor.Poruka);
                    
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

        private void ProtivnikPotez(string poruka)
        {
            if (poruka == "Protivnik je na potezu")
            {
                VremeIgre(false);
                formaKlijent.VisibleFalse();
                PozivFunkcija();
            }
        }
        private void JaPotez(string poruka)
        {
            if (poruka == "Vi ste na potezu")
            {
                formaKlijent.VisibleTrue();
                VremeIgre(true);
                PozivFunkcija();
            }
        }
        private void VremeIgre(bool potez)
        {
            Task.Run(async () =>
            {
                for (int i = 10; i >= 0; i--)
                {
                    await Task.Delay(1000);
                    if (i == 0 && potez) PosaljiPoruku("isteklo", "tekst", 1);
                    formaKlijent.lblVremeIgre(i);
                }
            });
        }

        private void PozivFunkcija()
        {
            formaKlijent.DodajProtivnik("");
            formaKlijent.OcistiTxt();
            formaKlijent.VisiblePocetni();
        }
    }
}
