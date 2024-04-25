using BookDiaries.DataAccess.Data;
using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.DataAccess.Repository
{
    public class UserCouponRepository : Repository<UserCoupon>, IUserCouponRepository
    {
        private readonly AppDbContext _db;
        public UserCouponRepository(AppDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(UserCoupon obj)
        {
            _db.UserCoupon.Update(obj);
        }
    }
}
