using System;

namespace Domen
{
    [Serializable]
    public class Odgovor
    {
        public string Poruka { get; set; }
        public int broj { get; set; }

        public Igrac Igrac { get; set; }
    }
    public enum Igrac
    {
        Protivnik, Ja, Kombinacija, Trazenje, Rezultat
    }

}
