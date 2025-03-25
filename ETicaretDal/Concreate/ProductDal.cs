using ETicaretBusisness.Concreate;
using ETicaretDal.Abstract;
using ETicaretData.Context;
using ETicaretData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ETicaretDal.Concreate
{
    public class ProductDal : GenericRepository<Product, ETicaretContext>, IProductDal
    {
        public Product GetById(int id)
        {
            using (var context = new ETicaretContext())
            {
                return context.Products
                    .Include(p => p.Category)
                    .FirstOrDefault(p => p.Id == id);
            }
        }

        public List<Product> GetAllWithCategory()
        {
            using (var context = new ETicaretContext())
            {
                return context.Products
                    .Include(p => p.Category)
                    .ToList();
            }
        }
    }
}
