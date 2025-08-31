using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace ProductInfos
{
    public partial class Form1 : Form
    {
        string connStr = "Server=localhost;Database=productdb;Uid=root;Pwd=;";
        MySqlConnection conn;
        MySqlDataAdapter adapter;
        DataTable dt;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;

            txtID.KeyDown += TextBox_KeyDown;
            txtName.KeyDown += TextBox_KeyDown;
            txtCategory.KeyDown += TextBox_KeyDown;
            txtPrice.KeyDown += TextBox_KeyDown;
            txtStock.KeyDown += TextBox_KeyDown;
            txtBrand.KeyDown += TextBox_KeyDown;
            txtDescription.KeyDown += TextBox_KeyDown;
            txtSupplier.KeyDown += TextBox_KeyDown;
            txtSearch.KeyDown += TextBox_KeyDown;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Populate ComboBox for Status
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Available");
            cmbStatus.Items.Add("Out of Stock");
            cmbStatus.Items.Add("Discontinued");
            cmbStatus.SelectedIndex = 0;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();

                adapter = new MySqlDataAdapter("SELECT * FROM products", conn);
                dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // prevent beep

                TextBox current = sender as TextBox;

                if (current != null)
                {
                    if (current == txtSearch)
                    {
                        btnSearch.PerformClick();
                    }
                    else
                    {
                        this.SelectNextControl(current, true, true, true, true);
                    }
                }
            }
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = "INSERT INTO products (product_name, category, price, stock, brand, description, supplier, status) " +
                             "VALUES (@name, @cat, @price, @stock, @brand, @desc, @sup, @status)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@cat", txtCategory.Text);
                cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@stock", txtStock.Text);
                cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                cmd.Parameters.AddWithValue("@desc", txtDescription.Text);
                cmd.Parameters.AddWithValue("@sup", txtSupplier.Text);
                cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                cmd.ExecuteNonQuery();
            }
            LoadData();
            MessageBox.Show("✅ Added successfully!");   

        }

        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();

                string sql = "UPDATE products SET product_name=@name, category=@category, price=@price, stock=@stock, " +
                             "brand=@brand, description=@desc, supplier=@supplier, status=@status WHERE id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", txtID.Text);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@category", txtCategory.Text);
                cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@stock", txtStock.Text);
                cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                cmd.Parameters.AddWithValue("@desc", txtDescription.Text);
                cmd.Parameters.AddWithValue("@supplier", txtSupplier.Text);
                cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());
                cmd.ExecuteNonQuery();

                conn.Close();
                LoadData();
                MessageBox.Show("✅ Product updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product: " + ex.Message);
            }
        }

     


        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();

                string sql = "SELECT * FROM products WHERE product_name LIKE @search OR category LIKE @search OR brand LIKE @search";
                adapter = new MySqlDataAdapter(sql, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");

                dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching product: " + ex.Message);
            }
        }

        

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtID.Text = row.Cells["id"].Value.ToString();
                txtName.Text = row.Cells["product_name"].Value.ToString();
                txtCategory.Text = row.Cells["category"].Value.ToString();
                txtPrice.Text = row.Cells["price"].Value.ToString();
                txtStock.Text = row.Cells["stock"].Value.ToString();
                txtBrand.Text = row.Cells["brand"].Value.ToString();
                txtDescription.Text = row.Cells["description"].Value.ToString();
                txtSupplier.Text = row.Cells["supplier"].Value.ToString();
                cmbStatus.Text = row.Cells["status"].Value.ToString();
            }
        }
    }
}
