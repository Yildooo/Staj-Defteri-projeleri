using System;
using System.Windows.Forms;

namespace MetinAnalizAraci
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAnaliz_Click(object sender, EventArgs e)
        {
            string metin = txtMetin.Text;

            int karakterSayisi = metin.Length;

            int kelimeSayisi = metin.Split(' ').Length;

            int cumleSayisi = metin.Split('.', '!', '?').Length - 1;

            lblKarakter.Text = karakterSayisi.ToString();
            lblKelime.Text = kelimeSayisi.ToString();
            lblCumle.Text = cumleSayisi.ToString();
        }
    }
}
