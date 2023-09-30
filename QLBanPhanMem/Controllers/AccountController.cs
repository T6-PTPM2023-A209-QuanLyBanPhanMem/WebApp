using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLBanPhanMem.Models;
using Firebase.Auth.Providers;
using Firebase.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using QLBanPhanMem.Class;

namespace QLBanPhanMem.Controllers
{
    public class AccountController : Controller
    {
        private string realbaseurl = "https://chatapp-c35ec-default-rtdb.asia-southeast1.firebasedatabase.app/";
        private static readonly FirebaseAuthConfig config = new FirebaseAuthConfig()
        {
            ApiKey = "AIzaSyAwOrLG01nBCgfLrXje1eKhHoqmb-x33Yg",
            AuthDomain = "chatapp-c35ec.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
                {
                   new EmailProvider()
                }
        };
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        //GET: Account
        public async Task<IActionResult> Index()
        {
            return _context.Accounts != null ?
                        View(await _context.Accounts.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Accounts'  is null.");
        }

        //// GET: Account/Details/5
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null || _context.Accounts == null)
        //    {
        //        return NotFound();
        //    }

        //    var accountModel = await _context.Accounts
        //        .FirstOrDefaultAsync(m => m.Uid == id);
        //    if (accountModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(accountModel);
        //}

        //// GET: Account/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Account/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Password,Uid,FullName,Email")] AccountModel accountModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(accountModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(accountModel);
        //}

        // GET: Account/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            //Lấy 
            string? session = HttpContext.Session.GetString("email");
            @ViewBag.email = session;
            if (HttpContext.Session.GetString("uid") == null || id == null || _context.Accounts == null)
            {
                return RedirectToAction("SignIn","Account");
            }
            if (id != HttpContext.Session.GetString("uid"))
            {
                return NotFound();
            }
            // Xuất nội dung trong Account
            var accountModel = await _context.Accounts.FindAsync(id);
            if (accountModel == null)
            {
                return NotFound();
            }
            return View(accountModel);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Username,Uid,FullName,Email,CCCD,PhoneNumber,Address,SurPlus,Avatar")] AccountModel accountModel)
        {
            if (id != accountModel.Uid)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountModelExists(accountModel.Uid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewBag.notice = "Cập nhật thông tin thành công";
                return RedirectToAction(nameof(Edit));
            }
            return View(accountModel);
        }

        //// GET: Account/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null || _context.Accounts == null)
        //    {
        //        return NotFound();
        //    }

        //    var accountModel = await _context.Accounts
        //        .FirstOrDefaultAsync(m => m.Uid == id);
        //    if (accountModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(accountModel);
        //}

        //// POST: Account/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    if (_context.Accounts == null)
        //    {
        //        return Problem("Entity set 'AppDbContext.Accounts'  is null.");
        //    }
        //    var accountModel = await _context.Accounts.FindAsync(id);
        //    if (accountModel != null)
        //    {
        //        _context.Accounts.Remove(accountModel);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool AccountModelExists(string id)
        {
          return (_context.Accounts?.Any(e => e.Uid == id)).GetValueOrDefault();
        }
        public IActionResult SignIn()
        {
            if(HttpContext.Session.GetString("uid")!=null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(AccountModel model, string password)
        {
            var client = new FirebaseAuthClient(config);
            try
            {
                var result = await client.SignInWithEmailAndPasswordAsync(model.Email, password);
                if (result != null)
                {
                    if(result.User.Uid!=null&&model.Email!=null)
                    {
                        HttpContext.Session.Set("uid", System.Text.Encoding.UTF8.GetBytes(result.User.Uid));
                        HttpContext.Session.Set("email", System.Text.Encoding.UTF8.GetBytes(model.Email));
                        return RedirectToAction("Index", "Home");
                    }
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch(Exception ex)
            {                
                @ViewBag.Error = ex.Message;              
                return View();
            }                       
        }
        public IActionResult SignUp()
        {
            if (HttpContext.Session.GetString("uid") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(AccountModel model,string password)
        {
            var client = new FirebaseAuthClient(config);
            try
            {
                var result = await client.CreateUserWithEmailAndPasswordAsync(model.Email, password);
                var auth = await client.SignInWithEmailAndPasswordAsync(model.Email, password);
                if (result != null)
                {                   
                    var user = new AccountModel()
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        Uid = result.User.Uid,
                        Username = model.Email,
                        SurPlus = 0
                    };
                 try
                    {
                        _context.Accounts.Add(user);

                        await _context.SaveChangesAsync();
                    }
                    catch(Exception ex)
                    {
                        ViewBag.Error = ex.Message;
                        return View("SignUp");
                    }
                    // Thực hiện insert chỉ vào các cột Email, Uid và FullName
                   
                    if (result.User.Uid != null && model.Email != null)
                    {
                        HttpContext.Session.Set("uid", System.Text.Encoding.UTF8.GetBytes(result.User.Uid));
                        HttpContext.Session.Set("email", System.Text.Encoding.UTF8.GetBytes(model.Email));
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View("SignIn");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine(ex.ToString());
                return View("SignIn");
            }
        }
        public IActionResult SignOut()
        {
            
                var client = new FirebaseAuthClient(config);
                client.SignOut();
                HttpContext.Session.Clear();
                return RedirectToAction("SignIn");
            
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            // Chuyển hướng đến trang đăng nhập Facebook
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }
        public IActionResult TopUp()
        {            
            return View();
        }
        [HttpPost]
        public IActionResult TopUp(int soTien)
        {
            string? session = HttpContext.Session.GetString("uid");
            if (session == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var account = _context.Accounts.FirstOrDefault(a => a.Uid == session);
            if (account == null)
            {
                return NotFound();
            }
            account.SurPlus += soTien;
            try
            {
                _context.Update(account);
                _context.SaveChanges();
                ViewBag.notice = "Nạp tiền thành công";
                return View(ViewBag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ViewBag.notice = "Nạp tiền thất bại";
                return View(ViewBag);
            }


        }


    }
}
