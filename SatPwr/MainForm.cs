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
        int startTime;
        double outputPower;
        int timerMainCounter = 0;
        int errorCounter = 0;
        Random rd = new Random();

        public MainForm()
        {
            InitializeComponent();

            timerMain.Interval = 1000;
            timerMain.Enabled = false;

            satellitePower = new SatellitePower();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            if (timerMainCounter % 5 == 0)
            {
                satellitePower.CurrentErrorId = rd.Next(0, Enum.GetNames(typeof(PowerError)).Length);
                ListViewItem li = new ListViewItem();
                li.SubItems.Clear();
                li.SubItems[0].Text = errorCounter++.ToString();
                li.SubItems.Add(DateTime.Now.ToString());
                li.SubItems.Add(Enum.Parse(typeof(PowerError), satellitePower.CurrentErrorId.ToString()).ToString());
                li.SubItems.Add("未处理");
                listViewError.Items.Add(li);
                listViewError.Items[(listViewError.Items.Count) - 1].EnsureVisible();
                timerDealError.Enabled = true;
            }
            timerMainCounter++;
            outputPower = satellitePower.GetOutputPower();
            textBoxOutputPower.Text = outputPower.ToString("0.000000");
            textBoxTemprature.Text = satellitePower.CurrentTemprature.ToString();
            textBoxOpticalIntensity.Text = satellitePower.CurrentOpticalIntensity.ToString();

            int simulateTime = Environment.TickCount - startTime;
            int tickSecond = simulateTime / 1000;
            int tickDay = tickSecond / 60 / 60 / 24;
            int tickHour = (tickSecond / 60 / 60) % 24;
            int tickMinute = (tickSecond / 60) % 60;
            textBoxSimulateTime.Text = String.Format("{0:00}:{1:00} :{2:00}", tickHour, tickMinute, tickSecond % 60);
        }

        private void timerDealError_Tick(object sender, EventArgs e)
        {
            switch(satellitePower.CurrentErrorId)
            {
                case 0:
                case 4:
                case 5:
                case 7:
                    listViewError.Items[(listViewError.Items.Count) - 1].SubItems[3].Text = "已连另外的电池单元";
                    break;
                case 6:
                case 8:
                    listViewError.Items[(listViewError.Items.Count) - 1].SubItems[3].Text = "警告";
                    break;
                case 2:
                    listViewError.Items[(listViewError.Items.Count) - 1].SubItems[3].Text = "警告，已停止模拟";
                    toggleButtonStart.PerformClick();
                    break;
                case 1:
                case 3:
                    listViewError.Items[(listViewError.Items.Count) - 1].SubItems[3].Text = "已分流";
                    break;
            }
            timerDealError.Enabled = false;
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

        private void toggleButtonStart_Click(object sender, EventArgs e)
        {
            timerMain.Enabled = !timerMain.Enabled;
            toggleButtonStart.ToggleState = timerMain.Enabled;
            if (timerMain.Enabled == true)
            {
                startTime = Environment.TickCount;
            }
        }

    }
}
