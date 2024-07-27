using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TiemChungVaccine.Models;
namespace Web_TiemChungVaccine.Controllers
{
    public class GioHangController : Controller
    {
        //
        // GET: /GioHang/
        QL_VACCINEDataContext db = new QL_VACCINEDataContext();
        public ActionResult Index()
        {
            var maKhachHang = Session["maKH"]?.ToString();
            if (maKhachHang == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            var gioHang = db.GIOHANGs.Where(g => g.ma_khachhang == maKhachHang).ToList();
            var items = new List<GioHangViewModel>();

            foreach (var item in gioHang)
            {
                if (!string.IsNullOrEmpty(item.ma_vaccine))
                {
                    var vaccine = db.VACCINEs.FirstOrDefault(v => v.ma_vaccine == item.ma_vaccine);
                    items.Add(new GioHangViewModel
                    {
                        IdGioHang = item.id_giohang,
                        MaVaccine = item.ma_vaccine,
                        TenVaccine = vaccine?.ten_vaccine,
                        AnhVaccine = vaccine?.anh_vaccine,
                        SoLuong = item.soluong ?? 0,
                        GiaVaccine = (float)(vaccine?.gia_vacine ?? 0)
                    });
                }
                else if (!string.IsNullOrEmpty(item.ma_combo))
                {
                    var combo = db.COMBO_VACCINEs.FirstOrDefault(c => c.ma_combo == item.ma_combo);
                    if (combo != null)
                    {
                        var chiTietCombo = db.CHITIET_COMBO_VACCXINEs
                            .Where(c => c.ma_combo == item.ma_combo)
                            .FirstOrDefault();

                        var vaccineImage = chiTietCombo != null ? db.VACCINEs
                            .Where(v => v.ma_vaccine == chiTietCombo.ma_vaccine)
                            .Select(v => v.anh_vaccine)
                            .FirstOrDefault() : null;

                        items.Add(new GioHangViewModel
                        {
                            IdGioHang = item.id_giohang,
                            MaCombo = combo.ma_combo,
                            TenCombo = combo.ten_combo,
                            SoLuong = item.soluong ?? 0,
                            GiaCombo = (float)combo.gia_combo,
                            AnhVaccine = vaccineImage
                        });
                    }
                }
            }

            // Tính tổng tiền
            float tongTien = items.Sum(item => item.ThanhTien);
            ViewBag.TongTien = tongTien;

            return View(items);
        }
        //======================Thêm vào giỏ hàng ======================================
        [HttpPost]
        public ActionResult ThemVaoGioHang(string maOption, int soluong = 1, string loaiOption = "Vaccine")
        {
            var maKhachHang = Session["maKH"]?.ToString();
            if (maKhachHang == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            var khachHang = db.KHACHHANGs.FirstOrDefault(k => k.ma_khachhang == maKhachHang);
            if (khachHang == null)
            {
                return HttpNotFound("Khách hàng không tồn tại.");
            }

            // Kiểm tra xem mục đã tồn tại trong giỏ hàng chưa
            GIOHANG existingItem;
            if (loaiOption == "Vaccine")
            {
                existingItem = db.GIOHANGs.FirstOrDefault(g => g.ma_khachhang == maKhachHang && g.ma_vaccine == maOption);
            }
            else
            {
                existingItem = db.GIOHANGs.FirstOrDefault(g => g.ma_khachhang == maKhachHang && g.ma_combo == maOption);
            }

            if (existingItem == null)
            {
                // Nếu mục chưa tồn tại, thêm mục mới vào giỏ hàng
                var gioHang = new GIOHANG
                {
                    ma_khachhang = maKhachHang,
                    soluong = soluong
                };

                if (loaiOption == "Vaccine")
                {
                    Session["MaVaccine"] = maOption;
                    gioHang.ma_vaccine = maOption;
                }
                else
                {
                    Session["MaCombo"] = maOption;
                    gioHang.ma_combo = maOption;
                }

                db.GIOHANGs.InsertOnSubmit(gioHang);
            }

            db.SubmitChanges();

            // Cập nhật session "GioHangCount" để hiển thị số lượng sản phẩm trong giỏ hàng
            var updatedGioHangCount = db.GIOHANGs.Count(g => g.ma_khachhang == maKhachHang);
            Session["GioHangCount"] = updatedGioHangCount;


            return RedirectToAction("Index", "GioHang");
        }

        //=============================xóa================================
        [HttpPost]
        public ActionResult XoaKhoiGioHang(int idGioHang)
        {
            var item = db.GIOHANGs.FirstOrDefault(g => g.id_giohang == idGioHang);
            if (item != null)
            {
                db.GIOHANGs.DeleteOnSubmit(item);
                db.SubmitChanges();

                // Cập nhật lại số lượng sản phẩm trong giỏ hàng sau khi xóa mục
                var maKhachHang = Session["maKH"]?.ToString();
                if (maKhachHang != null)
                {
                    var updatedGioHangCount = db.GIOHANGs.Count(g => g.ma_khachhang == maKhachHang);
                    Session["GioHangCount"] = updatedGioHangCount;
                }
            }
            return RedirectToAction("Index");
        }

        //============================sửa=================================
        [HttpGet]
        public ActionResult ChinhSuaSoLuong(int idGioHang)
        {
            var item = db.GIOHANGs.FirstOrDefault(g => g.id_giohang == idGioHang);
            if (item == null)
            {
                return HttpNotFound();
            }

            var viewModel = new GioHangViewModel
            {
                IdGioHang = item.id_giohang,
                MaVaccine = item.ma_vaccine,
                TenVaccine = db.VACCINEs.FirstOrDefault(v => v.ma_vaccine == item.ma_vaccine)?.ten_vaccine,
                AnhVaccine = db.VACCINEs.FirstOrDefault(v => v.ma_vaccine == item.ma_vaccine)?.anh_vaccine,
                SoLuong = item.soluong ?? 0,
            };

            return View("ChinhSuaSoLuong", viewModel);
        }

        [HttpPost]
        public ActionResult CapNhatSoLuong(int idGioHang, int soLuong)
        {
            var item = db.GIOHANGs.FirstOrDefault(g => g.id_giohang == idGioHang);
            if (item != null)
            {
                item.soluong = soLuong;
                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }
        //==============================Đăng ký mũi tiêm Bước 1=========================
        public ActionResult DangKyMuiTiem()
        {
            var maKhachHang = Session["maKH"]?.ToString();
            if (maKhachHang == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            var khachHang = db.KHACHHANGs.FirstOrDefault(kh => kh.ma_khachhang == maKhachHang);
            if (khachHang == null)
            {
                return HttpNotFound("Khách hàng không tồn tại.");
            }

            // Lấy dữ liệu về vaccines và combo vaccines
            var allVaccines = db.VACCINEs.Select(v => new VaccineViewModel
            {
                MaVaccine = v.ma_vaccine,
                TenVaccine = v.ten_vaccine,
                GiaVaccine = Convert.ToDecimal(v.gia_vacine),
                Note = v.note
            }).ToList();

            // Lấy dữ liệu về giỏ hàng của người dùng hiện tại
            var gioHang = db.GIOHANGs.Where(g => g.ma_khachhang == maKhachHang).ToList();

            // Tạo danh sách chứa thông tin về các mục trong giỏ hàng
            var vaccinesInCart = gioHang.Select<GIOHANG, object>(item =>
            {
                if (!string.IsNullOrEmpty(item.ma_combo))
                {
                    var comboVaccine = db.COMBO_VACCINEs.FirstOrDefault(cv => cv.ma_combo == item.ma_combo);
                    if (comboVaccine != null)
                    {
                        return new ComboVaccineViewModel
                        {
                            MaComboVaccine = comboVaccine.ma_combo,
                            TenComboVaccine = comboVaccine.ten_combo,
                            GiaComboVaccine = Convert.ToDecimal(comboVaccine.gia_combo)
                        };
                    }
                }
                else
                {
                    var vaccine = db.VACCINEs.FirstOrDefault(v => v.ma_vaccine == item.ma_vaccine);
                    if (vaccine != null)
                    {
                        return new VaccineViewModel
                        {
                            MaVaccine = vaccine.ma_vaccine,
                            TenVaccine = vaccine.ten_vaccine,
                            GiaVaccine = Convert.ToDecimal(vaccine.gia_vacine),
                            Note = vaccine.note
                        };
                    }
                }
                return null;
            }).Where(vm => vm != null).ToList();

            // Lấy mã vaccine và mã combo từ dữ liệu đã lấy được (ví dụ ở đây lấy từ giỏ hàng)
            var maVaccine = gioHang.Where(g => !string.IsNullOrEmpty(g.ma_vaccine)).Select(g => g.ma_vaccine).FirstOrDefault();
            var maCombo = gioHang.Where(g => !string.IsNullOrEmpty(g.ma_combo)).Select(g => g.ma_combo).FirstOrDefault();

            // Tạo danh sách chứa thông tin về tất cả các loại vaccine và combo vaccine
            var allComboVaccines = db.COMBO_VACCINEs.Select(cv => new ComboVaccineViewModel
            {
                MaComboVaccine = cv.ma_combo,
                TenComboVaccine = cv.ten_combo
            }).ToList();

            var allVaccineOptions = allVaccines.Select(v => new VaccineOptionViewModel
            {
                MaOption = v.MaVaccine,
                TenOption = v.TenVaccine,
                LoaiOption = "Vaccine"
            }).ToList();

            var allComboOptions = allComboVaccines.Select(cv => new VaccineOptionViewModel
            {
                MaOption = cv.MaComboVaccine,
                TenOption = cv.TenComboVaccine,
                LoaiOption = "Combo Vaccine"
            }).ToList();

            var combinedOptions = allVaccineOptions.Concat(allComboOptions).ToList();
            var maVaccineList = gioHang.Where(g => !string.IsNullOrEmpty(g.ma_vaccine)).Select(g => g.ma_vaccine).ToList();
            var maComboList = gioHang.Where(g => !string.IsNullOrEmpty(g.ma_combo)).Select(g => g.ma_combo).ToList();

            // Gán giá trị cho ViewBag
            ViewBag.TenKhachHang = khachHang.hoten_khachhang;
            ViewBag.SoDiemTichLuy = khachHang.diemthanthiet;
            ViewBag.Vaccines = combinedOptions;
            ViewBag.VaccinesInCart = vaccinesInCart;
            ViewBag.CurrentStep = 1;
            ViewBag.MaVaccine = maVaccine;
            ViewBag.MaCombo = maCombo;
            ViewBag.MaVaccineList = maVaccineList;
            ViewBag.MaComboList = maComboList;

            return View();
        }
        //===============================Điều khoản thanh toán Bước 2 ======================
        [HttpPost]
        public ActionResult DieuKhoanThanhToan(string action)
        {
            // Khai báo các biến bên ngoài khối if
            decimal sotiengiamgia = 0;
            decimal phiLuuKho = 0;
            decimal tongThanhToan = 0;

            // Lấy mã khách hàng từ Session
            var maKhachHang = Session["maKH"]?.ToString();

            // Nếu không có mã khách hàng, chuyển hướng đến trang đăng nhập
            if (maKhachHang == null)
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            // Kết nối đến cơ sở dữ liệu và lấy thông tin tài khoản khách hàng
            var db = new QL_VACCINEDataContext();
            var taiKhoan = db.KHACHHANGs.FirstOrDefault(tk => tk.ma_khachhang == maKhachHang);

            // Nếu tài khoản tồn tại, gán thông tin vào ViewBag
            if (taiKhoan != null)
            {
                ViewBag.TenTaiKhoan = taiKhoan.hoten_khachhang;
                ViewBag.SoDienThoaiTaiKhoan = taiKhoan.sdt_khachhang;
            }

            // Lấy dữ liệu từ Request.Form
            string hoTenNguoiTiem = Request.Form["name"];
            DateTime ngaySinhNguoiTiem;
            if (!string.IsNullOrEmpty(Request.Form["dob"]))
            {
                ngaySinhNguoiTiem = DateTime.Parse(Request.Form["dob"]);
            }
            else
            {
                // Gán giá trị mặc định hoặc xử lý khi không có dữ liệu
                ngaySinhNguoiTiem = DateTime.MinValue; // Ví dụ gán giá trị mặc định
            }

            string diaChiNguoiTiem = Request.Form["address"];
            string hoTenNguoiLienHe = Request.Form["contact-name"];
            string gioiTinh = Request.Form["sex"];
            string moiQuanHe = Request.Form["relationship"];
            string soDienThoai = Request.Form["phone"];
            string maVoucher = Request.Form["ma-voucher"];
            int sdtNguoiLienHe = 0;
            if (!string.IsNullOrEmpty(soDienThoai))
            {
                int.TryParse(soDienThoai, out sdtNguoiLienHe);
            }

            DateTime ngayMuonTiem;

            if (!string.IsNullOrEmpty(Request.Form["desired-date"]))
            {
                ngayMuonTiem = DateTime.Parse(Request.Form["desired-date"]);
            }
            else
            {
                // Gán giá trị mặc định hoặc xử lý khi không có dữ liệu
                ngayMuonTiem = DateTime.MinValue; // Ví dụ gán giá trị mặc định
            }

            // Lấy danh sách mã vaccine từ Request.Form
            var maVaccineList = Request.Form.GetValues("maVaccineList");
            var maComboList = Request.Form.GetValues("maComboList");
            ViewBag.MaVaccineList = maVaccineList;
            ViewBag.MaComboList = maComboList;

            // Nếu không phải là action "ThanhToan", trả về View với CurrentStep được set là 2
            //if (action != "ThanhToan")
            //{
            //    ViewBag.CurrentStep = 2;

            //    // Lưu thông tin vào ViewBag để truy cập từ view
            //    ViewBag.TenKhachHang = hoTenNguoiTiem;
            //    ViewBag.NgaySinhNguoiTiem = ngaySinhNguoiTiem.ToString("yyyy-MM-dd");
            //    ViewBag.DiaChiNguoiTiem = diaChiNguoiTiem;
            //    ViewBag.TenNguoiTiem = hoTenNguoiLienHe;
            //    ViewBag.GioiTinhNguoiTiem = gioiTinh;
            //    ViewBag.MoiQuanHe = moiQuanHe;
            //    ViewBag.SoDienThoai = soDienThoai;
            //    ViewBag.NgayMuonTiem = ngayMuonTiem.ToString("yyyy-MM-dd");
            //    ViewBag.MaVoucher = maVoucher;
            //    List<string> tenVaccineList = new List<string>();
            //    List<decimal> giaVaccineList = new List<decimal>();
            //    List<string> tenComboList = new List<string>();
            //    List<decimal> giaComboList = new List<decimal>();
            //    // Truy vấn để lấy thông tin về tên và giá của vaccine từ cơ sở dữ liệu
            //    if (maVaccineList != null)
            //    {
            //        foreach (var maVaccine in maVaccineList)
            //        {
            //            var vaccine = db.VACCINEs.FirstOrDefault(v => v.ma_vaccine == maVaccine);
            //            if (vaccine != null)
            //            {
            //                tenVaccineList.Add(vaccine.ten_vaccine);
            //                giaVaccineList.Add(Convert.ToDecimal(vaccine.gia_vacine));
            //            }
            //        }
            //    }

            //    // Truy vấn để lấy thông tin về tên và giá của combo từ cơ sở dữ liệu
            //    if (maComboList != null)
            //    {
            //        foreach (var maCombo in maComboList)
            //        {
            //            var combo = db.COMBO_VACCINEs.FirstOrDefault(c => c.ma_combo == maCombo);
            //            if (combo != null)
            //            {
            //                tenComboList.Add(combo.ten_combo);
            //                giaComboList.Add(Convert.ToDecimal(combo.gia_combo));
            //            }
            //        }
            //    }

            //    // Lưu thông tin vào ViewBag để truy cập từ view
            //    ViewBag.TenVaccine = tenVaccineList;
            //    ViewBag.GiaVaccine = giaVaccineList;
            //    ViewBag.TenCombo = tenComboList;
            //    ViewBag.GiaCombo = giaComboList;

            //    // Tính toán phí lưu kho và tổng thanh toán
            //    decimal tongGiaVaccine = giaVaccineList.Sum();
            //    decimal tongGiaCombo = giaComboList.Sum();
            //    //
            //    //sotiengiamgia = (tongGiaVaccine + tongGiaCombo) * giagiamcuavc * 0.1M;
            //    phiLuuKho = (tongGiaVaccine + tongGiaCombo) * 0.2M;
            //    tongThanhToan = phiLuuKho + tongGiaVaccine + tongGiaCombo;

            //    // Gán giá trị cho ViewBag
            //    ViewBag.PhiLuuKho = phiLuuKho;
            //    ViewBag.TongThanhToan = tongThanhToan;
            //    // Trả về View
            //    return View();
            //}


            if (action != "ThanhToan")
            {
                ViewBag.CurrentStep = 2;

                // Lưu thông tin vào ViewBag để truy cập từ view
                ViewBag.TenKhachHang = hoTenNguoiTiem;
                ViewBag.NgaySinhNguoiTiem = ngaySinhNguoiTiem.ToString("yyyy-MM-dd");
                ViewBag.DiaChiNguoiTiem = diaChiNguoiTiem;
                ViewBag.TenNguoiTiem = hoTenNguoiLienHe;
                ViewBag.GioiTinhNguoiTiem = gioiTinh;
                ViewBag.MoiQuanHe = moiQuanHe;
                ViewBag.SoDienThoai = soDienThoai;
                ViewBag.NgayMuonTiem = ngayMuonTiem.ToString("yyyy-MM-dd");
                ViewBag.MaVoucher = maVoucher;
                List<string> tenVaccineList = new List<string>();
                List<decimal> giaVaccineList = new List<decimal>();
                List<string> tenComboList = new List<string>();
                List<decimal> giaComboList = new List<decimal>();

                // Truy vấn để lấy thông tin về tên và giá của vaccine từ cơ sở dữ liệu
                if (maVaccineList != null)
                {
                    foreach (var maVaccine in maVaccineList)
                    {
                        var vaccine = db.VACCINEs.FirstOrDefault(v => v.ma_vaccine == maVaccine);
                        if (vaccine != null)
                        {
                            tenVaccineList.Add(vaccine.ten_vaccine);
                            giaVaccineList.Add(Convert.ToDecimal(vaccine.gia_vacine));
                        }
                    }
                }

                // Truy vấn để lấy thông tin về tên và giá của combo từ cơ sở dữ liệu
                if (maComboList != null)
                {
                    foreach (var maCombo in maComboList)
                    {
                        var combo = db.COMBO_VACCINEs.FirstOrDefault(c => c.ma_combo == maCombo);
                        if (combo != null)
                        {
                            tenComboList.Add(combo.ten_combo);
                            giaComboList.Add(Convert.ToDecimal(combo.gia_combo));
                        }
                    }
                }

                // Lưu thông tin vào ViewBag để truy cập từ view
                ViewBag.TenVaccine = tenVaccineList;
                ViewBag.GiaVaccine = giaVaccineList;
                ViewBag.TenCombo = tenComboList;
                ViewBag.GiaCombo = giaComboList;

                // Tính toán phí lưu kho và tổng thanh toán
                decimal tongGiaVaccine = giaVaccineList.Sum();
                decimal tongGiaCombo = giaComboList.Sum();

                phiLuuKho = (tongGiaVaccine + tongGiaCombo) * 0.2M;
                // Kiểm tra và áp dụng giảm giá nếu có mã voucher
                decimal giamGia = 0;
                if (!string.IsNullOrEmpty(maVoucher))
                {
                    var voucher = db.VOUCHERs.FirstOrDefault(v => v.ma_voucher == maVoucher && v.ngayketthuc_voucher >= DateTime.Today && v.ngaybatdau_voucher <= DateTime.Today);
                    if (voucher != null)
                    {
                        giamGia = (tongGiaVaccine + tongGiaCombo + phiLuuKho) * (decimal)(voucher.giamgia_voucher / 100);
                    }
                }
                tongThanhToan = phiLuuKho + tongGiaVaccine + tongGiaCombo - giamGia;

                // Gán giá trị cho ViewBag
                ViewBag.PhiLuuKho = phiLuuKho;
                ViewBag.TongThanhToan = tongThanhToan;
                ViewBag.GiamGia = giamGia;

                // Trả về View
                return View();
            }


            // Nếu action là "ThanhToan"
            if (action == "ThanhToan")
            {
                // Thực hiện lưu dữ liệu vào cơ sở dữ liệu
                using (var dbContext = new QL_VACCINEDataContext())
                {
                    var latestDangKy = dbContext.NGUOITIEM_DANGKies.OrderByDescending(dk => dk.ma_dangky).FirstOrDefault();
                    string maDangKy = "DK001"; // Mặc định nếu chưa có mã nào
                    if (latestDangKy != null)
                    {
                        // Tách phần số từ mã đăng ký hiện tại và tăng lên 1
                        int latestNumber = int.Parse(latestDangKy.ma_dangky.Substring(2));
                        maDangKy = "DK" + (latestNumber + 1).ToString("D3");
                    }
                    string phiLuuKhoValue = Request.Form["PhiLuuKho"];
                    string tongThanhToanValue = Request.Form["TongThanhToan"];

                    // Khởi tạo biến để lưu giá trị đã chuyển đổi
                    double phiLuuKhoo;
                    double tongThanhToann;

                    // Chuyển đổi giá trị sang kiểu double
                    phiLuuKhoo = Convert.ToDouble(phiLuuKhoValue);
                    tongThanhToann = Convert.ToDouble(tongThanhToanValue);

                    // Khởi tạo biến để lưu giá trị đã chuyển đổi
                    var maVaccineListt = Request.Form["maVaccineListt"]?.ToString()?.Split(',');
                    var maComboListt = Request.Form["maComboListt"]?.ToString()?.Split(',');

                    // Lưu thông tin vào bảng NGUOITIEM_DANGKY
                    var nguoiTiem = new NGUOITIEM_DANGKY
                    {
                        ma_dangky = maDangKy,
                        hoten_nguoitiem = hoTenNguoiTiem,
                        ngaysinh_nguoitiem = ngaySinhNguoiTiem,
                        gioitinh_nguoitiem = gioiTinh,
                        diachi_nguoitiem = diaChiNguoiTiem,
                        hoten_nguoilienhe = hoTenNguoiLienHe,
                        moiquanhe_nguoitiem = moiQuanHe,
                        sdt_nguoilienhe = sdtNguoiLienHe,
                        ngay_muontiem = ngayMuonTiem,
                        ma_khachhang = maKhachHang,
                        ma_voucher = maVoucher,
                        phi_luukho = phiLuuKhoo, // Chuyển đổi rõ ràng từ decimal sang double?
                        tongthanhtoan = tongThanhToann, // Chuyển đổi rõ ràng từ decimal sang double?
                        ngay_dangky = DateTime.Now

                    };

                    dbContext.NGUOITIEM_DANGKies.InsertOnSubmit(nguoiTiem);
                    dbContext.SubmitChanges();
                    var soluong = 1;
                    // Lưu thông tin vào bảng NGUOITIEM_MUAVACCINE cho tất cả các vaccine
                    if (maVaccineListt != null)
                    {
                        foreach (var maVaccine in maVaccineListt)
                        {
                            var muavaccine = new NGUOITIEM_MUAVACCINE
                            {
                                ma_dangky = maDangKy,
                                ma_vaccine = maVaccine,
                                so_luong = soluong,
                            };

                            dbContext.NGUOITIEM_MUAVACCINEs.InsertOnSubmit(muavaccine);
                        }
                        dbContext.SubmitChanges();
                    }

                    // Lưu thông tin vào bảng NGUOITIEM_MUACOMBO cho tất cả các combo
                    if (maComboListt != null)
                    {
                        foreach (var maCombo in maComboListt)
                        {
                            var muacombo = new NGUOITIEM_MUACOMBO
                            {
                                ma_dangky = maDangKy,
                                ma_combo = maCombo,
                                so_luong = soluong,
                            };

                            dbContext.NGUOITIEM_MUACOMBOs.InsertOnSubmit(muacombo);
                        }
                        dbContext.SubmitChanges();
                    }

                    using (var dbb = new QL_VACCINEDataContext())
                    {
                        // Các bước lưu dữ liệu vào cơ sở dữ liệu như bạn đã thực hiện

                        // Xóa hết sản phẩm trong giỏ hàng của khách hàng
                        var gioHang = dbb.GIOHANGs.Where(g => g.ma_khachhang == maKhachHang); // Thay tenTaiKhoan bằng biến chứa tên tài khoản của khách hàng
                        foreach (var item in gioHang)
                        {
                            dbb.GIOHANGs.DeleteOnSubmit(item);
                        }
                        dbb.SubmitChanges();
                    }
                    return RedirectToAction("Buoc3", new { TongThanhToan = tongThanhToann, MaDangKy = maDangKy });
                }
            }
            return View();
        }

        //===================Chính sách và điều kiện đăng ký=====================
        [HttpPost]
        public ActionResult ChinhSachDieuKien(string action)
        {
            if (action == "dongY")
            {
                // Lưu dữ liệu từ Request.Form vào TempData
                TempData["name"] = Request.Form["name"];
                TempData["dob"] = Request.Form["dob"];
                TempData["address"] = Request.Form["address"];
                TempData["contact-name"] = Request.Form["contact-name"];
                TempData["sex"] = Request.Form["sex"];
                TempData["relationship"] = Request.Form["relationship"];
                TempData["phone"] = Request.Form["phone"];
                TempData["desired-date"] = Request.Form["desired-date"];
                TempData["ma-voucher"] = Request.Form["ma-voucher"];
                TempData["maVaccineList"] = Request.Form.GetValues("maVaccineList");
                TempData["maComboList"] = Request.Form.GetValues("maComboList");

                // Chuyển hướng đến trang DieuKhoanThanhToan
                return RedirectToAction("DieuKhoanThanhToan", "GioHang", new { action = "ThanhToan" });
            }
            else if (action == "quayLai")
            {
                // Quay lại bước 1
                return RedirectToAction("DangKyMuiTiem", "GioHang");
            }

            return View();
        }
        //=============Thực hiện đăng ký tiêm =======================
        public ActionResult Buoc3(string maDangKy, double tongThanhToan)
        {
            ViewBag.CurrentStep = 3;
            var maKhachHang = Session["maKH"]?.ToString();
            var db = new QL_VACCINEDataContext();
            var taiKhoan = db.KHACHHANGs.FirstOrDefault(tk => tk.ma_khachhang == maKhachHang);

            // Nếu tài khoản tồn tại, gán thông tin vào ViewBag
            if (taiKhoan != null)
            {
                ViewBag.SoDienThoaiTaiKhoan = taiKhoan.sdt_khachhang;
            }
            ViewBag.TongThanhToan = tongThanhToan;
            ViewBag.MaDangKy = maDangKy;

            using (var dbContext = new QL_VACCINEDataContext())
            {
                var purchasedVaccines = dbContext.NGUOITIEM_MUAVACCINEs
                    .Where(mv => mv.ma_dangky == maDangKy)
                    .Select(mv => mv.ma_vaccine)
                    .ToList();

                // Log purchasedVaccines
                Debug.WriteLine("Purchased Vaccines:");
                foreach (var vaccine in purchasedVaccines)
                {
                    Debug.WriteLine(vaccine);
                }

                var transactions = dbContext.NGUOITIEM_MUAVACCINEs
                    .GroupBy(mv => mv.ma_dangky)
                    .Select(g => new Transaction(g.Select(mv => mv.ma_vaccine).ToList()))
                    .ToList();

                // Log transactions
                Debug.WriteLine("Transactions Count: " + transactions.Count);
                foreach (var transaction in transactions)
                {
                    Debug.WriteLine(string.Join(", ", transaction.Items));
                }

                double[] possibleMinValues = {0.0001 };
                double minSupport = 0;
                double minConfidence = 0;

                foreach (var minSupportValue in possibleMinValues)
                {
                    foreach (var minConfidenceValue in possibleMinValues)
                    {
                        Debug.WriteLine($"MinSupport: {minSupportValue}, MinConfidence: {minConfidenceValue}");
                        var apriori = new Apriori(minSupportValue, minConfidenceValue);
                        var result = apriori.Run(transactions);

                        // Kiểm tra xem kết quả không rỗng
                        if (result.Rules.Any())
                        {
                            minSupport = minSupportValue;
                            minConfidence = minConfidenceValue;
                            break;
                        }
                        Debug.WriteLine("");
                    }
                    if (minSupport != 0 && minConfidence != 0)
                    {
                        break;
                    }
                }

                // Sử dụng minSupport và minConfidence đã chọn để chạy thuật toán
                var aprioriFinal = new Apriori(minSupport, minConfidence);
                var resultFinal = aprioriFinal.Run(transactions);

                // Log result
                Debug.WriteLine("Frequent Itemsets:");
                foreach (var itemset in resultFinal.FrequentItemsets)
                {
                    Debug.WriteLine(string.Join(", ", itemset));
                }

                Debug.WriteLine("Association Rules:");
                foreach (var rule in resultFinal.Rules)
                {
                    Debug.WriteLine($"Rule: {string.Join(", ", rule.X)} => {string.Join(", ", rule.Y)} (Support: {rule.Support}, Confidence: {rule.Confidence})");
                }

                var recommendedItems = resultFinal.Rules
                    .Where(r => purchasedVaccines.All(p => r.X.Contains(p)))
                    .SelectMany(r => r.Y)
                    .Distinct()
                    .Except(purchasedVaccines)
                    .ToList();

                // Log recommendedItems và giá trị minSupport, minConfidence tương ứng
                Debug.WriteLine($"Recommended Items (minSupport: {minSupport}, minConfidence: {minConfidence}):");
                foreach (var item in recommendedItems)
                {
                    Debug.WriteLine(item);
                }

                var recommendedVaccineDetails = dbContext.VACCINEs
                   .Where(v => recommendedItems.Contains(v.ma_vaccine))
                   .ToList();
                ViewBag.RecommendedVaccines = recommendedVaccineDetails;

            }

            return View();
        }

        //=============Kiểm tra lịch sử đang ký tiêm =======================
        [HttpPost]
        public JsonResult KiemTraLichSuTiem(string tenNguoiTiem, string maKhachHang, string loaiOption)
        {
            var khachHang = db.KHACHHANGs.FirstOrDefault(k => k.ma_khachhang == maKhachHang);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Khách hàng không tồn tại." });
            }

            var ngayHienTai = DateTime.Now;
            var dangKyCu = db.NGUOITIEM_DANGKies
                             .Where(d => d.hoten_nguoitiem == tenNguoiTiem && d.ma_khachhang == maKhachHang)
                             .OrderByDescending(d => d.ngay_dangky)
                             .FirstOrDefault();

            if (dangKyCu != null)
            {
                var thoiGianTruocDay = (loaiOption == "Vaccine") ? 3 : 9; // Kiểm tra thời gian 3 tháng cho vaccine, 9 tháng cho combo
                if ((ngayHienTai - dangKyCu.ngay_dangky).TotalDays < thoiGianTruocDay * 30)
                {
                    var message = (loaiOption == "Vaccine") ? "Vaccine" : "Combo vaccine";
                    return Json(new { success = false, message = $"Lần tiêm trước chưa đủ {thoiGianTruocDay} tháng. Bạn không thể đăng ký tiêm {message} mới." });
                }
            }

            return Json(new { success = true });
        }
        //=============Kiểm tra mã Voucher =======================
        public ActionResult CheckVoucher(string maVoucher)
        {
            // Kết nối với cơ sở dữ liệu và kiểm tra xem mã voucher có tồn tại không
            var voucher = db.VOUCHERs.FirstOrDefault(v => v.ma_voucher == maVoucher);

            if (voucher != null)
            {
                // Kiểm tra ngày kết thúc voucher
                if (voucher.ngayketthuc_voucher < DateTime.Now)
                {
                    // Mã voucher đã hết hạn, trả về thông báo lỗi
                    return Json(new { isValid = false, message = "Mã voucher không hợp lệ!" });
                }
                else if (voucher.ngaybatdau_voucher > DateTime.Now)
                {
                    // Mã voucher đã hết hạn, trả về thông báo lỗi
                    return Json(new { isValid = false, message = "Mã voucher chưa tới ngày sử dụng!" });
                }
                else
                {
                    // Mã voucher tồn tại và còn hiệu lực, thực hiện các xử lý tiếp theo
                    return Json(new { isValid = true, message = "Mã voucher hợp lệ!" });
                }
            }
            else
            {
                // Mã voucher không tồn tại, trả về thông báo lỗi
                return Json(new { isValid = false, message = "Mã voucher không hợp lệ!" });
            }
        }



    }
}
