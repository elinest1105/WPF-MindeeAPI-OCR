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
            Console.WriteLine(jsonObject.ToString());
            return new Invoice
            {
                Date = DateTime.Parse(jsonObject.date),
                SupplierName = jsonObject.supplier,
                InvoiceId = jsonObject.invoice_number,
                TotalAmount = decimal.Parse(jsonObject.total),
                IsIntegrated = false
            };
        }

        private void LvInvoices_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lvInvoices.SelectedItem is Invoice selectedInvoice)
            {
                InvoiceDetailView detailsWindow = new InvoiceDetailView(selectedInvoice);
                detailsWindow.Show();
            }
        }
    }
}