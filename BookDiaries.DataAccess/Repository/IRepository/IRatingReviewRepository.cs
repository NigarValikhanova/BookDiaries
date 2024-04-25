using BookDiaries.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.DataAccess.Repository.IRepository
{
    public interface IRatingReviewRepository : IRepository<RatingReview>
    {
        void Update(RatingReview obj);
    }
}
