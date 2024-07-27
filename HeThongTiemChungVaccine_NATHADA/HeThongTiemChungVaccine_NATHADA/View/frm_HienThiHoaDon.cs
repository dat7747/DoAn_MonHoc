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
    public partial class frm_HienThiHoaDon : Form
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        private string maHoaDon;
        private string tenDangNhap;
        public frm_HienThiHoaDon(string maHoaDon,string tenDangNhap)
        {
            InitializeComponent();
            this.tenDangNhap = tenDangNhap;
            this.maHoaDon = maHoaDon;
            try
            {
                ReportDocument reportDocument = new ReportDocument();

                // Đường dẫn tương đối của tệp báo cáo
                string reportPath = @"D:\Final\Nam_New\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\View\Report_HoaDon.rpt";

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
            SetTextObjectValue(reportDocument, "txttennhanvien", tenDangNhap);

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
        try
        {
            // Duyệt qua tất cả các phần trong báo cáo để tìm đối tượng TextObject
            bool found = false;
            foreach (Section section in reportDocument.ReportDefinition.Sections)
            {
                ReportObjects reportObjects = section.ReportObjects;
                foreach (ReportObject reportObject in reportObjects)
                {
                    if (reportObject is TextObject && reportObject.Name == textObjectName)
                    {
                        TextObject textObject = reportObject as TextObject;
                        if (textObject != null)
                        {
                            textObject.Text = value;
                            found = true;
                            break;
                        }
                    }
                }
                if (found) break;
            }

            if (!found)
            {
                Console.WriteLine($"Text object '{textObjectName}' not found in the report.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting text object '{textObjectName}': {ex.Message}");
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
                string tenComboStr = reader["ten_combo"] != DBNull.Value ? reader["ten_combo"].ToString() : string.Empty;
                string tenVaccineStr = reader["ten_vaccine"] != DBNull.Value ? reader["ten_vaccine"].ToString() : string.Empty;

                var tenCombo = tenComboStr.Split(new[] { ", " }, StringSplitOptions.None).ToList();
                var tenVaccine = tenVaccineStr.Split(new[] { ", " }, StringSplitOptions.None).ToList();

                var giaCombo = LayGiaCombo(tenCombo);
                var giaVaccine = LayGiaVaccine(tenVaccine);

                return (
                    reader["ma_hoadon"].ToString(),
                    reader["ma_dangky"].ToString(),
                    reader["hoten_khachhang"].ToString(),
                    reader["ten_voucher"].ToString(),
                    tenCombo,
                    tenVaccine,
                    giaVaccine,
                    giaCombo,
                    reader["hoten_nguoitiem"].ToString(),
                    Convert.ToDateTime(reader["ngay_dangky"]),
                    Convert.ToDateTime(reader["ngay_muontiem"]),
                    Convert.ToSingle(reader["phi_luukho"]),
                    Convert.ToSingle(reader["thanhTien"])
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

        private void frm_HienThiHoaDon_Load(object sender, EventArgs e)
        {

        }

    }
}
