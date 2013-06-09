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
        }

        private void buttonSetBattery_Click(object sender, EventArgs e)
        {
            double totalCapacity;
            double volt;

            totalCapacity = Int32.Parse(comboBoxBatteryCount.SelectedItem.ToString()) * Double.Parse(textBoxMaxCapacity.Text);
            battery.totalCapacity = totalCapacity;
            textBoxTotalCapacity.Text = totalCapacity.ToString();

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
    }
}
