using Microsoft.Win32;
using MindeeAPI_OCR.Models;
using MindeeAPI_OCR.Services;
using MindeeAPI_OCR.Views;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;

namespace MindeeAPI_OCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        public ObservableCollection<Invoice> Invoices { get; set; } = new ObservableCollection<Invoice>();
        public MainWindow()
        {
            InitializeComponent();
            lvInvoices.ItemsSource = Invoices;
        }

        private async void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "All files (*.*)|*.*|Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
            };

            if (dlg.ShowDialog () == true)
            {
                foreach (string filename in dlg.FileNames)
                {
                    try
                    {
                        string extractedDataJson = await App.MindeeClient.ExtractInvoiceDataAsync(filename);
                        Invoice newInvoice = ParseInvoiceData(extractedDataJson);
                        Invoices.Add(newInvoice);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error processing file {filename}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private Invoice ParseInvoiceData(string jsonData)
        {
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
            var document = jsonObject["document"];
            var prediction = document["inference"]["prediction"];
            Console.WriteLine(prediction);

            var invoice = new Invoice
            {
                InvoiceNumber = prediction["invoice_number"]["value"] != null ? (string)prediction["invoice_number"]["value"] : null,
                Date = prediction["date"]["value"] != null ? DateTime.Parse((string)prediction["date"]["value"]) : null,
                TotalAmount = prediction["total_amount"]["value"] != null ? decimal.Parse((string)prediction["total_amount"]["value"]) : 0,
                TotalTax = prediction["total_tax"]["value"] != null ? decimal.Parse((string)prediction["total_tax"]["value"]) : 0,
                TotalNet = prediction["total_net"]["value"] != null ? decimal.Parse((string)prediction["total_net"]["value"]) : 0,
                SupplierName = prediction["supplier_name"]["value"] != null ? (string)prediction["supplier_name"]["value"] : null,

                LineItems = ((JArray)prediction["line_items"]).Select(item => new LineItem
                {
                    Description = item["description"]?.Value<string>(),
                    ProductCode = item["product_code"]?.Value<string>(),
                    Quantity = item["quantity"] != null ? Convert.ToDouble(item["quantity"]) : 0,
                    UnitPrice = item["unit_price"] != null ? decimal.Parse((string)item["unit_price"]) : 0,
                    TotalAmount = item["total_amount"] != null ? decimal.Parse((string)(item["total_amount"])) : 0,
                    TaxRate = item["tax_rate"]?.Value<string>(),
                    Unit = item["unit"]?.Value<string>(),
                    Code = item["code"]?.Value<string>(),
                }).ToList()
            };

            return invoice;
        }

        private void LvInvoices_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lvInvoices.SelectedItem is Invoice selectedInvoice)
            {
                Console.WriteLine($"Opening details for invoice: {selectedInvoice.InvoiceNumber}");

                // Open the Invoice Details Window
                InvoiceDetailView detailsWindow = new InvoiceDetailView(selectedInvoice);
                detailsWindow.Show();
            }
        }
    }
}