using FreeSql;
using MachineLearn.Data;
using MachineLearn.Views;
using MachineLearnDatatcollection;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System.Configuration;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Windows;

namespace MachineLearn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Views.MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<Serilog.ILogger>(Log.Logger);
            containerRegistry.RegisterSingleton<IFreeSql>(() => { 
                return new FreeSqlBuilder()
             .UseConnectionString(DataType.Sqlite, @"Data Source=app.db")
             .UseAutoSyncStructure(true)
             .Build();
            });

            // 🟢 EF Core 注册
            containerRegistry.RegisterSingleton<DbContextOptions<AppDbContext>>(sp =>
            {
                var builder = new DbContextOptionsBuilder<AppDbContext>();
                builder.UseSqlite("Data Source=app.db");
                return builder.Options;
            });
            containerRegistry.Register<AppDbContext>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // 注册数据采集模块
            moduleCatalog.AddModule<DataCollectionModule>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigurationMapster();
            ConfigurationSeilog();
            ///<summary>
            ///prism初始化，触发所有流程
            ///├─ 构建容器
            ///├─ 调用 RegisterTypes(...)   ← 这里注册 Log.Logger
            ///└─ 调用 CreateShell()
            /// </summary>
            base.OnStartup(e);

            // 🟢 EF Core：自动建表 + 初始化种子数据
            using (var scope = Container.CreateScope())
            {
                var db = scope.Resolve<AppDbContext>();
                db.Database.EnsureCreated();  // 表不存在就创建
                if (!db.Menus.Any())
                {
                    var fileMenu = new MachineLearn.Model.MenuModel
                    {
                        Name = "文件",
                        RegionName = "MainRegion"
                    };
                    db.Menus.Add(fileMenu);
                    db.Menus.AddRange(
                        new MachineLearn.Model.MenuModel { Name = "新建", RegionName = "NewRegion", ParentId = fileMenu.Id },
                        new MachineLearn.Model.MenuModel { Name = "打开", RegionName = "OpenRegion", ParentId = fileMenu.Id },
                        new MachineLearn.Model.MenuModel { Name = "保存", RegionName = "SaveRegion", ParentId = fileMenu.Id }
                    );
                    db.SaveChanges();
                }
            }
        }

        private void ConfigurationMapster()
        {
            TypeAdapterConfig<User, UserDto>.NewConfig().Map(dest => dest.UserId, src => $"{src.FirstName}");
        }

        /// <summary>
        /// 配置serilog
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ConfigurationSeilog()
        {
            //var configuration = new ConfigurationBuilder().SetBasePath($@"{AppDomain.CurrentDomain.BaseDirectory}Configuration").AddJsonFile("appsettings.json").Build();
            //Log.Logger = new LoggerConfiguration.ReadFrom.Configuration(configuration)
            //   .CreateLogger();
            // 直接写代码配置 Serilog
            Log.Logger = new LoggerConfiguration()
         .MinimumLevel.Debug()
         .Enrich.FromLogContext()
         .WriteTo.Async(a => a.Map("Module", "Other", (module, writeTo) =>
         {
             writeTo.File($"logs/{module}/log-.log",
                 rollingInterval: RollingInterval.Day);
         }, sinkMapCountLimit: 30))
         .CreateLogger();


        }
        //protected override void OnInitialized()
        //{
        //    base.OnInitialized();
        //    var region = Container.Resolve<IRegionManager>();
        //    region.RegisterViewWithRegion("main", "DataCollectionView");
        //}
    }
}
