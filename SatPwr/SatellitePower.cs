using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frost.SatPwr
{
    class SatellitePower
    {
        private double[] temprature = new double[12]{3, 2.3, 2, 2.8, 5, 7, 10, 13.6, 11.2, 10, 6, 3};
        private double[] opticalIntensity = new double[12]{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        private double np = 1;
        private double ns = 1;

        public double CurrentTemprature { get; set; } //当前温度
        public double CuurentOpticalIntensity { get; set; } //当前光强系数
        public double OutputVolt { get; set; }
        public double Isc { get; set; } //短路电流
        public double Voc { get; set; } //开路电压
        public double Vmp { get; set; } //最佳功率点电压
        public double Imp { get; set; } //最佳功率点电流
        public double BatSize { get; set; } //太阳电池片尺寸

        public SatellitePower()
        { 
            BatSize = 32;
            Isc = BatSize * 0.0165;
            Voc = 2.6;
            Vmp = 2.276;
            Imp = BatSize * 0.01575;
            OutputVolt = Vmp;
        }

        public double GetOutputPower()
        {
            double current;
            double c1, c2;

            CurrentTemprature = temprature[System.DateTime.Now.Hour / 2];
            CuurentOpticalIntensity = opticalIntensity[System.DateTime.Now.Hour / 2];

            c2 = (Vmp/Voc -1) / Math.Log(Math.E, (1 - Imp/Isc));
            c1 = (1 - Imp/Isc) * Math.Exp(-Vmp/(c2 * Voc));
            current = np * Isc * (1 - c1 * (Math.Exp(OutputVolt / (c2 * ns * Voc)) - 1));

            return current * OutputVolt;
        }

    }
}
