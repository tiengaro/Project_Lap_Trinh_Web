using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OkCatStore.Models;

namespace OkCatStore.Controllers
{
    public class NguoiDungLayoutController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: NguoiDungLayout
        [HttpGet]
        public ActionResult Index()
        {
            var mPhongCach = data.PHONGCACHes.Select(x => x);
            return View(mPhongCach);
        }
        public ActionResult ThuongHieu()
        {
            var mThuongHieu = from th in data.THUONGHIEUs select th;
            return PartialView(mThuongHieu);
        }

        public ActionResult LoaiGiay()
        {
            var mLoaiGiay = from lg in data.PHONGCACHes select lg;
            return PartialView(mLoaiGiay);
        }

        public ActionResult ChonGiay()
        {
            var mChonGiay = from cg in data.LOAISPs select cg;
            return PartialView(mChonGiay);
        }

        public ActionResult ChonLoaiGiay()
        {
            var mChonLoaiGiay = from clg in data.PHONGCACHes select clg;
            return PartialView(mChonLoaiGiay);
        }

        public ActionResult LaySanPhamTheoLoaiGiay(int MAPC, int count)
        {
            var mSanPham = data.SANPHAMs.Where(x => x.MAPC == MAPC).Select(x => x).Take(count);
            return PartialView(mSanPham);
        }

        public ActionResult LayHinhSanPham(int MASP, int count)
        {
            var mHinhSanPham = data.HINHANHs.Where(x => x.MASP == MASP).Take(count);
            return PartialView(mHinhSanPham);
        }

        public ActionResult Details(int id)
        {
            var mSanPham = data.SANPHAMs.First(x => x.MASP == id);
            return View(mSanPham);
        }

        public ActionResult DetailsImage(int id, int count)
        {
            var mHinhSanPham = data.HINHANHs.Where(x => x.MASP == id).Take(count);
            return PartialView(mHinhSanPham);
        }

        public ActionResult SPTheoThuongHieu(int id)
        {
            var mSPTH = data.SANPHAMs.Where(x => x.MATH == id).Select(x => x);
            return View(mSPTH);
        }

        public ActionResult SanPhamTheoPhongCach(int id)
        {
            var mSPPC = data.SANPHAMs.Where(x => x.MAPC == id);
            return View(mSPPC);
        }

        public ActionResult GetTenThuongHien(int id)
        {
            var mThuongHieu = data.THUONGHIEUs.Where(x => x.MATH == id);
            return PartialView(mThuongHieu);
        }
        public ActionResult GetTenPhongCach(int id)
        {
            var mPhongCach = data.PHONGCACHes.Where(x => x.MAPC == id);
            return PartialView(mPhongCach);
        }
        public ActionResult GetSanPhamMoi()
        {
            var mSanPhamMoi = data.SANPHAMs.OrderByDescending(n => n.NGAYCAPNHAT).Take(4).ToList();
            return PartialView(mSanPhamMoi);
        }
        public ActionResult GetSanPhamLienQuan(int id)
        {
            var mSanPhamLienQuan = data.SANPHAMs.OrderByDescending(n => n.NGAYCAPNHAT).Take(10).Where(m => m.MATH == id);
            return PartialView(mSanPhamLienQuan);
        }
        public ActionResult GetSizeSanPham(int id)
        {
            bool mNam = false;
            bool mNu = false;
            var lstLoaiSP = data.CT_LOAISPs.Where(n => n.MASP == id);
            foreach(var item in lstLoaiSP)
            {
                if (item.MALOAISP.Contains("NA")) mNam = true;
                else if (item.MALOAISP.Contains("NU")) mNu = true;
            }
            if(mNam == true && mNu == true)
            {
                var lstSize = from s in data.SIZEs select s;
                return PartialView(lstSize);
            }
            else
            {
                if(mNam == true)
                {
                    var lstSize = data.SIZEs.Where(n => n.MALOAISP == "NA");
                    return PartialView(lstSize);
                }
                else
                {
                    var lstSize = data.SIZEs.Where(n => n.MALOAISP == "NU");
                    return PartialView(lstSize);
                }
            }
        }
        [HttpPost]
        public ActionResult TimSanPham(FormCollection collection)
        {
            var tenSP = collection["Search"];
            var lstSanPham = data.SANPHAMs.Where(n => n.TENSP.Contains(tenSP));
            return View(lstSanPham);
        }
        public ActionResult CachDoSize()
        {
            return View();
        }

        public ActionResult LienHe()
        {
            return View();
        }
        public ActionResult GetReviews(int id)
        {
            var mReviews = data.REVIEWs.Where(n => n.MASP == id).OrderByDescending(n => n.time);
            ViewBag.CountReviews = data.REVIEWs.Where(n => n.MASP == id).Count();
            return PartialView(mReviews);
        }

        [HttpPost]
        public ActionResult PostReview(FormCollection collection, REVIEW rv, string strURL, int id)
        {
            var name = collection["Name"];
            var email = collection["Email"];
            var phone = collection["Telephone"];
            var review = collection["Review"];
            var MASP = id;
            var time = DateTime.Now;

            rv.name = email;
            rv.email = email;
            rv.phone = phone;
            rv.review = review;
            rv.MASP = MASP;
            rv.time = time;
            data.REVIEWs.InsertOnSubmit(rv);
            data.SubmitChanges();

            return Redirect(strURL);
        }
        public ActionResult CreateTabs(int id)
        {
            var mSanPham = data.SANPHAMs.First(x => x.MASP == id);
            ViewBag.MaSP = mSanPham.MASP;
            ViewBag.TenSP = mSanPham.TENSP;
            var mReviews = data.REVIEWs.Where(x => x.MASP == id);
            return PartialView(mReviews);
        }
    }
}