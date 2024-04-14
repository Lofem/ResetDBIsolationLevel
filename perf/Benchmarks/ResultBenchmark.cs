#nullable enable
using System.Data;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ResetDBIsolationLevel;

namespace ResetDBIsolationLevel.Benchmarks;

[MemoryDiagnoser]
public class ResultBenchmark
{
    private const string PoolingFalse = "Pooling=False";
    private const string DockerConnectionString = "Data Source=localhost;Persist Security Info=False;TrustServerCertificate=True;Initial Catalog=test;User ID=sa;Password=Password1!;Application Name=creditconveyor-service;Command Timeout=600;";
    private const string CurrentConnectionString = DockerConnectionString;
    private Context? _simpleContext;
    private Context? _openDisposeInterceptorContext;
    private Context? _openPoolingFalseContext;

    [Benchmark(Baseline = true)]
    public void SimpleRead()
    {
        var context = GetSimpleContext();
        var result = context.Blogs?.FirstOrDefault();
    }

    [Benchmark]
    public void OpenDisposeInterceptor()
    {
        var context = GetOpenDisposeInterceptorContext();
        var result = context.Blogs?.FirstOrDefault();
    }

    [Benchmark]
    public void ClearPool()
    {
        SqlConnection.ClearAllPools();
        var context = GetSimpleContext();
        var result = context.Blogs?.FirstOrDefault();
    }

    [Benchmark]
    public void SimpleTransaction()
    {
        var context = GetSimpleContext();
        var tran = context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        var result = context.Blogs?.FirstOrDefault();
        tran.Commit();
    }

    [Benchmark]
    public void OpenCommit()
    {
        var context = GetSimpleContext();
        var tran = context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        tran.Commit();
        var result = context.Blogs?.FirstOrDefault();
    }

    [Benchmark]
    public void OpenDispose()
    {
        var context = GetSimpleContext();
        context.Database.BeginTransaction(IsolationLevel.ReadCommitted).Dispose();
        var result = context.Blogs?.FirstOrDefault();
    }

    [Benchmark]
    public void OpenPoolingFalse()
    {
        var context = GetPoolingFalseContext();
        var result = context.Blogs?.FirstOrDefault();
    }

    [Benchmark]
    public void SetLevelDirect()
    {
        var context = GetSimpleContext();
        context.Database.ExecuteSqlRaw("SET TRANSACTION ISOLATION LEVEL READ COMMITTED");
        var result = context.Blogs?.FirstOrDefault();
    }

    private Context GetSimpleContext()
    {
        if (_simpleContext == null)
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(CurrentConnectionString)
                .Options;
            _simpleContext = new Context(options);
        }

        return _simpleContext;
    }

    private Context GetOpenDisposeInterceptorContext()
    {
        if (_openDisposeInterceptorContext == null)
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(CurrentConnectionString)
                .IsolationLevelResetForPoolConnection()
                .Options;
            _openDisposeInterceptorContext = new Context(options);
        }

        return _openDisposeInterceptorContext;
    }

    private Context GetPoolingFalseContext()
    {
        if (_openPoolingFalseContext == null)
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(CurrentConnectionString + PoolingFalse)
                .Options;
            _openPoolingFalseContext = new Context(options);
        }

        return _openPoolingFalseContext;
    }
}