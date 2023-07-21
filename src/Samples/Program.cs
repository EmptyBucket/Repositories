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
using Npgsql;
using Repositories.Abstractions.Repositories;
using Repositories.Abstractions.Scopes;
using Repositories.Exts;
using Samples;

var postgreSettings = new NpgsqlConnectionStringBuilder
    { Host = "localhost", Database = "postgres", Username = "postgres", Password = "postgres" }.ConnectionString;
var serviceCollection = new ServiceCollection()
    .AddScopedDbContext<FooDbContext>((_, b) => b.UseNpgsql(postgreSettings))
    .AddUnitOfWork()
    .AddEntity<FirstEntity, FooDbContext>()
    .AddEntity<SecondEntity, FooDbContext>();
var serviceProvider = serviceCollection.BuildServiceProvider(true).CreateScope().ServiceProvider;

{
    var firstRepository = serviceProvider.GetRequiredService<IRepository<FirstEntity>>();
    await firstRepository.AddAsync(new FirstEntity("first"));
}

{
    var firstRepositoryFactory = serviceProvider.GetRequiredService<IRepositoryFactory<FirstEntity>>();
    var secondRepositoryFactory = serviceProvider.GetRequiredService<IRepositoryFactory<SecondEntity>>();
    var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<FooDbContext>>();
    var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

    await unitOfWork.RunAsync(async () =>
    {
        var firstRepository = firstRepositoryFactory.Create();
        var secondRepository = secondRepositoryFactory.Create();

        await firstRepository.AddAsync(new FirstEntity("second"));
        await secondRepository.AddAsync(new SecondEntity("third"));

        // do something in db context
        var fooDbContext = dbContextFactory.CreateDbContext();
    });
}