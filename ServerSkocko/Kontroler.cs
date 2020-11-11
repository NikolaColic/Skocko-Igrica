using Domen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSkocko
{
    public class Kontroler
    {
        private static Kontroler instance; 

        private Kontroler()
        {

        }
        public static Kontroler Instance
        {
            get
            {
                if (instance is null) instance = new Kontroler();
                return instance;
            }
        }

        public Odgovor KreirajOdgovor(string poruka, int broj, Igrac igrac)
        {
            Odgovor o = new Odgovor();
            o.Igrac = igrac;
            o.broj = broj;
            o.Poruka = poruka;
            return o;

        }
        public string FormirajString(int rez1, int rez2, string ime1, string ime2)
        {
            return $"Rezultat:\n{ime1} : {rez1} \n{ime2} : {rez2}";
        }

        public string Provera(Zahtev z, ref int brojPoena, string komb)
        {
            string[] niz = z.kombinacija.Split(' ');
            niz[3] = z.rec.TrimEnd();
            brojPoena -= 2;
            int broj = 0;
            int proveraPogodjeno = ProveraPogodjeno(niz, komb);
            string tekst = ProveraTacno(niz, ref broj, komb) + "Nije pogodjeno mesto: " + (proveraPogodjeno - broj) + "\n";

            if (tekst.Contains("Gotova igra")) tekst = $"Pogodili ste! Cestitamo! \n";
            return tekst;
        }
        public  string Pobednik(int poenaPrvi, int poenaDrugi, string ime1, string ime2)
        {
            return poenaPrvi > poenaDrugi ? $"Pobednik je {ime1}" : poenaDrugi > poenaPrvi ?
                 $"Pobednik je {ime2}" : "Nereseno";
        }
        public int ProveraBrojText(int brojpoena2)
        {
            return brojpoena2 == 10 ? 1 : brojpoena2 == 8 ? 2 : brojpoena2 == 6 ? 3 : brojpoena2 == 4 ? 4 : brojpoena2 == 2 ? 5 : 5;
        }
        private string ProveraTacno(string[] niz, ref int broj, string komb)
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

        private int ProveraPogodjeno(string[] niz, string komb)
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

    }
}
