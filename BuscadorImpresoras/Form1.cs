using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuscadorImpresoras
{
    public partial class Form1 : Form
    {
        private SNMPHelper manager = new SNMPHelper();
        private string printersFilePath = "recursos\\impresoras.csv";
        private string oidsFilePath = "recursos\\oids.csv";

        public Form1()
        {
            InitializeComponent();
            LoadPrinters();
        }
        private void LoadPrinters()
        {
            manager.LoadPrintersFromCsv(printersFilePath);
            manager.LoadOidsFromCsv(oidsFilePath);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = manager.GetPrintersInfo();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            LoadPrinters();
        }
    }
}
