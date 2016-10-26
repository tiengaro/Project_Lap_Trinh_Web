using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using OkCatStore.Models;
using System.IO;

namespace OkCatStore.Controllers
{
    public class QuanLySanPhamController : Controller
    {
        // GET: QuanLySanPham
        DataClasses1DataContext data = new DataClasses1DataContext();
        DataClasses1DataContext dataHinh = new DataClasses1DataContext();
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            // Gán các giá trị người dùng nhập liệu cho các biến 
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                //Gán giá trị cho đối tượng được tạo mới (ad)        

                CONFIG ad = data.CONFIGs.SingleOrDefault(n => n.TAIKHOAN == tendn && n.MATKHAU == matkhau);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "QuanLySanPham");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }


        public ActionResult Index(int? page)
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.SANPHAMs.ToList().OrderByDescending(n => n.NGAYCAPNHAT).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemMoi()
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            ViewBag.MATH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TENTH), "MATH", "TENTH");
            ViewBag.MAPC = new SelectList(data.PHONGCACHes.ToList().OrderBy(n => n.TENPC), "MAPC", "TENPC");

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoi(SANPHAM sanpham, HINHANH hinhanh, HttpPostedFileBase file, FormCollection coll)
        {
            ViewBag.MaTH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TENTH), "MATH", "TENTH");
            ViewBag.MaPC = new SelectList(data.PHONGCACHes.ToList().OrderBy(n => n.TENPC), "MAPC", "TENPC");
            // Kiem tra duong dan file
            if (file == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        file.SaveAs(path);
                    }

                    //Luu vao CSDL
                    data.SANPHAMs.InsertOnSubmit(sanpham);
                    data.SubmitChanges();
                    hinhanh.URLHINH = fileName;
                    hinhanh.MASP = sanpham.MASP;
                    data.HINHANHs.InsertOnSubmit(hinhanh);
                    data.SubmitChanges();
                }
                return RedirectToAction("Index");
            }
        }
        //Hiển thị sản phẩm
        public ActionResult ChiTiet(int id)
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MASP = sanpham.MASP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }
        //Xóa sản phẩm
        [HttpGet]
        public ActionResult Xoa(int id)
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MASP = sanpham.MASP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }

        [HttpPost]
        public ActionResult Xoa(int id, FormCollection coll)
        {
            SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MASP = sanpham.MASP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            try
            {
                IEnumerable<HINHANH> listHinhAnh = (data.HINHANHs.Where(n => n.MASP == sanpham.MASP)).ToList();
                data.HINHANHs.DeleteAllOnSubmit(listHinhAnh);
                IEnumerable<CT_LOAISP> listLoaiSP = (data.CT_LOAISPs.Where(n => n.MASP == sanpham.MASP)).ToList();
                data.CT_LOAISPs.DeleteAllOnSubmit(listLoaiSP);
                //data.SubmitChanges();
                data.SANPHAMs.DeleteOnSubmit(sanpham);
                data.SubmitChanges();
            }
            catch(Exception e)
            {
                ViewBag.Loi = "Sản phẩm này không thể xóa";
            }

            return RedirectToAction("Index");
        }
        //Chinh sửa sản phẩm
        [HttpGet]
        public ActionResult Sua(int id)
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MASP = sanpham.MASP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MATH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TENTH), "MATH", "TENTH", sanpham.MATH);
            ViewBag.MAPC = new SelectList(data.PHONGCACHes.ToList().OrderBy(n => n.TENPC), "MAPC", "TENPC", sanpham.MAPC);
            ViewBag.URLHinh = data.HINHANHs.SingleOrDefault(n => n.MASP == id).URLHINH ;
            return View(sanpham);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Sua(int id, HttpPostedFileBase file, FormCollection coll)
        {
            SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            HINHANH hinhanh = data.HINHANHs.SingleOrDefault(n => n.MASP == id);

            ViewBag.MATH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TENTH), "MATH", "TENTH");
            ViewBag.MAPC = new SelectList(data.PHONGCACHes.ToList().OrderBy(n => n.TENPC), "MAPC", "TENPC");
            if (file != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/images"), fileName);
                if (System.IO.File.Exists(path))
                    ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                else
                {
                    file.SaveAs(path);
                }
                hinhanh.URLHINH = fileName;
            }
            //Them vao CSDL
            if (ModelState.IsValid)
            {
                UpdateModel(sanpham);
                UpdateModel(hinhanh);
                data.SubmitChanges();
            }
            return RedirectToAction("Index");

        }
        public bool CheckAdmin()
        {
            CONFIG ad = (CONFIG) Session["Taikhoanadmin"] ;
            if (ad != null)
            {
                return true;
            }
            return false;
        }
    }
}
    