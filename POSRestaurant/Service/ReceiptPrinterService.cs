using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using ESCPOS_NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Service
{
    public class ReceiptPrinterService : IReceiptPrinterService
    {
        public void PrintAsync(string receiptData)
        {
            //NetworkPrinterSettings ps = new NetworkPrinterSettings();

            //var printer = new NetworkPrinter("192.168.0.100", 9100); // Replace with your printer's IP
            //var e = new EPSON();

            //// Convert receiptData to ESC/POS commands
            //var commands = ByteSplicer.Combine(
            //    e.CenterAlign(),
            //    e.PrintLine("Store Name"),
            //    e.PrintLine("-------------------------"),
            //    e.LeftAlign(),
            //    e.PrintLine(receiptData),
            //    e.FeedLines(3),
            //    e.FullCut()
            //);

            //printer.Write(commands);
        }
    }
}
