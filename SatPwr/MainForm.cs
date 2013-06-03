using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frost.SatPwr
{
    public partial class MainForm : Form
    {
        SatellitePower satellitePower;
        double outputPower;

        public MainForm()
        {
            InitializeComponent();

            timerMain.Interval = 1000;
            timerMain.Enabled = true;

            satellitePower = new SatellitePower();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            outputPower = satellitePower.GetOutputPower();
            textBoxOutputPower.Text = outputPower.ToString("0.000000");
            textBoxTemprature.Text = satellitePower.CurrentTemprature.ToString();
            textBoxOpticalIntensity.Text = satellitePower.CurrentOpticalIntensity.ToString();
        }

        private void buttonSetBatPara_Click(object sender, EventArgs e)
        {
            satellitePower.OutputVolt = Double.Parse(textBoxOutputVolt.Text);
            satellitePower.BatSize = Double.Parse(textBoxBatSize.Text);
            satellitePower.Isc = Double.Parse(textBoxIsc.Text) * satellitePower.BatSize;
            satellitePower.Voc = Double.Parse(textBoxVoc.Text);
            satellitePower.Imp = Double.Parse(textBoxImp.Text) * satellitePower.BatSize;
            satellitePower.Vmp = Double.Parse(textBoxVmp.Text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            buttonSetBatPara.PerformClick();
        }
    }
}
