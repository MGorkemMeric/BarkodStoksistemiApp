using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace BarkodStoksistemiApp
{
    public partial class SaleDetailsForm : Form
    {
        private int _saleId;

        public SaleDetailsForm(int saleId)
        {

            InitializeComponent();
            _saleId = saleId;       // Önce ID’yi set et
            LoadSaleDetails();      // Sonra detayları yükle
        }

        private void SaleDetailsForm_Load(object sender, EventArgs e)
        {
            LoadSaleDetails();
        }



        private void LoadSaleDetails()
        {
            string connectionString = DatabaseHelper.GetConnectionString();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
            SELECT 
                p.Barcode AS 'Barkod',
                p.Name AS 'Ürün Adı',
                si.Quantity AS 'Adet',
                si.PriceAtSale AS 'Birim Fiyat',
                (si.Quantity * si.PriceAtSale) AS 'Toplam Fiyat'
            FROM SaleItems si
            JOIN Products p ON si.ProductId = p.Id
            WHERE si.SaleId = $saleId;
        ";
                command.Parameters.AddWithValue("$saleId", _saleId);

                using (var reader = command.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    dgvSaleDetails.DataSource = dt;

                    dgvSaleDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
        }
    }

}
