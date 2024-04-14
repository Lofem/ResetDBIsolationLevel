# ResetDBIsolationLevel
Extension for resetting the isolation level on pooled connections
Usage example:
```
var options = new DbContextOptionsBuilder<Context>()
    .UseSqlServer(CurrentConnectionString)
    .IsolationLevelResetForPoolConnection()
    .Options;
var context = new Context(options);
```

Example for DbContext configure:
```
.AddDbContextPool<Context>(o => o
    .UseSqlServer(connectionString)
    .IsolationLevelResetForPoolConnection())
```

nuget link https://www.nuget.org/packages/ResetDBIsolationLevel
