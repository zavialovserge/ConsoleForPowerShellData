using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace ConsoleApp1
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
    internal class Program
    {
        
        static void Main(string[] args)
        {
            string script = @"C:\Work\12345.ps1";
            List<Product> product = GetDatafromPowerShellLib(script);
            List<Product> productInString = GetDataInStringFromProcess(script);
            List<Product> productInBytes = GetDataInBytesFromProcess(script);
        }


        public static List<Product> GetDatafromPowerShellLib(string scriptPath)
        {
            string script = File.ReadAllText(scriptPath);
            List<Product> products = new List<Product>();
            using (PowerShell powershell = PowerShell.Create().AddScript(scriptPath))
            {
                string errorMsg = string.Empty;
                PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
                powershell.Streams.Error.DataAdded += (object sender, DataAddedEventArgs e) =>
                {
                    errorMsg = ((PSDataCollection<ErrorRecord>)sender)[e.Index].ToString();
                };
                if (string.IsNullOrEmpty(errorMsg))
                {
                    foreach (PSObject result in powershell.Invoke())
                    {
                        Product product = new Product()
                        {
                            ID = (int)result.Members["Id"].Value,
                            Name = ((string)result.Members["Name"].Value).Trim(),
                            Description = ((string)result.Members["Description"].Value).Trim()
                        };
                        products.Add(product);
                    }
                }
            }
            return products;
        }
        private static List<Product> GetDataInBytesFromProcess(string script)
        {
            List<Product> products = new List<Product>();
            ProcessStartInfo processStartInfo = getProcessStartInfo(script);
            var powerShellOutputBytes = ExecuteProcessAndGetBytes(processStartInfo);
            string powerShellOutput = Encoding.UTF8.GetString(powerShellOutputBytes);
            return products;
        }
        private static ProcessStartInfo getProcessStartInfo(string script)
        {
            var scriptArguments = "-ExecutionPolicy Bypass -File \"" + script + "\"";
            var processStartInfo = new ProcessStartInfo("powershell.exe", scriptArguments);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            return processStartInfo;
        }
        private static List<Product> GetDataInStringFromProcess(string script)
        {
            ProcessStartInfo processStartInfo = getProcessStartInfo(script);
            List<Product> products = new List<Product>();
            using (Process process = Process.Start(processStartInfo))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                }
            }
            return products;
        }
        static byte[] ExecuteProcessAndGetBytes(ProcessStartInfo psi)
        {
            using (Process process = Process.Start(psi))
            {
                if (process != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        process.StandardOutput.BaseStream.CopyTo(memoryStream);

                        return memoryStream.ToArray();
                    }
                }
                else
                {
                    throw new InvalidOperationException("Failed to start process.");
                }
            }
        }
        
    }
}
