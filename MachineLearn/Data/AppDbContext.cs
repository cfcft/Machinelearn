using Microsoft.EntityFrameworkCore;
using MachineLearn.Model;

namespace MachineLearn.Data
{
    // EF Core DbContext：定义数据库连接和实体映射
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 实体表
        public DbSet<MenuModel> Menus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置 MenuModel
            modelBuilder.Entity<MenuModel>(entity =>
            {
                entity.HasKey(m => m.Id);                       // 主键
                entity.Property(m => m.Id).ValueGeneratedOnAdd(); // 自增
                entity.Property(m => m.Name).IsRequired();      // 必填
                entity.Property(m => m.RegionName);

                // 忽略 UI 层的子集合和父引用（不存数据库）
                entity.Ignore(m => m.children);
                entity.Ignore(m => m.Parent);
            });
        }
    }
}
