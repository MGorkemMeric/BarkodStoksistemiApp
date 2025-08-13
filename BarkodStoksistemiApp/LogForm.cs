using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace BarkodStoksistemiApp
{
    public partial class LogForm : Form
    {
        private DataTable logsTable; // Arama için tabloyu saklayacağız

        public LogForm()
        {
            InitializeComponent();
            LoadLogs();

            // Arama kutusuna yazıldıkça filtre uygula
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadLogs()
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT Date, Action, Barcode, ProductName, OldDetails, NewDetails FROM Logs ORDER BY Date DESC";

                using (var adapter = new SQLiteDataAdapter(query, connection))
                {
                    logsTable = new DataTable();
                    adapter.Fill(logsTable);
                    dataGridViewLogs.DataSource = logsTable;

                    if (dataGridViewLogs.Columns.Contains("Id"))
                        dataGridViewLogs.Columns["Id"].Visible = false;
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (logsTable == null) return;

            string filterText = txtSearch.Text.Replace("'", "''"); // SQL injection engellemek için tek tırnak kaçar
            if (string.IsNullOrWhiteSpace(filterText))
            {
                logsTable.DefaultView.RowFilter = ""; // Filtre yok
            }
            else
            {
                // Burada istediğin kolonlarda arama yapabilirsin
                logsTable.DefaultView.RowFilter =
                    $"Date LIKE '%{filterText}%' OR " +
                    $"Action LIKE '%{filterText}%' OR " +
                    $"Barcode LIKE '%{filterText}%' OR " +
                    $"ProductName LIKE '%{filterText}%' OR " +
                    $"OldDetails LIKE '%{filterText}%' OR " +
                    $"NewDetails LIKE '%{filterText}%'";
            }
        }
    }
}
