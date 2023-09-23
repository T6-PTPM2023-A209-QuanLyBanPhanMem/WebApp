﻿using System;
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
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Edit(string id, [Bind("Password,Uid,FullName,Email")] AccountModel accountModel)
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
                return RedirectToAction(nameof(Index));
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
                    HttpContext.Session.Set("uid", System.Text.Encoding.UTF8.GetBytes(result.User.Uid));
                    HttpContext.Session.Set("email", System.Text.Encoding.UTF8.GetBytes(model.Email));
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch(Exception ex)
            {
                
                @ViewBag.Error = "Email hoặc mật khẩu không đúng";
                return View();
            }
            
            
        }
        public IActionResult SignUp()
        {
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
                    

                    var user = new AccountModel();
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.Uid = result.User.Uid;
                    user.Username = model.Email;
                    // Thực hiện insert chỉ vào các cột Email, Uid và FullName
                    _context.Accounts.Add(user);

                    await _context.SaveChangesAsync();
                    HttpContext.Session.Set("uid", System.Text.Encoding.UTF8.GetBytes(result.User.Uid));
                    HttpContext.Session.Set("email", System.Text.Encoding.UTF8.GetBytes(model.Email));
                    return RedirectToAction("Index", "Home");
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
        
    }
}