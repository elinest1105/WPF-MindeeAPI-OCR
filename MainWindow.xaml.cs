﻿using Microsoft.Win32;
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
                InvoiceNumber = (string)prediction["invoice_number"]["value"],
                Date = prediction["date"]["value"] != null ? DateTime.Parse((string)prediction["date"]["value"]) : null,
                TotalAmount = prediction["total_amount"]["value"] != null ? decimal.Parse((string)prediction["total_amount"]["value"]) : 0,
                SupplierName = (string)prediction["supplier_name"]["value"],
                //Supplier = new Supplier
                //{
                //    Name = (string)prediction["supplier_name"]["value"],
                //    Address = (string)prediction["supplier_address"]["value"],
                //    Email = (string)prediction["supplier_email"]["value"],
                //    PhoneNumber = (string)prediction["supplier_phone_number"]["value"]
                //},
                //Customer = new Customer
                //{
                //    Name = (string)prediction["customer_name"]["value"],
                //    Address = (string)prediction["customer_address"]["value"],
                //    Id = (string)prediction["customer_id"]["value"]
                //},
                //LineItems = ((JArray)prediction["line_items"]).Select(item => new LineItem
                //{
                //    Description = (string)item["description"],
                //    ProductCode = (string)item["product_code"],
                //    Quantity = (double)item["quantity"],
                //    UnitPrice = (decimal)item["unit_price"],
                //    TotalAmount = (decimal)item["total_amount"]
                //}).ToList()
            };

            return invoice;
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