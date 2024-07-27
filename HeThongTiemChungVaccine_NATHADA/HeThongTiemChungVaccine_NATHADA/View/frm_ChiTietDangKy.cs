using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeThongTiemChungVaccine_NATHADA.View
{
    public partial class frm_ChiTietDangKy : Form
    {
        private string madangky;
        ConnSQL connect = new ConnSQL();
        public frm_ChiTietDangKy(string madangky)
        {
            InitializeComponent();
            this.madangky = madangky;
            DisplayChiTietDangKy();
        }

        private void DisplayChiTietDangKy()
        {
            try
            {
                using (SqlConnection connection = connect.KetNoiCSDL())
                {
                    connection.Open();

                    string query = @"
                SELECT 
                    nd.ma_dangky,
                    nd.hoten_nguoitiem,
                    nd.ngaysinh_nguoitiem,
                    nd.gioitinh_nguoitiem,
                    nd.diachi_nguoitiem,
                    nd.hoten_nguoilienhe,
                    nd.sdt_nguoilienhe,
                    kh.hoten_khachhang,
                    nd.phi_luukho,
                    nd.tongthanhtoan,
                    nd.ngay_dangky,
                    nd.ngay_muontiem,
                    nd.moiquanhe_nguoitiem,
                    vc.ten_vaccine,
                    cb.ten_combo,
                    nmv.so_luong as so_luong_vaccine,
                    vc.gia_vacine as gia_vaccine, -- Đảm bảo tên cột này
                    nmc.so_luong as so_luong_combo,
                    cb.gia_combo as gia_combo -- Đảm bảo cột này tồn tại trong bảng COMBO_VACCINE
                FROM 
                    NGUOITIEM_DANGKY nd
                LEFT JOIN 
                    KHACHHANG kh ON nd.ma_khachhang = kh.ma_khachhang
                LEFT JOIN 
                    NGUOITIEM_MUAVACCINE nmv ON nd.ma_dangky = nmv.ma_dangky
                LEFT JOIN 
                    VACCINE vc ON nmv.ma_vaccine = vc.ma_vaccine
                LEFT JOIN 
                    NGUOITIEM_MUACOMBO nmc ON nd.ma_dangky = nmc.ma_dangky
                LEFT JOIN 
                    COMBO_VACCINE cb ON nmc.ma_combo = cb.ma_combo
                WHERE 
                    nd.ma_dangky = @MaDangKy";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@MaDangKy", madangky);

                    SqlDataReader reader = cmd.ExecuteReader();
                    double totalVaccinePrice = 0;
                    double totalComboPrice = 0;
                    HashSet<string> displayedVaccines = new HashSet<string>();
                    HashSet<string> displayedCombos = new HashSet<string>();

                    while (reader.Read())
                    {
                        txtmadangky.Text = reader["ma_dangky"].ToString();
                        txttennguoitiem.Text = reader["hoten_nguoitiem"].ToString();
                        txtngaysinhnguoitiem.Text = reader["ngaysinh_nguoitiem"].ToString();
                        txtgioitinh.Text = reader["gioitinh_nguoitiem"].ToString();
                        txtdiachi.Text = reader["diachi_nguoitiem"].ToString();
                        txtnguoilienhe.Text = reader["hoten_nguoilienhe"].ToString();
                        txtsdt.Text = reader["sdt_nguoilienhe"].ToString();
                        txtmoiquanhe.Text = reader["moiquanhe_nguoitiem"].ToString();
                        txtphiluukho.Text = reader["phi_luukho"].ToString();
                        txttongthanhtoan.Text = reader["tongthanhtoan"].ToString();
                        txtngaydangky.Text = reader["ngay_dangky"].ToString();
                        txtngaytiem.Text = reader["ngay_muontiem"].ToString();

                        // Hiển thị tên vaccine và combo
                        if (!reader.IsDBNull(reader.GetOrdinal("ten_vaccine")))
                        {
                            string vaccineName = reader["ten_vaccine"].ToString();
                            if (!displayedVaccines.Contains(vaccineName))
                            {
                                txttenvaccine.Text += vaccineName + Environment.NewLine;
                                displayedVaccines.Add(vaccineName);
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("so_luong_vaccine")) && !reader.IsDBNull(reader.GetOrdinal("gia_vaccine")))
                            {
                                totalVaccinePrice += Convert.ToDouble(reader["so_luong_vaccine"]) * Convert.ToDouble(reader["gia_vaccine"]);
                            }
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ten_combo")))
                        {
                            string comboName = reader["ten_combo"].ToString();
                            if (!displayedCombos.Contains(comboName))
                            {
                                txttencombo.Text += comboName + Environment.NewLine;
                                displayedCombos.Add(comboName);
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("so_luong_combo")) && !reader.IsDBNull(reader.GetOrdinal("gia_combo")))
                            {
                                totalComboPrice += Convert.ToDouble(reader["so_luong_combo"]) * Convert.ToDouble(reader["gia_combo"]);
                            }
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }


        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
