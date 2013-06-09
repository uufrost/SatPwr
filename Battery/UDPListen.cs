using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Frost.Battery
{
    public class UDPListen
    {
        private string state = "close";

        public string State
        {
            get { return state; }
        }

        public event msgReceiptHandler msgReceiptEvent;

        IPEndPoint listenEndPoint;
        UdpClient client;

        IPEndPoint remoteEndPoint;

        bool con = true;

        public UDPListen(IPEndPoint ip)
        {
            listenEndPoint = ip;
            client = new UdpClient(listenEndPoint);
        }

        public void open()
        {
            if (con)
            {
                state = "open";
                try
                {
                    byte[] msg = client.Receive(ref remoteEndPoint);

                    msgReceiptEventArg e = new msgReceiptEventArg();
                    e.data = msg;
                    e.requestIP = remoteEndPoint.Address.ToString();
                    e.requestPoint = remoteEndPoint.Port.ToString();

                    if (msgReceiptEvent != null)
                    { 
                        msgReceiptEvent(this, e); 
                    }
                }
                catch
                {
                    close();
                }

                open();
            }
        }

        public void close()
        {

            con = false;
            client.Close();
            state = "close";
        }
    }

    public delegate void msgReceiptHandler(object sender, msgReceiptEventArg e);

    public class msgReceiptEventArg : System.EventArgs
    {
        public byte[] data;
        public string requestIP;
        public string requestPoint;
    }
}
