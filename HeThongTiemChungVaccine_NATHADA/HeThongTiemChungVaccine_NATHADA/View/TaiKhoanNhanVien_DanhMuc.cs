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
    public partial class TaiKhoanNhanVien_DanhMuc : Form
    {
        DataColumn[] key = new DataColumn[1];
        Control_TKNV tknv = new Control_TKNV();
        Control_NhanVien nv = new Control_NhanVien();
        string table = "TAIKHOAN";
        public TaiKhoanNhanVien_DanhMuc()
        {
            InitializeComponent();
            dgv_tknv.AllowUserToAddRows = false;
            dgv_tknv.ReadOnly = true;
            btn_luu.Enabled = false;
        }
        void AddHeader()
        {
            dgv_tknv.Columns.Clear();
            dgv_tknv.Columns.Add("ma_nhanvien", "Mã Nhân viên");
            dgv_tknv.Columns[0].DataPropertyName = "ma_nhanvien";
            dgv_tknv.Columns.Add("tendangnhap", "Tên dăng nhập");
            dgv_tknv.Columns[1].DataPropertyName = "tendangnhap";
            dgv_tknv.Columns.Add("matkhau", "Mật khẩu");
            dgv_tknv.Columns[2].DataPropertyName = "matkhau";
        }
        void loadDTG()
        {
            if (dgv_tknv.DataSource != null)
                dgv_tknv.Rows.Clear();
            DataTable dtTKNV = tknv.select(table);
            dgv_tknv.DataSource = dtTKNV;
            key[0] = dtTKNV.Columns[0];
            dtTKNV.PrimaryKey = key;
            dgv_tknv.RowTemplate.Height = 30;
        }
        void loadAllTKNV()
        {
            AddHeader();
            loadDTG();
            Theater();
        }
        private void Theater()
        {
            dgv_tknv.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dgv_tknv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv_tknv.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            int totalWidth = dgv_tknv.Width - dgv_tknv.RowHeadersWidth;
            int columnWidth = totalWidth / 3;
            dgv_tknv.Columns[0].Width = columnWidth;
            dgv_tknv.Columns[1].Width = columnWidth;
            dgv_tknv.Columns[2].Width = columnWidth;
        }
        private void loadNhanVien()
        {
            string table = "NHANVIEN";
            DataTable dtNhanvien = nv.select(table);
            cbb_manv.DataSource = dtNhanvien;
            cbb_manv.DisplayMember = "ma_nhanvien";
            cbb_manv.ValueMember = "ma_nhanvien";
        }
        private void TaiKhoanNhanVien_DanhMuc_Load(object sender, EventArgs e)
        {
            loadAllTKNV();
            loadNhanVien();
        }

        private void btn_them_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(tb_tendn.Text) || string.IsNullOrWhiteSpace(tb_passnv.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra độ dài mật khẩu tối thiểu 6 ký tự
                if (tb_passnv.Text.Length < 6)
                {
                    MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Model_TKNV newtknv = new Model_TKNV();
                newtknv.manavi = cbb_manv.SelectedValue.ToString();
                newtknv.tdn = tb_tendn.Text;
                newtknv.matk = tb_passnv.Text;

                if (tknv.checkTrungMa(newtknv.manavi, table) == 1)
                {
                    MessageBox.Show("Mỗi nhân viên chỉ được có 1 tài khoản duy nhất!");
                    tb_tendn.Clear();
                    tb_passnv.Clear();
                    tb_tendn.Focus();
                    return;
                }

                if (tknv.checkTrungTenDN(newtknv.tdn, table) == 1)
                {
                    MessageBox.Show("Trùng tên đăng nhập rồi");
                    return;
                }

                tknv.insert(newtknv, table);
                tb_tendn.Clear();
                tb_passnv.Clear();
                tb_tendn.Focus();
                MessageBox.Show("Tạo tài khoản cho nhân viên mới thành công!");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex != null ? ex.Message : "Lỗi rồi !");
            }
        }

        private void dgv_tknv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgv_tknv.Rows[e.RowIndex].Selected = true;
                DataGridViewRow row = dgv_tknv.Rows[e.RowIndex];
                dgv_tknv.AllowUserToAddRows = false;
                dgv_tknv.ReadOnly = true;
                string maNhanVien = row.Cells["ma_nhanvien"].Value.ToString();
                cbb_manv.SelectedValue = maNhanVien;

                tb_tendn.Text = row.Cells["tendangnhap"].Value.ToString();
                tb_passnv.Text = row.Cells["matkhau"].Value.ToString();
            }
        }

        private void dgv_tknv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (dgv_tknv.CurrentRow != null)
            //{
            //    cbb_manv.SelectedValue = dgv_tknv.CurrentRow.Cells[0].Value.ToString();
            //    tb_tendn.Text = dgv_tknv.CurrentRow.Cells[1].Value.ToString();
            //    tb_passnv.Text = dgv_tknv.CurrentRow.Cells[2].Value.ToString();

            //}
        }

        private void dgv_tknv_SelectionChanged(object sender, EventArgs e)
        {
            //if (dgv_tknv.CurrentRow != null)
            //{
            //    cbb_manv.SelectedValue = dgv_tknv.CurrentRow.Cells[0].Value.ToString();
            //    tb_tendn.Text = dgv_tknv.CurrentRow.Cells[1].Value.ToString();
            //    tb_passnv.Text = dgv_tknv.CurrentRow.Cells[2].Value.ToString();

            //}
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                Model_TKNV newtknv = new Model_TKNV();
                newtknv.manavi = cbb_manv.SelectedValue.ToString();
                newtknv.tdn = tb_tendn.Text;
                newtknv.matk = tb_passnv.Text;

                if (newtknv.tdn.Trim() == "thanhnhan11")
                {
                    MessageBox.Show("Bạn không thể xóa Admin!");
                    return;
                }
                else
                {

                    if (tknv.checkTrungMa(newtknv.manavi, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản của nhân viên '{newtknv.manavi}'?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            if (newtknv.manavi == "NV001")
                            {
                                MessageBox.Show("Bạn không thể xóa Admin!");
                                return;
                            }
                            tknv.delete(newtknv, table);
                            MessageBox.Show("Xóa Tài khoản nhân viên thành công!");
                            tb_tendn.Clear();
                            tb_passnv.Clear();
                            tb_tendn.Focus();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Mã nhân viên không tồn tại hoặc không thể xóa tài khoản Admin. Vui lòng kiểm tra lại");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Lỗi rồi nha ");
            }
        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                Model_TKNV newtknv = new Model_TKNV();
                newtknv.manavi = cbb_manv.SelectedValue.ToString();
                newtknv.tdn = tb_tendn.Text;
                newtknv.matk = tb_passnv.Text;
                if (tknv.checkTrungMa(newtknv.manavi, table) == 1)
                {
                    btn_luu.Enabled = true;
                    dgv_tknv.ReadOnly = false;
                }
                else
                {
                    MessageBox.Show("Mã tài khoản nhân viên không tồn tại. Hoặc bạn chưa chon mã nhã Nhân viên!!!");
                }
            }
            catch
            {
                MessageBox.Show("Chưa có mã Nhân viên");
            }
        }

        private void btn_luu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(tb_tendn.Text) || string.IsNullOrWhiteSpace(tb_passnv.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Model_TKNV newtknv = new Model_TKNV();
                newtknv.manavi = cbb_manv.SelectedValue.ToString();
                newtknv.tdn = tb_tendn.Text;
                newtknv.matk = tb_passnv.Text;
                if (tknv.checkTrungMa(newtknv.manavi, table) == 1)
                {
                    if (tknv.checkTrungTenDN(newtknv.tdn, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"Trùng tên Nhà Cung Cấp '{newtknv.tdn}'. Bạn có muốn sửa không ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            tknv.update(newtknv, table);
                            MessageBox.Show("Sửa thành công!");
                            tb_tendn.Clear();
                            tb_passnv.Clear();
                            btn_luu.Enabled = false;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    tknv.update(newtknv, table);
                    MessageBox.Show("Sửa thành công!");
                    tb_tendn.Clear();
                    tb_passnv.Clear();
                    btn_luu.Enabled = false;
                    return;
                }
                else
                {
                    MessageBox.Show("Mã Nhân viên không tồn tại. Hoặc bạn chọn Mã Nhân viên!!!");
                }
            }
            catch
            {
                MessageBox.Show("Mã Nhân viên không tồn tại!");
            }
        }
    }
}
