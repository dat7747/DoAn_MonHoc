using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using HeThongTiemChungVaccine_NATHADA.Control;
using HeThongTiemChungVaccine_NATHADA.Model;
using System.Text.RegularExpressions;
namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class ComboVaccine_DanhMuc : Form
    {
        //logic đóng/mở textbox
        bool isEditing = false;
        Control_Combo control_Combo = new Control_Combo();
        string table = "COMBO_VACCINE";
        public ComboVaccine_DanhMuc()
        {
            InitializeComponent();
            // Đăng ký sự kiện CellClick cho DataGridView
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            // Đặt FormBorderStyle thành FixedDialog để ngăn người dùng chỉnh sửa kích thước form
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            //ConfigureDataGridView();
            txtMaCombo.Enabled = false;
            txtTenCombo.Enabled = false;
            txtGiaCombo.Enabled = false;
        }
        //thiết lập cho datagridview
        private void ConfigureDataGridView()
        {
            // Thiết lập DataGridView chỉ cho phép xem
            dataGridView1.ReadOnly = true;

            // Thiết lập chế độ chọn toàn bộ dòng
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Không cho phép thêm dòng mới trực tiếp trên DataGridView
            dataGridView1.AllowUserToAddRows = false;

            // Không cho phép chỉnh sửa hàng
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
        }
        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 100;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string maCombo = row.Cells["ma_combo"].Value.ToString();

                var result = MessageBox.Show("Chọn YES để tiếp tục chỉnh sửa hoặc NO để xem chi tiết", "Chọn hành động", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes) // Chỉnh sửa combo
                {
                    // Hiển thị dữ liệu lên các TextBox
                    txtMaCombo.Text = row.Cells["ma_combo"].Value.ToString();
                    txtTenCombo.Text = row.Cells["ten_combo"].Value.ToString();
                    txtGiaCombo.Text = row.Cells["gia_combo"].Value.ToString();

                    //txtMaCombo.Enabled = true;
                    //txtTenCombo.Enabled = true;
                    //txtGiaCombo.Enabled = true;
                }
                else if (result == DialogResult.No) // Xem chi tiết combo
                {
                    frmChiTietComBo formChiTietCombo = new frmChiTietComBo(maCombo);
                    formChiTietCombo.ShowDialog();
                }
            }
        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ma_combo", "Mã Combo");
            dataGridView1.Columns[0].DataPropertyName = "ma_combo";

            dataGridView1.Columns.Add("ten_combo", "Tên Combo");
            dataGridView1.Columns[1].DataPropertyName = "ten_combo";

            dataGridView1.Columns.Add("gia_combo", "Giá");
            dataGridView1.Columns[2].DataPropertyName = "gia_combo";

        }
        void LoadComboData()
        {
            dataGridView1.DataSource = control_Combo.selectCombo(table);
        }
        void LoadAllCombo()
        {
            AddHeader();
            LoadComboData();
            Theater();
        }





        private void ComboVaccine_DanhMuc_Load(object sender, EventArgs e)
        {
            LoadAllCombo();
            Theater();

        }
        //thêm combo
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenCombo.Text) || string.IsNullOrEmpty(txtGiaCombo.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtMaCombo.Text = "";
            if (!isEditing)
            {
                // Chuyển sang chế độ chỉnh sửa
                isEditing = true;

                // Kích hoạt các TextBox
                txtTenCombo.Enabled = true;
                txtGiaCombo.Enabled = true;

                // Thiết lập các TextBox về giá trị mặc định
                txtTenCombo.Text = "";
                txtGiaCombo.Text = "";
            }
            else
            {
                // Kiểm tra xem giá combo có phải là số và lớn hơn không
                float giaCombo;
                if (!float.TryParse(txtGiaCombo.Text, out giaCombo) || giaCombo <= 0)
                {
                    MessageBox.Show("Giá Combo phải là số và lớn hơn không!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lưu và kết thúc chế độ chỉnh sửa
                string tenCombo = txtTenCombo.Text;

                // Gọi phương thức để thêm dữ liệu
                Control_Combo controlCombo = new Control_Combo();
                bool success = controlCombo.InsertCombo(tenCombo, giaCombo);

                if (success)
                {
                    // Hiển thị thông báo thành công
                    MessageBox.Show("Combo vaccine đã được thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Thiết lập các TextBox về giá trị mặc định
                    txtTenCombo.Text = "";
                    txtGiaCombo.Text = "";
                }
                else
                {
                    // Hiển thị thông báo lỗi
                    MessageBox.Show("Thêm combo vaccine không thành công!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Kết thúc chế độ chỉnh sửa
                isEditing = false;

                // Vô hiệu hóa các TextBox
                txtTenCombo.Enabled = false;
                txtGiaCombo.Enabled = false;
            }
            LoadAllCombo();
        }
        //xóa combo
        private void button1_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có dòng được chọn không
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Hiển thị hộp thoại xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa dòng này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Nếu người dùng chọn Yes
                if (result == DialogResult.Yes)
                {
                    // Lấy dòng được chọn
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                    // Lấy mã combo vaccine từ dòng được chọn
                    string maCombo = selectedRow.Cells["ma_combo"].Value.ToString();

                    // Thực hiện xóa dữ liệu từ cơ sở dữ liệu
                    bool success = control_Combo.DeleteComboVaccine(maCombo);

                    // Nếu xóa thành công, xóa dòng từ DataGridView
                    if (success)
                    {
                        dataGridView1.Rows.Remove(selectedRow);
                        MessageBox.Show("Dữ liệu đã được xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Đã xảy ra lỗi khi xóa dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //cập nhật dữ liệu 
        private void button3_Click(object sender, EventArgs e)
        {
            if (!isEditing)
            {
                // Chuyển sang chế độ chỉnh sửa
                isEditing = true;

                // Kích hoạt các TextBox
                txtTenCombo.Enabled = true;
                txtGiaCombo.Enabled = true;
            }
            else
            {
                // Lưu và kết thúc chế độ chỉnh sửa
                string maCombo = txtMaCombo.Text;
                string tenCombo = txtTenCombo.Text;
                float giaCombo;

                // Kiểm tra giá trị của txtGiaCombo
                if (!float.TryParse(txtGiaCombo.Text, out giaCombo) || giaCombo <= 0)
                {
                    MessageBox.Show("Giá vaccine phải là số và lớn hơn 0!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Gọi phương thức cập nhật dữ liệu
                Control_Combo controlCombo = new Control_Combo();
                bool success = controlCombo.UpdateCombo(maCombo, tenCombo, giaCombo);

                // Hiển thị thông báo kết quả
                if (success)
                {
                    MessageBox.Show("Dữ liệu đã được cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Cập nhật dữ liệu không thành công!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Kết thúc chế độ chỉnh sửa
                isEditing = false;

                // Vô hiệu hóa các TextBox
                txtMaCombo.Enabled = false;
                txtTenCombo.Enabled = false;
                txtGiaCombo.Enabled = false;

                // Tải lại dữ liệu
                LoadAllCombo();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            txtMaCombo.Text = "";
            txtTenCombo.Text = "";
            txtGiaCombo.Text = "";
            txtMaCombo.Enabled = false;
            txtTenCombo.Enabled = false;
            txtGiaCombo.Enabled = false;
            MessageBox.Show("Bạn đã hủy thao tác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
