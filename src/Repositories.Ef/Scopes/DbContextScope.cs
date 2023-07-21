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

namespace Repositories.Scopes;

internal class DbContextScope<TDbContext> : IDbContextScope<TDbContext> where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _dbContextFactory;
    private readonly Lazy<TDbContext> _dbContext;

    public DbContextScope(IDbContextFactory<TDbContext> dbContextFactory, IDbContextScope<TDbContext>? parent = null)
    {
        _dbContextFactory = dbContextFactory;
        _dbContext = new Lazy<TDbContext>(dbContextFactory.CreateDbContext);
        Parent = parent;
    }

    public IDbContextScope<TDbContext>? Parent { get; }

    public IDbContextScope<TDbContext>? Child { get; private set; }

    public bool IsDisposed { get; private set; }

    public TDbContext DbContext => _dbContext.Value;

    public IDbContextScope<TDbContext> Begin() => Child = new DbContextScope<TDbContext>(_dbContextFactory, this);

    public async ValueTask DisposeAsync()
    {
        if (IsDisposed) return;

        if (Child is not null) await Child.DisposeAsync();

        if (_dbContext.IsValueCreated)
        {
            await _dbContext.Value.SaveChangesAsync().ConfigureAwait(false);
            await _dbContext.Value.DisposeAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
        IsDisposed = true;
    }
}