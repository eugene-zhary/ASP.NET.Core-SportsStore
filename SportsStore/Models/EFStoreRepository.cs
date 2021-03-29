using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class EFStoreRepository : IStoreRepository
    {
        private StoreDbContext context;

        public IQueryable<Product> Products => context.Products;

        public EFStoreRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public void SaveProduct(Product prod)
        {
            context.SaveChanges();
        }

        public void CreateProduct(Product prod)
        {
            context.Add(prod);
            context.SaveChanges();
        }

        public void DeleteProduct(Product prod)
        {
            context.Remove(prod);
            context.SaveChanges();
        }
    }
}
