// MIT License
// 
// Copyright (c) 2020 Alexey Politov
// https://github.com/EmptyBucket/Repositories
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstractions.Repositories;

namespace Repositories.Repositories;

internal class Repository<T, TDbContext> : IRepository<T>
    where T : class, IExprEntity
    where TDbContext : DbContext
{
    private readonly DbSet<T> _dbSet;

    public Repository(TDbContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
    }

    public async Task<IReadOnlyCollection<T>> GetManyAsync(Expression<Func<T, bool>>? where = null,
        IEnumerable<Expression<Func<T, object>>>? orderBy = null, int? offset = null, int? count = null,
        CancellationToken cancellationToken = default)
    {
        return await BuildQueryable(where, orderBy, offset, count).ToArrayAsync(cancellationToken);
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>>? where = null,
        IEnumerable<Expression<Func<T, object>>>? orderBy = null, int? offset = null, int? count = null,
        CancellationToken cancellationToken = default)
    {
        return await BuildQueryable(where, orderBy, offset, count).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>>? where = null,
        IEnumerable<Expression<Func<T, object>>>? orderBy = null, int? offset = null, int? count = null,
        CancellationToken cancellationToken = default)
    {
        return await BuildQueryable(where, orderBy, offset, count).FirstAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddManyAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task UpdateManyAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteManyAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    private IQueryable<T> BuildQueryable(Expression<Func<T, bool>>? where,
        IEnumerable<Expression<Func<T, object>>>? orderBy, int? offset, int? count)
    {
        var queryable = _dbSet.AsQueryable();

        if (where is not null) queryable = queryable.Where(where);

        if (orderBy is not null)
        {
            var expressions = orderBy.ToArray();

            if (expressions.Length > 0)
            {
                var orderedQueryable = queryable.OrderBy(expressions.First());

                for (var i = 1; i < expressions.Length; i++)
                    orderedQueryable = orderedQueryable.ThenBy(expressions[i]);

                queryable = orderedQueryable;
            }
        }

        if (offset is not null) queryable = queryable.Skip(offset.Value);

        if (count is not null) queryable = queryable.Take(count.Value);

        return queryable;
    }
}