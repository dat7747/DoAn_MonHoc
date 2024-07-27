using Hangfire;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Web_TiemChungVaccine.Models
{
    public static class HangfireJobs
    {
        public static void ScheduleJobs()
        {
            LogMessage("ScheduleJobs method called.");

            // Lập lịch công việc CheckAndSendEmailJob
            RecurringJob.AddOrUpdate("CheckAndSendEmailJob", () => CheckAndSendEmailJob(), Cron.Daily(01, 19));
            LogMessage("Recurring job 'CheckAndSendEmailJob' scheduled.");
        }

        public static void CheckAndSendEmailJob()
        {
            try
            {
                LogMessage("CheckAndSendEmailJob started.");
                using (var db = new QL_VACCINEDataContext())
                {
                    var query = from kh in db.KHACHHANGs
                                join nd in db.NGUOITIEM_DANGKies on kh.ma_khachhang equals nd.ma_khachhang
                                where nd.ngay_muontiem == DateTime.Today.AddDays(1)
                                select new
                                {
                                    MaKhachHang = kh.ma_khachhang,
                                    Email = kh.email_khachhang,
                                    HoTen = nd.hoten_nguoitiem,
                                    NgayTiem = nd.ngay_muontiem
                                };

                    LogMessage($"Found {query.Count()} customers with upcoming vaccination.");
                    foreach (var item in query)
                    {
                        LogMessage($"Preparing to send email to {item.Email} for {item.HoTen} with vaccination date {item.NgayTiem}");
                        SendReminderEmail(item.Email, item.HoTen, item.NgayTiem);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error in CheckAndSendEmailJob: {ex.Message}");
            }
        }


        //private static void SendReminderEmail(string email, string hoTen, DateTime ngayTiem)
        //{
        //    try
        //    {
        //        MailMessage m = new MailMessage(
        //            new MailAddress("nathadatiemchungvaccine2000@gmail.com", "NATHADA"),
        //            new MailAddress(email));
        //        m.Subject = "Nhắc nhở tiêm vaccine";
        //        m.Body = string.Format("Xin chào {0}, ngày mai là ngày bạn đã đăng ký tiêm vaccine vào ngày {1}. Vui lòng chuẩn bị sẵn sàng!", hoTen, ngayTiem.ToShortDateString());
        //        m.IsBodyHtml = true;

        //        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
        //        {
        //            Credentials = new System.Net.NetworkCredential("nathadatiemchungvaccine2000@gmail.com", "bdxs lrux wwte uqmp"),
        //            EnableSsl = true
        //        };

        //        smtp.Send(m);
        //        LogMessage($"Email reminder successfully sent to {email}");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage($"Error sending email: {ex.Message}");
        //    }
        //}

        private static void SendReminderEmail(string email, string hoTen, DateTime ngayTiem)
        {
            try
            {
                DateTime ngayTiemMoi = ngayTiem.AddDays(1);

                MailMessage m = new MailMessage(
                    new MailAddress("nathadatiemchungvaccine2000@gmail.com", "NATHADA"),
                    new MailAddress(email));
                m.Subject = "Nhắc nhở tiêm vaccine";
                m.Body = string.Format("Xin chào {0}, ngày mai là ngày bạn đã đăng ký tiêm vaccine vào ngày {1}. Vui lòng chuẩn bị sẵn sàng!", hoTen, ngayTiemMoi.ToString("dd/MM/yyyy"));
                m.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential("nathadatiemchungvaccine2000@gmail.com", "bdxs lrux wwte uqmp"),
                    EnableSsl = true
                };

                smtp.Send(m);
                LogMessage($"Email reminder successfully sent to {email}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error sending email: {ex.Message}");
            }
        }

        public static void LogMessage(string message)
        {
            string logFilePath = "~/App_Data/email_log.txt"; // Đảm bảo đường dẫn này tồn tại

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to write log: {ex.Message}");
            }
        }
    }
}