using System.Runtime.InteropServices;

namespace GrowRoomEnvironment.Web.ViewModels.Mappers
{
    public static class AutoMapperExtensions
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        public static bool IsEqualTo(this byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null)
                return false;
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        public static string GetString(this byte[] b1)
        {
            string value = string.Empty;
            foreach (byte b in b1)
                value += b.ToString("X2") + " ";
            return value.TrimEnd();
        }

    }
}
