using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretBusisness.Abstract
{
    public interface IGenericREpository<Tentity>
        where Tentity:class,new()
    {
        List<Tentity> GetAll(Expression<Func<Tentity, bool>> filter=null);
        Tentity Get(int id);
        Tentity Get(Expression<Func<Tentity, bool>> filter);
        void Add(Tentity tentity);
        void Update(Tentity tentity);
        void Delete(Tentity tentity);
        void Delete(int id);//Get aynı işi yapar....

    }
}
