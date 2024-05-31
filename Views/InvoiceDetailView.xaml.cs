using MindeeAPI_OCR.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MindeeAPI_OCR.Views
{
    /// <summary>
    /// Interaction logic for InvoiceDetailView.xaml
    /// </summary>
    public partial class InvoiceDetailView : Window
    {
        private Invoice _invoice;

        private string _connectionString = "server=localhost;port=3306;database=mindee;username=root;password=root;";
        public InvoiceDetailView(Invoice invoice)
        {
            InitializeComponent();
            _invoice = invoice;
            DataContext = _invoice;
            EnsureDatabaseSchema();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InvoiceExists(_invoice.InvoiceNumber))
                {
                    MessageBox.Show("An invoice with this number already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    SaveInvoice(_invoice);
                    MessageBox.Show("Invoice saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save invoice. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool InvoiceExists(string invoiceNumber)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM Invoices WHERE InvoiceNumber = @InvoiceNumber", connection);
                cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private async Task EnsureDatabaseSchema()
        {
            string[] sqlStatements = new string[]
            {
                @"
                CREATE TABLE IF NOT EXISTS Invoices (
                    InvoiceId INT AUTO_INCREMENT PRIMARY KEY,
                    InvoiceNumber VARCHAR(255) UNIQUE,
                    SupplierName VARCHAR(255),
                    Date DATE,
                    TotalNet DECIMAL(10, 2),
                    TotalTax DECIMAL(10, 2),
                    TotalAmount DECIMAL(10, 2)
                );
                ",
                @"
                CREATE TABLE IF NOT EXISTS LineItems (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    InvoiceNumber VARCHAR(255),
                    Description VARCHAR(255),
                    ProductCode VARCHAR(255),
                    Quantity VARCHAR(255),
                    UnitPrice DECIMAL(10, 2),
                    TotalAmount DECIMAL(10, 2),
                    TaxRate VARCHAR(255),
                    Unit VARCHAR(255),
                    Code VARCHAR(255),
                    FOREIGN KEY (InvoiceNumber) REFERENCES Invoices(InvoiceNumber)
                );
                "
            };

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                foreach (var sql in sqlStatements)
                {
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        try
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                        catch (MySqlException ex)
                        {
                            // Log error or handle it as needed
                            Console.WriteLine($"Error creating database schema: {ex.Message}");
                            throw; // Re-throw if you want to propagate the error up the call stack
                        }
                    }
                }
            }
        }

        private void SaveInvoice(Invoice invoice)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                // Save the main invoice first
                var cmd = new MySqlCommand("INSERT INTO Invoices (InvoiceNumber, Date, TotalAmount, TotalTax, TotalNet, SupplierName) VALUES (@InvoiceNumber, @Date, @TotalAmount, @TotalTax, @TotalNet, @SupplierName)", connection);
                cmd.Parameters.AddWithValue("@InvoiceNumber", invoice.InvoiceNumber);
                cmd.Parameters.AddWithValue("@Date", invoice.Date);
                cmd.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                cmd.Parameters.AddWithValue("@TotalTax", invoice.TotalTax);
                cmd.Parameters.AddWithValue("@TotalNet", invoice.TotalNet);
                cmd.Parameters.AddWithValue("@SupplierName", invoice.SupplierName);
                cmd.ExecuteNonQuery();

                // Save line items
                foreach (var item in invoice.LineItems)
                {
                    var itemCmd = new MySqlCommand("INSERT INTO LineItems (InvoiceNumber, Description, ProductCode, Quantity, UnitPrice, TotalAmount, TaxRate, Unit, Code) VALUES (@InvoiceNumber, @Description, @ProductCode, @Quantity, @UnitPrice, @TotalAmount, @TaxRate, @Unit, @Code)", connection);
                    itemCmd.Parameters.AddWithValue("@InvoiceNumber", invoice.InvoiceNumber);
                    itemCmd.Parameters.AddWithValue("@Description", item.Description);
                    itemCmd.Parameters.AddWithValue("@ProductCode", item.ProductCode);
                    itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    itemCmd.Parameters.AddWithValue("@TotalAmount", item.TotalAmount);
                    itemCmd.Parameters.AddWithValue("@TaxRate", item.TaxRate);
                    itemCmd.Parameters.AddWithValue("@Unit", item.Unit);
                    itemCmd.Parameters.AddWithValue("@Code", item.Code);
                    itemCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
