using System;
using System.Collections.Generic;
using System.Text;

namespace Domen
{
    [Serializable]
    public class Zahtev
    {
        public string kombinacija { get; set; }
        public string rec { get; set; }
        public int brojReci { get; set; }
    }
}
