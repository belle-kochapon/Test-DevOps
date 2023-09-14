using Prom.LPR.Api.Models;

namespace Prom.LPR.Api.Database.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(DataContext ctx)
        {
            context = ctx;
        }

        public MUser AddUser(MUser user)
        {
            try
            {
                context!.Users!.AddAsync(user);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return user;
        }

        public IEnumerable<MUser> GetUsers()
        {
            try
            {
                var arr = context!.Users!.ToList();
                return arr;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsEmailExist(string email)
        {
            try
            {
                var cnt = context!.Users!.Where(p => p!.UserEmail!.Equals(email)).Count();
                return cnt >= 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsUserNameExist(string userName)
        {
            try
            {
                var cnt = context!.Users!.Where(p => p!.UserName!.Equals(userName)).Count();
                return cnt >= 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}