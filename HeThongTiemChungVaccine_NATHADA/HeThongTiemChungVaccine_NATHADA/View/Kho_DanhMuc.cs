using HeThongTiemChungVaccine_NATHADA.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class Kho_DanhMuc : Form
    {
        Control_Kho controlKho = new Control_Kho();
        public Kho_DanhMuc()
        {
            InitializeComponent();
            dataGridView1.AllowUserToAddRows = false;
            cbbdonvitinh.Enabled = true;
            txtsoluong.Enabled = true;
            LoadData();
        }
        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 50;
        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ma_vaccine", "Mã Vaccine");
            dataGridView1.Columns[0].DataPropertyName = "ma_vaccine";

            dataGridView1.Columns.Add("ten_vaccine", "Tên Vaccine");
            dataGridView1.Columns[1].DataPropertyName = "ten_vaccine";

            dataGridView1.Columns.Add("soluong_vaccine", "Số lượng");
            dataGridView1.Columns[2].DataPropertyName = "soluong_vaccine";

            dataGridView1.Columns.Add("donvitinh", "Đơn vị tính");
            dataGridView1.Columns[3].DataPropertyName = "donvitinh";
        }

        private void Kho_DanhMuc_Load(object sender, EventArgs e)
        {
            LoadData();
            cbbdonvitinh.Items.AddRange(new string[] { "Gói", "Ống"});
            cbbdonvitinh.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadTenVaccine();
            Theater();
        }

        private void LoadData()
        {
            AddHeader();
            DataTable dt = controlKho.LayDanhSachKho();
            dataGridView1.DataSource = dt;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                cbbtenvaccine.SelectedValue = row.Cells["ma_vaccine"].Value.ToString();
                txtsoluong.Text = row.Cells["soluong_vaccine"].Value.ToString();
                cbbdonvitinh.SelectedItem = row.Cells["donvitinh"].Value.ToString();

            }
        }


        private void LoadTenVaccine()
        {

            Control_Vaccine controlVaccine = new Control_Vaccine();
            DataTable dtVaccine = controlVaccine.LayDanhSachVaccine();
            cbbtenvaccine.DataSource = dtVaccine;
            cbbtenvaccine.DisplayMember = "ten_vaccine";
            cbbtenvaccine.ValueMember = "ma_vaccine";
            cbbtenvaccine.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            // Kiểm tra các trường dữ liệu đã được nhập đầy đủ chưa
            if (string.IsNullOrEmpty(cbbtenvaccine.SelectedValue?.ToString()) ||
                string.IsNullOrEmpty(txtsoluong.Text) ||
                string.IsNullOrEmpty(cbbdonvitinh.Text) )
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtsoluong.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là một số nguyên dương.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lấy dữ liệu từ các điều khiển
            string maVaccine = cbbtenvaccine.SelectedValue.ToString();
            soLuong = Convert.ToInt32(txtsoluong.Text);
            string donViTinh = cbbdonvitinh.Text;

            // Gọi phương thức để thêm dữ liệu
            Control_Kho controlKho = new Control_Kho();
            controlKho.ThemDuLieu(maVaccine, soLuong, donViTinh);
            LoadData();
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem đã chọn dòng nào chưa
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng bạn muốn xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị hộp thoại xác nhận xóa
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa Vaccine này khỏi kho?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Nếu người dùng chọn Yes, thực hiện xóa
            if (result == DialogResult.Yes)
            {
                // Lấy mã vaccine và mã nhà cung cấp từ dòng đã chọn
                string maVaccine = cbbtenvaccine.SelectedValue.ToString();

                // Gọi phương thức xóa
                Control_Kho controlKho = new Control_Kho();
                controlKho.XoaDuLieu(maVaccine);

                // Load lại dữ liệu sau khi xóa
                LoadData();
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            cbbdonvitinh.Enabled  = true;
            txtsoluong.Enabled = true;
            btnluu.Enabled = true;
        }

        private void btnluu_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtsoluong.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là một số nguyên dương.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string maVaccine = cbbtenvaccine.SelectedValue.ToString();
            soLuong = Convert.ToInt32(txtsoluong.Text);
            string donViTinh = cbbdonvitinh.Text;

            Control_Kho controlKho = new Control_Kho();
            controlKho.SuaDuLieu(maVaccine, soLuong, donViTinh);

            LoadData();
            txtsoluong.Enabled = false;
            cbbdonvitinh.Enabled =  false;
            btnluu.Enabled = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txttimkiem_TextChanged(object sender, EventArgs e)
        {
            string keyword = txttimkiem.Text.Trim();

            Control_Kho controlKho = new Control_Kho();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadData(); 
            }
            else
            {
                DataTable dataTable = controlKho.TimKiemTheoTenVaccine(keyword);
                dataGridView1.DataSource = dataTable;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
