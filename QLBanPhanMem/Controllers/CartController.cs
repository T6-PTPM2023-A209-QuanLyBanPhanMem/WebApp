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
            ViewBag.uid = HttpContext.Session.GetString("uid");

            string? maTK = HttpContext.Session.GetString("uid");
           
            var hoadon = _context.HoaDons
                .FirstOrDefault(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
            if(hoadon==null)
            {
                return View();
            }
            else
            {
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
                    
        }
        public async Task<IActionResult> AddToCart(int productID)
        {
            try
            {
                string? maTK = HttpContext.Session.GetString("uid");
                string? maHD = HttpContext.Session.GetString("uid") + DateTime.Now.ToString("ddMMyyyyHHmmss");
                var hoadon = await _context.HoaDons
                    .FirstOrDefaultAsync(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");

                if (hoadon == null)
                {
                    hoadon = new HoaDonModel
                    {
                        MAHD = maHD,
                        MATK = maTK,
                        THOIGIANLAP = DateTime.Now,
                        TONGTIEN = 0,
                        TINHTRANG = "Chưa thanh toán"
                    };
                    _context.HoaDons.Add(hoadon);
                    await _context.SaveChangesAsync();
                    
                    var cthd = new ChiTietHoaDonModel();
                    cthd = new ChiTietHoaDonModel
                    {
                        MAHD = hoadon.MAHD,
                        MAPM = productID,
                        SOLUONG = 1,
                        THANHTIEN = (await _context.PhanMems.FirstOrDefaultAsync(pm => pm.MAPM == productID)).DONGIA
                    };
                    _context.CTHDs.Add(cthd);
                    await _context.SaveChangesAsync();
                    int? soluong = cthd.SOLUONG;
                    int? tongtien = (int)(await _context.CTHDs.Where(ct => ct.MAHD == hoadon.MAHD).SumAsync(ct => ct.THANHTIEN)).Value * soluong;
                    hoadon.TONGTIEN = tongtien;
                    _context.Update(hoadon);
                    await _context.SaveChangesAsync();
                }
                else if (hoadon != null)
                {
                    var cthd = await _context.CTHDs.FirstOrDefaultAsync(ct => ct.MAHD == hoadon.MAHD && ct.MAPM == productID);
                    if (cthd != null)
                    {
                        cthd.SOLUONG = cthd.SOLUONG + 1;
                        _context.Update(cthd);
                        await _context.SaveChangesAsync();
                    }
                    else if (cthd == null)
                    {
                        cthd = new ChiTietHoaDonModel
                        {
                            MAHD = hoadon.MAHD,
                            MAPM = productID,
                            SOLUONG = 1,
                            THANHTIEN = (await _context.PhanMems.FirstOrDefaultAsync(pm => pm.MAPM == productID)).DONGIA
                        };
                        _context.CTHDs.Add(cthd);
                        await _context.SaveChangesAsync();
                    }
                    int? soluong = cthd.SOLUONG;
                    int? tongtien = (int)(await _context.CTHDs.Where(ct => ct.MAHD == hoadon.MAHD).SumAsync(ct => ct.THANHTIEN)).Value * soluong;
                    hoadon.TONGTIEN = tongtien;
                    _context.Update(hoadon);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây và tạo đối tượng ProblemDetails
                var problemDetails = new ProblemDetails
                {
                    Title = "Lỗi xử lý yêu cầu",
                    Status = 500, // Hoặc một mã trạng thái HTTP phù hợp khác
                    Detail = ex.Message
                };

                return StatusCode(problemDetails.Status.Value, problemDetails);
            }
        }

        private async void ThemHoaDon(HoaDonModel hoadon, string maHD, string maTK)
        {
            hoadon = new HoaDonModel
            {
                MAHD = maHD,
                MATK = maTK,
                THOIGIANLAP = DateTime.Now,
                TONGTIEN = 0,
                TINHTRANG = "Chưa thanh toán"
            };
            _context.HoaDons.Add(hoadon);
            await _context.SaveChangesAsync();
        }
        private async void ThemCTHD(HoaDonModel hoadon,ChiTietHoaDonModel cthd,int productID)
        {
            cthd = new ChiTietHoaDonModel
            {
                MAHD = hoadon.MAHD,
                MAPM = productID,
                SOLUONG = 1,
                THANHTIEN = (await _context.PhanMems.FirstOrDefaultAsync(pm => pm.MAPM == productID)).DONGIA
            };
            _context.CTHDs.Add(cthd);
            await _context.SaveChangesAsync();
        }
        private async void CapNhatTongTienHD(HoaDonModel hoadon, ChiTietHoaDonModel cthd)
        {
            
            int? soluong = await _context.CTHDs.Where(ct => ct.MAHD == hoadon.MAHD).SumAsync(ct => ct.SOLUONG);
            int? tongtien = (int)(await _context.CTHDs.Where(ct => ct.MAHD == hoadon.MAHD).SumAsync(ct => ct.THANHTIEN)).Value * soluong;
            hoadon.TONGTIEN = tongtien;
            _context.Update(hoadon);
            await _context.SaveChangesAsync();
        }
        private async void addCTHD(int productID, string maHD)
        {
            
            
            //int soluong = (int)cthd.SOLUONG;
            //int tongtien = (int)_context.CTHDs.Where(ct => ct.MAHD == maHD).Sum(ct => ct.THANHTIEN) * soluong;
            //var hoadon = _context.HoaDons.FirstOrDefaultAsync(hd => hd.MAHD == maHD).Result;
            //hoadon.TONGTIEN = tongtien;
            //try
            //{
            //    _context.Update(hoadon);
            //    await _context.SaveChangesAsync();
            //}
            //catch(Exception e)
            //{
            //    Problem("Lỗi cập nhật tổng tiền");
            //}
        }
        //public async Task<IActionResult> AddToCart(int productId)
        //{
        //    string? maTK = HttpContext.Session.GetString("uid");
        //    string? maHD = HttpContext.Session.GetString("uid") + DateTime.Now.ToString("ddMMyyyyHHmmss");
        //    var hoadon = _context.HoaDons
        //    .FirstOrDefault(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
        //    if (hoadon==null)
        //    {
        //        var order = new HoaDonModel
        //        {
        //            MAHD = maTK + DateTime.Now.ToString("ddMMyyyyHHmmss"), // Mã hóa đơn là mã khách hàng + thời gian lập
        //            MATK = maTK,
        //            THOIGIANLAP = DateTime.Now,
        //            TONGTIEN = 0, // Ban đầu đặt là 0
        //            TINHTRANG = "Chưa thanh toán"
        //        };
        //        _context.HoaDons.Add(order);
        //        _context.SaveChanges();

        //        var detail = new ChiTietHoaDonModel
        //        {
        //            MAHD = hoadon.MAHD,
        //            MAPM = productId,
        //            SOLUONG = 1,
        //            THANHTIEN = _context.PhanMems
        //                            .FirstOrDefault(pm => pm.MAPM == productId).DONGIA
        //        };
        //        _context.CTHDs.Add(detail);
        //        _context.SaveChanges();
        //        detail = _context.CTHDs.FirstOrDefaultAsync(ct => ct.MAHD == hoadon.MAHD && ct.MAPM == productId).Result;
        //        int soluong = (int)detail.SOLUONG;
        //        int tongtien = (int)_context.CTHDs.Where(ct => ct.MAHD == hoadon.MAHD).Sum(ct => ct.THANHTIEN);
        //        hoadon.TONGTIEN = tongtien;
        //        _context.Update(hoadon);
        //        _context.SaveChanges();
        //    } 
        //    else
        //    {
        //        hoadon = _context.HoaDons.FirstOrDefaultAsync(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán").Result;
        //        string mahd = hoadon.MAHD;
        //        var cthd = _context.CTHDs.FirstOrDefaultAsync(ct => ct.MAHD == mahd && ct.MAPM == productId).Result;
        //        if (cthd != null)
        //        {
        //            cthd.SOLUONG = cthd.SOLUONG + 1;
        //        }
        //        else if (cthd == null)
        //        {
        //            var detail = new ChiTietHoaDonModel
        //            {
        //                MAHD = hoadon.MAHD,
        //                MAPM = productId,
        //                THANHTIEN = _context.PhanMems
        //                            .FirstOrDefault(pm => pm.MAPM == productId).DONGIA
        //            };
        //            _context.CTHDs.Add(detail);
        //        }
        //        int soluong = (int)cthd.SOLUONG;
        //        _context.SaveChanges();
        //        int tongtien = (int)_context.CTHDs.Where(ct => ct.MAHD == hoadon.MAHD).Sum(ct => ct.THANHTIEN)*soluong;
        //        hoadon.TONGTIEN = tongtien;
        //        _context.Update(hoadon);
        //        _context.SaveChanges();
        //    }
        //    int dem = 0;
        //    if (_context.CTHDs != null)
        //    {               
        //        dem = await _context.CTHDs
        //       .Where(p => string.IsNullOrEmpty(maHD) || p.MAHD == maHD)
        //       .CountAsync();               
        //        HttpContext.Session.SetString("dem", dem.ToString());
        //    }
        //    return RedirectToAction("Index", "Cart");
        //}
        public IActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProcessPayment()
        {
            string maTK = HttpContext.Session.GetString("uid");
            var hoadon = _context.HoaDons
                .FirstOrDefault(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
            
            if (hoadon == null) { 
            
            }
            if(hoadon.TINHTRANG=="Chưa thanh toán")
            {
                hoadon.TINHTRANG = "Đã thanh toán";
                _context.Update(hoadon);
                await _context.SaveChangesAsync();
            }
            var account = _context.Accounts
                .FirstOrDefault(tk => tk.Uid == maTK);
            if(account.SurPlus<hoadon.TONGTIEN)
            {
                ViewBag.error = "Số dư không đủ để thanh toán";
                return RedirectToAction("Index", "Cart");
            }
            else
            {
                account.SurPlus = account.SurPlus - hoadon.TONGTIEN;
                _context.Update(account);
                await _context.SaveChangesAsync();
            }    
            

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            string maTK = HttpContext.Session.GetString("uid");
            var hoadon = _context.HoaDons
                .FirstOrDefault(hd => hd.MATK == maTK && hd.TINHTRANG == "Chưa thanh toán");
            if (hoadon == null)
            {
                return Problem("Null");
            }
            else
            {
                string maHD = hoadon.MAHD;
                var ct = _context.CTHDs
                    .FirstOrDefault(ct => ct.MAHD == maHD && ct.MAPM == id);

                if (ct != null)
                {
                    _context.CTHDs.Remove(ct);
                    await _context.SaveChangesAsync();
                }
                //Cập nhật lại giá tiền
                int tongtien = (int)_context.CTHDs.Where(ct => ct.MAHD == hoadon.MAHD).Sum(ct => ct.THANHTIEN);
                hoadon.TONGTIEN = tongtien;
                _context.Update(hoadon);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Cart");
            }
        }
        public IActionResult TopUp()
        {
            return View();
        }
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest();
        //    }

        //    var cthd = await _context.CTHDs.FindAsync(id);
        //    if (cthd == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.CTHDs.Remove(cthd);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}










    }
}
