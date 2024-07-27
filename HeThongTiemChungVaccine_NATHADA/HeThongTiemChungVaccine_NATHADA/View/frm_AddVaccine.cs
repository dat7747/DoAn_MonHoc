using HeThongTiemChungVaccine_NATHADA.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class frm_AddVaccine : Form
    {
        private Control_Vaccine control = new Control_Vaccine();
        private Control_LoaiVC lvc = new Control_LoaiVC();

        public frm_AddVaccine()
        {
            InitializeComponent();         

        }
        private void LoadLoaiVaccine()
        {
            string table = "LOAIVACCINE";
            DataTable dtLoaiVaccine = lvc.select(table);
            cbbmaloaivaccine.DataSource = dtLoaiVaccine;
            cbbmaloaivaccine.DisplayMember = "ten_loaivaccine"; // Hiển thị tên loại vaccine
            cbbmaloaivaccine.ValueMember = "ma_loaivaccine";    // Lấy giá trị mã loại vaccine
        }
        private void btnhuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public class ComboBoxItem
        {
            public string DisplayText { get; set; }
            public int Value { get; set; }
        }

        // Sau đó, thiết lập các mục và thêm chúng vào ComboBox trong form của bạn
        private void SetupComboBox()
        {
            // Tạo danh sách các mục
            List<ComboBoxItem> items = new List<ComboBoxItem>
            {
                new ComboBoxItem { DisplayText = "Dành cho trên 1 tuổi", Value = 1 },
                new ComboBoxItem { DisplayText = "Dành cho trên 3 tuổi", Value = 3 },
                new ComboBoxItem { DisplayText = "Dành cho trên 9 tuổi", Value = 9 },
                new ComboBoxItem { DisplayText = "Dành cho trên 15 tuổi", Value = 15 },
                new ComboBoxItem { DisplayText = "Dành cho trên 18 tuổi", Value = 18 }
            };

            // Thiết lập DataSource, DisplayMember và ValueMember của ComboBox
            txtnote.DataSource = items;
            txtnote.DisplayMember = "DisplayText";
            txtnote.ValueMember = "Value";
        }
        private void frm_AddVaccine_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void frm_AddVaccine_Load(object sender, EventArgs e)
        {
            LoadLoaiVaccine();
            SetupComboBox();
        }

        private void cbbmaloaivaccine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbmaloaivaccine.SelectedValue != null)
            {
                string maLoaiVaccine = cbbmaloaivaccine.SelectedValue.ToString();
            }
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem tất cả các trường thông tin có được nhập đầy đủ không
                if (string.IsNullOrWhiteSpace(txttenvaccine.Text) || string.IsNullOrWhiteSpace(txtanhvaccine.Text) ||
                    string.IsNullOrWhiteSpace(txtthongtinvaccine.Text) || string.IsNullOrWhiteSpace(txtdoituong.Text) ||
                    string.IsNullOrWhiteSpace(txtphacdo.Text) || string.IsNullOrWhiteSpace(txttinhtrang.Text) ||
                    string.IsNullOrWhiteSpace(txtgiavaccine.Text) || cbbmaloaivaccine.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem giá vaccine có phải là số không
                if (!float.TryParse(txtgiavaccine.Text, out float giaVaccine))
                {
                    MessageBox.Show("Giá vaccine phải là số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem giá vaccine có lớn hơn 0 không
                if (giaVaccine <= 0)
                {
                    MessageBox.Show("Giá vaccine phải lớn hơn 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem ngày sản xuất có lớn hơn hạn sử dụng không
                DateTime ngaySanXuat = dtpkngaysanxuat.Value;
                DateTime hanSuDung = dtpkhansudung.Value;
                if (ngaySanXuat > hanSuDung)
                {
                    MessageBox.Show("Ngày sản xuất không được lớn hơn hạn sử dụng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem ngày hết hạn có bé hơn ngày hiện tại không
                if (hanSuDung < DateTime.Today)
                {
                    MessageBox.Show("Ngày hết hạn phải lớn hơn hoặc bằng ngày hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lưu ảnh vào đường dẫn cần thiết
                string imageName = Path.GetFileName(txtanhvaccine.Text);
                string sourcePath = Path.GetDirectoryName(txtanhvaccine.Text);
                string destinationPath = @"D:\KhoaLuanKySu\WEB\Dat_New_1606_17h45p\New11\Web_TiemChungVaccine\Web_TiemChungVaccine\Web_TiemChungVaccine\Web_TiemChungVaccine\Images\" + imageName;
                File.Copy(Path.Combine(sourcePath, imageName), destinationPath, true);

                // Thêm Vaccine
                control.AddNewVaccine(txttenvaccine.Text, cbbmaloaivaccine.SelectedValue.ToString(), imageName,
                    txtthongtinvaccine.Text, txtdoituong.Text, txtphacdo.Text, txttinhtrang.Text, giaVaccine,
                    ngaySanXuat, hanSuDung, txtnote.SelectedValue.ToString(), txtphongbenh.Text, txtnguongoc.Text);

                MessageBox.Show("Thêm Vaccine thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Xóa nội dung
                ClearTextBoxes();
                btnbrowse.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm Vaccine mới: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";
            openFileDialog.Title = "Chọn ảnh";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedImagePath = openFileDialog.FileName;
                txtanhvaccine.Text = selectedImagePath; // Gán đường dẫn đầy đủ của tệp vào txtanhvaccine.Text
            }
            btnbrowse.Enabled = false;
        }

        private void ClearTextBoxes()
        {
            txttenvaccine.Clear();
            txtanhvaccine.Clear();
            txtthongtinvaccine.Clear();
            txtdoituong.Clear();
            txtphacdo.Clear();
            txttinhtrang.Clear();
            txtgiavaccine.Clear();
            txtphongbenh.Clear();
            txtnguongoc.Clear();
        }

    }
}
