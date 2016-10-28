using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OkCatStore.Models;
using Facebook;
using System.Configuration;

namespace OkCatStore.Controllers
{
    public class TaiKhoanController : Controller
    {

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: TaiKhoan
        public ActionResult AccountDialog()
        {
            KHACHHANG kh = Session["TaiKhoan"] as KHACHHANG;
            ViewBag.KhachHang = layKhachHang();
            return PartialView();

        }

        private KHACHHANG layKhachHang()
        {
            KHACHHANG kh = Session["TaiKhoan"] as KHACHHANG;
            return kh;
        }
        public ActionResult MyAccount(int id)
        {
            KHACHHANG kh = data.KHACHHANGs.First(n => n.MAKH == id);
            return View(kh);
        }
        [HttpPost]
        public ActionResult MyAccount(int id, FormCollection collection)
        {
            var email = collection["Email"];
            //var matkhaumoi = collection["NewPassword"];
            //var matkhauhientai = collection["OldPassword"];
            var hoten = collection["Name"];
            var diachi = collection["Address"];
            var dienthoai = collection["Phone"];
            KHACHHANG kh = data.KHACHHANGs.First(n => n.MAKH == id);
            //if (string.Equals(matkhauhientai, kh.MATKHAU) == true){
            kh.EMAILKH = email;
            kh.HOTENKH = hoten;
            kh.DIACHIKH = diachi;
            kh.SODTKH = dienthoai;
            UpdateModel(kh);
            data.SubmitChanges();
            ViewData["ThanhCong"] = "Cập Nhật Thành Công";
            //}
            //else
            //{
            //    ViewData["Loi"] = "Sai Mật Khẩu";
            //}
            Session["TaiKhoan"] = kh;
            return MyAccount(id);
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACHHANG kh, string strURL)
        {
            var email = collection["Email"];
            var matkhau = collection["Password"];
            var hoten = collection["Name"];
            var diachi = collection["Address"];
            var dienthoai = collection["Phone"];

            kh.EMAILKH = email;
            kh.MATKHAU = matkhau;
            kh.HOTENKH = hoten;
            kh.DIACHIKH = diachi;
            kh.SODTKH = dienthoai;
            try
            {
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
            }
            catch(Exception e)
            {

            }

            return Redirect(strURL);
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection, string strURL)
        {
            var email = collection["Email"];
            var password = collection["Password"];

            KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.EMAILKH == email && n.MATKHAU == password);
            if (kh != null)
            {
                ViewBag.Thongbao = "Đăng nhập thành công";
                Session["TaiKhoan"] = kh;
                return RedirectToAction("MyAccount", 
                    new { id = kh.MAKH});
            }
            else
                ViewBag.Thongbao = "Đăng nhập thất bại";
            return Redirect(strURL);
        }

        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return Redirect("/");
        }

        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });

            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });


            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                // Get the user's information, like email, first name, middle name etc
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;

                var user = data.KHACHHANGs.SingleOrDefault(x => x.EMAILKH == email);
                if (user == null)
                {
                    KHACHHANG kh = new KHACHHANG();
                    kh.EMAILKH = email;
                    kh.HOTENKH = lastname + " " + middlename + " " + firstname;
                    kh.MATKHAU = "123456";
                    data.KHACHHANGs.InsertOnSubmit(kh);
                    data.SubmitChanges();

                    Session["TaiKhoan"] = kh;

                    return RedirectToAction("MyAccount",
                        new { id = kh.MAKH });
                }
                else
                {
                    Session["TaiKhoan"] = user;
                }

            }
            return Redirect("/");
        }

        public ActionResult DoiMatKhau(int id)
        {
            KHACHHANG kh = data.KHACHHANGs.First(n => n.MAKH == id);
            return View(id);
        }
        [HttpPost]
        public ActionResult DoiMatKhau(int id, FormCollection coll)
        {
            var oldpassword = coll["OldPassword"];
            var newpassword = coll["NewPassword"];
            var confirmnewPassword = coll["ConfirmNewPassword"];
            KHACHHANG kh = data.KHACHHANGs.First(n => n.MAKH == id);
            if (string.Equals(oldpassword, kh.MATKHAU) == false)
            {
                ViewData["Loi1"] = "Sai Mật Khẩu Hiện Tại";
                return DoiMatKhau(id);
            }
            if (string.Equals(newpassword, confirmnewPassword) == false)
            {
                ViewData["Loi2"] = "Mật khẩu không trùng khớp";
                return DoiMatKhau(id);
            }
            kh.MATKHAU = newpassword;
            UpdateModel(kh);
            data.SubmitChanges();
            ViewData["ThanhCong"] = "Cập Nhật Thành Công";
            Session["TaiKhoan"] = kh;
            return DoiMatKhau(id);
        }
    }
}