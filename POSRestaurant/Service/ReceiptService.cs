using iText.Barcodes;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using POSRestaurant.Data;
using POSRestaurant.Models;
using POSRestaurant.Service.LoggerService;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Cell = iText.Layout.Element.Cell;
using Image = iText.Layout.Element.Image;

namespace POSRestaurant.Service
{
    /// <summary>
    /// Service to do all the necessary calculations, conversions and eventually printing of the receipt
    /// </summary>
    public class ReceiptService
    {
        /// <summary>
        /// Setting a constant page width in mm
        /// </summary>
        private const float PAGE_WIDTH_MM = 80f; // 80mm thermal paper width

        /// <summary>
        /// Setting a factor for converting mm to points
        /// </summary>
        private const float MM_TO_POINTS = 2.83465f; // Conversion factor from mm to points

        // Printer specifications
        /// <summary>
        /// The DPI for printing
        /// </summary>
        private const int DPI = 203;
        
        /// <summary>
        /// The paper width in mm
        /// </summary>
        private const int PAPER_WIDTH_MM = 72;
        
        /// <summary>
        /// The paper height in mm
        /// </summary>
        private const int PAPER_HEIGHT_MM = 3276;

        /// <summary>
        /// The scale at which we will print, so the text does not blur out
        /// </summary>
        private const int SCALE = 64;

        // Calculate printer dimensions in dots
        /// <summary>
        /// Setting constant width in dots for bitmap
        /// </summary>
        private const int MAX_PRINTER_WIDTH_DOTS = (int)((PAPER_WIDTH_MM * DPI) / 25.4); // Convert mm to dots

        /// <summary>
        /// Setting contant height in dot for bitmap
        /// </summary>
        private const int MAX_PRINTER_HEIGHT_DOTS = (int)((PAPER_HEIGHT_MM * DPI) / 25.4);

        /// <summary>
        /// DIed LogService
        /// </summary>
        private readonly LogService _logger;

        /// <summary>
        /// Constructor for the service
        /// </summary>
        /// <param name="logger">DI the LogService</param>
        public ReceiptService(LogService logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// To generate the pdf file from the billmodel we receive
        /// </summary>
        /// <param name="data">BillModel for receipt data</param>
        /// <returns>Returns a byte array of pdf data to print</returns>
        public async Task<byte[]> GenerateReceipt(BillModel data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Calculate dynamic height based on content
                    float estimatedHeight = CalculateEstimatedHeight(data);

                    // Create PDF with custom page size
                    var pageSize = new PageSize(PAGE_WIDTH_MM * MM_TO_POINTS, estimatedHeight * MM_TO_POINTS);
                    var writer = new PdfWriter(ms);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf, pageSize);

                    // Set margins (minimal for thermal paper)
                    document.SetMargins(5, 5, 5, 5);

                    // Add header
                    AddHeader(document, data);

                    // Add items
                    AddItems(document, data.Items);

                    // Add totals
                    AddTotals(document, data);

                    // Add footer
                    AddFooter(document);

                    document.Close();
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-GenerateReceipt Error", ex);
                return null;
            }
        }

        /// <summary>
        /// Calculating the estimated height for the pdf
        /// Varying as per the number of items
        /// </summary>
        /// <param name="data">BillModel to calculate height</param>
        /// <returns>Returns the estimated height in float</returns>
        private float CalculateEstimatedHeight(BillModel data)
        {
            // Basic height calculation (adjust based on your needs)
            float height = 100; // Header space
            height += data.Items.Count * 5; // Items space
            height += 50; // Totals space
            height += 5; // Footer space
            return height+100;
        }

        /// <summary>
        /// Add the header, on top of the pdf
        /// </summary>
        /// <param name="document">Document to which we add details</param>
        /// <param name="data">Bill model to add data</param>
        private void AddHeader(Document document, BillModel data)
        {
            try
            {
                //Paragraph header = new Paragraph(data.RestrauntName)
                //    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                //    .SetFontSize(12);
                //document.Add(header);

                ImageData imageData = ImageDataFactory.Create("gokul.scale-100.png");
                Image image = new Image(imageData);

                // Set size if needed
                image.SetWidth(150);  // Width in points
                                      //image.SetHeight(100); // Height in points

                //image.SetAutoScale(true);

                image.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);  // This centers the image
                image.SetMarginTop(20);    // Remove top margin
                image.SetMarginBottom(20); // Remove bottom margin

                // Add to document
                document.Add(image);

                document.Add(new Paragraph(data.Address)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(8));

                if (!string.IsNullOrWhiteSpace(data.GSTIn))
                    document.Add(new Paragraph($"GSTIN - {data.GSTIn}")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(8));


                document.Add(new Paragraph($"Name: {data.CustomerName}")
                    .SetFontSize(8));

                iText.Layout.Element.Table table = new iText.Layout.Element.Table(new float[] { 1, 1 })
                            .UseAllAvailableWidth();

                table.AddCell(new Cell(1, 1).Add(new Paragraph($"Date: {data.TimeStamp}").SetFontSize(8))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                if (data.OrderType == Data.OrderTypes.DineIn)
                    table.AddCell(new Cell(1, 1).Add(new Paragraph($"Dine In #: {data.TableNo}").SetFontSize(8))
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                else if (data.OrderType == Data.OrderTypes.Pickup)
                    table.AddCell(new Cell(1, 1).Add(new Paragraph($"Pickup Order").SetFontSize(8))
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                table.AddCell(new Cell(1, 1).Add(new Paragraph($"Order #: {data.BillNo}").SetFontSize(8))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                table.AddCell(new Cell(1, 1).Add(new Paragraph($"Token: {data.TokenNos}").SetFontSize(8))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                if (data.OrderType == Data.OrderTypes.DineIn)
                {
                    table.AddCell(new Cell(1, 1).Add(new Paragraph($"Cashier: {data.Cashier}").SetFontSize(8))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    table.AddCell(new Cell(1, 1).Add(new Paragraph($"Waiter: {data.WaiterAssigned}").SetFontSize(8))
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                }
                else if (data.OrderType == Data.OrderTypes.Pickup)
                {
                    table.AddCell(new Cell(1, 1).Add(new Paragraph($"Source: {data.Source}").SetFontSize(8))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                    if (data.Source == EnumExtensions.GetDescription(PickupSources.Swiggy) || data.Source == EnumExtensions.GetDescription(PickupSources.Zomato))
                        table.AddCell(new Cell(1, 1).Add(new Paragraph($"Reference No.: {data.ReferenceNo}").SetFontSize(8))
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                    table.AddCell(new Cell(1, 1).Add(new Paragraph($"Delivery: {data.DeliveryPersonName}").SetFontSize(8))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                }

                document.Add(table);
            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-AddHeader Error", ex);
            }
        }

        /// <summary>
        /// Method to add the bill items in the pdf
        /// </summary>
        /// <param name="document">Document to which we are adding items</param>
        /// <param name="items">List of KOTItemBillModel to add in pdf</param>
        private void AddItems(Document document, List<KOTItemBillModel> items)
        {
            try
            {
                iText.Layout.Element.Table table = new iText.Layout.Element.Table(new float[] { 3, 1, 1, 1 })
                        .SetWidth(UnitValue.CreatePercentValue(100));

                LineSeparator topLine = new LineSeparator(new SolidLine());
                document.Add(topLine);

                table.AddCell(new Cell(1, 1).Add(new Paragraph("Item").SetFontSize(8))
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                table.AddCell(new Cell(1, 1).Add(new Paragraph("Qty").SetFontSize(8))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                table.AddCell(new Cell(1, 1).Add(new Paragraph("Price").SetFontSize(8))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                table.AddCell(new Cell(1, 1).Add(new Paragraph("Total").SetFontSize(8))
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                table.AddCell(new Cell().Add(new Paragraph().SetFontSize(1)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph().SetFontSize(1)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph().SetFontSize(1)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph().SetFontSize(1)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(new SolidBorder(1)));

                // Add items
                foreach (var item in items)
                {
                    table.AddCell(new Cell().Add(new Paragraph(item.Name).SetFontSize(8))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph(item.Quantity.ToString()).SetFontSize(8))
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph(item.Price.ToString("F2")).SetFontSize(8))
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph((item.Quantity * item.Price).ToString("F2")).SetFontSize(8))
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                }

                document.Add(table);

                LineSeparator bottomLine = new LineSeparator(new SolidLine());
                document.Add(bottomLine);
            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-AddItems Error", ex);
            }
        }

        /// <summary>
        /// Add the totals section in the pdf
        /// </summary>
        /// <param name="document">Document to which we are adding details</param>
        /// <param name="data">Data to add in totals</param>
        private void AddTotals(Document document, BillModel data)
        {
            try
            {
                document.Add(new Paragraph($"Total Qty: {data.TotalQty}")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                        .SetFontSize(8));

                document.Add(new Paragraph($"SubTotal: {data.SubTotal:F2}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    .SetFontSize(8));

                if (data.IsDiscountGiven)
                {
                    if (data.IsFixedBased)
                    {
                        document.Add(new Paragraph($"Fixed Discount: {data.DiscountFixed}")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                        .SetFontSize(8));
                    }
                    else if (data.IsPercentageBased)
                    {
                        document.Add(new Paragraph($"Discount@{data.DiscountPercentage}: {(data.SubTotal * data.DiscountPercentage / 100):F2}")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                        .SetFontSize(8));
                    }
                    document.Add(new Paragraph($"SubTotal After: {data.SubTotalAfterDiscount:F2}")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                        .SetFontSize(8));
                }

                if (data.UsginGST)
                {
                    document.Add(new Paragraph($"CGST@{data.CGST}: {data.CGSTAmount:F2}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    .SetFontSize(8));

                    document.Add(new Paragraph($"SGST@{data.SGST}: {data.SGSTAmount:F2}")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                        .SetFontSize(8));
                }

                document.Add(new Paragraph($"RoundOff: {data.RoundOff:F2}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    .SetFontSize(8));

                var grandTotalPara = new Paragraph($"Grand Total: {data.GrandTotal:F2}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    .SetFontSize(8);

                grandTotalPara.SetBorderTop(new SolidBorder(1));
                grandTotalPara.SetBorderBottom(new SolidBorder(1));


                document.Add(grandTotalPara);

                document.Add(new Paragraph($"FSSAI No: {data.FassaiNo}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(8));


                string qrContent = $"upi://pay?pa=paytm.s191jue@pty&pn=Gokul Pav Bhaji&am={data.GrandTotal}&cu=INR&tn=Bill #{data.BillNo}";
                BarcodeQRCode qrCode = new BarcodeQRCode(qrContent);

                // Create the QR code image
                float qrSize = 150; // Size in points
                iText.Layout.Element.Image qrCodeImage = new iText.Layout.Element.Image(qrCode.CreateFormXObject(DeviceRgb.BLACK, 4, document.GetPdfDocument()))
                    .SetWidth(qrSize)
                    .SetHeight(qrSize)
                    .SetMarginTop(10)
                    .SetMarginBottom(10);

                document.Add(new Paragraph()
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(qrCodeImage));
            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-AddTotals Error", ex);
            }
        }

        /// <summary>
        /// To add the footer in th pdf
        /// </summary>
        /// <param name="document">Document to which we are adding the footer</param>
        private void AddFooter(Document document)
        {
            try
            {
                document.Add(new Paragraph("Thank you for visiting Gokul Pav Bhaji! Your cravings made our day. See you again soon!")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(8));
            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-AddFooter Error", ex);
            }
        }

        /// <summary>
        /// Method to start the process of printing the receipt
        /// </summary>
        /// <param name="pdfData">The pdf byte data that was generated</param>
        /// <returns>Returns Task, nothing</returns>
        public async Task PrintReceipt(byte[] pdfData)
        {
            var image = await ConvertPdfToBitmap(pdfData, 203); // 203 DPI
        }

        /// <summary>
        /// The process of printing, because pdf is complicated
        /// We convert it to a bitmap image first
        /// </summary>
        /// <param name="pdfData">Pdf data to convert</param>
        /// <param name="dpi">The DPI at which we need to convert</param>
        /// <returns>Returns the generated bitmap</returns>
        private async Task<Bitmap> ConvertPdfToBitmap(byte[] pdfData, int dpi)
        {
            // Temporary PDF file path
            string tempPdfFile = System.IO.Path.GetTempFileName();
            tempPdfFile = "E:\\ResumeBuilding\\bills.pdf";

            try
            {
                // Write PDF to temp file
                await File.WriteAllBytesAsync(tempPdfFile, pdfData);

                // Ghostscript conversion command
                string tempImageFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "receipt.bmp");
                tempImageFile = "E:\\ResumeBuilding\\receipt.bmp";


                var processInfo = new ProcessStartInfo
                {
                    FileName = "gswin64c.exe", // Ghostscript executable
                    Arguments = $"-dQUIET -dNOPAUSE -dBATCH -sDEVICE=bmpmono -r{dpi} -sOutputFile=\"{tempImageFile}\" \"{tempPdfFile}\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit(5000);
                }

                try
                {
                    PrintImage(tempImageFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Printing error: {ex.Message}");
                }

                // Load and return bitmap
                return new Bitmap(tempImageFile);
            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-ConvertPdfToBitmap Error", ex);
                return null;
            }
            finally
            {
                // Cleanup temporary files
                if (File.Exists(tempPdfFile)) File.Delete(tempPdfFile);
            }

        }

        /// <summary>
        /// Method to print the generate bitmap at given path
        /// </summary>
        /// <param name="imagePath">Path of the bitmap file</param>
        private void PrintImage(string imagePath)
        {
            try
            {
                using (var originalBitmap = new Bitmap(imagePath))
                {
                    // Calculate dimensions to fit page while maintaining aspect ratio
                    var (fitWidth, fitHeight) = CalculateFitDimensions(
                        originalBitmap.Width,
                        originalBitmap.Height,
                        MAX_PRINTER_WIDTH_DOTS,
                        MAX_PRINTER_HEIGHT_DOTS
                    );

                    // Resize image to fit printer width
                    using (var resizedBitmap = ResizeImage(originalBitmap, fitWidth, fitHeight))
                    {
                        byte[] commands = GenerateESCPOSImageCommands(resizedBitmap);
                        SendToPrinter("POS80 Printer", commands);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-PrintImage Error", ex);
            }
        }

        /// <summary>
        /// Calculating the dimesions, based on the image size and paper size for printing
        /// </summary>
        /// <param name="imageWidth">bitmap image width</param>
        /// <param name="imageHeight">bitmap image height</param>
        /// <param name="maxWidth">max width for paper</param>
        /// <param name="maxHeight">max height for paper</param>
        /// <returns>Retursn the new width and height for resultant image</returns>
        private (int width, int height) CalculateFitDimensions(int imageWidth, int imageHeight, int maxWidth, int maxHeight)
        {
            double scaleWidth = (double)maxWidth / imageWidth;
            double scaleHeight = (double)maxHeight / imageHeight;
            double scale = Math.Min(scaleWidth, scaleHeight);

            int fitWidth = (int)(imageWidth * scale);
            int fitHeight = (int)(imageHeight * scale);

            // Ensure width is divisible by 8 (requirement for most thermal printers)
            fitWidth = (fitWidth / 8) * 8;

            return (fitWidth, fitHeight);
        }

        /// <summary>
        /// Resizing the existing bitmap iamge to the new size to fit on the printing paper
        /// </summary>
        /// <param name="original">Original bitmap image</param>
        /// <param name="width">New width</param>
        /// <param name="height">new height</param>
        /// <returns>New generated bitmap image</returns>
        private Bitmap ResizeImage(Bitmap original, int width, int height)
        {
            var resized = new Bitmap(width, height);
            try
            {
                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    // Draw with proper scaling
                    graphics.DrawImage(original, 0, 0, width, height);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-ResizeImage Error", ex);
            }
            return resized;
        }

        /// <summary>
        /// Generating the commands to send to the 
        /// </summary>
        /// <param name="originalBitmap"></param>
        /// <returns></returns>
        private byte[] GenerateESCPOSImageCommands(Bitmap originalBitmap)
        {
            List<byte> commands = new List<byte>();
            try
            {
                // Initialize printer
                commands.AddRange(new byte[] { 0x1B, 0x40 });  // ESC @

                // Disable white line spacing
                commands.AddRange(new byte[] { 0x1B, 0x33, 0 }); // Set line spacing to 0

                using (Bitmap bitmap = ConvertToMonochrome(originalBitmap))
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;

                    // Process the image in sections of 24 dots height
                    // but with overlap to prevent white lines
                    for (int y = 0; y < height; y += 24)
                    {
                        int remainingHeight = Math.Min(24, height - y);

                        // Print command for bit image mode
                        commands.Add(0x1B);
                        commands.Add(0x2A);
                        commands.Add(33);    // m = 33 for 24-dot double-density
                        commands.Add((byte)(width & 0xFF));
                        commands.Add((byte)((width >> 8) & 0xFF));

                        for (int x = 0; x < width; x++)
                        {
                            byte[] verticalSlice = new byte[3] { 0, 0, 0 };

                            for (int k = 0; k < remainingHeight && k < 24; k++)
                            {
                                if (y + k < height)
                                {
                                    System.Drawing.Color pixelColor = bitmap.GetPixel(x, y + k);
                                    if (pixelColor.GetBrightness() < 0.5)
                                    {
                                        verticalSlice[k / 8] |= (byte)(0x80 >> (k % 8));
                                    }
                                }
                            }

                            commands.AddRange(verticalSlice);
                        }

                        // Only add minimal line feed
                        if (y + 24 < height)
                        {
                            commands.Add(0x0A); // Line feed
                        }
                    }
                }

                // Reset line spacing to default
                commands.AddRange(new byte[] { 0x1B, 0x32 });

                // Feed and cut
                commands.AddRange(new byte[] {
                0x1B, 0x4A, 0x40,  // Feed 64 dots
                0x1D, 0x56, 0x41, 0x00  // Full cut
                });

            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-GenerateESCPOCImageCommands Error", ex);
            }
            return commands.ToArray();
        }

        private void SendToPrinter(string printerName, byte[] data)
        {
            IntPtr printer = IntPtr.Zero;

            try
            {
                if (!OpenPrinter(printerName, out printer, IntPtr.Zero))
                {
                    throw new Exception($"Cannot open printer. Error: {Marshal.GetLastWin32Error()}");
                }

                var di = new DOCINFOA
                {
                    pDocName = "Thermal Print Job",
                    pDataType = "RAW"
                };

                if (!StartDocPrinter(printer, 1, di))
                {
                    throw new Exception($"StartDocPrinter failed. Error: {Marshal.GetLastWin32Error()}");
                }

                if (!StartPagePrinter(printer))
                {
                    throw new Exception($"StartPagePrinter failed. Error: {Marshal.GetLastWin32Error()}");
                }

                IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(data.Length);
                try
                {
                    Marshal.Copy(data, 0, pUnmanagedBytes, data.Length);
                    int bytesWritten;
                    if (!WritePrinter(printer, pUnmanagedBytes, data.Length, out bytesWritten))
                    {
                        throw new Exception($"WritePrinter failed. Error: {Marshal.GetLastWin32Error()}");
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pUnmanagedBytes);
                }

                EndPagePrinter(printer);
                EndDocPrinter(printer);
            }
            catch (Exception ex)
            {
                _logger.LogError("ReceiptService-SendToPrinter Error", ex);
            }
            finally
            {
                if (printer != IntPtr.Zero)
                {
                    ClosePrinter(printer);
                }
            }
        }

        private Bitmap ConvertToMonochrome(Bitmap original)
        {
            int width = original.Width;
            int height = original.Height;
            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(bmp))
            {
                var attributes = new ImageAttributes();

                // Create grayscale colormatrix
                var colorMatrix = new ColorMatrix(new float[][]
                {
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                attributes.SetColorMatrix(colorMatrix);
                attributes.SetThreshold(0.5f); // Adjust this threshold if needed

                graphics.DrawImage(original,
                    new System.Drawing.Rectangle(0, 0, width, height),
                    0, 0, width, height,
                    GraphicsUnit.Pixel,
                    attributes);
            }

            // Now process the image into true black and white
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color pixel = bmp.GetPixel(x, y);
                    int average = (pixel.R + pixel.G + pixel.B) / 3;
                    System.Drawing.Color newColor = average > 127 ? System.Drawing.Color.White : System.Drawing.Color.Black;
                    bmp.SetPixel(x, y, newColor);
                }
            }

            return bmp;
        }

        // P/Invoke declarations
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        [DllImport("winspool.drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

    }

}
