using System;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkockoKlijent
{
    public partial class frmKlijent1 : Form
    {
        public frmKlijent1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        public string komb = "Zvezda Karo Zvezda Karo";
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        public void lblDodaj(string tekst)
        {
            lblRezultat.Text = tekst;
        }
        public void lblVremeIgre(int broj)
        {
            if (broj == 0)
            {
                lblVreme.Text = "Isteklo vreme igre!";
                return;
            }
            lblVreme.Text = broj + "";
        }
        public void lblRezu(string tekst)
        {
            lblRez.Text = tekst;
        }
      


        private TextBox ProveraTextBox(string tekst)
        {
            int brojReci = 0;
            bool znak = false;
            if (!ProveraTacno(txtPrva, tekst, ref brojReci, ref znak))
            {
                if (brojReci < 3 && !znak) txtPrva.Text += tekst;
                return txtPrva;
            }
            if (!ProveraTacno(txtDruga, tekst, ref brojReci, ref znak))
            {
                if (brojReci < 3 && !znak) txtDruga.Text += tekst;
                return txtDruga;
            }
            if (!ProveraTacno(txtTreca, tekst, ref brojReci, ref znak))
            {
                if (brojReci < 3 && !znak) txtTreca.Text += tekst;
                return txtTreca;
            }

            if (!ProveraTacno(txtCetvrta, tekst, ref brojReci, ref znak))
            {
                if (brojReci < 3 && !znak) txtCetvrta.Text += tekst;
                return txtCetvrta;
            }
            if (!ProveraTacno(txtPeta, tekst, ref brojReci, ref znak))
            {
                if (brojReci < 3 && !znak) txtPeta.Text += tekst;
                return txtPeta;
            }
            if (!lblRezultat.Text.Contains("GOTOVA")) lblRezultat.Text = "Gotova je igra! Niste uspeli da pogodite";
            return null;

        }

        private bool ProveraTacno(TextBox txt, string rec, ref int brojReci, ref bool znak)
        {

            if (BrojSpace(txt, ref brojReci))
            {
                if (brojReci == 4) return true;
                if (brojReci == 3)
                {
                    txt.Text += rec;
                    znak = true;
                }
                Komunikacija.Instance.PosaljiPoruku(txt.Text, rec, brojReci);
                return true;
            }
            return false;
        }



        private bool BrojSpace(TextBox txt, ref int brojReci)
        {
            brojReci = txt.Text.Count(s => s == ' ');
            return txt.Text.Count(s => s == ' ') >= 3 ? true : false;
        }



       
        private void btnZvezda_Click_1(object sender, EventArgs e)
        {
            TextBox t = ProveraTextBox("Zvezda ");

            if (t is null) return;
        }

        private void btnSkocko_Click_1(object sender, EventArgs e)
        {
            TextBox t = ProveraTextBox("Skocko ");
            if (t is null) return;

        }

        private void btnTref_Click_1(object sender, EventArgs e)
        {
            TextBox t = ProveraTextBox("Tref ");
            if (t is null) return;
        }

        private void btnKaro_Click_1(object sender, EventArgs e)
        {
            TextBox t = ProveraTextBox("Karo ");
            if (t is null) return;
        }

        private void btnPik_Click_1(object sender, EventArgs e)
        {
            TextBox t = ProveraTextBox("Pik ");
            if (t is null) return;
        }

        private void btnHerc_Click_1(object sender, EventArgs e)
        {
            TextBox t = ProveraTextBox("Herc ");
            if (t is null) return;
        }

        private void frmKlijent1_Load(object sender, EventArgs e)
        {
            try
            {
                if (Komunikacija.Instance.PoveziSe())
                {
                    Komunikacija.Instance.PokreniNit();
                    Komunikacija.Instance.formaKlijent = this;
                }
                else
                {
                    MessageBox.Show("Server je ugasen!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Neuspesna konekcija na server! ");
            }
        }

        private void btnResetuj_Click_1(object sender, EventArgs e)
        {

            txtPrva.Text = "";
            txtDruga.Text = "";
            txtTreca.Text = "";
            txtCetvrta.Text = "";
            txtPeta.Text = "";
            Komunikacija.Instance.PosaljiPoruku("", "RESETUJ", 1);
        }

        public void VisibleFalse()
        {
            btnHerc.Visible = false;
            btnKaro.Visible = false;
            btnPik.Visible = false;
            btnSkocko.Visible = false;
            btnZvezda.Visible = false;
            btnTref.Visible = false;
        }
        public void VisibleTrue()
        {
            btnHerc.Visible = true;
            btnKaro.Visible = true;
            btnPik.Visible = true;
            btnSkocko.Visible = true;
            btnZvezda.Visible = true;
            btnTref.Visible = true;
        }

        public void VisibleTxt()
        {
            txtPrva.Visible = true;
            txtDruga.Visible = true;
            txtTreca.Visible = true;
            txtCetvrta.Visible = true;
            txtPeta.Visible = true;
            btnResetuj.Visible = true; 
        }
        public void VisiblePocetni()
        {
            lblIme.Visible = false;
            txtIme.Visible = false;
            lblTrazenje.Visible = false;
            btnIme.Visible = false;

        }
        public void DodajTrazenje(string tekst)
        {
            lblTrazenje.Text = tekst; 
        }

        public void DodajProtivnik(string tekst)
        {
            lblProtivnik.Text = tekst; 
        }

        private void frmKlijent1_Leave(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        public void PopuniTxt (string tekst, int broj)
        {
            if (broj == 1) txtPrva.Text = tekst;
            else if (broj == 2) txtDruga.Text = tekst;
            else if (broj == 3) txtTreca.Text = tekst;
            else if (broj == 4) txtCetvrta.Text = tekst;
            else txtPeta.Text = tekst; 
        }
        public void OcistiTxt()
        {
            txtPrva.Text = "";
            txtDruga.Text = "";
            txtTreca.Text = "";
            txtCetvrta.Text = "";
            txtPeta.Text = "";
        }

        private void btnIme_Click(object sender, EventArgs e)
        {
            string tekst = txtIme.Text; 
            if(tekst.Length <= 4)
            {
                lblTrazenje.Text = "Pogresno ime";
                return; 
            }
            lblTrazenje.Text = ""; 
            Komunikacija.Instance.PosaljiString(tekst);
        }

        private void txtPrva_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDruga_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTreca_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCetvrta_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPeta_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

