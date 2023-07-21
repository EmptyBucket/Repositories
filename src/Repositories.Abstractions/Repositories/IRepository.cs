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

namespace Repositories.Abstractions.Repositories;

public interface IRepository<T> where T : IExprEntity
{
    Task<IReadOnlyCollection<T>> GetManyAsync(Expression<Func<T, bool>>? where = null,
        IEnumerable<Expression<Func<T, object>>>? orderBy = null, int? offset = null, int? count = null,
        CancellationToken cancellationToken = default);

    Task<T?> FindAsync(Expression<Func<T, bool>>? where = null,
        IEnumerable<Expression<Func<T, object>>>? orderBy = null, int? offset = null, int? count = null,
        CancellationToken cancellationToken = default);

    Task<T> GetAsync(Expression<Func<T, bool>>? where = null, IEnumerable<Expression<Func<T, object>>>? orderBy = null,
        int? offset = null, int? count = null, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddManyAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateManyAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default);

    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    Task DeleteManyAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default);
}