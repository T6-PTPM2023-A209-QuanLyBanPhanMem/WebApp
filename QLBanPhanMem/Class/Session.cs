using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
namespace QLBanPhanMem.Class
{
    public class Session
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        

        public Session(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        

        public string GetSession(string key)
        {
            return _httpContextAccessor.HttpContext.Session.GetString(key);
            
        }
        public void SetSession(string uid, string email)
        {
            
            _httpContextAccessor.HttpContext.Session.Set("uid", System.Text.Encoding.UTF8.GetBytes(uid));
            _httpContextAccessor.HttpContext.Session.Set("email", System.Text.Encoding.UTF8.GetBytes(email));
        }
        public void RemoveSession()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            
        }
    }
}
