using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using OkCatStore.Models;

namespace OkCatStore.Controllers
{
    public class QuanLyKhachHangController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: QuanLyKhachHang
        public ActionResult Index(int? page)
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.KHACHHANGs.ToList().OrderBy(n => n.MAKH).ToPagedList(pageNumber, pageSize));
        }
        //Hiển thị sản phẩm
        public ActionResult ChiTiet(int id)
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            //Lay ra doi tuong sach theo ma
            KHACHHANG khachhang = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);
            ViewBag.MAKH = khachhang.MAKH;
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(khachhang);
        }
        //Xóa sản phẩm
        public ActionResult Xoa(int id)
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            //Lay ra doi tuong sach can xoa theo ma
            KHACHHANG khachhang = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);
            ViewBag.MAKH = khachhang.MAKH;
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(khachhang);
        }
        [HttpPost]
        public ActionResult Xoa(int id, FormCollection coll)
        {
            //Lay ra doi tuong sach can xoa theo ma
            KHACHHANG khachhang = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);
            ViewBag.MAKH = khachhang.MAKH;
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.KHACHHANGs.DeleteOnSubmit(khachhang);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
        public bool CheckAdmin()
        {
            CONFIG ad = (CONFIG)Session["Taikhoanadmin"];
            if (ad != null)
            {
                return true;
            }
            return false;
        }
    }
}