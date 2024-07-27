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
        }
        void loadAllLoaiVC()
        {
            AddHeader();
            loadDTG();
        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void btn_them_Click(object sender, EventArgs e)
        {
            try
            {
                Model_LoaiVaccine newlvc = new Model_LoaiVaccine();
                newlvc.maLoai = tb_maloai.Text;
                newlvc.tenLoai = tb_tenloai.Text;
                if (lvc.checkTrungMa(newlvc.maLoai, table) == 1)
                {
                    MessageBox.Show("Trùng mã loại vaccine có từ trước!");
                    return;
                }
                lvc.insert(newlvc, table);
                MessageBox.Show("Thêm loại vaccine thành công rồi nha!");
                tb_maloai.Clear();
                tb_tenloai.Clear();
                tb_maloai.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex != null ? ex.Message : "Lỗi rồi !");
            }
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            Model_LoaiVaccine newlvc = new Model_LoaiVaccine();
            newlvc.maLoai = tb_maloai.Text;
            newlvc.tenLoai = tb_tenloai.Text;
            if (lvc.checkTrungMa(newlvc.maLoai, table) == 1)
            {
                lvc.delete(newlvc, table);
                MessageBox.Show("Xóa thành công rồi nha!");
                tb_maloai.Clear();
                tb_tenloai.Clear();
                tb_maloai.Focus();
                return;
            }
            else
            {
                MessageBox.Show("Lỗi rồi nha ");
            }
        }

        private void LoaiVaccine_DanhMuc_Load(object sender, EventArgs e)
        {
            loadAllLoaiVC();
        }

        private void dgv_lvc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_lvc.CurrentRow != null)
            {
                tb_maloai.Text = dgv_lvc.CurrentRow.Cells[0].Value.ToString();
                tb_tenloai.Text = dgv_lvc.CurrentRow.Cells[1].Value.ToString();

            }
        }

        private void btn_sua_Click(object sender, EventArgs e)
        {
            Model_LoaiVaccine newlvc = new Model_LoaiVaccine();
            newlvc.maLoai = tb_maloai.Text;
            newlvc.tenLoai = tb_tenloai.Text;
            if (lvc.checkTrungMa(newlvc.maLoai, table) == 1)
            {

                btn_luu.Enabled = true;

            }
            else
                MessageBox.Show("Chưa có mã loại vaccine");
        }

        private void btn_luu_Click(object sender, EventArgs e)
        {
            Model_LoaiVaccine newlvc = new Model_LoaiVaccine();
            newlvc.maLoai = tb_maloai.Text;
            newlvc.tenLoai = tb_tenloai.Text;
            if (lvc.checkTrungMa(newlvc.maLoai, table) == 1)
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
                MessageBox.Show("Mã loại vaccine không tồn tại!");
            }
        }

        private void dgv_lvc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_lvc.CurrentRow != null)
            {
                tb_maloai.Text = dgv_lvc.CurrentRow.Cells[0].Value.ToString();
                tb_tenloai.Text = dgv_lvc.CurrentRow.Cells[1].Value.ToString();

            }
        }

        private void btn_dong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
