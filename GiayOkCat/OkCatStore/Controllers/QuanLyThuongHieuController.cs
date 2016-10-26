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
    public class QuanLyThuongHieuController : Controller
    {
        // GET: QuanLyThuongHieu
        DataClasses1DataContext data = new DataClasses1DataContext();
        DataClasses1DataContext dataHinh = new DataClasses1DataContext();
        public ActionResult Index(int? page)
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.THUONGHIEUs.ToList().OrderBy(n => n.MATH).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Sua(int id)
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            THUONGHIEU thuonghieu = data.THUONGHIEUs.SingleOrDefault(n => n.MATH == id);
            ViewBag.MATH = thuonghieu.MATH;
            if (thuonghieu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(thuonghieu);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Sua(int id, FormCollection coll)
        {
            THUONGHIEU thuonghieu = data.THUONGHIEUs.SingleOrDefault(n => n.MATH == id);
            ViewBag.MATH = thuonghieu.MATH;
            if (thuonghieu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            UpdateModel(thuonghieu);
            data.SubmitChanges();

            return RedirectToAction("Index");
        }
        public ActionResult ThemMoiTH()
        {
            //if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoiTH(THUONGHIEU thuonghieu)
        {
            data.THUONGHIEUs.InsertOnSubmit(thuonghieu);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }

        //Xóa sản phẩm
        public ActionResult Xoa(int id)
        {
            if (CheckAdmin() == false) return RedirectToAction("Login", "QuanLySanPham");
            //Lay ra doi tuong sach can xoa theo ma
            THUONGHIEU thuonghieu = data.THUONGHIEUs.SingleOrDefault(n => n.MATH == id);
            ViewBag.MATH = thuonghieu.MATH;
            if (thuonghieu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(thuonghieu);
        }

        [HttpPost]
        public ActionResult Xoa(int id, FormCollection coll)
        {
            //Lay ra doi tuong sach can xoa theo ma
            THUONGHIEU thuonghieu = data.THUONGHIEUs.SingleOrDefault(n => n.MATH == id);
            ViewBag.MATH = thuonghieu.MATH;
            if (thuonghieu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            try
            {
                data.THUONGHIEUs.DeleteOnSubmit(thuonghieu);
                data.SubmitChanges();
            }
            catch (Exception e)
            {
                ViewBag.Loi = "Không thể xóa sản phẩm này";
                return Xoa(id);
            }

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