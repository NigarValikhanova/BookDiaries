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
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private readonly AppDbContext _db;
        public CouponRepository(AppDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(Coupon obj)
        {
            _db.Coupons.Update(obj);
        }
    }
}
