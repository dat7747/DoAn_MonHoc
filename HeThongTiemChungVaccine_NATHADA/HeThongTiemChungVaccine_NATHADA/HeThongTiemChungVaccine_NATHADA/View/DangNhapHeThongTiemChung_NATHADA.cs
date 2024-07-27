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
using System.Data;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class DangNhapHeThongTiemChung_NATHADA : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=LAPTOP-IRLS8GIA; Initial Catalog=TiemChungVC2205; User ID=sa;Password=123");
        public static string UserName = "";
        public DangNhapHeThongTiemChung_NATHADA()
        {
            InitializeComponent();
        }
        int dem = 0;
        private void btn_dangnhap_Click(object sender, EventArgs e)
        {
            txt_taikhoan.Text = "thanhnhan11";
            txt_matkhau.Text = "123456";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DangNhap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tendangnhap", txt_taikhoan.Text.Trim());
                cmd.Parameters.AddWithValue("@matkhau", txt_matkhau.Text.Trim());

                UserName = txt_taikhoan.Text.Trim();
                SqlParameter returnParameter = new SqlParameter();
                returnParameter.ParameterName = "@ReturnVal";
                returnParameter.SqlDbType = SqlDbType.Int;
                returnParameter.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(returnParameter);

                cmd.ExecuteNonQuery();
                int result = (int)returnParameter.Value;

                if (result == 1)
                {
                    MessageBox.Show("Chào mừng Nhân viên " + UserName + " đăng nhập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Giaodientrangchu f = new Giaodientrangchu();
                    f.Show();
                }
                else if (result == 2)
                {
                    MessageBox.Show("Chào mừng khách hàng " + UserName + " đăng nhập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Giaodientrangchu f = new Giaodientrangchu();
                    f.Show();
                }
                else
                {
                    dem++;
                    MessageBox.Show("Bạn đã đăng nhập thất bại");
                    this.txt_taikhoan.Focus();
                    if (dem == 3)
                    {
                        MessageBox.Show("Tài khoản của bạn đã bị khóa");
                        btn_dangnhap.Enabled = false;
                        View.DangNhapThatBai hehe = new View.DangNhapThatBai();
                        hehe.Show();
                        this.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void DangNhapHeThongTiemChung_NATHADA_Load(object sender, EventArgs e)
        {
            txt_matkhau.PasswordChar = '*';
        }

        private void DangNhapHeThongTiemChung_NATHADA_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult r;
            r = MessageBox.Show("Bạn có muốn thoát chương trình không ? ", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (r == DialogResult.No)
                e.Cancel = true;
        }

        private void btn_thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chk_show_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_show.Checked)
            {
                txt_matkhau.PasswordChar = (char)0;
            }
            else
            {
                txt_matkhau.PasswordChar = '*';
            }
        }


    }
}
