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

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class LoaiVaccine_DanhMuc : Form
    {
        DataColumn[] key = new DataColumn[1];
        Control_LoaiVC lvc = new Control_LoaiVC();
        string table = "LOAIVACCINE";
        public LoaiVaccine_DanhMuc()
        {
            InitializeComponent();
            dgv_lvc.AllowUserToAddRows = false;
            dgv_lvc.ReadOnly = true;
            btn_luu.Enabled = false;
            tb_maloai.Enabled = false;
        }
        void AddHeader()
        {
            dgv_lvc.Columns.Clear();
            dgv_lvc.Columns.Add("ma_loaivaccine", "Mã Loại Vaccine");
            dgv_lvc.Columns[0].DataPropertyName = "ma_loaivaccine";
            dgv_lvc.Columns.Add("ten_loaivaccine", "Tên Loại Vaccine");
            dgv_lvc.Columns[1].DataPropertyName = "ten_loaivaccine";

        }

        void loadDTG()
        {
            if (dgv_lvc.DataSource != null)
                dgv_lvc.Rows.Clear();
            DataTable dtLVC = lvc.select(table);
            dgv_lvc.DataSource = dtLVC;
            key[0] = dtLVC.Columns[0];
            dtLVC.PrimaryKey = key;
            dgv_lvc.RowTemplate.Height = 30;
        }
        void loadAllLoaiVC()
        {
            AddHeader();
            loadDTG();
            Theater();
        }
        private void Theater()
        {
            dgv_lvc.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dgv_lvc.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv_lvc.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            int totalWidth = dgv_lvc.Width - dgv_lvc.RowHeadersWidth; 
            int columnWidth = totalWidth / 2; 
            dgv_lvc.Columns[0].Width = columnWidth;
            dgv_lvc.Columns[1].Width = columnWidth;
        }
        private void btn_them_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(tb_tenloai.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Model_LoaiVaccine newlvc = new Model_LoaiVaccine();
                newlvc.maLoai = tb_maloai.Text;
                newlvc.tenLoai = tb_tenloai.Text;
                if (lvc.checkTrungTen(newlvc.tenLoai, table) == 1)
                {
                    MessageBox.Show("Trùng tên loại vaccine có từ trước. Vui lòng nhập lại tên loại vaccine mới!");
                    return;
                }
                lvc.insert(newlvc, table);
                MessageBox.Show("Thêm loại vaccine thành công rồi!");
                tb_maloai.Clear();
                tb_tenloai.Clear();
                tb_maloai.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex != null ? ex.Message : "Lỗi rồi !");
            }
        }

        private void LoaiVaccine_DanhMuc_Load(object sender, EventArgs e)
        {
            loadAllLoaiVC();
        }
        private bool isClicked = false;

        private void dgv_lvc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgv_lvc_SelectionChanged(object sender, EventArgs e)
        {
        
        }

        private void dgv_lvc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgv_lvc.Rows[e.RowIndex].Selected = true;
                DataGridViewRow row = dgv_lvc.Rows[e.RowIndex];
                tb_maloai.Text = row.Cells["ma_loaivaccine"].Value.ToString();
                tb_tenloai.Text = row.Cells["ten_loaivaccine"].Value.ToString();
            }
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                Model_LoaiVaccine newlvc = new Model_LoaiVaccine();
                newlvc.maLoai = tb_maloai.Text;
                newlvc.tenLoai = tb_tenloai.Text;
                if (lvc.checkTrungMa(newlvc.maLoai, table) == 1)
                {
                    // Hiển thị thông báo xác nhận xóa Loại Vaccine
                    DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa Loại Vaccine '{newlvc.tenLoai}'?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        lvc.delete(newlvc, table);
                        MessageBox.Show($"Xóa loại vaccine '{newlvc.tenLoai}' thành công!");
                        tb_maloai.Clear();
                        tb_tenloai.Clear();
                        tb_maloai.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng click chọn loại vaccine cần xóa!!!");
                }
            }
            catch
            {
                MessageBox.Show("Lỗi rồi!!!");
            }
        }

        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                Model_LoaiVaccine newlvc = new Model_LoaiVaccine();
                newlvc.maLoai = tb_maloai.Text;
                newlvc.tenLoai = tb_tenloai.Text;
                if (lvc.checkTrungMa(newlvc.maLoai, table) == 1)
                {
                    btn_luu.Enabled = true;
                    dgv_lvc.ReadOnly = false;
                }
                else
                {
                    MessageBox.Show("Vui lòng click chọn loại vaccine cần sửa!!!");
                }
            }
            catch
            {
                MessageBox.Show("Chưa có mã Loại vaccine");
            }
        }

        private void btn_luu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(tb_tenloai.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Model_LoaiVaccine newlvc = new Model_LoaiVaccine();
                newlvc.maLoai = tb_maloai.Text;
                newlvc.tenLoai = tb_tenloai.Text;
                if (lvc.checkTrungMa(newlvc.maLoai, table) == 1)
                {
                    if (lvc.checkTrungTen(newlvc.tenLoai, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"Tên loại vaccine '{newlvc.tenLoai}' đã tồn tại. Bạn có muốn sửa không?", "Xác nhận", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            lvc.update(newlvc, table);
                            MessageBox.Show("Sửa thành công!");
                            tb_maloai.Clear();
                            tb_tenloai.Clear();
                            tb_maloai.Focus();
                            btn_luu.Enabled = false;
                            return;
                        }
                        else
                        {
                            return;
                        }

                    }
                    lvc.update(newlvc, table);
                    MessageBox.Show("Sửa thành công!");
                    tb_maloai.Clear();
                    tb_tenloai.Clear();
                    tb_maloai.Focus();
                    btn_luu.Enabled = false;
                    return;
                }
                else
                {
                    MessageBox.Show("Mã Loại vaccine không tồn tại. Hoặc bạn chưa nhập mã Loại vaccine!!!");
                }
            }
            catch
            {
                MessageBox.Show("Mã Loại vaccine không tồn tại!");
            }
        }

        private void btn_timkiem_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = tb_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    DataTable dtLVC = lvc.FindMa(searchText, table);
                    if (dtLVC.Rows.Count > 0)
                    {
                        dgv_lvc.DataSource = dtLVC;
                        key[0] = dtLVC.Columns[0];
                        dtLVC.PrimaryKey = key;

                        // Hiển thị thông báo khi tìm kiếm thành công
                        MessageBox.Show($"Đã tìm thấy {dtLVC.Rows.Count} loại vaccine có từ khóa '{searchText}'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy kết quả phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
