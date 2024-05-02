using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindeeAPI_OCR.Models
{
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public DateTime? Date { get; set; }
        public DateTime DueDate { get; set; }
        public string SupplierName { get; set; }
        public Supplier Supplier { get; set; }
        public Customer Customer { get; set; }
        public List<LineItem> LineItems { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class Supplier
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Id { get; set; }
    }

    public class LineItem
    {
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
