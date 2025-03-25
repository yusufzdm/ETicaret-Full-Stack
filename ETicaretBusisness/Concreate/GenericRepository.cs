using ETicaretBusisness.Abstract;
using ETicaretData.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretBusisness.Concreate
{
    public class GenericRepository<Tentity, Tcontext> : IGenericREpository<Tentity>
        where Tentity : class, new()
        where Tcontext : IdentityDbContext<AppUser, AppRole, int>, new()
    {
        public void Add(Tentity tentity)
        {
            using (var db = new Tcontext())
            {
                db.Set<Tentity>().Add(tentity);
                db.SaveChanges();
            }
        }

        public void Delete(Tentity tentity)
        {
            using (var db = new Tcontext())
            {
                db.Entry(tentity).State = EntityState.Deleted;
                db.SaveChanges();
                //db.Set<Tentity>().Remove(tentity);
                //db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var db = new Tcontext())
            {
                var nesne = db.Set<Tentity>().Find(id);
                db.Set<Tentity>().Remove(nesne);
                db.SaveChanges();
            }
        }

        public Tentity Get(int id)
        {
            using (var db = new Tcontext())
            {
                var nesne = db.Set<Tentity>().Find(id);
                return nesne;
            }
        }

        public Tentity Get(Expression<Func<Tentity, bool>> filter)
        {
            using (var db = new Tcontext())
            {
                var nesne = db.Set<Tentity>().Find(filter);
                return nesne;
            }
        }

        public List<Tentity> GetAll(Expression<Func<Tentity, bool>> filter = null)
        {
            using (var db = new Tcontext())
            {
                return filter == null ? db.Set<Tentity>().ToList() : db.Set<Tentity>().Where(filter).ToList();
            }
        }

        public void Update(Tentity tentity)
        {
            using (var db = new Tcontext())
            {
                db.Entry(tentity).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
    }
}
