using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindeeAPI_OCR.Models
{
    public class Invoice
    {
        public DateTime Date { get; set; }
        public string SupplierName {  get; set; }
        public string InvoiceId { get; set; }
        public Int32 TotalAmount { get; set;}
        public Boolean IsIntegrated { get; set;}

    }
}
