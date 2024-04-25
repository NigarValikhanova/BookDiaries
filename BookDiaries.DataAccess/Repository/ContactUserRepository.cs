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
    public class ContactUserRepository : Repository<ContactUser>, IContactUserRepository
    {
        private readonly AppDbContext _db;
        public ContactUserRepository(AppDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(ContactUser obj)
        {
            _db.ContactUsers.Update(obj);
        }
    }
}
