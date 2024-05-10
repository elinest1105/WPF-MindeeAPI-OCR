using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindeeAPI_OCR.Models
{
    public class InvoiceData
    {
        public string InvoiceNumber { get; set; }
        public DateTime? Date { get; set; }
        public string SupplierName { get; set; }
        public List<TableItem> TableItems { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalTax { get; set; }
        public decimal? TotalNet { get; set; }
    }


    public class TableItem
    {
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string TotalAmount { get; set; }
        public string TaxRate { get; set; }
        public string Unit { get; set; }
        public string Code { get; set; }
    }
}
