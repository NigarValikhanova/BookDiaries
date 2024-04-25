using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ILanguageRepository Language { get; }
        IAuthorRepository Author {  get; }
        IShoppingCartRepository ShoppingCart {  get; }
        IWishListRepository WishList {  get; }
        IAppUserRepository AppUser { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }
        ICouponRepository Coupon { get; }
        IUserCouponRepository UserCoupon { get; }
        ISliderRepository Slider { get; }
        IContactUserRepository ContactUser { get; }
        IProductImageRepository ProductImage { get; }
        IBlogRepository Blog { get; }
        IRatingReviewRepository RatingReview { get; }


        void Save();
    }
}
