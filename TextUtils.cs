using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsComApp
{
    class TextUtils
    {
        public static string ByteArrayToHex(byte[] bytes, int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                // 将每个字节转换为两位的十六进制数  
                sb.AppendFormat("{0:x2}", bytes[i]);
                if (i < bytes.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return new byte[0];
            }

            if (hex.Length % 2 != 0)
            {
                hex = 0 + hex;
            }

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((Convert.ToInt32(hex.Substring(i << 1, 2), 16)));
            }

            return arr;
        }
    }
}
