using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class NhanVien_DanhMuc : Form
    {
        DataColumn[] key = new DataColumn[1];
        Control_NhanVien nv = new Control_NhanVien();
        string table = "NHANVIEN";
        public NhanVien_DanhMuc()
        {
            InitializeComponent();
            dgv_nv.AllowUserToAddRows = false;
            dgv_nv.ReadOnly = true;
            btn_luu.Enabled = false;
            cbb_gioitinh.Items.Add("Nam");
            cbb_gioitinh.Items.Add("Nữ");
            cbb_gioitinh.Items.Add("Khác");
            cbb_gioitinh.SelectedIndex = 0;
            cbb_quyennv.Items.Add("Nhanvien");
            cbb_quyennv.Items.Add("Admin");
            cbb_quyennv.SelectedIndex = 0;
            cbb_quyennv.Enabled = false;
            tb_manv.Enabled = false;

        }
        void AddHeader()
        {
            dgv_nv.Columns.Clear();
            dgv_nv.Columns.Add("ma_nhanvien", "Mã NV");
            dgv_nv.Columns[0].DataPropertyName = "ma_nhanvien";
            dgv_nv.Columns.Add("hoten_nhanvien", "Họ tên NV");
            dgv_nv.Columns[1].DataPropertyName = "hoten_nhanvien";
            dgv_nv.Columns.Add("diachi_nhanvien", "Địa chỉ");
            dgv_nv.Columns[2].DataPropertyName = "diachi_nhanvien";
            dgv_nv.Columns.Add("sdt_nhanvien", "SĐT NV");
            dgv_nv.Columns[3].DataPropertyName = "sdt_nhanvien";
            dgv_nv.Columns.Add("email_nhanvien", "Email");
            dgv_nv.Columns[4].DataPropertyName = "email_nhanvien";
            dgv_nv.Columns.Add("cccd_nhanvien", "CCCD");
            dgv_nv.Columns[5].DataPropertyName = "cccd_nhanvien";
            dgv_nv.Columns.Add("ngaysinh_nhanvien", "Ngày sinh");
            dgv_nv.Columns[6].DataPropertyName = "ngaysinh_nhanvien";
            dgv_nv.Columns[6].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgv_nv.Columns.Add("gioitinh_nhanvien", "Giới tính");
            dgv_nv.Columns[7].DataPropertyName = "gioitinh_nhanvien";
            dgv_nv.Columns.Add("anh_nhanvien", "Ảnh nhân viên");
            dgv_nv.Columns[8].DataPropertyName = "anh_nhanvien";
            dgv_nv.Columns.Add("quyen_nhanvien", "Quyền");
            dgv_nv.Columns[9].DataPropertyName = "quyen_nhanvien";

        }
        void loadDTG()
        {
            DataTable dtNV = nv.select(table);
            if (dgv_nv.DataSource != null)
            {
                // Xóa dữ liệu cũ
                dgv_nv.DataSource = null;
                dgv_nv.Rows.Clear();
            }
            dgv_nv.DataSource = dtNV;
            key[0] = dtNV.Columns[0];
            dtNV.PrimaryKey = key;
            dgv_nv.RowTemplate.Height = 30;
        }
        void loadAllNV()
        {
            AddHeader();
            loadDTG();
            Theater();
        }
        private void Theater()
        {
            dgv_nv.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dgv_nv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv_nv.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dgv_nv.Columns[0].Width = 100;
            dgv_nv.Columns[1].Width = 100;
            dgv_nv.Columns[2].Width = 100;
            dgv_nv.Columns[3].Width = 100;
            dgv_nv.Columns[4].Width = 100;
            dgv_nv.Columns[5].Width = 100;
            dgv_nv.Columns[6].Width = 100;
            dgv_nv.Columns[7].Width = 100;
            dgv_nv.Columns[8].Width = 100;
            dgv_nv.Columns[9].Width = 100;
        }
        private void btn_them_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(tb_hoten.Text) || string.IsNullOrWhiteSpace(tb_diachi.Text) ||
                    string.IsNullOrWhiteSpace(tb_sdt.Text) || string.IsNullOrWhiteSpace(tb_email.Text) ||
                    string.IsNullOrWhiteSpace(tb_cccd.Text) || string.IsNullOrWhiteSpace(tb_tenfile.Text) ||
                    pb_anhnv.Image == null || cbb_gioitinh.SelectedIndex == -1 || cbb_quyennv.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem tên nhân viên không được chứa số
                if (tb_hoten.Text.Any(char.IsDigit))
                {
                    MessageBox.Show("Tên nhân viên không được chứa số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem số điện thoại Nhân viên có phải là số không, có đúng 10 hoặc 11 số, và bắt đầu từ số 0
                if (!Regex.IsMatch(tb_sdt.Text, @"^0\d{9,10}$"))
                {
                    MessageBox.Show("Số điện thoại Nhân viên phải là số, có 10 hoặc 11 số, và bắt đầu từ số 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem CCCD có đủ 12 số và không chứa ký tự
                if (!Regex.IsMatch(tb_cccd.Text, @"^\d{12}$"))
                {
                    MessageBox.Show("CCCD phải là số và có đủ 12 số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem ngày sinh có bé hơn ngày hiện tại không
                DateTime ngaySinh = dt_ngaysinh.Value;
                if (ngaySinh >= DateTime.Today)
                {
                    MessageBox.Show("Ngày sinh phải bé hơn ngày hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                tb_email.Text = tb_email.Text.Trim();
                // Kiểm tra định dạng email
                try
                {
                    var addr = new System.Net.Mail.MailAddress(tb_email.Text);
                    if (addr.Address != tb_email.Text)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    MessageBox.Show("Email không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Model_NhanVien newnv = new Model_NhanVien();
                newnv.maNV = tb_manv.Text;
                newnv.tenNV = tb_hoten.Text;
                newnv.dchi = tb_diachi.Text;
                newnv.sdt = tb_sdt.Text;
                newnv.eemail = tb_email.Text;
                newnv.cccd = tb_cccd.Text;
                if (DateTime.TryParse(dt_ngaysinh.Text, out DateTime birthDate))
                {
                    newnv.birthday = birthDate;
                }
                else
                {
                    MessageBox.Show("Ngày sinh không hợp lệ.");
                    return;
                }
                newnv.gtinh = cbb_gioitinh.SelectedItem.ToString();
                string tenfile = tb_tenfile.Text;
                string[] nameParts = tenfile.Split(' ');
                string imageName = string.Join("_", nameParts) + ".jpg";
                string imagePath = Path.Combine(Application.StartupPath, @"D:\Final\Total\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\AnhTheNV\", imageName);

                // Kiểm tra xem tên file ảnh đã tồn tại hay chưa
                if (File.Exists(imagePath))
                {
                    MessageBox.Show("Tên file ảnh đã tồn tại. Vui lòng nhập tên file khác.");
                    return;
                }

                pb_anhnv.Image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                newnv.anhnv = imageName;
                newnv.qnv = cbb_quyennv.SelectedItem.ToString();
                if (nv.checkTrungSDT(newnv.sdt, table) == 1)
                {
                    MessageBox.Show("Số điện thoại đã có từ trước!");
                    return;
                }
                if (nv.checkTrungCCCD(newnv.cccd, table) == 1)
                {
                    MessageBox.Show("Trùng CCCD rồi!");
                    return;
                }
                if (nv.checkTrungEmail(newnv.eemail, table) == 1)
                {
                    MessageBox.Show("Trùng Email rồi!");
                    return;
                }
                nv.insert(newnv, table);
                MessageBox.Show("Thêm Nhân viên mới thành công!");
                tb_manv.Clear();
                tb_hoten.Clear();
                tb_diachi.Clear();
                tb_sdt.Clear();
                tb_email.Clear();
                tb_cccd.Clear();
                tb_tenfile.Clear();
                pb_anhnv.Image = null;
                tb_hoten.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex != null ? ex.Message : "Lỗi rồi !");
            }
        }

        private void NhanVien_DanhMuc_Load(object sender, EventArgs e)
        {
            loadAllNV();
        }

        private void dgv_nv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgv_nv.Rows[e.RowIndex].Selected = true;
                DataGridViewRow row = dgv_nv.Rows[e.RowIndex];
                dgv_nv.AllowUserToAddRows = false;
                dgv_nv.ReadOnly = true;
                tb_manv.Text = row.Cells["ma_nhanvien"].Value.ToString();
                tb_hoten.Text = row.Cells["hoten_nhanvien"].Value.ToString();
                tb_diachi.Text = row.Cells["diachi_nhanvien"].Value.ToString();
                tb_sdt.Text = row.Cells["sdt_nhanvien"].Value.ToString();
                tb_email.Text = row.Cells["email_nhanvien"].Value.ToString();
                tb_cccd.Text = row.Cells["cccd_nhanvien"].Value.ToString();
                dt_ngaysinh.Text = row.Cells["ngaysinh_nhanvien"].Value.ToString();
                string gioiTinh = row.Cells["gioitinh_nhanvien"].Value.ToString();
                cbb_gioitinh.SelectedItem = gioiTinh;
                //pb_anhnv.Text = row.Cells["anh_nhanvien"].Value.ToString();
                string imageName = dgv_nv.CurrentRow.Cells["anh_nhanvien"].Value.ToString();
                pb_anhnv.Text = imageName;
                string imagePath = Path.Combine(Application.StartupPath, @"D:\Final\Total\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\AnhTheNV\", imageName);
                if (File.Exists(imagePath))
                {
                    try
                    {
                        pb_anhnv.Image = Image.FromFile(imagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}");
                        pb_anhnv.Image = null;
                    }
                }
                else
                {
                    pb_anhnv.Image = null;
                }
                string quyenNV = row.Cells["quyen_nhanvien"].Value.ToString();
                cbb_quyennv.SelectedItem = quyenNV;
                tb_tenfile.Text = row.Cells["anh_nhanvien"].Value.ToString();
            }
        }

        private void dgv_nv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (dgv_nv.CurrentRow != null)
            //{
            //    tb_manv.Text = dgv_nv.CurrentRow.Cells[0].Value.ToString();
            //    tb_hoten.Text = dgv_nv.CurrentRow.Cells[1].Value.ToString();
            //    tb_diachi.Text = dgv_nv.CurrentRow.Cells[2].Value.ToString();
            //    tb_sdt.Text = dgv_nv.CurrentRow.Cells[3].Value.ToString();
            //    tb_email.Text = dgv_nv.CurrentRow.Cells[4].Value.ToString();
            //    tb_cccd.Text = dgv_nv.CurrentRow.Cells[5].Value.ToString();
            //    dt_ngaysinh.Text = dgv_nv.CurrentRow.Cells[6].Value.ToString();
            //    cbb_gioitinh.Text = dgv_nv.CurrentRow.Cells[7].Value.ToString();
            //    string imageName = dgv_nv.CurrentRow.Cells[8].Value.ToString();

            //    // Tải ảnh từ thư mục AnhTheNV và hiển thị trong PictureBox
            //    string imagePath = Path.Combine(Application.StartupPath, @"D:\KhoaLuanKySu\FORM\Nam_KLKS_2605_19h39p\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\AnhTheNV", imageName);
            //    if (File.Exists(imagePath))
            //    {
            //        try
            //        {
            //            pb_anhnv.Image = Image.FromFile(imagePath);
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}");
            //            pb_anhnv.Image = null;
            //        }
            //    }
            //    else
            //    {
            //        pb_anhnv.Image = null;
            //    }
            //    cbb_quyennv.Text = dgv_nv.CurrentRow.Cells[9].Value.ToString();
            //}
        }

        private void btn_openfile_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Choose Image(*.JPG; *.PNG; *.GIF)|*.jpg; *.png; *.gif";

            if (opf.ShowDialog() == DialogResult.OK)
            {
                pb_anhnv.Image = Image.FromFile(opf.FileName);
            }
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                Model_NhanVien newnv = new Model_NhanVien();
                newnv.maNV = tb_manv.Text;
                newnv.tenNV = tb_hoten.Text;
                newnv.dchi = tb_diachi.Text;
                newnv.sdt = tb_sdt.Text;
                newnv.eemail = tb_email.Text;
                newnv.cccd = tb_cccd.Text;
                if (DateTime.TryParse(dt_ngaysinh.Text, out DateTime birthDate))
                {
                    newnv.birthday = birthDate;
                }
                else
                {
                    MessageBox.Show("Ngày sinh không hợp lệ.");
                    return;
                }
                newnv.gtinh = cbb_gioitinh.SelectedItem.ToString();
                newnv.anhnv = pb_anhnv.Text;
                newnv.qnv = cbb_quyennv.SelectedItem.ToString();

                // Hiển thị thông báo xác nhận xóa Nhân viên
                DialogResult confirmDelete = MessageBox.Show($"Bạn có chắc chắn muốn xóa Nhân viên '{newnv.tenNV}'?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmDelete == DialogResult.Yes)
                {
                    // Xóa file ảnh của nhân viên
                    string imagePath = Path.Combine(@"D:\Final\Total\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\AnhTheNV\", newnv.anhnv); // Tạo đường dẫn đầy đủ cho file ảnh
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                        Console.WriteLine($"Đã xóa file ảnh: {imagePath}");
                    }

                    if (nv.checkTrungMa(newnv.maNV, table) == 1)
                    {
                        if (newnv.qnv == "Admin")
                        {
                            MessageBox.Show("Bạn không thể xóa chính mình!");
                            return;
                        }
                        nv.delete(newnv, table);
                        MessageBox.Show("Xóa Nhân viên thành công !!!");
                        tb_manv.Clear();
                        tb_hoten.Clear();
                        tb_diachi.Clear();
                        tb_sdt.Clear();
                        tb_email.Clear();
                        tb_cccd.Clear();
                        pb_anhnv.Image = null;
                        tb_hoten.Focus();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Mã Nhân viên không tồn tại. Hoặc bạn chưa nhập mã Nhân viên!!!");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Không thể xóa vì tài khoản nhân viên còn tồn tại!");
                loadDTG();
            }
        }

        private void dgv_nv_SelectionChanged(object sender, EventArgs e)
        {
            //if (dgv_nv.CurrentRow != null)
            //{
            //    tb_manv.Text = dgv_nv.CurrentRow.Cells[0].Value.ToString();
            //    tb_hoten.Text = dgv_nv.CurrentRow.Cells[1].Value.ToString();
            //    tb_diachi.Text = dgv_nv.CurrentRow.Cells[2].Value.ToString();
            //    tb_sdt.Text = dgv_nv.CurrentRow.Cells[3].Value.ToString();
            //    tb_email.Text = dgv_nv.CurrentRow.Cells[4].Value.ToString();
            //    tb_cccd.Text = dgv_nv.CurrentRow.Cells[5].Value.ToString();
            //    dt_ngaysinh.Text = dgv_nv.CurrentRow.Cells[6].Value.ToString();
            //    cbb_gioitinh.Text = dgv_nv.CurrentRow.Cells[7].Value.ToString();
            //    string imageName = dgv_nv.CurrentRow.Cells[8].Value.ToString();

            //    // Tải ảnh từ thư mục AnhTheNV và hiển thị trong PictureBox
            //    string imagePath = Path.Combine(Application.StartupPath, @"D:\KhoaLuanKySu\FORM\Nam_KLKS_2605_19h39p\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\AnhTheNV", imageName);
            //    if (File.Exists(imagePath))
            //    {
            //        try
            //        {
            //            pb_anhnv.Image = Image.FromFile(imagePath);
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}");
            //            pb_anhnv.Image = null;
            //        }
            //    }
            //    else
            //    {
            //        pb_anhnv.Image = null;
            //    }
            //    cbb_quyennv.Text = dgv_nv.CurrentRow.Cells[9].Value.ToString();
            //}
        }

        private void btn_tao_Click(object sender, EventArgs e)
        {
            TaiKhoanNhanVien_DanhMuc frmTKNV = new TaiKhoanNhanVien_DanhMuc();
            hienthi(frmTKNV);
        }
        private void hienthi(Form form)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(form);
            this.panel1.Tag = form;

            form.BringToFront();
            form.Show();
        }

        private void btn_luu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(tb_hoten.Text) || string.IsNullOrWhiteSpace(tb_diachi.Text) ||
                    string.IsNullOrWhiteSpace(tb_sdt.Text) || string.IsNullOrWhiteSpace(tb_email.Text) ||
                    string.IsNullOrWhiteSpace(tb_cccd.Text) || string.IsNullOrWhiteSpace(tb_tenfile.Text) ||
                    pb_anhnv.Image == null || cbb_gioitinh.SelectedIndex == -1 || cbb_quyennv.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem tên nhân viên không được chứa số
                if (tb_hoten.Text.Any(char.IsDigit))
                {
                    MessageBox.Show("Tên nhân viên không được chứa số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem số điện thoại Nhân viên có phải là số không, có đúng 10 số, và bắt đầu từ số 0
                if (!Regex.IsMatch(tb_sdt.Text, @"^0\d{9}$"))
                {
                    MessageBox.Show("Số điện thoại Nhân viên phải là số, có 10 số, và bắt đầu từ số 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem CCCD có đủ 12 số và không chứa ký tự
                if (!Regex.IsMatch(tb_cccd.Text, @"^\d{12}$"))
                {
                    MessageBox.Show("CCCD phải là số và có đủ 12 số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem ngày sinh có bé hơn ngày hiện tại không
                DateTime ngaySinh = dt_ngaysinh.Value;
                if (ngaySinh >= DateTime.Today)
                {
                    MessageBox.Show("Ngày sinh phải bé hơn ngày hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                tb_email.Text = tb_email.Text.Trim();
                // Kiểm tra định dạng email
                try
                {
                    var addr = new System.Net.Mail.MailAddress(tb_email.Text);
                    if (addr.Address != tb_email.Text)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    MessageBox.Show("Email không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                tb_tenfile.Enabled = true;
                Model_NhanVien newnv = new Model_NhanVien();
                newnv.maNV = tb_manv.Text;
                newnv.tenNV = tb_hoten.Text;
                newnv.dchi = tb_diachi.Text;
                newnv.sdt = tb_sdt.Text;
                newnv.eemail = tb_email.Text;
                newnv.cccd = tb_cccd.Text;
                if (DateTime.TryParse(dt_ngaysinh.Text, out DateTime birthDate))
                {
                    newnv.birthday = birthDate;
                }
                else
                {
                    MessageBox.Show("Ngày sinh không hợp lệ.");
                    return;
                }
                newnv.gtinh = cbb_gioitinh.SelectedItem.ToString();
                string tenfile = tb_tenfile.Text;
                string[] nameParts = tenfile.Split(' ');
                string imageName = string.Join("_", nameParts) + ".jpg";
                string imagePath = Path.Combine(Application.StartupPath, @"D:\Final\Total\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\AnhTheNV\", imageName);

                // Kiểm tra xem tên file ảnh đã tồn tại hay chưa
                if (File.Exists(imagePath))
                {
                    MessageBox.Show("Tên file ảnh đã tồn tại. Vui lòng nhập tên file khác.");
                    return;
                }

                pb_anhnv.Image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                newnv.anhnv = imageName;
                newnv.qnv = cbb_quyennv.SelectedItem.ToString();
                if (nv.checkTrungMa(newnv.maNV, table) == 1)
                {
                    if (nv.checkTrungCCCD(newnv.cccd, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"CCCD Nhân viên '{newnv.cccd}' đã tồn tại. Bạn có muốn sửa không ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            nv.update(newnv, table);
                            MessageBox.Show("Sửa Nhân viên thành công!");
                            ClearFields();
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (nv.checkTrungSDT(newnv.sdt, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"SĐT Nhân viên '{newnv.sdt}' đã tồn tại. Bạn có muốn sửa không ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            nv.update(newnv, table);
                            MessageBox.Show("Sửa Nhân viên thành công!");
                            ClearFields();
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (nv.checkTrungEmail(newnv.eemail, table) == 1)
                    {
                        DialogResult result = MessageBox.Show($"Email Nhân viên '{newnv.eemail}' đã tồn tại. Bạn có muốn sửa không ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            nv.update(newnv, table);
                            MessageBox.Show("Sửa Nhân viên thành công!");
                            ClearFields();
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    nv.update(newnv, table);
                    MessageBox.Show("Sửa Nhân viên thành công!");
                    ClearFields();
                    return;
                }
                else
                {
                    MessageBox.Show("Mã Nhân viên không tồn tại. Hoặc bạn chưa nhập mã Nhân viên!!!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mã Nhân viên không tồn tại!");
            }
        }
        private void ClearFields()
        {
            tb_manv.Clear();
            tb_hoten.Clear();
            tb_diachi.Clear();
            tb_sdt.Clear();
            tb_email.Clear();
            tb_cccd.Clear();
            tb_tenfile.Clear();
            pb_anhnv.Image = null;
            tb_hoten.Focus();
        }
        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                tb_tenfile.Enabled = false;
                Model_NhanVien newnv = new Model_NhanVien();
                newnv.maNV = tb_manv.Text;
                newnv.tenNV = tb_hoten.Text;
                newnv.dchi = tb_diachi.Text;
                newnv.sdt = tb_sdt.Text;
                newnv.eemail = tb_email.Text;
                newnv.cccd = tb_cccd.Text;
                if (DateTime.TryParse(dt_ngaysinh.Text, out DateTime birthDate))
                {
                    newnv.birthday = birthDate;
                }
                else
                {
                    MessageBox.Show("Ngày sinh không hợp lệ.");
                    return;
                }
                newnv.gtinh = cbb_gioitinh.SelectedItem.ToString();
                newnv.anhnv = pb_anhnv.Text;
                newnv.qnv = cbb_quyennv.SelectedItem.ToString();
                newnv.anhnv = tb_tenfile.Text;
                if (nv.checkTrungMa(newnv.maNV, table) == 1)
                {
                    btn_luu.Enabled = true;
                    dgv_nv.ReadOnly = false;
                }
                else
                {
                    MessageBox.Show("Mã Nhân viên không tồn tại. Hoặc bạn chưa nhập mã nhân viên!!!");
                }
            }
            catch
            {
                MessageBox.Show("Chưa có mã Nhân viên");
            }
        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btn_timkiem_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = tb_timkiem.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    DataTable dtNV = nv.FindMa(searchText, table);
                    if (dtNV.Rows.Count > 0)
                    {
                        dgv_nv.DataSource = dtNV;
                        key[0] = dtNV.Columns[0];
                        dtNV.PrimaryKey = key;

                        // Hiển thị thông báo khi tìm kiếm thành công
                        MessageBox.Show($"Đã tìm thấy {dtNV.Rows.Count} nhân viên có từ khóa '{searchText}'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void cbb_gioitinh_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
