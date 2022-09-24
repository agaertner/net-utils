using System.Runtime.InteropServices;

namespace agaertner.NetUtils
{
    public static class StructExtensions
    {
        public static bool GetBytes<T>(this T structure, out byte[] bytes) where T : struct
        {
            bytes = null;

            int size = Marshal.SizeOf(structure);
            byte[] arr = new byte[size];

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(structure, ptr, false);
                Marshal.Copy(ptr, arr, 0, size);
            }
            catch (SystemException)
            {
                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            bytes = arr;
            return true;
        }
    }
}
