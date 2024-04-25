using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.Models
{
    public class UserCoupon
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int CouponId { get; set; }
        public Coupon Coupon { get; set; }
        public bool IsUsed { get; set; }
    }
    
}
