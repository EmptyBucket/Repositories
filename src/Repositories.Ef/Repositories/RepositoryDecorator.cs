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
using Repositories.Abstractions.Repositories;

namespace Repositories.Repositories;

internal abstract class RepositoryDecorator<T> : IRepository<T> where T : IExprEntity
{
	private readonly IRepository<T> _repository;

	protected RepositoryDecorator(IRepository<T> repository) => _repository = repository;

	public virtual Task<T?> FindAsync(Expression<Func<T, bool>>? where = null,
		IEnumerable<Expression<Func<T, object>>>? orderBy = null, int? offset = null, int? count = null,
		CancellationToken cancellationToken = default) =>
		_repository.FindAsync(where, orderBy, offset, count, cancellationToken);

	public virtual Task<T> GetAsync(Expression<Func<T, bool>>? where = null,
		IEnumerable<Expression<Func<T, object>>>? orderBy = null, int? offset = null, int? count = null,
		CancellationToken cancellationToken = default) =>
		_repository.GetAsync(where, orderBy, offset, count, cancellationToken);

	public virtual Task<IReadOnlyCollection<T>> GetManyAsync(Expression<Func<T, bool>>? where = null,
		IEnumerable<Expression<Func<T, object>>>? orderBy = null, int? offset = null, int? count = null,
		CancellationToken cancellationToken = default) =>
		_repository.GetManyAsync(where, orderBy, offset, count, cancellationToken);

	public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
		_repository.AddAsync(entity, cancellationToken);

	public virtual Task AddManyAsync(IReadOnlyCollection<T> entities,
		CancellationToken cancellationToken = default) =>
		_repository.AddManyAsync(entities, cancellationToken);

	public virtual Task UpdateAsync(T item, CancellationToken cancellationToken = default) =>
		_repository.UpdateAsync(item, cancellationToken);

	public virtual Task UpdateManyAsync(IReadOnlyCollection<T> entities,
		CancellationToken cancellationToken = default) =>
		_repository.UpdateManyAsync(entities, cancellationToken);

	public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default) =>
		_repository.DeleteAsync(entity, cancellationToken);

	public virtual async Task DeleteManyAsync(IReadOnlyCollection<T> entities,
		CancellationToken cancellationToken = default) =>
		await _repository.DeleteManyAsync(entities, cancellationToken);
}