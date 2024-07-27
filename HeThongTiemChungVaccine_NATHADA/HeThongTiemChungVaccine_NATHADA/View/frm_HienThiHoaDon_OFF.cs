using CrystalDecisions.CrystalReports.Engine;
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
    public partial class frm_HienThiHoaDon_OFF : Form
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        private string maHoaDon;
        private string tennhanvien;
        private string tenDangNhap;
        Control_NhanVien controlNhanVien = new Control_NhanVien();
        public frm_HienThiHoaDon_OFF(string maHoaDon, string tenDangNhap)
        { 
            InitializeComponent();
            this.tenDangNhap = tenDangNhap;
            this.maHoaDon = maHoaDon;
            this.tennhanvien = LayHoTenNhanVienTuTenDangNhap(tenDangNhap);
            try
            {
                ReportDocument reportDocument = new ReportDocument();

                // Đường dẫn tương đối của tệp báo cáo
                string reportPath = @"D:\Final\Nam_New\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\View\Report_HoaDon_off.rpt";

                reportDocument.Load(reportPath);

                ThietLapGiaTriBaoCao(reportDocument, maHoaDon);

                crystalReportViewer1.ReportSource = reportDocument;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị báo cáo: " + ex.Message);
            }
           
        }
        private void ThietLapGiaTriBaoCao(ReportDocument reportDocument, string maHoaDon)
        {
            var thongTin = LayThongTinHoaDon(maHoaDon);

            SetTextObjectValue(reportDocument, "txtmahoadon", thongTin.maHoaDon);
            SetTextObjectValue(reportDocument, "txtmadangyk", thongTin.maDangKy);
            SetTextObjectValue(reportDocument, "txttenkhachhang", thongTin.tenKhachHang);
            SetTextObjectValue(reportDocument, "txttenvoucher", thongTin.tenVoucher);
            SetTextObjectValue(reportDocument, "txttennguoitiem", thongTin.tenNguoiTiem);
            SetTextObjectValue(reportDocument, "txtngaydangky", thongTin.ngayDangKy.ToString("dd/MM/yyyy"));
            SetTextObjectValue(reportDocument, "txtngaymuontiem", thongTin.ngayMuonTiem.ToString("dd/MM/yyyy"));
            SetTextObjectValue(reportDocument, "txttennhanvien", tennhanvien);

            // Chuẩn hóa giá trị phí lưu kho và thành tiền
            string phiLuuKhoText = thongTin.phiLuuKho.ToString("#,##0") + " VNĐ";
            string thanhTienText = thongTin.thanhTien.ToString("#,##0") + " VNĐ";

            // Ghép nối tên và giá combo
            string tenComboText = string.Join("\n", thongTin.tenCombo);
            string giaComboText = string.Join("\n", thongTin.giaCombo.Select(g => g?.ToString("#,##0.00") + " VNĐ" ?? "0 VNĐ"));

            // Ghép nối tên và giá vaccine
            string tenVaccineText = string.Join("\n", thongTin.tenVaccine);
            string giaVaccineText = string.Join("\n", thongTin.giaVaccine.Select(g => g?.ToString("#,##0.00") + " VNĐ" ?? "0 VNĐ"));

            // Đặt giá trị vào các TextObject tương ứng
            SetTextObjectValue(reportDocument, "txttencombo", tenComboText);
            SetTextObjectValue(reportDocument, "txtgiacombo", giaComboText);
            SetTextObjectValue(reportDocument, "txttenvaccine", tenVaccineText);
            SetTextObjectValue(reportDocument, "txtgiavaccine", giaVaccineText);
            SetTextObjectValue(reportDocument, "txtphiluukho", phiLuuKhoText);
            SetTextObjectValue(reportDocument, "txtthanhtien", thanhTienText);
        }

        private void SetTextObjectValue(ReportDocument reportDocument, string textObjectName, string value)
        {
            TextObject textObject = reportDocument.ReportDefinition.Sections["Section3"].ReportObjects[textObjectName] as TextObject;
            if (textObject != null)
            {
                textObject.Text = value;
            }
        }

        private (string maHoaDon, string maDangKy, string tenKhachHang, string tenVoucher, List<string> tenCombo, List<string> tenVaccine, List<float?> giaVaccine, List<float?> giaCombo, string tenNguoiTiem, DateTime ngayDangKy, DateTime ngayMuonTiem, float phiLuuKho, float thanhTien) LayThongTinHoaDon(string maHoaDon)
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"
            
                 SELECT 
                            hd.ma_hoadon, 
                            dk.ma_dangky, 
                            ISNULL(kh.hoten_khachhang, '') AS hoten_khachhang, 
                            ISNULL(vo.ten_voucher, '') AS ten_voucher, 
                            ISNULL(STUFF((SELECT ', ' + c.ten_combo
                                   FROM COMBO_VACCINE c
                                   INNER JOIN NGUOITIEM_MUACOMBO ntc ON c.ma_combo = ntc.ma_combo
                                   WHERE ntc.ma_dangky = dk.ma_dangky
                                   FOR XML PATH('')), 1, 1, ''), '') AS ten_combo,
                            ISNULL(STUFF((SELECT ', ' + v.ten_vaccine
                                   FROM VACCINE v
                                   INNER JOIN NGUOITIEM_MUAVACCINE ntv ON v.ma_vaccine = ntv.ma_vaccine
                                   WHERE ntv.ma_dangky = dk.ma_dangky
                                   FOR XML PATH('')), 1, 1, ''), '') AS ten_vaccine,
                            dk.hoten_nguoitiem, 
                            dk.ngay_dangky, 
                            dk.ngay_muontiem,
                            ISNULL(dk.phi_luukho, 0) AS phi_luukho, 
                            ISNULL(dk.tongthanhtoan, 0) AS thanhTien
                        FROM HOADON hd
                        LEFT JOIN KHACHHANG kh ON hd.ma_khachhang = kh.ma_khachhang
                        LEFT JOIN VOUCHER vo ON hd.ma_voucher = vo.ma_voucher
                        INNER JOIN NGUOITIEM_DANGKY dk ON hd.ma_dangky = dk.ma_dangky
                        WHERE hd.ma_hoadon = @maHoaDon";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@maHoaDon", maHoaDon);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    maHoaDon = reader["ma_hoadon"] != DBNull.Value ? reader["ma_hoadon"].ToString() : string.Empty;
                    string maDangKy = reader["ma_dangky"] != DBNull.Value ? reader["ma_dangky"].ToString() : string.Empty;
                    string tenKhachHang = reader["hoten_khachhang"] != DBNull.Value ? reader["hoten_khachhang"].ToString() : string.Empty;
                    string tenVoucher = reader["ten_voucher"] != DBNull.Value ? reader["ten_voucher"].ToString() : string.Empty;
                    string tenNguoiTiem = reader["hoten_nguoitiem"] != DBNull.Value ? reader["hoten_nguoitiem"].ToString() : string.Empty;
                    DateTime ngayDangKy = reader["ngay_dangky"] != DBNull.Value ? Convert.ToDateTime(reader["ngay_dangky"]) : DateTime.MinValue;
                    DateTime ngayMuonTiem = reader["ngay_muontiem"] != DBNull.Value ? Convert.ToDateTime(reader["ngay_muontiem"]) : DateTime.MinValue;
                    float phiLuuKho = reader["phi_luukho"] != DBNull.Value ? Convert.ToSingle(reader["phi_luukho"]) : 0f;
                    float thanhTien = reader["thanhTien"] != DBNull.Value ? Convert.ToSingle(reader["thanhTien"]) : 0f;

                    // Xử lý chuỗi ten_combo và ten_vaccine
                    string tenComboStr = reader["ten_combo"] != DBNull.Value ? reader["ten_combo"].ToString() : string.Empty;
                    string tenVaccineStr = reader["ten_vaccine"] != DBNull.Value ? reader["ten_vaccine"].ToString() : string.Empty;

                    var tenCombo = tenComboStr.Split(new[] { ", " }, StringSplitOptions.None).ToList();
                    var tenVaccine = tenVaccineStr.Split(new[] { ", " }, StringSplitOptions.None).ToList();

                    var giaCombo = LayGiaCombo(tenCombo);
                    var giaVaccine = LayGiaVaccine(tenVaccine);

                    return (
                        maHoaDon,
                        maDangKy,
                        tenKhachHang,
                        tenVoucher,
                        tenCombo,
                        tenVaccine,
                        giaVaccine,
                        giaCombo,
                        tenNguoiTiem,
                        ngayDangKy,
                        ngayMuonTiem,
                        phiLuuKho,
                        thanhTien
                    );
                }
                else
                {
                    throw new Exception("No data found for the specified maHoaDon.");
                }

            }
        }

        private List<float?> LayGiaCombo(List<string> tenCombo)
        {
            List<float?> giaComboList = new List<float?>();

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                connection.Open();
                foreach (var ten in tenCombo)
                {
                    // Loại bỏ khoảng trắng từ tên combo
                    string tenComboTrimmed = ten.Trim();

                    string query = "SELECT gia_combo FROM COMBO_VACCINE WHERE ten_combo = @tenCombo";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@tenCombo", tenComboTrimmed);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        giaComboList.Add(Convert.ToSingle(result));
                    }
                    else
                    {
                        giaComboList.Add(null);
                    }
                }
            }

            return giaComboList;
        }

        private List<float?> LayGiaVaccine(List<string> tenVaccine)
        {
            List<float?> giaVaccineList = new List<float?>();

            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                connection.Open();
                foreach (var ten in tenVaccine)
                {
                    // Loại bỏ khoảng trắng từ tên vaccine
                    string tenVaccineTrimmed = ten.Trim();

                    string query = "SELECT gia_vacine FROM VACCINE WHERE ten_vaccine = @tenVaccine";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@tenVaccine", tenVaccineTrimmed);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        giaVaccineList.Add(Convert.ToSingle(result));
                    }
                    else
                    {
                        giaVaccineList.Add(null);
                    }
                }
            }

            return giaVaccineList;
        }

        private string LayHoTenNhanVienTuTenDangNhap(string tenDangNhap)
        {
            string hoTen = "";

            // Kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                // Mở kết nối
                connection.Open();

                // Tạo command để truy vấn dữ liệu
                using (SqlCommand command = new SqlCommand("SELECT NV.hoten_nhanvien FROM NHANVIEN NV JOIN TAIKHOAN TK ON NV.ma_nhanvien = TK.ma_nhanvien WHERE TK.tendangnhap = @tenDangNhap", connection))
                {
                    // Thêm tham số cho câu truy vấn
                    command.Parameters.AddWithValue("@tenDangNhap", tenDangNhap);

                    // Thực thi truy vấn và gán kết quả cho biến hoTen
                    hoTen = (string)command.ExecuteScalar();
                }
            }

            return hoTen;
        }

        private void frm_HienThiHoaDon_OFF_Load(object sender, EventArgs e)
        {

        }
    }
}
