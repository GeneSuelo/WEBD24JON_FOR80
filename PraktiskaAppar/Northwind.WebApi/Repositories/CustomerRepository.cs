using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using Northwind.EntityModels;

namespace Northwind.WebApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(30),
        };

        private NorthwindDatabaseContext _db;
        public CustomerRepository(NorthwindDatabaseContext db,
            IMemoryCache memoryCache)
        {
            _db = db;
            _memoryCache = memoryCache;
        }

        public async Task<Customer?> CreateAsync(Customer c)
        {
            c.CustomerId = c.CustomerId.ToUpper(); //Normalisera till versaler
            //Lägga till det till databasen
            EntityEntry<Customer> added = await _db.Customers.AddAsync(c);
            int affected = await _db.SaveChangesAsync();
            if (affected == 1)
            {
                //Lägga till det i cachen
                _memoryCache.Set(c.CustomerId, c, _cacheEntryOptions);
                return c;
            }
            
            return null;
        }

        public async Task<bool?> DeleteAsync(string id)
        {
            id = id.ToUpper(); //Normalisera till versaler

            //Försöka hämta Customer från databasen
            Customer? c = await _db.Customers.FindAsync(id);
            if(c is null) return null;

            //Ta bort från databasen
            _db.Customers.Remove(c);
            int affected = await _db.SaveChangesAsync();
            //Om det är borttaget från databasen ta bort från cachen
            if (affected == 1)
            {
                //Ta bort från cachen
                _memoryCache.Remove(id);
                return true;
            }
            return null;
        }

        public Task<Customer[]> RetrieveAllAsync()
        {
            return _db.Customers.ToArrayAsync();
        }

        public Task<Customer?> RetrieveAsync(string id)
        {
            id = id.ToUpper(); //Normalisera till versaler

            // Försöka hämta från cachen först
            if (_memoryCache.TryGetValue(id, out Customer? fromCache))
                return Task.FromResult(fromCache);

            // Om inte finns i cachen, hämta från databasen
            Customer? fromDb = _db.Customers.FirstOrDefault(c => c.CustomerId == id);

            // Om inte finns i databasen, returnera null
            if(fromDb is null)
                return Task.FromResult(fromDb);

            // Om finns i databasen, lägg till i cachen
            _memoryCache.Set(fromDb.CustomerId, fromDb, _cacheEntryOptions);
            return Task.FromResult(fromDb)!;
        }


        public async Task<Customer?> UpdateAsync(Customer c)
        {
            c.CustomerId = c.CustomerId.ToUpper(); //Normalisera till versaler
          
            _db.Customers.Update(c);
            int affected = await _db.SaveChangesAsync();
            if (affected == 1)
            {
                //Lägga till det i cachen
                _memoryCache.Set(c.CustomerId, c, _cacheEntryOptions);
                return c;
            }
            return null;
        }
    }
}
