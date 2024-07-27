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
    public partial class frm_HienThiPhieuNhapHang : Form
    {
        ConnSQL connect = new ConnSQL();
        DataSet ds;
        SqlDataAdapter da;
        DataTable dt;
        private string maPhieuNhap;
        public frm_HienThiPhieuNhapHang(string maPhieuNhap)
        {
            InitializeComponent();
            this.maPhieuNhap = maPhieuNhap;
            try
            {
                ReportDocument reportDocument = new ReportDocument();

                // Đường dẫn tương đối của tệp báo cáo
                string reportPath = @"D:\Final\Nam_New\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\HeThongTiemChungVaccine_NATHADA\View\Report_PhieuNhap.rpt";

                reportDocument.Load(reportPath);


               ThietLapGiaTriBaoCao(reportDocument, maPhieuNhap);

                crystalReportViewer1.ReportSource = reportDocument;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị báo cáo: " + ex.Message);
            }
        }

        private void frm_HienThiPhieuNhapHang_Load(object sender, EventArgs e)
        {
        }
        private void ThietLapGiaTriBaoCao(ReportDocument reportDocument, string maHoaDon)
        {
            var thongTin = LayThongTinPhieuNhap(maPhieuNhap);

            SetTextObjectValue(reportDocument, "txtmaphieunhap", thongTin.maPhieuNhap);
            SetTextObjectValue(reportDocument, "txtmanhacungcap", thongTin.tenNhaCungCap);
            SetTextObjectValue(reportDocument, "txtmavaccine", thongTin.maVaccine);
            SetTextObjectValue(reportDocument, "txttenvaccine", thongTin.tenVaccine);
            SetTextObjectValue(reportDocument, "txtsoluong", thongTin.soLuong.ToString());
            SetTextObjectValue(reportDocument, "txtgia", thongTin.giaVaccine.ToString("F2"));
            SetTextObjectValue(reportDocument, "txtthanhtien", thongTin.thanhTien.ToString("F2"));
            SetTextObjectValue(reportDocument, "txttennhanvien", thongTin.tenNhanVien);
        }

        private void SetTextObjectValue(ReportDocument reportDocument, string textObjectName, string value)
        {
            TextObject textObject = reportDocument.ReportDefinition.Sections["Section3"].ReportObjects[textObjectName] as TextObject;
            if (textObject != null)
            {
                textObject.Text = value;
            }
        }


        

        private (string maPhieuNhap, string tenNhaCungCap, string maVaccine, string tenVaccine, int soLuong, float giaVaccine, float thanhTien, string tenNhanVien) LayThongTinPhieuNhap(string maPhieuNhap)
        {
            using (SqlConnection connection = connect.KetNoiCSDL())
            {
                string query = @"SELECT pn.ma_phieunhap, ncc.ten_nhacungcap, ct.ma_vaccine, vc.ten_vaccine, 
                                    ct.so_luong, ct.gia_vaccine, pn.tong_tien, nv.hoten_nhanvien
                             FROM PHIEUNHAP pn
                             INNER JOIN NHACUNGCAP ncc ON pn.ma_nhacungcap = ncc.ma_nhacungcap
                             INNER JOIN CHITIET_PHIEUNHAP ct ON pn.ma_phieunhap = ct.ma_phieunhap
                             INNER JOIN VACCINE vc ON ct.ma_vaccine = vc.ma_vaccine
                             INNER JOIN NHANVIEN nv ON pn.ma_nhanvien = nv.ma_nhanvien
                             WHERE pn.ma_phieunhap = @maPhieuNhap";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@maPhieuNhap", maPhieuNhap);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string maPhieuNhapValue = reader["ma_phieunhap"].ToString();
                    string tenNhaCungCap = reader["ten_nhacungcap"].ToString();
                    string maVaccine = reader["ma_vaccine"].ToString();
                    string tenVaccine = reader["ten_vaccine"].ToString();
                    int soLuong = Convert.ToInt32(reader["so_luong"]);
                    float giaVaccine = Convert.ToSingle(reader["gia_vaccine"]);
                    float thanhTien = Convert.ToSingle(reader["tong_tien"]);
                    string tenNhanVien = reader["hoten_nhanvien"].ToString();

                    return (maPhieuNhapValue, tenNhaCungCap, maVaccine, tenVaccine, soLuong, giaVaccine, thanhTien, tenNhanVien);
                }
                else
                {
                    throw new Exception("Không tìm thấy thông tin phiếu nhập.");
                }
            }
        }
    }
}
