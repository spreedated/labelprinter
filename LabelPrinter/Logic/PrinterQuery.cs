using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace LabelWhisper.Logic
{
    public static class PrinterQuery
    {
        internal static readonly string[] blacklistedPrinterNames = [
                "pdf",
                "onenote",
                "xps",
                "nul"
            ];

        internal static readonly string[] blacklistedPrinterDrivers = [
                "xps"
            ];

        internal static readonly string[] blacklistedPrinterPorts = [
                "nul"
            ];

        public static IEnumerable<string> GetInstalledPrinters()
        {
            if (OperatingSystem.IsWindows())
            {
                ManagementObjectSearcher searcher = new("SELECT * FROM Win32_Printer");

                foreach (ManagementObject printer in searcher.Get().Cast<ManagementObject>())
                {
                    string name = printer["Name"]?.ToString();
                    string port = printer["PortName"]?.ToString();
                    string driver = printer["DriverName"]?.ToString();

                    //todo: here!
                    if (string.IsNullOrEmpty(name) && (blacklistedPrinterNames.Contains(name.ToLower()) || blacklistedPrinterDrivers.Contains(driver.ToLower()) || blacklistedPrinterPorts.Contains(port.ToLower())))
                    {
                        continue;
                    }

                    yield return printer["Name"]?.ToString();
                }
            }
            else if (OperatingSystem.IsLinux())
            {
                Process process = new()
                {
                    StartInfo = new()
                    {
                        FileName = "lpstat",
                        Arguments = "-e",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        yield return line.Trim();
                    }
                }

                process.WaitForExit();
            }
            else
            {
                yield return "Unsupported OS";
            }
        }
    }
}
