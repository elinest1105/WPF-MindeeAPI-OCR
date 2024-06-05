using MindeeAPI_OCR.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public partial class InvoiceDetailView : Window
    {
        private Invoice _invoice;
        private string _connectionString = "Server=DESKTOP-H75KJ60\\INSTANCE2024;Database=mindee;User Id=sa;Password=root;";

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
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Invoices WHERE InvoiceNumber = @InvoiceNumber", connection);
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
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Invoices')
                CREATE TABLE Invoices (
                    InvoiceId INT IDENTITY(1,1) PRIMARY KEY,
                    InvoiceNumber VARCHAR(255) UNIQUE,
                    SupplierName VARCHAR(255),
                    Date DATE,
                    TotalNet DECIMAL(10, 2),
                    TotalTax DECIMAL(10, 2),
                    TotalAmount DECIMAL(10, 2)
                );
                ",
                @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LineItems')
                CREATE TABLE LineItems (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    InvoiceId INT,
                    Description VARCHAR(255),
                    ProductCode VARCHAR(255),
                    Quantity VARCHAR(255),
                    UnitPrice DECIMAL(10, 2),
                    TotalAmount DECIMAL(10, 2),
                    TaxRate VARCHAR(255),
                    Unit VARCHAR(255),
                    Code VARCHAR(255),
                    FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId)
                );
                "
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                foreach (var sql in sqlStatements)
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show($"Error creating database schema: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            throw;
                        }
                    }
                }
            }
        }

        private void SaveInvoice(Invoice invoice)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand("INSERT INTO Invoices (InvoiceNumber, Date, TotalAmount, TotalTax, TotalNet, SupplierName) OUTPUT INSERTED.InvoiceId VALUES (@InvoiceNumber, @Date, @TotalAmount, @TotalTax, @TotalNet, @SupplierName)", connection);
                cmd.Parameters.AddWithValue("@InvoiceNumber", invoice.InvoiceNumber ?? string.Empty);
                if(invoice.Date.HasValue)
                {
                    cmd.Parameters.AddWithValue("@Date", invoice.Date);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Date", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount ?? 0);
                cmd.Parameters.AddWithValue("@TotalTax", invoice.TotalTax ?? 0);
                cmd.Parameters.AddWithValue("@TotalNet", invoice.TotalNet ?? 0);
                cmd.Parameters.AddWithValue("@SupplierName", invoice.SupplierName ?? string.Empty);
                int invoiceId = (int)cmd.ExecuteScalar();

                foreach (var item in invoice.LineItems)
                {
                    var itemCmd = new SqlCommand("INSERT INTO LineItems (InvoiceId, Description, ProductCode, Quantity, UnitPrice, TotalAmount, TaxRate, Unit, Code) VALUES (@InvoiceId, @Description, @ProductCode, @Quantity, @UnitPrice, @TotalAmount, @TaxRate, @Unit, @Code)", connection);

                    // Add parameters here
                    itemCmd.Parameters.AddWithValue("@InvoiceId", invoiceId);
                    itemCmd.Parameters.AddWithValue("@Description", item.Description ?? string.Empty);
                    itemCmd.Parameters.AddWithValue("@ProductCode", item.ProductCode ?? string.Empty);
                    itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity ?? string.Empty);
                    itemCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice ?? "0"); 
                    itemCmd.Parameters.AddWithValue("@TotalAmount", item.TotalAmount ?? "0");
                    itemCmd.Parameters.AddWithValue("@TaxRate", item.TaxRate ?? "0");
                    itemCmd.Parameters.AddWithValue("@Unit", item.Unit ?? string.Empty);
                    itemCmd.Parameters.AddWithValue("@Code", item.Code ?? string.Empty); 

                    itemCmd.ExecuteNonQuery();
                }
            }
        }
    }
}