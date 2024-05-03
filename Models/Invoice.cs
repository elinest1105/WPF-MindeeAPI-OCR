using System;
using System.CodeDom;
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
        public string SupplierName { get; set; }
        public List<LineItem> LineItems { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalTax { get; set; }
        public decimal? TotalNet { get; set; }
    }


    public class LineItem
    {
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public double? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public string TaxRate { get; set; }
        public string Unit { get; set; }
        public string Code { get; set; }
    }
}
