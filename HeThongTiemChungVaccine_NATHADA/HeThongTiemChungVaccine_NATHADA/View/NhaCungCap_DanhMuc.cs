using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class NhaCungCap_DanhMuc : Form
    {
        DataColumn[] key = new DataColumn[1];
        Control_NhaCungCap ncc = new Control_NhaCungCap();
        string table = "NHACUNGCAP";
        public NhaCungCap_DanhMuc()
        {
            InitializeComponent();
            dgv_ncc.AllowUserToAddRows = false;
            dgv_ncc.ReadOnly = true;
            btn_luu.Enabled = false;
            tb_mancc.Enabled = false;
        }
        void AddHeader()
        {
            dgv_ncc.Columns.Clear();
            dgv_ncc.Columns.Add("ma_nhacungcap", "Mã Nhà cung cấp");
            dgv_ncc.Columns[0].DataPropertyName = "ma_nhacungcap";
            dgv_ncc.Columns.Add("ten_nhacungcap", "Tên Nhà cung cấp");
            dgv_ncc.Columns[1].DataPropertyName = "ten_nhacungcap";
            dgv_ncc.Columns.Add("diachi_nhacungcap", "Địa chỉ Nhà cung cấp");
            dgv_ncc.Columns[2].DataPropertyName = "diachi_nhacungcap";
            dgv_ncc.Columns.Add("sdt_nhacungcap", "SĐT Nhà cung cấp");
            dgv_ncc.Columns[3].DataPropertyName = "sdt_nhacungcap";
        }
        void loadDTG()
        {
            DataTable dtNCC = ncc.select(table);
            if (dgv_ncc.DataSource != null)
            {
                // Xóa dữ liệu cũ
                dgv_ncc.DataSource = null;
                dgv_ncc.Rows.Clear();
            }
            dgv_ncc.DataSource = dtNCC;
            key[0] = dtNCC.Columns[0];
            dtNCC.PrimaryKey = key;
            dgv_ncc.RowTemplate.Height = 30;
        }
        void loadAllNhaCC()
        {
            AddHeader();
            loadDTG();
            Theater();
        }
        private void Theater()
        {
            dgv_ncc.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dgv_ncc.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv_ncc.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            int totalWidth = dgv_ncc.Width - dgv_ncc.RowHeadersWidth;
            int columnWidth = totalWidth / 4;
            dgv_ncc.Columns[0].Width = columnWidth;
            dgv_ncc.Columns[1].Width = columnWidth;
            dgv_ncc.Columns[2].Width = columnWidth;
            dgv_ncc.Columns[3].Width = columnWidth;
        }
        private void btn_them_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(tb_tenncc.Text) || string.IsNullOrWhiteSpace(tb_diachincc.Text) ||
                    string.IsNullOrWhiteSpace(tb_sdtncc.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem số điện thoại Nhà cung cấp có phải là số không
                if (!float.TryParse(tb_sdtncc.Text, out _))
                {
                    MessageBox.Show("Số điện thoại Nhà cung cấp phải là số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Kiểm tra xem số điện thoại Nhà cung cấp 10 đến 11 số, bắt đầu bằng số 0 và không được nhập số âm
                if (tb_sdtncc.Text.Length < 10 || tb_sdtncc.Text.Length > 11 || !tb_sdtncc.Text.StartsWith("0") || int.TryParse(tb_sdtncc.Text, out int phoneNumber) && phoneNumber < 0)
                {
                    MessageBox.Show("Số điện thoại Nhà cung cấp phải từ 10 đến 11 số, bắt đầu bằng số 0 và không được nhập số âm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Model_NhaCungCap newncc = new Model_NhaCungCap();
                //newncc.mancc = tb_mancc.Text;
                newncc.tenncc = tb_tenncc.Text;
                newncc.dcncc = tb_diachincc.Text;
                newncc.sdtncc = tb_sdtncc.Text;
                if (ncc.checkTrungTen(newncc.tenncc, table) == 1)
                {
                    MessageBox.Show("Trùng tên Nhà Cung Cấp có từ trước. Vui lòng nhập lại tên Nhà Cung Cấp mới!");
                    return;
                }
                else if (ncc.checkTrungSDT(newncc.sdtncc, table) == 1)
                {
                    MessageBox.Show("Số điện thoại đã tồn tại. Vui lòng nhập lại Só điện thoại của Nhà Cung Cấp mới!");
                    return;
                }
                ncc.insert(newncc, table);
                MessageBox.Show("Thêm Nhà Cung Cấp thành công rồi!");
                tb_mancc.Clear();
                tb_tenncc.Clear();
                tb_diachincc.Clear();
                tb_sdtncc.Clear();
                tb_mancc.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex != null ? ex.Message : "Lỗi rồi !");
            }
        }

        private void NhaCungCap_DanhMuc_Load(object sender, EventArgs e)
        {
            loadAllNhaCC();
        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_ncc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (dgv_ncc.CurrentRow != null)
            //{
            //    tb_mancc.Text = dgv_ncc.CurrentRow.Cells[0].Value.ToString();
            //    tb_tenncc.Text = dgv_ncc.CurrentRow.Cells[1].Value.ToString();
            //    tb_diachincc.Text = dgv_ncc.CurrentRow.Cells[2].Value.ToString();
            //    tb_sdtncc.Text = dgv_ncc.CurrentRow.Cells[3].Value.ToString();

            //}
        }

        private void dgv_ncc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgv_ncc.Rows[e.RowIndex].Selected = true;
                DataGridViewRow row = dgv_ncc.Rows[e.RowIndex];
                dgv_ncc.AllowUserToAddRows = false;
                dgv_ncc.ReadOnly = true;
                tb_mancc.Text = row.Cells["ma_nhacungcap"].Value.ToString();
                tb_tenncc.Text = row.Cells["ten_nhacungcap"].Value.ToString();
                tb_diachincc.Text = row.Cells["diachi_nhacungcap"].Value.ToString();
                tb_sdtncc.Text = row.Cells["sdt_nhacungcap"].Value.ToString();
            }
        }

        private void dgv_ncc_SelectionChanged(object sender, EventArgs e)
        {
            //if (dgv_ncc.CurrentRow != null)
            //{
            //    tb_mancc.Text = dgv_ncc.CurrentRow.Cells[0].Value.ToString();
            //    tb_tenncc.Text = dgv_ncc.CurrentRow.Cells[1].Value.ToString();
            //    tb_diachincc.Text = dgv_ncc.CurrentRow.Cells[2].Value.ToString();
            //    tb_sdtncc.Text = dgv_ncc.CurrentRow.Cells[3].Value.ToString();

            //}
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                Model_NhaCungCap newncc = new Model_NhaCungCap();
                newncc.mancc = tb_mancc.Text;
                newncc.tenncc = tb_tenncc.Text;
                newncc.dcncc = tb_sdtncc.Text;
                newncc.sdtncc = tb_sdtncc.Text;
                if (ncc.checkTrungMa(newncc.mancc, table) == 1)
                {
                    // Hiển thị thông báo xác nhận xóa nhà cung cấp
                    DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhà cung cấp '{newncc.tenncc}'?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ncc.delete(newncc, table);
                        MessageBox.Show("Xóa thành công rồi nha!");
                        tb_mancc.Clear();
                        tb_tenncc.Clear();
                        tb_diachincc.Clear();
                        tb_sdtncc.Clear();
                        tb_mancc.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Mã Nhà cung cấp không tồn tại. Hoặc bạn chưa nhập mã Nhà cung cấp!!!");
                }
            }
            catch
            {
                MessageBox.Show("Không thể xóa do có tham chiếu. Hãy làm mới lại dữ liệu!!!");
                loadDTG();
            }
        }

        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                Model_NhaCungCap newncc = new Model_NhaCungCap();
                newncc.mancc = tb_mancc.Text;
                newncc.tenncc = tb_tenncc.Text;
                newncc.dcncc = tb_diachincc.Text;
                newncc.sdtncc = tb_sdtncc.Text;
                if (ncc.checkTrungMa(newncc.mancc, table) == 1)
                {
                    btn_luu.Enabled = true;
                    dgv_ncc.ReadOnly = false;
                }
                else
                {
                    MessageBox.Show("Mã Nhà cung cấp không tồn tại. Hoặc bạn chưa nhập mã Nhà cung cấp!!!");
                }
            }
            catch
            {
                MessageBox.Show("Chưa có mã Nhà cung cấp");
            }
        }

        private void btn_luu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(tb_tenncc.Text) || string.IsNullOrWhiteSpace(tb_diachincc.Text) ||
                    string.IsNullOrWhiteSpace(tb_sdtncc.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem số điện thoại Nhà cung cấp có phải là số không
                if (!Regex.IsMatch(tb_sdtncc.Text, @"^0\d{9}$"))
                {
                    MessageBox.Show("Số điện thoại Nhà cung cấp phải bắt đầu bằng số 0 và có đúng 10 chữ số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Model_NhaCungCap newncc = new Model_NhaCungCap();
                newncc.mancc = tb_mancc.Text;
                newncc.tenncc = tb_tenncc.Text;
                newncc.dcncc = tb_diachincc.Text;
                newncc.sdtncc = tb_sdtncc.Text;
                if (ncc.checkTrungMa(newncc.mancc, table) == 1)
                {
                    if (ncc.checkTrungTen(newncc.tenncc, table) == 1 && ncc.checkTrungSDT(newncc.sdtncc, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"Trùng tên Nhà Cung Cấp '{newncc.tenncc}' và Số điện thoại '{newncc.sdtncc}' đã tồn tại. Bạn có muốn sửa không ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            ncc.update(newncc, table);
                            MessageBox.Show("Sửa thành công!");
                            tb_mancc.Clear();
                            tb_tenncc.Clear();
                            tb_diachincc.Clear();
                            tb_sdtncc.Clear();
                            tb_mancc.Focus();
                            btn_luu.Enabled = false;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (ncc.checkTrungTen(newncc.tenncc, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"Trùng tên Nhà Cung Cấp '{newncc.tenncc}' có từ trước. Bạn có muốn sửa tên không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            ncc.update(newncc, table);
                            MessageBox.Show("Sửa thành công!");
                            tb_mancc.Clear();
                            tb_tenncc.Clear();
                            tb_diachincc.Clear();
                            tb_sdtncc.Clear();
                            tb_mancc.Focus();
                            btn_luu.Enabled = false;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (ncc.checkTrungSDT(newncc.sdtncc, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"Số điện thoại '{newncc.sdtncc}' đã tồn tại. Bạn có muốn sửa số điện thoại không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            ncc.update(newncc, table);
                            MessageBox.Show("Sửa thành công!");
                            tb_mancc.Clear();
                            tb_tenncc.Clear();
                            tb_diachincc.Clear();
                            tb_sdtncc.Clear();
                            tb_mancc.Focus();
                            btn_luu.Enabled = false;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    ncc.update(newncc, table);
                    MessageBox.Show("Sửa thành công!");
                    tb_mancc.Clear();
                    tb_tenncc.Clear();
                    tb_diachincc.Clear();
                    tb_sdtncc.Clear();
                    tb_mancc.Focus();
                    btn_luu.Enabled = false;
                    return;
                }
                else
                {
                    MessageBox.Show("Mã Nhà cung cấp không tồn tại. Hoặc bạn chưa nhập mã Nhà cung cấp!!!");
                }
            }
            catch
            {
                MessageBox.Show("Mã Nhà cung cấp không tồn tại!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgv_ncc.SelectedRows.Count > 0)
            {
                // Lấy mã nhà cung cấp từ dòng được chọn
                string maNhaCungCap = dgv_ncc.SelectedRows[0].Cells["ma_nhacungcap"].Value.ToString();

                // Mở form frm_Vaccine_NCC và truyền mã nhà cung cấp
                frm_Vaccine_NCC formVaccineNCC = new frm_Vaccine_NCC(maNhaCungCap);
                formVaccineNCC.Show();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhà cung cấp.");
            }
        }

        private void tb_tenncc_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_timkiem_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = tb_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    DataTable dtNCC = ncc.FindMa(searchText, table);
                    if (dtNCC.Rows.Count > 0)
                    {
                        dgv_ncc.DataSource = dtNCC;
                        key[0] = dtNCC.Columns[0];
                        dtNCC.PrimaryKey = key;

                        // Hiển thị thông báo khi tìm kiếm thành công
                        MessageBox.Show($"Đã tìm thấy {dtNCC.Rows.Count} nhà cung cấp có từ khóa '{searchText}'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
