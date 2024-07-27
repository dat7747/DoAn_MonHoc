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
    public partial class frm_SetTrangThaiTiem : Form
    {
        Control_TrangthaitiemVaccine control = new Control_TrangthaitiemVaccine();
        string table = "TIEMVACCINE_MUI";
        public frm_SetTrangThaiTiem()
        {
            InitializeComponent();
            SetupComboBox();
            LoadData();
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
                DataTable dt = control.select(table);
                dataGridView1.DataSource = dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void frm_SetTrangThaiTiem_Load(object sender, EventArgs e)
        {
            LoadData();
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[1].Width = 60;
            dataGridView1.Columns[2].Width = 150;
            dataGridView1.Columns[3].Width = 150;
            dataGridView1.Columns[4].Width = 60;
            dataGridView1.Columns[5].Width = 70;
            dataGridView1.Columns[6].Width = 60;
            dataGridView1.Columns[7].Width = 80;
            dataGridView1.Columns[7].DefaultCellStyle.Format = "dd/MM/yyyy";
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Theater();
        }
        private Dictionary<int, string> statusMap = new Dictionary<int, string>
        {
            { 0, "Chưa tiêm" },
            { 1, "Đã tiêm" }
        };
        private void SetupComboBox()
        {
            // Gán nguồn dữ liệu cho ComboBox từ Dictionary
            comboBox1.DataSource = new BindingSource(statusMap, null);
            comboBox1.DisplayMember = "Value"; // Hiển thị giá trị được chọn
            comboBox1.ValueMember = "Key"; // Giá trị thực sự được chọn
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy giá trị trạng thái đã tiêm từ dòng được chọn
                string trangThaiTiem = dataGridView1.Rows[e.RowIndex].Cells["Trạng thái tiêm"].Value.ToString();

                // Kiểm tra giá trị trạng thái đã tiêm và thiết lập giá trị tương ứng cho combobox
                if (trangThaiTiem == "Đã tiêm")
                {
                    comboBox1.SelectedIndex = 1; // Chọn giá trị "Đã tiêm" trong combobox
                }
                else
                {
                    comboBox1.SelectedIndex = 0; // Chọn giá trị "Chưa tiêm" trong combobox
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Trạng thái tiêm")
            {
                if (e.Value != null && e.Value.ToString() == "Chưa tiêm")
                {
                    // Thiết lập màu đỏ cho trạng thái chưa tiêm
                    e.CellStyle.BackColor = Color.Red;
                    // Thiết lập màu chữ cho trạng thái chưa tiêm
                    e.CellStyle.ForeColor = Color.White;
                    // Thiết lập kiểu font cho trạng thái chưa tiêm
                    e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
                else
                {
                    // Thiết lập kiểu font cho trạng thái đã tiêm
                    e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);

                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            try
            {
                // Lấy từ khóa từ textBox1
                string keyword = textBox1.Text.Trim();

                // Tìm kiếm dữ liệu dựa trên từ khóa
                DataTable dt = control.SearchData(table, keyword);

                // Hiển thị kết quả tìm kiếm trong dataGridView1
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnluu_Click(object sender, EventArgs e)
        {


            // Kiểm tra xem đã chọn hàng trong DataGridView chưa
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Lấy giá trị trạng thái hiện tại từ cột "Trạng thái tiêm" của hàng được chọn
                string trangThaiTiem = dataGridView1.SelectedRows[0].Cells["Trạng thái tiêm"].Value.ToString();

                // Kiểm tra xem trạng thái hiện tại có khác với giá trị mới từ combobox hay không
                int newValuee = (int)comboBox1.SelectedValue;
                if (trangThaiTiem == statusMap[newValuee])
                {
                    MessageBox.Show("Trạng thái tiêm không thay đổi.");
                    return; // Không cần thực hiện lưu trạng thái mới
                }

                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có muốn lưu thay đổi không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Lấy thông tin từ DataGridView
                    string maDangKy = dataGridView1.SelectedRows[0].Cells["STT"].Value.ToString();
                    int soMuiTiem = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Mũi vaccine"].Value);
                    DateTime? ngayTiem = dataGridView1.SelectedRows[0].Cells["Ngày tiêm"].Value as DateTime?;

                    // Kiểm tra giá trị từ combobox
                    int newValue = (int)comboBox1.SelectedValue;
                    if (newValue == 1) // Nếu chọn "Đã tiêm"
                    {
                        // Kiểm tra xem có đủ điều kiện để set trạng thái đã tiêm hay không
                        if (CanSetDaTiem(maDangKy, soMuiTiem))
                        {
                            // Lấy ngày tiêm mới là ngày hệ thống
                            ngayTiem = DateTime.Now;

                            try
                            {
                                // Gọi phương thức cập nhật trạng thái đã tiêm và ngày tiêm
                                control.UpdateStatus(maDangKy, newValue, ngayTiem.Value);

                                // Hiển thị thông báo thành công
                                MessageBox.Show("Cập nhật trạng thái tiêm thành công.");

                                // Reload dữ liệu sau khi cập nhật
                                LoadData();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi: " + ex.Message);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Các mũi tiêm phải cách nhau ít nhất 1 tháng để được chuyển sang trạng thái đã tiêm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (newValue == 0) // Nếu chọn "Chưa tiêm"
                    {
                        // Gọi phương thức cập nhật trạng thái chưa tiêm và ngày tiêm = null
                        ngayTiem = null;

                        try
                        {
                            // Gọi phương thức cập nhật trạng thái chưa tiêm
                            control.UpdateStatus(maDangKy, newValue, ngayTiem);

                            // Hiển thị thông báo thành công
                            MessageBox.Show("Cập nhật trạng thái tiêm thành công.");

                            // Reload dữ liệu sau khi cập nhật
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi: " + ex.Message);
                        }
                    }
                }

                else
                {
                    // Người dùng đã chọn "No", hiển thị thông báo "Bạn đã hủy"
                    MessageBox.Show("Bạn đã hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để cập nhật trạng thái tiêm.");
            }
        }

        private bool CanSetDaTiem(string maDangKy, int soMuiTiem)
        {
            if (soMuiTiem <= 1)
            {
                return true; // Nếu chỉ có 1 mũi tiêm, luôn có thể cập nhật trạng thái đã tiêm
            }

            // Lấy ngày muốn tiêm từ bảng NGUOITIEM_DANGKY
            DateTime? ngayMuonTiem = control.GetNgayMuonTiem(maDangKy);

            // Nếu không có ngày muốn tiêm, không thể kiểm tra điều kiện
            if (!ngayMuonTiem.HasValue)
            {
                return false;
            }

            // Lấy ngày tiêm của mũi tiêm cuối cùng
            DateTime? ngayTiemCuoi = control.GetNgayTiemCuoi(maDangKy);

            // Nếu không tìm thấy ngày tiêm của mũi tiêm cuối cùng
            if (!ngayTiemCuoi.HasValue)
            {
                return false;
            }

            // Kiểm tra ngày tiêm cuối cùng có lớn hơn hoặc bằng ngày muốn tiêm không
            if (ngayTiemCuoi.Value >= ngayMuonTiem.Value)
            {
                return false;
            }

            // Kiểm tra khoảng cách thời gian từ ngày tiêm cuối cùng đến ngày hiện tại
            DateTime ngayTiemHienTai = DateTime.Now;
            TimeSpan timeSpan = ngayTiemHienTai - ngayTiemCuoi.Value;

            // Kiểm tra khoảng cách thời gian có lớn hơn 1 tháng hay không
            return timeSpan.TotalDays >= 30;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
