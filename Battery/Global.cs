using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Frost.Battery
{
    class Global
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

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        public struct SolarPanelInfo
        {
            public double OutputPower;
            public int CurrentErrorId;
        };

        public static void ByteArrayToStructure(byte[] bytearray, ref object obj, int startoffset)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            // 从结构体指针构造结构体
            obj = Marshal.PtrToStructure(i, obj.GetType());
            try
            {
                // 将字节数组复制到结构体指针
                Marshal.Copy(bytearray, startoffset, i, len);
            }
            catch (Exception ex) { Console.WriteLine("ByteArrayToStructure FAIL: error " + ex.ToString()); }
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);  //释放内存，与 AllocHGlobal() 对应
        }
    }
}
