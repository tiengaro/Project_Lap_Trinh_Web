using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OkCatStore.Models;

namespace OkCatStore.Controllers
{
    public class GioHangController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        DataClasses1DataContext dataHinh = new DataClasses1DataContext();
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if(lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }

            return lstGioHang;
        }

        public ActionResult ThemGioHang(int iMaSP, string strURL)
        {
            List<GioHang> lstGioHang = LayGioHang();

            GioHang sanPham = lstGioHang.Find(n => n.iMaSP == iMaSP);
            if (sanPham == null)
            {
                sanPham = new GioHang(iMaSP);
                lstGioHang.Add(sanPham);
                return Redirect(strURL);
            }
            else
            {
                sanPham.iSoLuong++;
                return Redirect(strURL);
            }
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if(lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if(lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return dTongTien;
        }

        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "NguoiDungLayout");
            }
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                ViewData["Login"] = "Bạn chưa đăng nhập tài khoản, mời bạn đăng nhập để có thể đặt hàng!";
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        public ActionResult ThongTinGioHang_Partialview()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        
        public ActionResult XoaGioHang (int iMaSP)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanPham = lstGioHang.SingleOrDefault(n => n.iMaSP == iMaSP);
            if(sanPham != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSP == iMaSP);
                return RedirectToAction("GioHang");
            }
            if(lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "NguoiDungLayout");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapNhatGioHang(int iMaSp, FormCollection coll)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanPham = lstGioHang.SingleOrDefault(n => n.iMaSP == iMaSp);
            if(sanPham != null)
            {
                sanPham.iSoLuong = int.Parse(coll["sl"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("GioHang");
            }
            if(Session["GioHang"] == null)
            {
                return RedirectToAction("Index","NguoiDungLayout");
            }

            List<GioHang>  lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection coll, string radio)
        {
            if(radio == "Thanh toán trực tuyến")
            {
                return RedirectToAction("Onepay", "GioHang");
            }
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            List<GioHang> gh = LayGioHang();
            ddh.MAKH = kh.MAKH;
            ddh.NGAYDAT = DateTime.Now;
            var ngayGiao = string.Format("{0:MM/dd/yyyy}", coll["NgayGiao"]);
            var hinhThuc = 1;
            ddh.NGAYGIAO = DateTime.Parse(ngayGiao);
            ddh.TRANGTHAIGIAOHANG = false;
            ddh.TRANGTHAITHANHTOAN = false;
            ddh.MAHTTT = hinhThuc;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach(var item in gh)
            {
                CT_DATHANG ctdh = new CT_DATHANG();
                ctdh.MADDH = ddh.MADDH;
                ctdh.MASP = item.iMaSP;
                ctdh.SOLUONGDAT = item.iSoLuong;
                ctdh.DONGIA = (decimal)item.dDonGia;
                data.CT_DATHANGs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanGioHang", "GioHang");
        }
        public ActionResult XacNhanGioHang()
        {
            return View();
        }
        public ActionResult Onepay()
        {
            double t = TongTien();
            string amount = (t * 100).ToString();
            string url = RedirectOnepay("Thanh Toán", amount, "192.186.0.1");
            return Redirect(url);
        }
        public string RedirectOnepay(string codeInvoice, string amount, string ip)
        {
            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest(OnePayProperty.Url_ONEPAY_TEST);
            conn.SetSecureSecret(OnePayProperty.HASH_CODE);

            // Gan cac thong so de truyen sang cong thanh toan onepay
            conn.AddDigitalOrderField("AgainLink", OnePayProperty.AGAIN_LINK);
            conn.AddDigitalOrderField("Title", "Tich hop onepay vao web asp.net mvc3,4");
            conn.AddDigitalOrderField("vpc_Locale", OnePayProperty.PAYGATE_LANGUAGE);
            conn.AddDigitalOrderField("vpc_Version", OnePayProperty.VERSION);
            conn.AddDigitalOrderField("vpc_Command", OnePayProperty.COMMAND);
            conn.AddDigitalOrderField("vpc_Merchant", OnePayProperty.MERCHANT_ID);
            conn.AddDigitalOrderField("vpc_AccessCode", OnePayProperty.ACCESS_CODE);
            conn.AddDigitalOrderField("vpc_MerchTxnRef", "Thanh Toán");
            conn.AddDigitalOrderField("vpc_OrderInfo", codeInvoice);
            conn.AddDigitalOrderField("vpc_Amount", amount);
            conn.AddDigitalOrderField("vpc_ReturnURL", Url.Action("OnepayResponse", "GioHang", null, Request.Url.Scheme, null));

            // Thong tin them ve khach hang. De trong neu khong co thong tin
            conn.AddDigitalOrderField("vpc_SHIP_Street01", "");
            conn.AddDigitalOrderField("vpc_SHIP_Provice", "");
            conn.AddDigitalOrderField("vpc_SHIP_City", "");
            conn.AddDigitalOrderField("vpc_SHIP_Country", "");
            conn.AddDigitalOrderField("vpc_Customer_Phone", "");
            conn.AddDigitalOrderField("vpc_Customer_Email", "");
            conn.AddDigitalOrderField("vpc_Customer_Id", "");
            conn.AddDigitalOrderField("vpc_TicketNo", ip);

            string url = conn.Create3PartyQueryString();
            return url;
        }
        public ActionResult OnepayResponse()
        {
            string hashvalidateResult = "";

            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest(OnePayProperty.Url_ONEPAY_TEST);
            conn.SetSecureSecret(OnePayProperty.HASH_CODE);

            // Xu ly tham so tra ve va du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(Request.QueryString);

            // Lay tham so tra ve tu cong thanh toan
            string vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode");
            string amount = conn.GetResultField("vpc_Amount");
            string localed = conn.GetResultField("vpc_Locale");
            string command = conn.GetResultField("vpc_Command");
            string version = conn.GetResultField("vpc_Version");
            string cardType = conn.GetResultField("vpc_Card");
            string orderInfo = conn.GetResultField("vpc_OrderInfo");
            string merchantID = conn.GetResultField("vpc_Merchant");
            string authorizeID = conn.GetResultField("vpc_AuthorizeId");
            string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef");
            string transactionNo = conn.GetResultField("vpc_TransactionNo");
            string acqResponseCode = conn.GetResultField("vpc_AcqResponseCode");
            string txnResponseCode = vpc_TxnResponseCode;
            string message = conn.GetResultField("vpc_Message");

            // Kiem tra 2 tham so tra ve quan trong nhat
            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {
                return View("PaySuccess");
            }
            else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
            {
                return View("PayPending");
            }
            else
            {
                return View("PayUnSuccess");
            }
        }
    }
}