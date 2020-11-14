using Domen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSkocko
{
    public class Obrada
    {
        
        IgracSkocko igrac1;
        IgracSkocko igrac2;
        private NetworkStream tok1;
        private NetworkStream tok2;
        private BinaryFormatter formatter;
        public string komb = "Zvezda Karo Zvezda Karo";
        int brojPoena = 10;
        
        public Obrada(IgracSkocko igrac1, IgracSkocko igrac2)
        {
            this.igrac1 = igrac1;
            this.igrac2 = igrac2;
            this.igrac1.Poena = -5;
            this.igrac2.Poena = -5;
            tok2 = new NetworkStream(this.igrac2.Soket);
            tok1 = new NetworkStream(this.igrac1.Soket);
            formatter = new BinaryFormatter();
            
        }

        public void ObradiZahtev(CancellationToken token)
        {

            PosaljiOba(Kontroler.Instance.KreirajOdgovor("Igrac pocinje!", 1, Igrac.Ja));
            bool kraj = false;
            bool specPoruka = false;
            int brojac = 0;
            while (!kraj)
            {
                try
                {
                    if (token.IsCancellationRequested )
                    {
                        return;
                    }
                    if (!specPoruka)
                    {
                        Thread.Sleep(2000);
                        PosaljiJednom(tok1,Kontroler.Instance.KreirajOdgovor("Vi ste na potezu", 1, Igrac.Ja));
                        PosaljiJednom(tok2,Kontroler.Instance.KreirajOdgovor("Protivnik je na potezu", 1, Igrac.Ja));
                        if(brojac ==0) PosaljiOba(Kontroler.Instance.KreirajOdgovor(Kontroler.Instance.FormirajString(0, 0, igrac1.Ime, igrac2.Ime), 1, Igrac.Rezultat));
                        specPoruka = true;
                    }
                    Zahtev poruka = (Zahtev)formatter.Deserialize(tok1);
                    PosaljiJednom(tok2, Kontroler.Instance.KreirajOdgovor(poruka.kombinacija, Kontroler.Instance.ProveraBrojText(brojPoena), Igrac.Kombinacija));

                    string tekst = Kontroler.Instance.Provera(poruka, ref brojPoena, komb);
                    PosaljiJednom(tok1,Kontroler.Instance.KreirajOdgovor(tekst,1, Igrac.Ja));
                    PosaljiJednom(tok2,Kontroler.Instance.KreirajOdgovor("Protivnik je ostvario sledeci rezultat:\n" + tekst, 1, Igrac.Protivnik));

                    if (tekst.Contains("Cestitamo") || brojPoena == 0)
                    {
                        tok1 = new NetworkStream(igrac2.Soket);
                        tok2 = new NetworkStream(igrac1.Soket);
                        specPoruka = false;
                        brojac++;
                        if (brojac < 2)
                        {
                            igrac1.Poena = brojPoena + 2;
                            PosaljiOba(Kontroler.Instance.KreirajOdgovor(Kontroler.Instance.FormirajString(igrac1.Poena, 0, igrac1.Ime, igrac2.Ime), 1, Igrac.Rezultat));
                        }

                        if (brojac == 2)
                        {
                            Thread.Sleep(3000);
                            igrac2.Poena = brojPoena +2;
                            PosaljiOba(Kontroler.Instance.KreirajOdgovor($"Gotova igra prijatelji dragi!\n{Kontroler.Instance.Pobednik(igrac1.Poena, igrac2.Poena, igrac1.Ime, igrac2.Ime)} ", 1, Igrac.Protivnik));
                            PosaljiOba(Kontroler.Instance.KreirajOdgovor(Kontroler.Instance.FormirajString(igrac1.Poena, igrac2.Poena, igrac1.Ime, igrac2.Ime), 1, Igrac.Rezultat));
                            return;
                        }
                        brojPoena = 10;
                    }
                }
                catch (Exception)
                {
                    kraj = true;
                    igrac1.Soket.Close();
                    igrac2.Soket.Close();
                    return;
                }
            }
           
        }

       
        private void PosaljiOba(Odgovor o)
        {
            formatter.Serialize(tok1, o);
            formatter.Serialize(tok2, o);
        }
        private void PosaljiJednom(NetworkStream tok,Odgovor o)
        {
            formatter.Serialize(tok, o);
            
        }
       

       

    }
}
