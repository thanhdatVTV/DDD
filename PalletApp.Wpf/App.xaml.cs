using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PalletApp.Application.Services;
using PalletApp.Domain.Repositories;
using PalletApp.Infrastructure;
using PalletApp.Infrastructure.Repositories;
using PalletApp.Wpf.ViewModels;

namespace PalletApp.Wpf;

public partial class App : System.Windows.Application
{
    private ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        var connectionString = "Server=DESKTOP-SFFNR67\\SQLEXPRESS;Database=PalletAppDbDDD;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";
        
        services.AddDbContext<PalletAppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IPalletRepository, PalletRepository>();
        services.AddScoped<IBatchRepository, BatchRepository>();
        services.AddScoped<IBinLocationRepository, BinLocationRepository>();
        services.AddScoped<IPrintRepository, PrintRepository>();
        services.AddScoped<IPalletAppService, PalletAppService>();

        services.AddTransient<PalletListViewModel>();
        services.AddTransient<ScanBatchViewModel>();
        // Other ViewModels are created with parameters in demo
        
        services.AddSingleton<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<PalletAppDbContext>();
            dbContext.Database.EnsureCreated();
        }

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
        base.OnStartup(e);
    }
}
