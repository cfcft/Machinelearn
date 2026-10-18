using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Prism.Mvvm;

namespace MachineLearn.Model
{
    // 菜单模型：同时作为 MVVM 实体和 EF Core 数据库实体
    public class MenuModel : BindableBase
    {
        // ① EF Core 主键（自增）
        public int Id { get; set; }

        // ② 必填：菜单名称
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        // ③ 导航区域名
        private string _regionname = string.Empty;
        public string RegionName
        {
            get => _regionname;
            set => SetProperty(ref _regionname, value);
        }

        // ④ 自引用外键（父菜单 ID，顶层菜单为 null）
        public int? ParentId { get; set; }

        // ⑤ EF Core 忽略：UI 层的子集合
        [JsonIgnore]
        public ObservableCollection<MenuModel> children { get; set; } = new();

        // ⑥ EF Core 忽略：父引用（导航属性）
        [JsonIgnore]
        public MenuModel? Parent { get; set; }
    }
}
