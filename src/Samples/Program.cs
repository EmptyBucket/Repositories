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

﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Repositories.Abstractions.Repositories;
using Repositories.Abstractions.Scopes;
using Repositories.Exts;

var postgreSettings = new NpgsqlConnectionStringBuilder
    { Host = "localhost", Database = "postgres", Username = "postgres", Password = "postgres" }.ConnectionString;
var serviceCollection = new ServiceCollection()
    .AddScopedDbContext<FooDbContext>((_, b) => b.UseNpgsql(postgreSettings))
    .AddUnitOfWork()
    .AddEntity<FirstEntity, FooDbContext>()
    .AddEntity<SecondEntity, FooDbContext>();
var serviceProvider = serviceCollection.BuildServiceProvider().CreateScope().ServiceProvider;

{
    var firstRepository = serviceProvider.GetRequiredService<IRepository<FirstEntity>>();
    await firstRepository.AddAsync(new FirstEntity("first"));
}

{
    var firstRepositoryFactory = serviceProvider.GetRequiredService<IRepositoryFactory<FirstEntity>>();
    var secondRepositoryFactory = serviceProvider.GetRequiredService<IRepositoryFactory<SecondEntity>>();
    var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

    await unitOfWork.RunAsync(async () =>
    {
        var firstRepository = firstRepositoryFactory.Create();
        var secondRepository = secondRepositoryFactory.Create();

        await firstRepository.AddAsync(new FirstEntity("second"));
        await secondRepository.AddAsync(new SecondEntity("third"));
    });
}

public class FooDbContext : DbContext
{
    public FooDbContext(DbContextOptions<FooDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FirstEntity>().HasKey(f => f.Id);
        modelBuilder.Entity<FirstEntity>().Property(f => f.Name);

        modelBuilder.Entity<SecondEntity>().HasKey(f => f.Id);
        modelBuilder.Entity<SecondEntity>().Property(f => f.Name);
    }
}

public class FirstEntity : IEntity<Guid, FirstEntity>
{
    private FirstEntity(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public FirstEntity(string name) : this(Guid.NewGuid(), name)
    {
    }

    public Guid Id { get; }

    public string Name { get; }
}

public class SecondEntity : IEntity<Guid, FirstEntity>
{
    private SecondEntity(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public SecondEntity(string name) : this(Guid.NewGuid(), name)
    {
    }

    public Guid Id { get; }

    public string Name { get; }
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FooDbContext>
{
    public FooDbContext CreateDbContext(string[] args)
    {
        var postgreSettings = new NpgsqlConnectionStringBuilder
                { Host = "localhost", Database = "postgres", Username = "postgres", Password = "postgres" }
            .ConnectionString;
        var dbContextOptions = new DbContextOptionsBuilder<FooDbContext>().UseNpgsql(postgreSettings).Options;
        return new FooDbContext(dbContextOptions);
    }
}