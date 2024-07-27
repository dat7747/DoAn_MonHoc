using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeThongTiemChungVaccine_NATHADA.Control;
using HeThongTiemChungVaccine_NATHADA.Model;
namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class frmChiTietComBo : Form
    {
        private string maCombo;
        private Control_Combo controlCombo = new Control_Combo();
        String LOAIVACCINE = "LOAIVACCINE";
        //logic đóng/mở textbox
        bool isEditing = false;
        public frmChiTietComBo(String maCombo)
        {
            InitializeComponent();
            // Đăng ký sự kiện CellClick cho DataGridView
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            // Đặt FormBorderStyle thành FixedDialog để ngăn người dùng chỉnh sửa kích thước form
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            // Gán giá trị maCombo từ tham số vào biến thành viên maCombo
            this.maCombo = maCombo;
            // Load chi tiết combo khi form được khởi tạo
            LoadChiTietCombo();
            textboxEnableFalse();
            txtLoaiVaccine.Text = "";
            txtLoaiVaccine.Enabled = false;
            ConfigureDataGridView();
            cboVaccine.DropDownStyle = ComboBoxStyle.DropDownList;
        }
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

            Load_select_null();
        }
        private void Load_select_null()
        {
            // Xóa bỏ lựa chọn dòng hiện tại
            dataGridView1.ClearSelection();
            // Đảm bảo rằng không có ô nào được chọn
            dataGridView1.CurrentCell = null;
            // Thiết lập giá trị ban đầu của ComboBox là null
            cboVaccine.SelectedIndex = -1;
            txtSL.Text = "";
            txtLoaiVaccine.Text = "";
        }
        private void LoadChiTietCombo()
        {
            DataTable dt = controlCombo.selectChiTietCombo("CHITIET_COMBO_VACCXINE", maCombo);
            AddHeader();
            dataGridView1.DataSource = dt;
            LoadVaccine();
            //không chọn dòng nào khi loadS
            dataGridView1.ClearSelection();
            txtMaCombo.Text = maCombo;
        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();


            dataGridView1.Columns.Add("ten_vaccine", "Tên Vaccine");
            dataGridView1.Columns[0].DataPropertyName = "ten_vaccine";
            dataGridView1.Columns[0].Width = 650;

            dataGridView1.Columns.Add("soluong_vaccine", "Số lượng");
            dataGridView1.Columns[1].DataPropertyName = "soluong_vaccine";
            dataGridView1.Columns[1].Width = 60;

            dataGridView1.Columns.Add("ten_loaivaccine", "Loại Vaccine");
            dataGridView1.Columns[2].DataPropertyName = "ten_loaivaccine";

            // Add the missing 'ma_vaccine' column
            dataGridView1.Columns.Add("ma_vaccine", "Mã Vaccine");
            dataGridView1.Columns["ma_vaccine"].DataPropertyName = "ma_vaccine";
            dataGridView1.Columns["ma_vaccine"].Visible = false;
        }
        private void LoadVaccine()
        {
            // Nạp dữ liệu vào ComboBox vaccine
            DataTable dtVaccine = controlCombo.selectVaccineWithLoaiVaccine();
            cboVaccine.DataSource = dtVaccine;
            cboVaccine.DisplayMember = "ten_vaccine";
            cboVaccine.ValueMember = "ma_vaccine";


        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                cboVaccine.Text = row.Cells["ten_vaccine"].Value.ToString();
                txtSL.Text = row.Cells["soluong_vaccine"].Value.ToString();
            }
        }
        void textboxclear()
        {
            cboVaccine.Text = "";
            txtSL.Text = "";
        }
        void textboxEnableFalse()
        {
            txtMaCombo.Enabled = false;
            cboVaccine.Enabled = false;
            txtSL.Enabled = false;
        }
        void textboxEnableTrue()
        {
            cboVaccine.Enabled = true;
            txtSL.Enabled = true;
        }
        private void frmChiTietComBo_Load(object sender, EventArgs e)
        {
            Theater();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //thêm
        private void button2_Click(object sender, EventArgs e)
        {
            if (!isEditing)
            {
                // Chuyển sang chế độ chỉnh sửa
                isEditing = true;
                textboxEnableTrue();
                textboxclear();
                button2.Text = "Lưu";
            }
            else
            {
                // Thực hiện thêm khi đang trong chế độ chỉnh sửa
                // Thêm chi tiết combo

                string maVaccine = cboVaccine.SelectedValue.ToString();
                int soLuong;

                // Kiểm tra nếu txtSL có giá trị là số nguyên hợp lệ và lớn hơn 0
                if (!int.TryParse(txtSL.Text, out soLuong) || soLuong <= 0)
                {
                    MessageBox.Show("Số lượng phải là số nguyên hợp lệ và lớn hơn 0!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool success = controlCombo.InsertChiTietCombo(maCombo, maVaccine, soLuong);
                if (success)
                {
                    MessageBox.Show("Thêm chi tiết combo thành công!");
                    textboxclear();
                    textboxEnableFalse();

                    LoadChiTietCombo();
                }
                else
                {
                    MessageBox.Show("Tồn tại vaccine trong combo");
                }

                // Chuyển trở lại chế độ không chỉnh sửa
                isEditing = false;
                textboxEnableFalse();
                button2.Text = "Thêm";
            }

            Load_select_null();
            dataGridView1.ClearSelection();
            LoadChiTietCombo();

            Load_select_null();
        }

        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 50;
        }

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

                    // Lấy mã  vaccine từ dòng được chọn
                    string mavaccine = selectedRow.Cells["ma_vaccine"].Value.ToString();

                    // Thực hiện xóa dữ liệu từ cơ sở dữ liệu

                    bool success = controlCombo.DeleteChiTietCombo(maCombo, mavaccine);

                    // Nếu xóa thành công, xóa dòng từ DataGridView
                    if (success)
                    {
                        dataGridView1.Rows.Remove(selectedRow);
                        MessageBox.Show("Dữ liệu đã được xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dataGridView1.ClearSelection();
                    }
                    else
                    {
                        MessageBox.Show("Bạn đã hủy thao tác Xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dataGridView1.ClearSelection();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            dataGridView1.ClearSelection();
            LoadChiTietCombo();
            Load_select_null();
        }

        //update
        private void button3_Click(object sender, EventArgs e)
        {
            if (!IsFieldsNotEmpty())
            {
                MessageBox.Show("Vui lòng chọn 1 dòng để tiếp tục!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!isEditing)
            {
                // Chuyển sang chế độ chỉnh sửa
                isEditing = true;
                txtSL.Enabled = true;
                button3.Text = "Lưu";
            }
            else
            {
                string vaccine = cboVaccine.SelectedValue.ToString();
                int soLuong;

                // Kiểm tra nếu txtSL có giá trị là số nguyên hợp lệ và lớn hơn 0
                if (!int.TryParse(txtSL.Text, out soLuong) || soLuong <= 0)
                {
                    MessageBox.Show("Số lượng phải là số nguyên hợp lệ và lớn hơn 0!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Gọi phương thức cập nhật dữ liệu
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                Control_Combo controlCombo = new Control_Combo();
                bool success = controlCombo.UpdateChiTietCombo(maCombo, vaccine, soLuong);

                // Hiển thị thông báo kết quả
                if (success)
                {
                    MessageBox.Show("Dữ liệu đã được cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadChiTietCombo();
                }
                else
                {
                    MessageBox.Show("Cập nhật dữ liệu không thành công!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Chuyển trở lại chế độ không chỉnh sửa
                isEditing = false;
                textboxEnableFalse();
                button3.Text = "Chỉnh sửa";
                Load_select_null();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                // Chuyển sang chế độ chỉnh sửa
                isEditing = false;
                textboxEnableFalse();
                MessageBox.Show("Bạn đã hủy thao tác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textboxclear();
                button2.Text = "Thêm";
                button3.Text = "Sửa";
            }
            Load_select_null();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool IsFieldsNotEmpty()
        {
            if (string.IsNullOrWhiteSpace(txtMaCombo.Text) ||
                string.IsNullOrWhiteSpace(cboVaccine.Text) ||
                string.IsNullOrWhiteSpace(txtSL.Text))
            {
                return false;
            }
            return true;
        }

        private void cboVaccine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboVaccine.SelectedValue != null)
            {
                DataRowView selectedDataRow = cboVaccine.SelectedItem as DataRowView;
                if (selectedDataRow != null)
                {
                    // Lấy loại Vaccine từ DataRow
                    string loaiVaccine = selectedDataRow["ten_loaivaccine"].ToString();
                    txtLoaiVaccine.Text = loaiVaccine;
                }
            }
        }
    }
}
