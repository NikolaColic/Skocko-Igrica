using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerSkocko
{
    public partial class frmServer : Form
    {
        public frmServer()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Server s = new Server();
                s.PokreniServer();
                btnPokreni.Enabled = true;
                btnZaustavi.Enabled = false;
                MessageBox.Show("Uspesno ste zaustavili server!");
            }
            catch (Exception)
            {
                MessageBox.Show("Greska prilikom zaustavljanja servera!");
            }
        }

        private void btnPokreni_Click(object sender, EventArgs e)
        {
            try
            {
                Server s = new Server();
                s.PokreniServer();
                btnPokreni.Enabled = false;
                btnZaustavi.Enabled = true;
                MessageBox.Show("Uspesno ste pokrenuli server!");
            }
            catch (Exception)
            {
                MessageBox.Show("Greska prilikom pokretanja servera!");
            }
        }
    }
}
