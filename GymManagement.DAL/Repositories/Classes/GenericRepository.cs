using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _Set;
        public GenericRepository(GymDbContext dbContext)
        {
            _dbContext = dbContext;
            _Set = _dbContext.Set<TEntity>();
        }
        public void Add(TEntity entity)
        {
          _Set.Add(entity);
            
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return _Set.AsNoTracking().AnyAsync(predicate, ct);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null, CancellationToken ct = default)
        {
            if (condition == null)
            {
                return await _Set.AsNoTracking().CountAsync( ct);
            }
            else
            {
                return await _Set.AsNoTracking().CountAsync(condition,ct);
            }
        }

        public void Delete(TEntity entity)
        {
            _Set.Remove(entity);
           
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _Set : _Set.AsNoTracking();
            return await query.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _Set : _Set.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        {
                   return await _Set.FindAsync(id, ct);
        }

        public void Update(TEntity entity)
        {
           _Set.Update(entity);           
        }
    }
}
