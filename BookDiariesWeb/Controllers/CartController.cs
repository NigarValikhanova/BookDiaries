using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using BookDiaries.Models.ViewModels;
using BookDiaries.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Numerics;
using System.Security.Claims;

namespace BookDiariesWeb.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public IActionResult Index()
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    ShoppingCartVM = new()
        //    {
        //        ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId,
        //        includeProperties: "Product"),
        //        OrderHeader = new()
        //    };

        //    foreach (var cart in ShoppingCartVM.ShoppingCartList)
        //    {
        //        cart.Price = GetPriceBasedOnQuantity(cart);
        //        ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        //    }

        //    return View(ShoppingCartVM);
        //}

        public IActionResult Index(string couponCode)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId,
                    includeProperties: "Product"),
                ProductImageLists = _unitOfWork.ProductImage.GetAll(),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = _unitOfWork.Coupon.Get(c => c.Code == couponCode);
                if (coupon != null)
                {
                    // Check if the user has already used the coupon
                    var userCoupon = _unitOfWork.UserCoupon.Get(uc => uc.UserId == userId && uc.CouponId == coupon.Id);
                    if (userCoupon != null && userCoupon.IsUsed)
                    {
                        // If the user has already used the coupon, display a message indicating that
                        // the coupon has already been used
                        TempData["CouponMessage"] = "You have already used this coupon.";
                    }
                    else
                    {
                        // Apply discount from the coupon to the order total
                        ShoppingCartVM.OrderHeader.OrderTotal -= (ShoppingCartVM.OrderHeader.OrderTotal * coupon.DiscountPercent / 100);

                        // Update the UserCoupon table to mark the coupon as used
                        if (userCoupon == null)
                        {
                            userCoupon = new UserCoupon
                            {
                                UserId = userId,
                                CouponId = coupon.Id,
                                IsUsed = true
                            };
                            _unitOfWork.UserCoupon.Add(userCoupon);
                        }
                        else
                        {
                            userCoupon.IsUsed = true;
                            _unitOfWork.UserCoupon.Update(userCoupon);
                        }
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    // Handle invalid coupon code (e.g., display an error message)
                    TempData["CouponMessage"] = "Invalid coupon code.";
                }
            }

            return View(ShoppingCartVM);
        }


        public IActionResult CheckOut()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.AppUser = _unitOfWork.AppUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.AppUser.FirstName;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.AppUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.AppUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.AppUser.City;
            ShoppingCartVM.OrderHeader.Country = ShoppingCartVM.OrderHeader.AppUser.Country;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.AppUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }


        [HttpPost]
        [ActionName("CheckOut")]
        public IActionResult CheckOutPOST()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product");

            // Calculate total purchase amount
            var totalPurchaseAmount = CalculateTotalPurchaseAmount(ShoppingCartVM.ShoppingCartList);

            // Process purchase
            ProcessPurchase(totalPurchaseAmount, userId);

            // Create Stripe session and redirect to payment page
            var sessionUrl = CreateStripeSessionAndRedirect(totalPurchaseAmount, ShoppingCartVM.OrderHeader.Id);

            // Assign coupon if total purchase amount is $200 or more and the user hasn't been assigned the coupon yet
            AssignCouponIfQualified(userId, totalPurchaseAmount);

            // Redirect to Stripe session URL
            return Redirect(sessionUrl);
        }

        private void ProcessPurchase(double totalPurchaseAmount, string userId)
        {
            // Update order header
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.AppUserId = userId;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

                // Decrement stock quantity of the product
                var product = _unitOfWork.Product.Get(p => p.Id == cart.ProductId);
                product.StockQuantity -= cart.Count; // Decrement by the quantity purchased
                _unitOfWork.Product.Update(product);
            }

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
        }

        private string CreateStripeSessionAndRedirect(double totalPurchaseAmount, int orderId)
        {
            var domain = "https://localhost:7175/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"cart/OrderConfirmation?id={orderId}",
                CancelUrl = domain + "cart/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            return session.Url;
        }

        private void AssignCouponIfQualified(string userId, double totalPurchaseAmount)
        {
            var couponThresholds = new Dictionary<double, int>
            {
                { 100, 1 },
                { 50, 2 },
                { 300, 3 }
                //{ 400, 4 },
                //{ 200, 5 },
                //{ 150, 6 }
            };

            foreach (var threshold in couponThresholds.OrderByDescending(t => t.Key))
            {
                // Check if the total purchase amount meets or exceeds the threshold
                if (totalPurchaseAmount >= threshold.Key)
                {
                    // Check if the user has already been assigned the coupon
                    var userCoupon = _unitOfWork.UserCoupon.Get(uc => uc.UserId == userId && uc.CouponId == threshold.Value);

                    if (userCoupon == null)
                    {
                        // If the user has not been assigned the coupon, assign it
                        var newCoupon = new UserCoupon
                        {
                            UserId = userId,
                            CouponId = threshold.Value
                        };

                        _unitOfWork.UserCoupon.Add(newCoupon);
                        _unitOfWork.Save();

                        // Since we've found the highest threshold met, we can break the loop
                        break;
                    }
                }
            }
        }

        private double CalculateTotalPurchaseAmount(IEnumerable<ShoppingCart> shoppingCartItems)
        {
            double totalPurchaseAmount = 0;
            foreach (var cartItem in shoppingCartItems)
            {
                cartItem.Price = GetPriceBasedOnQuantity(cartItem);
                totalPurchaseAmount += (cartItem.Price * cartItem.Count);
            }
            return totalPurchaseAmount;
        }

        public IActionResult OrderConfirmation(int Id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == Id, includeProperties: "AppUser");
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentID(Id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(Id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.AppUserId == orderHeader.AppUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();


            return View(Id);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            if (cartFromDb.Count <= 1)
            {
                //remove that from cart
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.AppUserId == cartFromDb.AppUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
               .GetAll(u => u.AppUserId == cartFromDb.AppUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }

        //[HttpPost]
        //[ActionName("CheckOut")]
        //public IActionResult CheckOutPOST()
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId,
        //        includeProperties: "Product");

        //    ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
        //    ShoppingCartVM.OrderHeader.AppUserId = userId;

        //    AppUser appUser = _unitOfWork.AppUser.Get(u => u.Id == userId);

        //    foreach (var cart in ShoppingCartVM.ShoppingCartList)
        //    {
        //        cart.Price = GetPriceBasedOnQuantity(cart);
        //        ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

        //        // Decrement stock quantity of the product
        //        var product = _unitOfWork.Product.Get(p => p.Id == cart.ProductId);
        //        product.StockQuantity -= cart.Count; // Decrement by the quantity purchased
        //        _unitOfWork.Product.Update(product);
        //    }

        //    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        //    ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
        //    _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
        //    _unitOfWork.Save();

        //    foreach (var cart in ShoppingCartVM.ShoppingCartList)
        //    {
        //        OrderDetail orderDetail = new()
        //        {
        //            ProductId = cart.ProductId,
        //            OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
        //            Price = cart.Price,
        //            Count = cart.Count
        //        };
        //        _unitOfWork.OrderDetail.Add(orderDetail);
        //        _unitOfWork.Save();
        //    }

        //    if (appUser.Id != null)
        //    {
        //        var domain = "https://localhost:7175/";
        //        var options = new Stripe.Checkout.SessionCreateOptions
        //        {
        //            SuccessUrl = domain + $"cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
        //            CancelUrl = domain + "cart/index",
        //            LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
        //            Mode = "payment",
        //        };

        //        foreach (var item in ShoppingCartVM.ShoppingCartList)
        //        {
        //            var sessionLineItem = new SessionLineItemOptions
        //            {
        //                PriceData = new SessionLineItemPriceDataOptions
        //                {
        //                    UnitAmount = (long)(item.Price * 100),
        //                    Currency = "usd",
        //                    ProductData = new SessionLineItemPriceDataProductDataOptions
        //                    {
        //                        Name = item.Product.Title
        //                    }
        //                },
        //                Quantity = item.Count
        //            };
        //            options.LineItems.Add(sessionLineItem);
        //        }

        //        var service = new Stripe.Checkout.SessionService();
        //        Stripe.Checkout.Session session = service.Create(options);

        //        _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
        //        _unitOfWork.Save();
        //        Response.Headers.Add("Location", session.Url);
        //        double totalPurchaseAmount = 0;

        //        foreach (var cartItem in ShoppingCartVM.ShoppingCartList)
        //        {
        //            totalPurchaseAmount += (cartItem.Price * cartItem.Count);
        //        }

        //        // Check if the total purchase amount is $200 or more
        //        if (totalPurchaseAmount >= 200)
        //        {
        //            // Check if the user has already been assigned the coupon
        //            var userCoupon = _unitOfWork.UserCoupon.Get(uc => uc.UserId == userId && uc.CouponId == 3); // Assuming 1 is the ID of the coupon

        //            if (userCoupon == null)
        //            {
        //                // If the user has not been assigned the coupon, assign it
        //                var newCoupon = new UserCoupon
        //                {
        //                    UserId = userId,
        //                    CouponId = 3 // Assuming 1 is the ID of the coupon
        //                };

        //                _unitOfWork.UserCoupon.Add(newCoupon);
        //                _unitOfWork.Save();
        //            }
        //        }

        //        return new StatusCodeResult(303);
        //    }

        //    return RedirectToAction(nameof(OrderConfirmation), new { Id = ShoppingCartVM.OrderHeader.Id });
        //}
    }
}
