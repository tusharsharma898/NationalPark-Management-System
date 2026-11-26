using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project_APIs.Data;
using Project_APIs.Models;
using Project_APIs.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project_APIs.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSetting _appSetting;
        public UserRepository(ApplicationDbContext context,IOptions<AppSetting> appSetting)
        {
            _context = context;
            _appSetting = appSetting.Value;
        }
        public User Authenticate(string username, string password)
        {
            var userInDb = _context.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
            if(userInDb == null) return null;
            //JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSetting.Secret);
            var tokenDiscriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userInDb.Id.ToString()),
                    new Claim(ClaimTypes.Role, userInDb.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDiscriptor);
            userInDb.Token = tokenHandler.WriteToken(token);
            //
            userInDb.Password = "";
            return userInDb;
        }

        public bool IsUniqueUser(string username)
        {
            var userInDb = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (userInDb == null)
                return true; return false;
        }

        public User Register(string username, string password)
        {
            User user = new User()
            {
                UserName = username,
                Password = password,
                Role = "Admin"
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;

        }
    }
}
