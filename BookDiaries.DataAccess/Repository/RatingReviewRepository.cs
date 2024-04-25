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
    public class RatingReviewRepository : Repository<RatingReview>, IRatingReviewRepository
    {
        private readonly AppDbContext _db;
        public RatingReviewRepository(AppDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(RatingReview obj)
        {
            _db.RatingReviews.Update(obj);
        }
    }
}
