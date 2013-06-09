using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Frost.SatPwr
{
    class Global
    {
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

        public static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
    }
}
