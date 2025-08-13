using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace BarkodStoksistemiApp
{
    public partial class Veresiye : Form
    {
        public Veresiye()
        {
            InitializeComponent();

            this.Load += Veresiye_Load;  // Load eventini manuel bağladık
            dvgVeresiye.CellContentClick += dvgVeresiye_CellContentClick; // Event bağlama
        }

        private void Veresiye_Load(object sender, EventArgs e)
        {
            string connectionString = DatabaseHelper.GetConnectionString();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
            SELECT 
                SaleId AS 'Satış No',
                SaleDate AS 'Tarih',
                PaymentMethod AS 'Ödeme Şekli',
                TotalAmount AS 'Toplam Tutar',
                Description AS 'Açıklama'
            FROM Sales
            WHERE PaymentMethod = 'Veresiye'
            ORDER BY SaleDate DESC;
        ";

                using (var reader = command.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    dvgVeresiye.AutoGenerateColumns = true;
                    dvgVeresiye.DataSource = dt;
                    dvgVeresiye.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }

            // Buton sütunları ekle (sadece 1 kere ekle)
            if (!dvgVeresiye.Columns.Contains("Complete"))
            {
                DataGridViewButtonColumn completeButton = new DataGridViewButtonColumn();
                completeButton.Name = "Complete";
                completeButton.HeaderText = "İşlem";
                completeButton.Text = "Satışı Tamamla";
                completeButton.UseColumnTextForButtonValue = true;
                dvgVeresiye.Columns.Add(completeButton);
            }

            if (!dvgVeresiye.Columns.Contains("GoToSale"))
            {
                DataGridViewButtonColumn goToSaleButton = new DataGridViewButtonColumn();
                goToSaleButton.Name = "GoToSale";
                goToSaleButton.HeaderText = "Satışa Git";
                goToSaleButton.Text = "Satışa Git";
                goToSaleButton.UseColumnTextForButtonValue = true;
                dvgVeresiye.Columns.Add(goToSaleButton);
            }
        }


        private void dvgVeresiye_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dvgVeresiye.Rows[e.RowIndex].IsNewRow) return;

            string columnName = dvgVeresiye.Columns[e.ColumnIndex].Name;
            int saleId = Convert.ToInt32(dvgVeresiye.Rows[e.RowIndex].Cells["Satış No"].Value);

            if (columnName == "Complete")
            {
                var confirm = MessageBox.Show(
                    "Bu veresiye satışı tamamlandı mı? Sadece listeden kaldırılacaktır.",
                    "Onay",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    dvgVeresiye.Rows.RemoveAt(e.RowIndex);
                }
            }
            else if (columnName == "GoToSale")
            {
                // Satış detay formunu aç
                SaleDetailsForm detailsForm = new SaleDetailsForm(saleId);
                detailsForm.ShowDialog();
            }
        }
    }
}
