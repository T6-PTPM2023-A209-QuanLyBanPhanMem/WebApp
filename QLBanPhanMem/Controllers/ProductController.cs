using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLBanPhanMem.Models;
using Microsoft.EntityFrameworkCore.Query;
namespace QLBanPhanMem.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index(string search = "")
        {
            ViewBag.giohang = HttpContext.Session.GetString("dem");
            ViewBag.email = HttpContext.Session.GetString("email");
            // Bắt đầu với truy vấn không có điều kiện tìm kiếm
            IQueryable<PhanMemModel> query = _context.PhanMems.Include(p => p.NhaPhatHanh);

            // Nếu có từ khóa tìm kiếm, áp dụng điều kiện tìm kiếm vào truy vấn
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.TENPM.Contains(search) || // Tìm theo tên phần mềm
                    p.MOTA.Contains(search) || // Tìm theo mô tả
                    p.NhaPhatHanh.TENNPH.Contains(search) // Tìm theo tên nhà phát hành
                );
            }

            // Chuyển kết quả của truy vấn thành danh sách và truyền vào view
            var result = await query.ToListAsync();
            return View(result);
        }



        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PhanMems == null)
            {
                return NotFound();
            }

            var phanMemModel = await _context.PhanMems
                .Include(p => p.NhaPhatHanh)
                .FirstOrDefaultAsync(m => m.MAPM == id);
            if (phanMemModel == null)
            {
                return NotFound();
            }
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.uid = HttpContext.Session.GetString("uid");
            ViewBag.giohang = HttpContext.Session.GetString("dem");
            return View(phanMemModel);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["MANPH"] = new SelectList(_context.NhaPhatHanhs, "MANPH", "TENNPH");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MAPM,TENPM,MOTA,MANPH,NGAYPHATHANH,THOIHAN,DONVITHOIHAN,DONGIA,SOLUONG,HINHANH")] PhanMemModel phanMemModel)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(phanMemModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["MANPH"] = new SelectList(_context.NhaPhatHanhs, "MANPH", "TENNPH", phanMemModel.MANPH);
                return View(phanMemModel);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PhanMems == null)
            {
                return NotFound();
            }

            var phanMemModel = await _context.PhanMems.FindAsync(id);
            if (phanMemModel == null)
            {
                return NotFound();
            }
            ViewData["MANPH"] = new SelectList(_context.NhaPhatHanhs, "MANPH", "TENNPH", phanMemModel.MANPH);
            return View(phanMemModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("MAPM,TENPM,MOTA,MANPH,NGAYPHATHANH,THOIHAN,DONVITHOIHAN,DONGIA,SOLUONG,HINHANH")] PhanMemModel phanMemModel)
        {
            if (id != phanMemModel.MAPM)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phanMemModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhanMemModelExists(phanMemModel.MAPM))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MANPH"] = new SelectList(_context.NhaPhatHanhs, "MANPH", "TENNPH", phanMemModel.MANPH);
            return View(phanMemModel);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PhanMems == null)
            {
                return NotFound();
            }

            var phanMemModel = await _context.PhanMems
                .Include(p => p.NhaPhatHanh)
                .FirstOrDefaultAsync(m => m.MAPM == id);
            if (phanMemModel == null)
            {
                return NotFound();
            }

            return View(phanMemModel);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.PhanMems == null)
            {
                return Problem("Entity set 'AppDbContext.PhanMems'  is null.");
            }
            var phanMemModel = await _context.PhanMems.FindAsync(id);
            if (phanMemModel != null)
            {
                _context.PhanMems.Remove(phanMemModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult AddToCart(int? id)
        {
            if (id == null || _context.PhanMems == null)
            {
                return NotFound();
            }

            var phanMemModel = _context.PhanMems
                .Include(p => p.NhaPhatHanh)
                .FirstOrDefault(m => m.MAPM == id);
            if (phanMemModel == null)
            {
                return NotFound();
            }
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.uid = HttpContext.Session.GetString("uid");
            return View(phanMemModel);
        }
        private bool PhanMemModelExists(int? id)
        {
          return (_context.PhanMems?.Any(e => e.MAPM == id)).GetValueOrDefault();
        }
    }
}
