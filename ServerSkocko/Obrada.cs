using Domen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace ServerSkocko
{
    public class Obrada
    {
        //Socket igrac1;
        //Socket igrac2;
        IgracSkocko igrac1;
        IgracSkocko igrac2;
        private NetworkStream tok1;
        private NetworkStream tok2;
        private BinaryFormatter formatter;
        public string komb = "Zvezda Karo Zvezda Karo";
        int brojPoena = 10;
        int poenaPrvi = -5;
        int poenaDrugi = -5;

        //public Obrada(Socket igrac1, Socket igrac2)
        //{
        //    this.igrac1 = igrac1;
        //    this.igrac2 = igrac2;
        //    tok2 = new NetworkStream(this.igrac2);
        //    tok1 = new NetworkStream(this.igrac1);
        //    formatter = new BinaryFormatter();
        //}

        public Obrada(IgracSkocko igrac1, IgracSkocko igrac2)
        {
            this.igrac1 = igrac1;
            this.igrac2 = igrac2; 
            tok2 = new NetworkStream(this.igrac2.Soket);
            tok1 = new NetworkStream(this.igrac1.Soket);
            formatter = new BinaryFormatter();
        }

        public void ObradiZahtev()
        {
            PosaljiOba(KreirajOdgovor("Igrac pocinje!", 1, Igrac.Ja));
            bool kraj = false;
            bool specPoruka = false;
            int brojac = 0;
            while (!kraj)
            {
                try
                {
                    if (!specPoruka)
                    {
                        Thread.Sleep(2000);
                        PosaljiPrvi(KreirajOdgovor("Vi ste na potezu", 1, Igrac.Ja));
                        PosaljiDrugi(KreirajOdgovor("Protivnik je na potezu", 1, Igrac.Ja));
                        if(brojac ==0)
                        {
                            PosaljiOba(KreirajOdgovor(FormirajString(0,0), 1, Igrac.Rezultat));
                        }
                        specPoruka = true;
                    }
                    Zahtev poruka = (Zahtev)formatter.Deserialize(tok1);
                    if (poruka.rec == "resetuj")
                    {
                        brojPoena = 10;
                        continue;
                    }
                    PosaljiDrugi(KreirajOdgovor(poruka.kombinacija, ProveraBrojText(brojPoena), Igrac.Kombinacija));

                    string tekst = Provera(poruka);
                    PosaljiPrvi(KreirajOdgovor(tekst,1, Igrac.Ja));
                    PosaljiDrugi(KreirajOdgovor("Protivnik je ostvario sledeci rezultat:\n" + tekst, 1, Igrac.Protivnik));

                    if (tekst.Contains("Cestitamo") || brojPoena == 0)
                    {
                        tok1 = new NetworkStream(igrac2.Soket);
                        tok2 = new NetworkStream(igrac1.Soket);





                        //ovo je komentar
                        specPoruka = false;
                        brojac++;
                        if (brojac < 2)
                        {
                            poenaPrvi = brojPoena + 2;
                            PosaljiOba(KreirajOdgovor(FormirajString(poenaPrvi, 0), 1, Igrac.Rezultat));


                        }


                        if (brojac == 2)
                        {
                            Thread.Sleep(3000);
                            poenaDrugi = brojPoena +2;
                            PosaljiOba(KreirajOdgovor($"Gotova igra prijatelji dragi!\n{Pobednik()} ", 1, Igrac.Protivnik));
                            PosaljiOba(KreirajOdgovor(FormirajString(poenaPrvi, poenaDrugi), 1, Igrac.Rezultat));
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
                }
            }

        }

        private string FormirajString(int rez1, int rez2)
        {
            return $"Rezultat:\n{igrac1.Ime} : {rez1} \n{igrac2.Ime} : {rez2}";
        }
        private string Pobednik()
        {
            return poenaPrvi > poenaDrugi ? $"Pobednik je {igrac1.Ime}" : poenaDrugi > poenaPrvi ?
                 $"Pobednik je {igrac2.Ime}" : "Nereseno";
        }
        private int ProveraBrojText(int brojpoena2)
        {
            return brojpoena2 == 10 ? 1 : brojpoena2 == 8 ? 2 : brojpoena2 == 6 ? 3 : brojpoena2 == 4 ? 4 : brojpoena2 == 2 ? 5 : 5;
        }
        private string Provera(Zahtev z)
        {
            string[] niz = z.kombinacija.Split(' ');
            niz[3] = z.rec.TrimEnd();
            brojPoena -= 2;
            int broj = 0;
            int proveraPogodjeno = ProveraPogodjeno(niz);
            string tekst = ProveraTacno(niz, ref broj) + "Nije pogodjeno mesto: " + (proveraPogodjeno - broj) +"\n";
                
            if (tekst.Contains("Gotova igra")) tekst = $"Pogodili ste! Cestitamo! \n";
            return tekst;
        }
        private string ProveraTacno(string[] niz, ref int broj)
        {
            string[] nizKomb = komb.Split(' ');
            int brojac = 0;
            for (int i = 0; i <= 3; i++)
            {
                if (niz[i] == nizKomb[i]) brojac++;
            }
            broj = brojac; 
            return brojac == 4 ? "Na pravom mestu je : " + brojac + "\n Gotova igra!" : "Na pravom mestu je : " + brojac + "\n";
        }

        private int ProveraPogodjeno(string[] niz)
        {
            string[] niz2 = niz.Distinct().ToArray();
            string[] nizKomb = komb.Split(' ');
            string[] nizKomb2 = nizKomb.Distinct().ToArray();
            int brojac = 0;
            for (int i = 0; i < niz2.Length; i++)
            {
                for (int j = 0; j < nizKomb2.Length; j++)
                {
                    if (niz2[i] == nizKomb2[j])
                    {
                        int kombBroj = nizKomb.Count((s) => s == niz2[i]);
                        int pokusajBroj = niz.Count((s) => s == niz2[i]);
                        brojac += (kombBroj > pokusajBroj) ? pokusajBroj : kombBroj;
                    }

                }
            }

            return brojac;
        }

        //Slanje poruka
        private void PosaljiOba(Odgovor o)
        {
            formatter.Serialize(tok1, o);
            formatter.Serialize(tok2, o);
        }
        private void PosaljiPrvi(Odgovor o)
        {
            formatter.Serialize(tok1, o);
        }
        private void PosaljiDrugi(Odgovor o)
        {
            formatter.Serialize(tok2, o);
        }

        private Odgovor KreirajOdgovor(string poruka, int broj, Igrac igrac)
        {
            Odgovor o = new Odgovor();
            o.Igrac = igrac;
            o.broj = broj;
            o.Poruka = poruka;
            return o;
            
        }

    }
}
