using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frost.SatPwr
{
    public enum PowerError
    { 
        电池片开路,
        电池片参数变化,
        电池片短路,
        互连片开路,
        导线焊点开焊,
        接插件开路,
        接插件短路,
        隔离二极管开路,
        隔离二极管内阻变大
    }    
    
    class SatellitePower
    {
        private double[] temprature = new double[12]{2, 2.3, 2, 2.8, 5, 7, 10, 13.6, 11.2, 10, 6, 3};
        private double[] opticalIntensity = new double[12]{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        private double np = 1;
        private double ns = 1;



        public double CurrentTemprature { get; set; } //当前温度
        public double CurrentOpticalIntensity { get; set; } //当前光强系数
        public double OutputVolt { get; set; }
        public double Isc { get; set; } //短路电流
        public double Voc { get; set; } //开路电压
        public double Vmp { get; set; } //最佳功率点电压
        public double Imp { get; set; } //最佳功率点电流
        public double BatSize { get; set; } //太阳电池片尺寸
        public int CurrentErrorId { get; set; } //当前错误id

        public SatellitePower()
        { 
            BatSize = 32;
            Isc = BatSize * 0.0165;
            Voc = 2.6;
            Vmp = 2.276;
            Imp = BatSize * 0.01575;
            OutputVolt = Vmp;
            CurrentErrorId = -1;
        }

        public double GetOutputPower()
        {
            double current;
            double c1, c2;
            double rs = 1; //电池串联电阻
            double t0 = 5; //温度基准
            double s0 = 1; //太阳常数系数基准
            double deltaI, deltaV; //环境影响
            double fc = 0.01; //电流的温度系数
            double fv = 0.01; //电压的温度系数
            double vmpe, voce, isce, impe; //环境影响修正后的值

            CurrentTemprature = temprature[System.DateTime.Now.Hour / 2];
            CurrentOpticalIntensity = opticalIntensity[System.DateTime.Now.Hour / 2];

            //光强影响
            deltaI = (CurrentOpticalIntensity - s0) * Isc;
            deltaV = -deltaI * rs;

            //温度影响
            deltaI += fc * (CurrentTemprature - t0);
            deltaV += (fv + fc * rs) * (CurrentTemprature - t0);

            vmpe = Vmp + deltaV;
            voce = Voc + deltaV;
            isce = Isc + deltaV;
            impe = Imp + deltaV;

            c2 = (vmpe/voce -1) / Math.Log(Math.E, (1 - impe/isce));
            c1 = (1 - impe/isce) * Math.Exp(-vmpe/(c2 * voce));
            current = np * isce * (1 - c1 * (Math.Exp(OutputVolt / (c2 * ns * voce)) - 1));

            return current * OutputVolt;
        }

    }
}
