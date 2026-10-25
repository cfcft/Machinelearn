using FreeSql;
using MachineLearn.Data;
using MachineLearn.Views;
using MachineLearnDatatcollection;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using SqlSugar;
using System.Configuration;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using DbType = SqlSugar.DbType;
using Log = Serilog.Log;

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
            containerRegistry.RegisterInstance<ILogger>(Serilog.Log.Logger);
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
            ///<summary>
            ///注册sqlsugar
            /// </summary>
            SqlSugarClient sqlSugar=new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "你的数据库连接字符串",
                DbType = DbType.SqlServer, // 根据实际情况修改
                IsAutoCloseConnection = true, // 自动释放连接
                InitKeyType = InitKeyType.Attribute // 从特性读取主键自增信息
            }, db => {
                // 在这里进行全局配置
                // 例如：打印执行的 SQL 语句到调试输出
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    System.Diagnostics.Debug.WriteLine(sql);
                };

                // 也可以在这里设置全局超时时间（单位：秒）
                // db.Ado.CommandTimeOut = 30;
            });
            containerRegistry.RegisterInstance<ISqlSugarClient>(sqlSugar);
            containerRegistry.RegisterInstance<ILogger>(Serilog.Log.Logger);
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
            Serilog.Log.Logger = new LoggerConfiguration()
         .MinimumLevel.Debug()
         .Enrich.FromLogContext()
         .WriteTo.Async(a => a.Map("Module", "Other", (module, writeTo) =>
         {
             writeTo.File($"logs/{module}/log-.log",
                 rollingInterval: RollingInterval.Day);
         }, sinkMapCountLimit: 30))
         .CreateLogger();
            Serilog.Log.Logger = new LoggerConfiguration().MinimumLevel.Warning().Enrich.FromLogContext().WriteTo.Async(a => a.Map("warn", "Other", (warn, writeTo) =>
            {
                writeTo.File($"logs/{warn}/log-.log",
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
