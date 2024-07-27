using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class Voucher_DanhMuc : Form
    {
        DataColumn[] key = new DataColumn[1];
        Control_Voucher voc = new Control_Voucher();
        string table = "VOUCHER";
        public Voucher_DanhMuc()
        {
            InitializeComponent();
            dgv_voc.AllowUserToAddRows = false;
            dgv_voc.ReadOnly = true;
            btn_luu.Enabled = false;
            tb_mavoc.Enabled = false;
        }
        void AddHeader()
        {
            dgv_voc.Columns.Clear();
            dgv_voc.Columns.Add("ma_voucher", "Mã Voucher");
            dgv_voc.Columns[0].DataPropertyName = "ma_voucher";
            dgv_voc.Columns.Add("ten_voucher", "Tên Voucher");
            dgv_voc.Columns[1].DataPropertyName = "ten_voucher";
            dgv_voc.Columns.Add("ngaybatdau_voucher", "Ngày bắt đầu");
            dgv_voc.Columns[2].DataPropertyName = "ngaybatdau_voucher";
            dgv_voc.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgv_voc.Columns.Add("ngayketthuc_voucher", "Ngày kết thúc");
            dgv_voc.Columns[3].DataPropertyName = "ngayketthuc_voucher";
            dgv_voc.Columns[3].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgv_voc.Columns.Add("giamgia_voucher", "Giảm giá");
            dgv_voc.Columns[4].DataPropertyName = "giamgia_voucher";
        }
        void loadDTG()
        {
            if (dgv_voc.DataSource != null)
                dgv_voc.Rows.Clear();
            DataTable dtVOC = voc.select(table);
            dgv_voc.DataSource = dtVOC;
            key[0] = dtVOC.Columns[0];
            dtVOC.PrimaryKey = key;
            dgv_voc.RowTemplate.Height = 30;
        }
        void loadAllVoC()
        {
            AddHeader();
            loadDTG();
            Theater();
        }
        private void Theater()
        {
            dgv_voc.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dgv_voc.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv_voc.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            int totalWidth = dgv_voc.Width - dgv_voc.RowHeadersWidth;
            int columnWidth = totalWidth / 5;
            dgv_voc.Columns[0].Width = columnWidth;
            dgv_voc.Columns[1].Width = columnWidth;
            dgv_voc.Columns[2].Width = columnWidth;
            dgv_voc.Columns[3].Width = columnWidth;
            dgv_voc.Columns[4].Width = columnWidth;
        }
        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                Model_Voucher newvoc = new Model_Voucher();
                newvoc.mavoc = tb_mavoc.Text;
                newvoc.tenvoc = tb_tenvoc.Text;
                if (DateTime.TryParse(dt_ngaybd.Text, out DateTime ngaybatd))
                {
                    newvoc.ngaybatd = ngaybatd;
                }
                else
                {
                    newvoc.ngaybatd = DateTime.MinValue;
                }

                if (DateTime.TryParse(dt_ngaykt.Text, out DateTime ngaykett))
                {
                    newvoc.ngaykett = ngaykett;
                }
                else
                {
                    newvoc.ngaykett = DateTime.MinValue;
                }
                if (float.TryParse(tb_giamgia.Text, out float ggvc))
                {
                    newvoc.ggvc = ggvc;
                }
                else
                {
                    newvoc.ggvc = 0f;
                }
                if (voc.checkTrungMa(newvoc.mavoc, table) == 1)
                {
                    btn_luu.Enabled = true;
                    dgv_voc.ReadOnly = false;
                }
                else
                {
                    MessageBox.Show("Mã Voucher không tồn tại. Hoặc bạn chưa nhập mã Voucher!!!");
                }
            }
            catch
            {
                MessageBox.Show("Chưa có mã Voucher");
            }
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                Model_Voucher newvoc = new Model_Voucher();
                newvoc.mavoc = tb_mavoc.Text;
                newvoc.tenvoc = tb_tenvoc.Text;
                if (DateTime.TryParse(dt_ngaybd.Text, out DateTime ngaybatd))
                {
                    newvoc.ngaybatd = ngaybatd;
                }
                else
                {
                    newvoc.ngaybatd = DateTime.MinValue;
                }

                if (DateTime.TryParse(dt_ngaykt.Text, out DateTime ngaykett))
                {
                    newvoc.ngaykett = ngaykett;
                }
                else
                {
                    newvoc.ngaykett = DateTime.MinValue;
                }
                if (float.TryParse(tb_giamgia.Text, out float ggvc))
                {
                    newvoc.ggvc = ggvc;
                }
                else
                {
                    newvoc.ggvc = 0f;
                }
                if (voc.checkTrungMa(newvoc.mavoc, table) == 1)
                {
                    // Hiển thị thông báo xác nhận xóa Voucher
                    DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa Voucher '{newvoc.tenvoc}'?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        voc.delete(newvoc, table);
                        MessageBox.Show("Xóa Voucher thành công rồi!");
                        tb_mavoc.Clear();
                        tb_tenvoc.Clear();
                        tb_giamgia.Clear();
                        tb_tenvoc.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Mã Voucher không tồn tại. Hoặc bạn chưa nhập mã Voucher!!!");
                }
            }
            catch
            {
                MessageBox.Show("Lỗi rồi nha ");
            }
            return;
        }

        private void Voucher_DanhMuc_Load(object sender, EventArgs e)
        {
            loadAllVoC();
        }

        private void dgv_voc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgv_voc.Rows[e.RowIndex].Selected = true;
                DataGridViewRow row = dgv_voc.Rows[e.RowIndex];
                dgv_voc.AllowUserToAddRows = false;
                dgv_voc.ReadOnly = true;
                tb_mavoc.Text = row.Cells["ma_voucher"].Value.ToString();
                tb_tenvoc.Text = row.Cells["ten_voucher"].Value.ToString();
                dt_ngaybd.Text = row.Cells["ngaybatdau_voucher"].Value.ToString();
                dt_ngaykt.Text = row.Cells["ngayketthuc_voucher"].Value.ToString();
                tb_giamgia.Text = row.Cells["giamgia_voucher"].Value.ToString();
            }
        }

        private void dgv_voc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (dgv_voc.CurrentRow != null)
            //{
            //    tb_mavoc.Text = dgv_voc.CurrentRow.Cells[0].Value.ToString();
            //    tb_tenvoc.Text = dgv_voc.CurrentRow.Cells[1].Value.ToString();
            //    dt_ngaybd.Text = dgv_voc.CurrentRow.Cells[2].Value.ToString();
            //    dt_ngaykt.Text = dgv_voc.CurrentRow.Cells[3].Value.ToString();
            //    tb_giamgia.Text = dgv_voc.CurrentRow.Cells[4].Value.ToString();

            //}
        }

        private void dgv_voc_SelectionChanged(object sender, EventArgs e)
        {
            //if (dgv_voc.CurrentRow != null)
            //{
            //    tb_mavoc.Text = dgv_voc.CurrentRow.Cells[0].Value.ToString();
            //    tb_tenvoc.Text = dgv_voc.CurrentRow.Cells[1].Value.ToString();
            //    dt_ngaybd.Text = dgv_voc.CurrentRow.Cells[2].Value.ToString();
            //    dt_ngaykt.Text = dgv_voc.CurrentRow.Cells[3].Value.ToString();
            //    tb_giamgia.Text = dgv_voc.CurrentRow.Cells[4].Value.ToString();

            //}
        }

        private void btn_them_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(tb_tenvoc.Text) || string.IsNullOrWhiteSpace(tb_giamgia.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem giá giảm có phải là số không
                if (!float.TryParse(tb_giamgia.Text, out float giamGia))
                {
                    MessageBox.Show("Giảm giá voucher phải là số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem giá giảm có lớn hơn 0 không
                if (giamGia <= 0)
                {
                    MessageBox.Show("Giảm giá voucher phải lớn hơn 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra ngày bắt đầu và ngày kết thúc
                DateTime ngayBatDau = dt_ngaybd.Value;
                DateTime ngayKetThuc = dt_ngaykt.Value;
                DateTime ngayHienTai = DateTime.Today;

                if (ngayKetThuc <= ngayHienTai)
                {
                    MessageBox.Show("Ngày kết thúc phải lớn hơn ngày hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ngayBatDau <= ngayHienTai || ngayBatDau >= ngayKetThuc)
                {
                    MessageBox.Show("Ngày bắt đầu phải lớn hơn ngày hiện tại và bé hơn ngày kết thúc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo mới voucher
                Model_Voucher newvoc = new Model_Voucher
                {
                    mavoc = tb_mavoc.Text,
                    tenvoc = tb_tenvoc.Text,
                    ngaybatd = ngayBatDau,
                    ngaykett = ngayKetThuc,
                    ggvc = (float)Math.Round(giamGia, 3, MidpointRounding.ToEven)
                };

                // Kiểm tra trùng tên voucher
                if (voc.checkTrungTen(newvoc.tenvoc, table) == 1)
                {
                    MessageBox.Show("Trùng tên Voucher có từ trước. Vui lòng nhập lại tên Voucher mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xóa nội dung các textbox và focus vào tên voucher
                tb_mavoc.Clear();
                tb_tenvoc.Clear();
                tb_giamgia.Clear();
                tb_tenvoc.Focus();

                // Thêm voucher mới vào bảng
                voc.insert(newvoc, table);

                MessageBox.Show("Thêm Voucher mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex != null ? ex.Message : "Lỗi rồi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_luu_Click(object sender, EventArgs e)
        {
            try
            {
                Model_Voucher newvoc = new Model_Voucher();
                newvoc.mavoc = tb_mavoc.Text;
                newvoc.tenvoc = tb_tenvoc.Text;
                if (DateTime.TryParse(dt_ngaybd.Text, out DateTime ngaybatd))
                {
                    newvoc.ngaybatd = ngaybatd;
                }
                else
                {
                    newvoc.ngaybatd = DateTime.MinValue;
                }

                if (DateTime.TryParse(dt_ngaykt.Text, out DateTime ngaykett))
                {
                    newvoc.ngaykett = ngaykett;
                }
                else
                {
                    newvoc.ngaykett = DateTime.MinValue;
                }
                if (float.TryParse(tb_giamgia.Text, out float ggvc))
                {
                    newvoc.ggvc = ggvc;
                }
                else
                {
                    newvoc.ggvc = 0f;
                }
                if (voc.checkTrungMa(newvoc.mavoc, table) == 1)
                {
                    if (voc.checkTrungTen(newvoc.tenvoc, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"Tên voucher '{newvoc.tenvoc}' đã tồn tại. Bạn có muốn sửa không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                            if (string.IsNullOrWhiteSpace(tb_tenvoc.Text) || string.IsNullOrWhiteSpace(tb_giamgia.Text))
                            {
                                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            if (!float.TryParse(tb_giamgia.Text, out float giamGia) || giamGia <= 0)
                            {
                                MessageBox.Show("Giảm giá voucher phải là số và lớn hơn 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }


                            DateTime ngayBatDau = dt_ngaybd.Value;
                            DateTime ngayKethuc = dt_ngaykt.Value;

                            if (ngayBatDau >= ngayKethuc)
                            {
                                MessageBox.Show("Ngày bắt đầu phải bé hơn ngày kết thúc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (ngayKethuc < DateTime.Today)
                            {
                                MessageBox.Show("Ngày kết thúc phải hơn hoặc bằng ngày hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            voc.update(newvoc, table);
                            MessageBox.Show("Sửa Voucher thành công!");
                            tb_mavoc.Clear();
                            tb_tenvoc.Clear();
                            tb_giamgia.Clear();
                            tb_tenvoc.Focus();
                            btn_luu.Enabled = false;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    voc.update(newvoc, table);
                    MessageBox.Show("Sửa Voucher thành công!");
                    tb_mavoc.Clear();
                    tb_tenvoc.Clear();
                    tb_giamgia.Clear();
                    tb_tenvoc.Focus();
                    btn_luu.Enabled = false;
                    return;
                }
                else
                {
                    MessageBox.Show("Mã Voucher không tồn tại. Hoặc bạn chưa nhập mã Voucher!!!");
                }
            }
            catch
            {
                MessageBox.Show("Mã Voucher không tồn tại!");
            }
        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
