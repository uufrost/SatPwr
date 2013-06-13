using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Frost.Battery
{
    public partial class MainForm : Form
    {
        Battery battery;
        double loadPower; //负载功率
        UDPListen udpListen;
        System.Threading.Thread threadListen;
        Global.SolarPanelInfo solarPanelInfo;
        double solarOutputPower;
        int solarCurrentErrorId;
        IPAddress localIP = IPAddress.Any;
        const int localPort = 8002;
        int udpTimeout = 0;

        public MainForm()
        {
            InitializeComponent();
            try
            {
                foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = ip;
                        break;
                    }
                }
                udpListen = new UDPListen(new IPEndPoint(localIP, localPort));
                System.Threading.ThreadStart threadStartListen;

                threadStartListen = new System.Threading.ThreadStart(udpListen.open);
                udpListen.msgReceiptEvent += new msgReceiptHandler(listen_msgReceiptEvent);
                threadListen = new System.Threading.Thread(threadStartListen);
                threadListen.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBoxBatteryCount.SelectedItem = comboBoxBatteryCount.Items[0];
            battery = new Battery(Double.Parse(textBoxVolt.Text));
            buttonSetBattery.PerformClick();
        }

        private void listen_msgReceiptEvent(object sender, msgReceiptEventArg e)
        {
            this.Invoke(new ThreadUIHandle(ThreadUIHandleMethod), new object[] { e.data });
        }
        delegate void ThreadUIHandle(object data);
        void ThreadUIHandleMethod(object data)
        {
            object obj = solarPanelInfo;
            Global.ByteArrayToStructure((byte[])data, ref obj, 0);
            solarPanelInfo = (Global.SolarPanelInfo)obj;
            solarOutputPower = solarPanelInfo.OutputPower;
            solarCurrentErrorId = solarPanelInfo.CurrentErrorId;
            textBoxSolarOutputPower.Text = solarOutputPower.ToString("0.000000");
            udpTimeout = 0;
            textBoxSolarStatue.Text = "已连接";
            textBoxSolarStatue.BackColor = Color.Green;
            textBoxSolarStatue.ForeColor = Color.White;
        }

        private void buttonSetBattery_Click(object sender, EventArgs e)
        {
            double totalCapacity;
            double volt;

            totalCapacity = Int32.Parse(comboBoxBatteryCount.SelectedItem.ToString()) * Double.Parse(textBoxMaxCapacity.Text);
            battery.totalCapacity = totalCapacity;
            textBoxTotalCapacity.Text = totalCapacity.ToString();
            battery.CurrentCapacity = Double.Parse(textBoxCurrentCapacity.Text);
            volt = Double.Parse(textBoxVolt.Text);
            battery.Volt = volt;
        }

        private void buttonSetLoadPower_Click(object sender, EventArgs e)
        {
            loadPower = Double.Parse(textBoxLoadPower.Text);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadListen.Abort();
            udpListen.close();
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            udpTimeout++;
            if (udpTimeout > 2)
            {
                textBoxSolarStatue.Text = "未连接";
                textBoxSolarStatue.BackColor = Color.Red;
                textBoxSolarStatue.ForeColor = Color.White;
                solarOutputPower = 0;
            }
            double deltaPower;
            deltaPower = solarOutputPower - loadPower;
            if (deltaPower > 0)
            {
                if (battery.CurrentCapacity < battery.totalCapacity)
                {
                    textBoxBatteryStatue.Text = "正在充电";
                    battery.CurrentCapacity += 0.1;
                }
                else
                {
                    textBoxBatteryStatue.Text = "待机";
                }
            }
            else
            {
                if (battery.CurrentCapacity >= 0)
                {
                    textBoxBatteryStatue.Text = "正在放电";
                    battery.CurrentCapacity -= 0.1;
                }
                else
                {
                    textBoxBatteryStatue.Text = "电量耗尽";
                }
            }
            textBoxCurrentCapacity.Text = battery.CurrentCapacity.ToString();
        }
    }
}
