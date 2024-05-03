using MindeeAPI_OCR.Models;
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
        public InvoiceDetailView(Invoice invoice)
        {
            InitializeComponent();
            DataContext = invoice;
        }
    }
}
