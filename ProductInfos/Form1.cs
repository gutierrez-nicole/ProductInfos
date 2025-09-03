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
            txtID.KeyDown += Control_KeyDown;
            txtName.KeyDown += Control_KeyDown;
            cmbCategory.KeyDown += Control_KeyDown;
            txtPrice.KeyDown += Control_KeyDown;
            txtStock.KeyDown += Control_KeyDown;
            cmbBrand.KeyDown += Control_KeyDown;
            txtDescription.KeyDown += Control_KeyDown;
            txtSupplier.KeyDown += Control_KeyDown;
            cmbStatus.KeyDown += Control_KeyDown;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            // Populate ComboBox for Status
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Available");
            cmbStatus.Items.Add("Out of Stock");
            cmbStatus.Items.Add("Discontinued");
            cmbStatus.SelectedIndex = 0;

            LoadCategories();
            LoadBrands();

            LoadData();
        }



        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // para hindi tumunog/beep
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }


        private void LoadCategories()
        {
            try
            {
                // Default values
                string[] defaultCategories = { "Electronics", "Clothing", "Food", "Furniture", "Stationery", "Phone" };
                cmbCategory.Items.Clear();
                cmbCategory.Items.AddRange(defaultCategories);

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT DISTINCT category FROM products WHERE category IS NOT NULL AND category <> ''";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string cat = reader["category"].ToString();
                        if (!cmbCategory.Items.Contains(cat)) // avoid duplicates
                        {
                            cmbCategory.Items.Add(cat);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message);
            }
        }

        private void LoadBrands()
        {
            try
            {
                // Default values
                string[] defaultBrands = { "Sony", "Samsung", "Apple", "LG", "Acer", "Food", "Electronics", "Clothing" };
                cmbBrand.Items.Clear();
                cmbBrand.Items.AddRange(defaultBrands);

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT DISTINCT brand FROM products WHERE brand IS NOT NULL AND brand <> ''";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string brand = reader["brand"].ToString();
                        if (!cmbBrand.Items.Contains(brand)) // avoid duplicates
                        {
                            cmbBrand.Items.Add(brand);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading brands: " + ex.Message);
            }
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

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = "INSERT INTO products (product_name, category, price, stock, brand, description, supplier, status) " +
                             "VALUES (@name, @cat, @price, @stock, @brand, @desc, @sup, @status)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@cat", cmbCategory.Text);
                cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@stock", txtStock.Text);
                cmd.Parameters.AddWithValue("@brand", cmbBrand.Text);
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
                cmd.Parameters.AddWithValue("@category", cmbCategory.Text);
                cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@stock", txtStock.Text);
                cmd.Parameters.AddWithValue("@brand", cmbBrand.Text);
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

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtID.Text = row.Cells["id"].Value.ToString();
                txtName.Text = row.Cells["product_name"].Value.ToString();
                cmbCategory.Text = row.Cells["category"].Value.ToString();
                txtPrice.Text = row.Cells["price"].Value.ToString();
                txtStock.Text = row.Cells["stock"].Value.ToString();
                cmbBrand.Text = row.Cells["brand"].Value.ToString();
                txtDescription.Text = row.Cells["description"].Value.ToString();
                txtSupplier.Text = row.Cells["supplier"].Value.ToString();
                cmbStatus.Text = row.Cells["status"].Value.ToString();
            }
        }
    }
}
