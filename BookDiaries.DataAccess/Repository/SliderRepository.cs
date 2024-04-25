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
    public class SliderRepository : Repository<Slider>, ISliderRepository
    {
        private readonly AppDbContext _db;
        public SliderRepository(AppDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(Slider obj)
        {
            var objFromDb = _db.Sliders.FirstOrDefault(u => u.Id == obj.Id);
            if(objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.StartingPrice = obj.StartingPrice;
                objFromDb.DiscountPercent = obj.DiscountPercent;
                objFromDb.IsUsed = obj.IsUsed;               
                if(obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
           
        }
    }
}
