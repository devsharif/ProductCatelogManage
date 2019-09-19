using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductCatalog
{
    public partial class Form1 : Form
    {
        DataSet ds;
        SqlDataAdapter da;
        string conStr;
        public Form1()
        {
            InitializeComponent();
            conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Path.GetFullPath(@"..\..\") + "ProductDB.mdf;Trusted_Connection=True";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindGridData();
        }

        private void BindGridData()
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                ds = new DataSet();
                da = new SqlDataAdapter("Select * from Products", con);
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(ds);
                this.dataGridView1.DataSource = ds.Tables[0];
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            txCat.SelectedIndex = -1;
            txName.Clear();
            txPrice.Clear();
            txStock.Clear();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                txCat.SelectedItem = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                txName.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                txPrice.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                txStock.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("Insert into products(Category, ProductName, Price, Stock) Values(@c, @n, @p, @s)", con);
                cmd.Parameters.AddWithValue("@c", txCat.SelectedItem);
                cmd.Parameters.AddWithValue("@n", txName.Text);
                cmd.Parameters.AddWithValue("@p", txPrice.Text);
                cmd.Parameters.AddWithValue("@s", txStock.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                BindGridData();
                ClearAll();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("Update Products Set Category=@c, ProductName=@n, Price=@p, Stock=@s Where ProductId=@i", con);
                cmd.Parameters.AddWithValue("@c", txCat.SelectedItem);
                cmd.Parameters.AddWithValue("@n", txName.Text);
                cmd.Parameters.AddWithValue("@p", txPrice.Text);
                cmd.Parameters.AddWithValue("@s", txStock.Text);
                cmd.Parameters.AddWithValue("@i", dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                BindGridData();
                ClearAll();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show("Confirm Delete!", "Confirm!", buttons);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand cmd = new SqlCommand("Delete Products Where ProductId=@i", con);
                    cmd.Parameters.AddWithValue("@i", dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    BindGridData();
                    ClearAll();
                }
            }
        }

        private void txFind_TextChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(ds.Tables[0]);
            dataGridView1.DataSource = dv;
            dv.RowFilter = $"ProductName Like '%{txFind.Text}%'";
        }
    }
}
