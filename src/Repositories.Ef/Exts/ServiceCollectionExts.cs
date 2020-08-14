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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Repositories.Abstractions.Repositories;
using Repositories.Abstractions.Scopes;
using Repositories.Repositories;
using Repositories.Scopes;

namespace Repositories.Exts;

public static class ServiceCollectionExts
{
    public static IServiceCollection AddScopedDbContext<TDbContext>(this IServiceCollection serviceCollection,
        Action<IServiceProvider, DbContextOptionsBuilder> action, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        serviceCollection.AddDbContextFactory<TDbContext>(action);
        serviceCollection.Add(new ServiceDescriptor(typeof(IScopeHead<IDbContextScope<TDbContext>>), p =>
        {
            var dbContextFactory = p.GetRequiredService<IDbContextFactory<TDbContext>>();
            var dbContextScope = new DbContextScope<TDbContext>(dbContextFactory);
            return new ScopeHead<IDbContextScope<TDbContext>>(dbContextScope);
        }, lifetime));
        serviceCollection.Add(new ServiceDescriptor(typeof(IScopeHead<IDbContextScope>),
            p => p.GetRequiredService<IScopeHead<IDbContextScope<TDbContext>>>(), lifetime));

        return serviceCollection;
    }

    public static IServiceCollection AddUnitOfWork(this IServiceCollection serviceCollection,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IUnitOfWork), typeof(UnitOfWork), lifetime));

        return serviceCollection;
    }

    public static IServiceCollection AddEntity<T, TDbContext>(this IServiceCollection serviceCollection,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : class, IExprEntity
        where TDbContext : DbContext
    {
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IRepositoryFactory<T>),
            typeof(ScopedRepositoryFactory<T, TDbContext>), lifetime));
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IRepository<T>),
            p => p.GetRequiredService<IRepositoryFactory<T>>().Create(), lifetime));

        return serviceCollection;
    }
}