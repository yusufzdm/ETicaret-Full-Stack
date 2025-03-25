using ETicaretBusisness.Abstract;
using ETicaretData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretDal.Abstract
{
    public interface IProductDal : IGenericREpository<Product>
    {
        Product GetById(int id);
        List<Product> GetAllWithCategory();
    }
}
