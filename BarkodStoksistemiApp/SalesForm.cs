using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace BarkodStoksistemiApp
{
    public partial class SalesForm : Form
    {
        private DataTable salesDataTable;
        public SalesForm()
        {
            InitializeComponent();
        }

        private void SalesForm_Load(object sender, EventArgs e)
        {
            LoadSales();
        }

        private void LoadSales()
        {
            string connectionString = DatabaseHelper.GetConnectionString();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
            SELECT SaleId AS 'Satış No', 
                   SaleDate AS 'Tarih', 
                   PaymentMethod AS 'Ödeme Şekli', 
                   TotalAmount AS 'Toplam Tutar', 
                   Description AS 'Açıklama'
            FROM Sales
            ORDER BY SaleDate DESC;
        ";
                using (var reader = command.ExecuteReader())
                {
                    salesDataTable = new DataTable();
                    salesDataTable.Load(reader);

                    dgvSales.AutoGenerateColumns = true;
                    dgvSales.DataSource = salesDataTable.DefaultView;
                    dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }

            // Sütun ekleme sadece bir kere olmalı, buraya taşımak iyi olur
            if (!dgvSales.Columns.Contains("Details"))
            {
                DataGridViewButtonColumn detailsButtonColumn = new DataGridViewButtonColumn();
                detailsButtonColumn.Name = "Details";
                detailsButtonColumn.HeaderText = "Satış Detayları";
                detailsButtonColumn.Text = "Detayları Gör";
                detailsButtonColumn.UseColumnTextForButtonValue = true;
                dgvSales.Columns.Add(detailsButtonColumn);
            }
        }

        private void dgvSales_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvSales.Rows[e.RowIndex].IsNewRow)
                return;
            if (e.RowIndex >= 0 && dgvSales.Columns[e.ColumnIndex].Name == "Details")
            {
                int saleId = Convert.ToInt32(dgvSales.Rows[e.RowIndex].Cells["Satış No"].Value);
                SaleDetailsForm detailsForm = new SaleDetailsForm(saleId);
                detailsForm.ShowDialog();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string filterText = txtSearch.Text.Replace("'", "''"); // Güvenlik için

            if (string.IsNullOrWhiteSpace(filterText))
            {
                salesDataTable.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                // Tarih sütununu string'e çevirmek için Convert kullandık.
                string filter = string.Format(
                    "Convert([Satış No], 'System.String') LIKE '%{0}%' OR " +
                    "Convert([Tarih], 'System.String') LIKE '%{0}%' OR " +
                    "[Ödeme Şekli] LIKE '%{0}%' OR " +
                    "Convert([Toplam Tutar], 'System.String') LIKE '%{0}%' OR " +
                    "[Açıklama] LIKE '%{0}%'", filterText);

                salesDataTable.DefaultView.RowFilter = filter;
            }
        }
    }
}
