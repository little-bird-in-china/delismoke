using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BlueStone.Smoke.Msite.Untility
{
    public class StringUtility
    {
        public static byte[] HexStringToBytes(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
            {
                return null;
            }
            if (hexString.Length % 2 == 1)
            {
                throw new ArgumentException("参数不是有效的16进制字符串", "hexString");
            }
            int length = hexString.Length / 2;
            byte[] buffer = new byte[length];

            for (int i = 0; i < length; i++)
            {
                buffer[i] = Byte.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);// Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return buffer;
        }

        public static string BytesToHexString(byte[] buffer)
        {
            if (buffer == null) return null;
            StringBuilder hexBuilder = new StringBuilder();
            foreach (byte b in buffer)
            {
                hexBuilder.Append(b.ToString("X2"));
            }
            return hexBuilder.ToString();
        }
    }
}
