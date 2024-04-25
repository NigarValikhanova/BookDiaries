using BookDiaries.DataAccess.Data;
using BookDiaries.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ILanguageRepository Language { get; private set; }
        public IAuthorRepository Author { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IWishListRepository WishList { get; private set; }
        public IAppUserRepository AppUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public ICouponRepository Coupon { get; private set; }
        public IUserCouponRepository UserCoupon { get; private set; }
        public ISliderRepository Slider { get; private set; }
        public IContactUserRepository ContactUser { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public IBlogRepository Blog { get; private set; }
        public IRatingReviewRepository RatingReview { get; private set; }



        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Language = new LanguageRepository(_db);
            Author = new AuthorRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            WishList = new WishListRepository(_db);
            AppUser = new AppUserRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            Coupon = new CouponRepository(_db);
            UserCoupon = new UserCouponRepository(_db);
            Slider = new SliderRepository(_db);
            ContactUser = new ContactUserRepository(_db);
            ProductImage = new ProductImageRepository(_db);
            Blog = new BlogRepository(_db);
            RatingReview = new RatingReviewRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
