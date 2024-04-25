﻿using BookDiaries.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.DataAccess.Repository.IRepository
{
    public interface IUserCouponRepository : IRepository<UserCoupon>
    {
        void Update(UserCoupon obj);
    }
}
