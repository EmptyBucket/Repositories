# Repositories
Library for working with repositories and unit of work
#### Nuget:
* https://www.nuget.org/packages/ap.Repository.Abstractions/
* https://www.nuget.org/packages/ap.Repository.Ef/
## Usage
```csharp
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
```
