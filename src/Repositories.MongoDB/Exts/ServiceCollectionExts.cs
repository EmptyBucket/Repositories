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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Repositories.Abstractions.Repositories;
using Repositories.Mongo.Repositories;

namespace Repositories.Mongo.Exts;

internal static class ServiceCollectionExts
{
    public static IServiceCollection AddEntity<T>(this IServiceCollection serviceCollection,
        IMongoDatabase mongoDatabase, string? collectionName = default,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : IExprEntity
    {
        if (string.IsNullOrEmpty(collectionName)) collectionName = typeof(T).Name;

        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IMongoCollection<T>),
            _ => mongoDatabase.GetCollection<T>(collectionName), lifetime));
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IRepository<T>), typeof(Repository<T>), lifetime));

        return serviceCollection;
    }
}