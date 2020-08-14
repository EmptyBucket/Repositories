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

using Microsoft.EntityFrameworkCore;
using Repositories.Abstractions.Repositories;

namespace Repositories.Repositories;

internal class InstantRepository<T, TDbContext> : RepositoryDecorator<T>
	where T : IExprEntity
	where TDbContext : DbContext
{
	private readonly TDbContext _dbContext;

	public InstantRepository(IRepository<T> repository, TDbContext dbContext) : base(repository) =>
		_dbContext = dbContext;

	public override async Task AddAsync(T entity, CancellationToken cancellationToken = default)
	{
		await base.AddAsync(entity, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public override async Task AddManyAsync(IReadOnlyCollection<T> entities,
		CancellationToken cancellationToken = default)
	{
		await base.AddManyAsync(entities, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public override async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
	{
		await base.UpdateAsync(entity, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public override async Task UpdateManyAsync(IReadOnlyCollection<T> entities,
		CancellationToken cancellationToken = default)
	{
		await base.UpdateManyAsync(entities, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public override async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
	{
		await base.DeleteAsync(entity, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public override async Task DeleteManyAsync(IReadOnlyCollection<T> entities,
		CancellationToken cancellationToken = default)
	{
		await base.DeleteManyAsync(entities, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}