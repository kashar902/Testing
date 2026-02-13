using System.Runtime.InteropServices;
using System.Text;

namespace BloodConnect.Services.Helpers;

/// <summary>
/// Helper class for sending raw data to Windows printers
/// </summary>
public static class WindowsRawPrinter
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private class DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string? pDocName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string? pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)]
        public string? pDataType;
    }

    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

    [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    /// <summary>
    /// Send raw byte data to a Windows printer
    /// </summary>
    public static bool SendBytesToPrinter(string printerName, byte[] bytes)
    {
        IntPtr ptrUnmanagedBytes = IntPtr.Zero;
        IntPtr hPrinter = IntPtr.Zero;
        bool success = false;

        try
        {
            // Allocate unmanaged memory for bytes
            ptrUnmanagedBytes = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, ptrUnmanagedBytes, bytes.Length);

            // Open the printer
            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
            {
                return false;
            }

            // Start a document
            DOCINFOA di = new DOCINFOA
            {
                pDocName = "Blood Connect Print Job",
                pDataType = "RAW"
            };

            if (!StartDocPrinter(hPrinter, 1, di))
            {
                return false;
            }

            // Start a page
            if (!StartPagePrinter(hPrinter))
            {
                return false;
            }

            // Write bytes to the printer
            int bytesWritten;
            success = WritePrinter(hPrinter, ptrUnmanagedBytes, bytes.Length, out bytesWritten);

            // End the page
            EndPagePrinter(hPrinter);

            // End the document
            EndDocPrinter(hPrinter);
        }
        finally
        {
            // Clean up
            if (ptrUnmanagedBytes != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(ptrUnmanagedBytes);
            }

            if (hPrinter != IntPtr.Zero)
            {
                ClosePrinter(hPrinter);
            }
        }

        return success;
    }

    /// <summary>
    /// Send string data to a Windows printer
    /// </summary>
    public static bool SendStringToPrinter(string printerName, string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        return SendBytesToPrinter(printerName, bytes);
    }
}
