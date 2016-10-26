using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OkCatStore.Models
{
    public class GioHang
    {
        DataClasses1DataContext data = new DataClasses1DataContext();

        public int iMaSP { get; set; }
        public string sTenSP { get; set; }
        public string sAnhDaiDien { get; set; }
        public double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        public GioHang(int maSP)
        {
            iMaSP = maSP;
            SANPHAM sp = data.SANPHAMs.Single(n => n.MASP == iMaSP);
            sTenSP = sp.TENSP;
            dDonGia = double.Parse(sp.GIABAN.ToString());
            iSoLuong = 1;
            HINHANH img = data.HINHANHs.First(n => n.MASP == iMaSP);
            sAnhDaiDien = img.URLHINH;
        }
    }
}