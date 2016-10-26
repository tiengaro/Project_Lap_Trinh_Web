using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OkCatStore.Models;

namespace OkCatStore.Controllers
{
    public class NguoiDungController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: NguoiDung
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACHHANG kh)
        {
            //var tendangnhap = collection["User"];
            var email = collection["Email"];
            var matkhau = collection["Password"];
            //var xacnhanmatkhau = collection["ConfirmPassword"];
            var hoten = collection["Name"];
            //var diachi = collection["Address"];
            //var dienthoai = collection["Phone"];
            //var ngaysinh = String.Format("{0:MM/dd/YYYY}", collection["Birth"]);

            //if (String.IsNullOrEmpty(tendangnhap))
            //{
            //    ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            //}
            //else if (String.IsNullOrEmpty(email))
            //{
            //    ViewData["Loi2"] = "Email Không được bỏ trống";
            //}
            //else if (String.IsNullOrEmpty(matkhau))
            //{
            //    ViewData["Loi3"] = "Phải nhập mật khẩu";
            //}
            //else if (String.IsNullOrEmpty(xacnhanmatkhau))
            //{
            //    ViewData["Loi4"] = "Phải nhập lại mật khẩu";
            //}
            //else if (String.IsNullOrEmpty(hoten))
            //{
            //    ViewData["Loi5"] = "Phải nhập họ tên";
            //}
            //else if (String.IsNullOrEmpty(diachi))
            //{
            //    ViewData["Loi6"] = "Phải nhập điện thoại";
            //}
            //else if (String.IsNullOrEmpty(email))
            //{
            //    ViewData["Loi7"] = "Phải nhập email";
            //}
            //else
            //{
                //kh.HOTENKH = tendangnhap;
                kh.EMAILKH = email;
                kh.MATKHAU = matkhau;
                kh.HOTENKH = hoten;
                //kh.DIACHIKH = diachi;
                //kh.SODTKH = dienthoai;
                //kh.NGAYSINH = DateTime.Parse(ngaysinh);
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
            //}
            return this.Index();
        }
    }
}