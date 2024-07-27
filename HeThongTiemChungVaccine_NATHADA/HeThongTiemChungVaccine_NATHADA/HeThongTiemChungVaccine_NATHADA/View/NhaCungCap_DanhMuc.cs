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
    public partial class NhaCungCap_DanhMuc : Form
    {
        DataColumn[] key = new DataColumn[1];
        Control_NhaCungCap ncc = new Control_NhaCungCap();
        string table = "NHACUNGCAP";
        public NhaCungCap_DanhMuc()
        {
            InitializeComponent();
        }

        void AddHeader()
        {
            dgv_ncc.Columns.Clear();
            dgv_ncc.Columns.Add("ma_nhacungcap", "Mã Nhà Cung CẤp");
            dgv_ncc.Columns[0].DataPropertyName = "ma_nhacungcap";
            dgv_ncc.Columns.Add("ten_nhacungcap", "Tên Nhà Cung Cấp");
            dgv_ncc.Columns[1].DataPropertyName = "ten_nhacungcap";
            dgv_ncc.Columns.Add("diachi_nhacungcap", "Địa chỉ Nhà Cung CẤp");
            dgv_ncc.Columns[2].DataPropertyName = "diachi_nhacungcap";
            dgv_ncc.Columns.Add("sdt_nhacungcap", "SĐT Nhà Cung Cấp");
            dgv_ncc.Columns[3].DataPropertyName = "sdt_nhacungcap";

        }
        void loadDTG()
        {
            if (dgv_ncc.DataSource != null)
                dgv_ncc.Rows.Clear();
            DataTable dtNCC = ncc.select(table);
            dgv_ncc.DataSource = dtNCC;
            key[0] = dtNCC.Columns[0];
            dtNCC.PrimaryKey = key;
        }
        void loadAllNhaCC()
        {
            AddHeader();
            loadDTG();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void NhaCungCap_DanhMuc_Load(object sender, EventArgs e)
        {
            loadAllNhaCC();
        }

        private void dgv_ncc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_ncc.CurrentRow != null)
            {
                tb_mancc.Text = dgv_ncc.CurrentRow.Cells[0].Value.ToString();
                tb_tenncc.Text = dgv_ncc.CurrentRow.Cells[1].Value.ToString();
                tb_diachincc.Text = dgv_ncc.CurrentRow.Cells[2].Value.ToString();
                tb_sdtncc.Text = dgv_ncc.CurrentRow.Cells[3].Value.ToString();

            }
        }

        private void dgv_ncc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_ncc.CurrentRow != null)
            {
                tb_mancc.Text = dgv_ncc.CurrentRow.Cells[0].Value.ToString();
                tb_tenncc.Text = dgv_ncc.CurrentRow.Cells[1].Value.ToString();
                tb_diachincc.Text = dgv_ncc.CurrentRow.Cells[2].Value.ToString();
                tb_sdtncc.Text = dgv_ncc.CurrentRow.Cells[3].Value.ToString();

            }
        }
    }
}
