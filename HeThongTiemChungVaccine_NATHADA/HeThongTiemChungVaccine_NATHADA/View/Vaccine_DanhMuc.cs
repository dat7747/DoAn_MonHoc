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

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class Vaccine_DanhMuc : Form
    {
        private frm_AddVaccine frmAddVaccine;
        Control_Vaccine controlVaccine = new Control_Vaccine();
        public Vaccine_DanhMuc()
        {
            InitializeComponent();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;          
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Vaccine_DanhMuc_Load(object sender, EventArgs e)
        {
            LoadData();
            AddHeader();
            Theater();
            btnluu.Enabled = false;
        }
        private void Theater()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Tahoma"), 12f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[0].Width = 50;
        }
        private void LoadData()
        {
            try
            {
                DataTable dt = controlVaccine.Select();
                dataGridView1.DataSource = dt;
                // Tùy chọn hiển thị khác nếu cần
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }
        void AddHeader()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ma_vaccine", "Mã Vaccine");
            dataGridView1.Columns[0].DataPropertyName = "ma_vaccine";

            dataGridView1.Columns.Add("ten_vaccine", "Tên Vaccine");
            dataGridView1.Columns[1].DataPropertyName = "ten_vaccine";

            dataGridView1.Columns.Add("anh_vaccine", "Ảnh");
            dataGridView1.Columns[2].DataPropertyName = "anh_vaccine";

            dataGridView1.Columns.Add("thongtin_vaccine", "Thông tin");
            dataGridView1.Columns[3].DataPropertyName = "thongtin_vaccine";

            dataGridView1.Columns.Add("doituong", "Đối tượng");
            dataGridView1.Columns[4].DataPropertyName = "doituong";

            dataGridView1.Columns.Add("phacdolichtiem", "Phác đổ lịch tiêm");
            dataGridView1.Columns[5].DataPropertyName = "phacdolichtiem";

            dataGridView1.Columns.Add("tinhtrangvaccine", "Tình trạng");
            dataGridView1.Columns[6].DataPropertyName = "tinhtrangvaccine";

            dataGridView1.Columns.Add("gia_vacine", "Giá");
            dataGridView1.Columns[7].DataPropertyName = "gia_vacine";

            dataGridView1.Columns.Add("ngay_san_xuat", "Ngày sản xuất");
            dataGridView1.Columns[8].DataPropertyName = "ngay_san_xuat";

            dataGridView1.Columns.Add("hansudung_vaccine", "Hạn sử dụng");
            dataGridView1.Columns[9].DataPropertyName = "hansudung_vaccine";

            dataGridView1.Columns.Add("note", "Note");
            dataGridView1.Columns[10].DataPropertyName = "note";

            dataGridView1.Columns.Add("phongbenh", "Phòng bệnh");
            dataGridView1.Columns[11].DataPropertyName = "phongbenh";

            dataGridView1.Columns.Add("nguongoc", "Nguồn gốc");
            dataGridView1.Columns[12].DataPropertyName = "nguongoc";

            dataGridView1.Columns.Add("ten_loaivaccine", "Tên loại Vaccine ");
            dataGridView1.Columns[13].DataPropertyName = "ten_loaivaccine";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txttenvaccine.Text = row.Cells["ten_vaccine"].Value.ToString();
                txtgia.Text = row.Cells["gia_vacine"].Value.ToString();
                txthansudung.Text = Convert.ToDateTime(row.Cells["hansudung_vaccine"].Value).ToString("yyyy-MM-dd");
                txtnguongoc.Text = row.Cells["nguongoc"].Value.ToString();
                txtphongbenh.Text = row.Cells["phongbenh"].Value.ToString();
                txtdoituong.Text = row.Cells["doituong"].Value.ToString();
            }
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            if (frmAddVaccine == null || frmAddVaccine.IsDisposed)
            {
                frmAddVaccine = new frm_AddVaccine();
                frmAddVaccine.Show();
            }
            else
            {
                frmAddVaccine.Activate();
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string maVaccine = dataGridView1.SelectedRows[0].Cells["ma_vaccine"].Value.ToString();

                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa Vaccine này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        controlVaccine.XoaVaccine(maVaccine);

                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa Vaccine: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một Vaccine để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AllowEditing()
        {
            if (dataGridView1.Columns.Contains("ma_vaccine"))
            {
                dataGridView1.Columns["ma_vaccine"].ReadOnly = true;
            }

            // Kiểm tra cột "ten_loaivaccine"
            if (dataGridView1.Columns.Contains("ten_loaivaccine"))
            {
                dataGridView1.Columns["ten_loaivaccine"].ReadOnly = true;
            }

            // Kiểm tra cột "anh_vaccine"
            if (dataGridView1.Columns.Contains("anh_vaccine"))
            {
                dataGridView1.Columns["anh_vaccine"].ReadOnly = true;
            }

            // Kiểm tra cột "anh_vaccine"
            if (dataGridView1.Columns.Contains("anh_vaccine"))
            {
                dataGridView1.Columns["anh_vaccine"].ReadOnly = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly=false;
            btnluu.Enabled = true;
            AllowEditing();
        }

        private void btnluu_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ DataGridView và kiểm tra các giá trị hợp lệ
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Selected && !row.IsNewRow)
                    {
                        string maVaccine = row.Cells["ma_vaccine"].Value.ToString();
                        string tenVaccine = row.Cells["ten_vaccine"].Value.ToString();
                        string anhVaccine = row.Cells["anh_vaccine"].Value.ToString();
                        string thongTinVaccine = row.Cells["thongtin_vaccine"].Value.ToString();
                        string doiTuong = row.Cells["doituong"].Value.ToString();
                        string phacDo = row.Cells["phacdolichtiem"].Value.ToString();
                        string tinhTrang = row.Cells["tinhtrangvaccine"].Value.ToString();
                        string giaVaccineStr = row.Cells["gia_vacine"].Value.ToString();
                        string ngaySanXuatStr = row.Cells["ngay_san_xuat"].Value.ToString();
                        string hanSuDungStr = row.Cells["hansudung_vaccine"].Value.ToString();
                        string note = row.Cells["note"].Value.ToString();
                        string phongBenh = row.Cells["phongbenh"].Value.ToString();
                        string nguonGoc = row.Cells["nguongoc"].Value.ToString();

                        // Kiểm tra các trường không được trống
                        if (string.IsNullOrWhiteSpace(tenVaccine) || string.IsNullOrWhiteSpace(anhVaccine) || string.IsNullOrWhiteSpace(thongTinVaccine) ||
                            string.IsNullOrWhiteSpace(doiTuong) || string.IsNullOrWhiteSpace(phacDo) || string.IsNullOrWhiteSpace(tinhTrang) ||
                            string.IsNullOrWhiteSpace(giaVaccineStr) || string.IsNullOrWhiteSpace(ngaySanXuatStr) || string.IsNullOrWhiteSpace(hanSuDungStr) ||
                            string.IsNullOrWhiteSpace(note) || string.IsNullOrWhiteSpace(phongBenh) || string.IsNullOrWhiteSpace(nguonGoc))
                        {
                            MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Kiểm tra giá vaccine phải là số
                        if (!float.TryParse(giaVaccineStr, out float giaVaccine))
                        {
                            MessageBox.Show("Giá vaccine phải là số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Kiểm tra ngày sản xuất phải bé hơn hạn sử dụng
                        if (!DateTime.TryParse(ngaySanXuatStr, out DateTime ngaySanXuat) || !DateTime.TryParse(hanSuDungStr, out DateTime hanSuDung))
                        {
                            MessageBox.Show("Ngày sản xuất và hạn sử dụng phải là ngày hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (ngaySanXuat >= hanSuDung)
                        {
                            MessageBox.Show("Ngày sản xuất phải bé hơn hạn sử dụng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Kiểm tra hạn sử dụng không được bé hơn ngày hiện tại
                        if (hanSuDung < DateTime.Now)
                        {
                            MessageBox.Show("Hạn sử dụng không được bé hơn ngày hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Gọi phương thức UpdateVaccine từ Control_Vaccine
                        controlVaccine.UpdateVaccine(maVaccine, tenVaccine, anhVaccine, thongTinVaccine, doiTuong, phacDo, tinhTrang, giaVaccine, ngaySanXuat, hanSuDung, note, phongBenh, nguonGoc);
                    }
                }

                MessageBox.Show("Cập nhật Vaccine thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Đặt DataGridView về chế độ đọc và vô hiệu hóa nút Lưu
                dataGridView1.ReadOnly = false;
                btnluu.Enabled = false;

                // Tải lại dữ liệu
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật Vaccine: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
            string value = e.FormattedValue.ToString();

            // Kiểm tra nếu cột là "ngay_san_xuat" hoặc "hansudung_vaccine"
            if (columnName == "ngay_san_xuat" || columnName == "hansudung_vaccine")
            {
                // Kiểm tra giá trị ngày tháng
                if (!DateTime.TryParse(value, out DateTime dateValue))
                {
                    // Hiển thị thông báo lỗi bằng MessageBox
                    MessageBox.Show("Ngày tháng không hợp lệ. Vui lòng nhập lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Hủy bỏ việc di chuyển khỏi ô này
                }
            }
            else if (columnName == "gia_vacine")
            {
                // Kiểm tra giá vaccine phải là số và lớn hơn 0
                if (!float.TryParse(value, out float result) || result <= 0)
                {
                    // Hiển thị thông báo lỗi bằng MessageBox
                    MessageBox.Show("Giá vaccine phải là số và lớn hơn 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Hủy bỏ việc di chuyển khỏi ô này
                }
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        private void LoadData(string tenVaccine)
        {
            try
            {
                DataTable dt = controlVaccine.SearchVaccineByName(tenVaccine);
                dataGridView1.DataSource = dt; // Hiển thị kết quả lên DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txttiemkiem_TextChanged(object sender, EventArgs e)
        {
            LoadData(txttiemkiem.Text.Trim());
        }
    }
}
