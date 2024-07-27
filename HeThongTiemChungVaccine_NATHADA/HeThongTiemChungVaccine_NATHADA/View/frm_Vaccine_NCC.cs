using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class frm_Vaccine_NCC : Form
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        string maNhaCungCap;
        public frm_Vaccine_NCC(string maNhaCungCap)
        {
            InitializeComponent();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.maNhaCungCap = maNhaCungCap;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
        }

        private void frm_Vaccine_NCC_Load(object sender, EventArgs e)
        {
            AllHeader();
            LoadVaccineToComboBox();
            LoadVaccineCungCap();
        }



        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DisplayVaccineDetails(row);
            }
        }
        private void btnthem_Click(object sender, EventArgs e)
        {
            string tenVaccine = comboBox1.SelectedValue.ToString();
            string maVaccine = LayMaVaccine(tenVaccine);
            float giaVaccine;

            if (string.IsNullOrEmpty(maVaccine))
            {
                MessageBox.Show("Vaccine không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtgiavaccine.Text))
            {
                MessageBox.Show("Giá vaccine không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!float.TryParse(txtgiavaccine.Text, out giaVaccine) || giaVaccine <= 0)
            {
                MessageBox.Show("Giá vaccine không hợp lệ! Giá phải là số lớn hơn 0.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (KiemTraVaccineTonTai(maVaccine, maNhaCungCap))
            {
                MessageBox.Show("Vaccine này đã tồn tại cho nhà cung cấp này!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = "INSERT INTO VACCINE_NCC (ma_vaccine, ma_nhacungcap, gia_vaccine) VALUES (@maVaccine, @maNhaCungCap, @giaVaccine)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@maVaccine", maVaccine);
                cmd.Parameters.AddWithValue("@maNhaCungCap", maNhaCungCap);
                cmd.Parameters.AddWithValue("@giaVaccine", giaVaccine);

                connection.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm vaccine thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadVaccineCungCap(); // Tải lại danh sách vaccine của nhà cung cấp sau khi thêm
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm vaccine: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                connection.Close();
            }
        }
        private void LoadVaccineCungCap()
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"
           SELECT vc.ma_vaccine, vc.ten_vaccine, nccv.gia_vaccine, lv.ten_loaivaccine, vc.hansudung_vaccine 
            FROM VACCINE vc
            INNER JOIN VACCINE_NCC nccv ON vc.ma_vaccine = nccv.ma_vaccine 
            INNER JOIN LOAIVACCINE lv ON vc.ma_loaivaccine = lv.ma_loaivaccine
            WHERE nccv.ma_nhacungcap = @maNhaCungCap";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@maNhaCungCap", maNhaCungCap);

                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];

                dataGridView1.DataSource = dt;
            }
        }


        public void AllHeader()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ma_vaccine", "Mã Vaccine");
            dataGridView1.Columns[0].DataPropertyName = "ma_vaccine";

            dataGridView1.Columns.Add("ten_vaccine", "Tên Vaccine");
            dataGridView1.Columns[1].DataPropertyName = "ten_vaccine";

            dataGridView1.Columns.Add("gia_vaccine", "Giá Vaccine");
            dataGridView1.Columns[2].DataPropertyName = "gia_vaccine";

            dataGridView1.Columns.Add("ten_loaivaccine", "Tên loại vaccine");
            dataGridView1.Columns[3].DataPropertyName = "ten_loaivaccine";

            dataGridView1.Columns.Add("hansudung_vaccine", "Hạn Sử Dụng");
            dataGridView1.Columns[4].DataPropertyName = "hansudung_vaccine";
        }


        private void LoadVaccineToComboBox()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = "SELECT ten_vaccine FROM VACCINE";

                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "ten_vaccine";
            comboBox1.ValueMember = "ten_vaccine";
        }
        private void DisplayVaccineDetails(DataGridViewRow row)
        {
            comboBox1.SelectedValue = row.Cells["ten_vaccine"].Value.ToString();
            txtloaivaccine.Text = row.Cells["ten_loaivaccine"].Value.ToString();
            txtgiavaccine.Text = row.Cells["gia_vaccine"].Value.ToString();
        }
        private string LayMaVaccine(string tenVaccine)
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = "SELECT ma_vaccine FROM VACCINE WHERE ten_vaccine = @tenVaccine";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@tenVaccine", tenVaccine);

                connection.Open();
                var result = cmd.ExecuteScalar();
                connection.Close();

                if (result != null)
                {
                    return result.ToString();
                }
                else
                {
                    return null;
                }
            }
        }
        private bool KiemTraVaccineTonTai(string maVaccine, string maNhaCungCap)
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = "SELECT COUNT(*) FROM VACCINE_NCC WHERE ma_vaccine = @maVaccine AND ma_nhacungcap = @maNhaCungCap";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@maVaccine", maVaccine);
                cmd.Parameters.AddWithValue("@maNhaCungCap", maNhaCungCap);

                connection.Open();
                int count = (int)cmd.ExecuteScalar();
                connection.Close();

                return count > 0;
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string maVaccine = selectedRow.Cells["ma_vaccine"].Value.ToString();

            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa vaccine này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    string query = "DELETE FROM VACCINE_NCC WHERE ma_vaccine = @maVaccine AND ma_nhacungcap = @maNhaCungCap";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@maVaccine", maVaccine);
                    cmd.Parameters.AddWithValue("@maNhaCungCap", maNhaCungCap);

                    connection.Open();
                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa vaccine thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadVaccineCungCap(); 
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy vaccine để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa vaccine: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    connection.Close();
                }
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một dòng để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string maVaccine = selectedRow.Cells["ma_vaccine"].Value.ToString();

            // Lấy giá vaccine từ TextBox txtgiavaccine
            string newPriceStr = txtgiavaccine.Text;

            // Kiểm tra giá vaccine nhập vào
            if (!float.TryParse(newPriceStr, out float newPrice) || newPrice <= 0)
            {
                MessageBox.Show("Giá vaccine phải là một số lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn sửa giá vaccine này không?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    string query = "UPDATE VACCINE_NCC SET gia_vaccine = @newPrice WHERE ma_vaccine = @maVaccine AND ma_nhacungcap = @maNhaCungCap";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@newPrice", newPrice);
                    cmd.Parameters.AddWithValue("@maVaccine", maVaccine);
                    cmd.Parameters.AddWithValue("@maNhaCungCap", maNhaCungCap);

                    connection.Open();
                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sửa giá vaccine thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadVaccineCungCap(); 
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy vaccine để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi sửa giá vaccine: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    connection.Close();
                }

            }
        }
    }
}
