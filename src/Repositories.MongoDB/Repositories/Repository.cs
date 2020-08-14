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
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Repositories.Abstractions.Repositories;

namespace Repositories.Mongo.Repositories;

internal class Repository<T> : IRepository<T> where T : IExprEntity
{
	private readonly IMongoCollection<T> _mongoCollection;

	public Repository(IMongoCollection<T> mongoCollection)
	{
		_mongoCollection = mongoCollection;
	}

	public async Task<IReadOnlyCollection<T>> GetManyAsync(Expression<Func<T, bool>>? where = null,
		IEnumerable<Expression<Func<T, object>>>? orderBy = null, int? offset = null, int? count = null,
		CancellationToken cancellationToken = default)
	{
		return await BuildQueryable(where, orderBy, offset, count).ToListAsync(cancellationToken);
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
		await _mongoCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);
	}

	public async Task AddManyAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default)
	{
		await _mongoCollection.InsertManyAsync(entities, cancellationToken: cancellationToken);
	}

	public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
	{
		await _mongoCollection.ReplaceOneAsync(EqualIdExpr(entity), entity, cancellationToken: cancellationToken);
	}

	public async Task UpdateManyAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default)
	{
		using var session =
			await _mongoCollection.Database.Client.StartSessionAsync(cancellationToken: cancellationToken);

		session.StartTransaction();

		try
		{
			foreach (var entity in entities)
				await _mongoCollection.ReplaceOneAsync(EqualIdExpr(entity), entity,
					cancellationToken: cancellationToken);
		}
		catch
		{
			await session.AbortTransactionAsync(cancellationToken);
		}
		finally
		{
			await session.CommitTransactionAsync(cancellationToken);
		}
	}

	public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
	{
		await _mongoCollection.DeleteOneAsync(EqualIdExpr(entity), cancellationToken);
	}

	public async Task DeleteManyAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default)
	{
		await _mongoCollection.DeleteManyAsync(AnyEqualIdExpr(entities), cancellationToken);
	}

	private IMongoQueryable<T> BuildQueryable(Expression<Func<T, bool>>? where,
		IEnumerable<Expression<Func<T, object>>>? orderBy, int? offset, int? count)
	{
		var queryable = _mongoCollection.AsQueryable();

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

	private static Expression<Func<T, bool>> EqualIdExpr(T other)
	{
		var member = GetMemberExpr(other.IdExpr);

		var leftParam = Expression.Parameter(typeof(T));
		var left = member.Update(leftParam);
		var right = member.Update(Expression.Constant(other));

		var equal = Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), leftParam);
		return equal;
	}

	private static Expression<Func<T, bool>> AnyEqualIdExpr(IReadOnlyCollection<T> others)
	{
		if (!others.Any()) return o => false;

		var member = GetMemberExpr(others.First().IdExpr);

		var leftParam = Expression.Parameter(typeof(T));
		var left = member.Update(leftParam);
		var rightParam = Expression.Parameter(typeof(T));
		var right = member.Update(rightParam);

		var anyEqual = Expression.Lambda<Func<T, bool>>(Expression.Call(
			typeof(Enumerable), nameof(Enumerable.Any), new[] { typeof(IEnumerable<T>), typeof(Func<T, bool>) },
			Expression.Constant(others), Expression.Lambda(Expression.Equal(left, right), rightParam)), leftParam);
		return anyEqual;
	}

	private static MemberExpression GetMemberExpr(Expression expression)
	{
		var lambda = expression as LambdaExpression
		             ?? throw new ArgumentException($"{nameof(expression)} must be {nameof(LambdaExpression)}");
		var member = lambda.Body.NodeType switch
		{
			ExpressionType.Convert
				when lambda.Body is UnaryExpression { Operand: MemberExpression memberExpression } => memberExpression,
			ExpressionType.MemberAccess when lambda.Body is MemberExpression memberExpression => memberExpression,
			_ => throw new ArgumentException($"{nameof(expression)} must have {nameof(MemberExpression)}")
		};
		return member;
	}
}