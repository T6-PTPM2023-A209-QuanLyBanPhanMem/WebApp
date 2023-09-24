using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLBanPhanMem.Models;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Debugger.Contracts.HotReload;

namespace QLBanPhanMem.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public int? SOLUONG { get; private set; }

        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if(HttpContext.Session.GetString("uid") == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            ViewBag.giohang = HttpContext.Session.GetString("dem");
            ViewBag.email = HttpContext.Session.GetString("email");
            string maTK = HttpContext.Session.GetString("uid");
           
            var hoadon = _context.HoaDons
                .FirstOrDefault(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
            string maHD = hoadon.MAHD;
            ViewBag.tongtien = hoadon.TONGTIEN;
            int dem = 0;
            if (_context.CTHDs != null)
            {
                var cthd = await _context.CTHDs
                    .Include(p => p.PhanMem)
                    .Where(string.IsNullOrEmpty(maHD) ? p => p.MAHD == maHD : p => p.MAHD == maHD)
                    .ToListAsync();
                dem = await _context.CTHDs
               .Where(p => string.IsNullOrEmpty(maHD) || p.MAHD == maHD)
               .CountAsync();
                ViewBag.dem = dem;
                HttpContext.Session.SetString("dem", dem.ToString());
                
                return View(cthd);
            }
            
            return View();
           
        }
        public async Task<IActionResult> AddToCart(int productId)
        {
            string maTK = HttpContext.Session.GetString("uid");
            string maHD = HttpContext.Session.GetString("uid") + DateTime.Now.ToString("ddMMyyyyHHmmss");
            var hoadon = _context.HoaDons
            .FirstOrDefault(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
            if (hoadon==null)
            {
                var order = new HoaDonModel
                {
                    MAHD = maTK + DateTime.Now.ToString("ddMMyyyyHHmmss"), // Mã hóa đơn là mã khách hàng + thời gian lập
                    MATK = maTK,
                    THOIGIANLAP = DateTime.Now,
                    TONGTIEN = 0, // Ban đầu đặt là 0
                    TINHTRANG = "Chưa thanh toán"
                };
                _context.HoaDons.Add(order);
                _context.SaveChanges();

            } 
            else
            {
                var detail = new ChiTietHoaDonModel
                {
                    MAHD = hoadon.MAHD,
                    MAPM = productId,
                    SOLUONG = 1,
                    THANHTIEN = _context.PhanMems.FirstOrDefault(pm => pm.MAPM == productId).DONGIA
                };
                _context.CTHDs.Add(detail);
                _context.SaveChanges();
                int tongtien = (int)_context.CTHDs.Where(ct => ct.MAHD == hoadon.MAHD).Sum(ct => ct.THANHTIEN);
                hoadon.TONGTIEN = tongtien;
                _context.Update(hoadon);
                _context.SaveChanges();
            }
            int dem = 0;
            if (_context.CTHDs != null)
            {
                
                dem = await _context.CTHDs
               .Where(p => string.IsNullOrEmpty(maHD) || p.MAHD == maHD)
               .CountAsync();               
                HttpContext.Session.SetString("dem", dem.ToString());
            }
            return RedirectToAction("Index", "Home");
        }

           
        

    }
}
