using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using MachineLearn.Model;

namespace MachineLearn.ViewModels
{
    public class MainWindowViewModel:BindableBase
    {
        #region 字段
        public EventAggregator EventAggregator;
        
        #endregion

        #region 属性
        public ObservableCollection<MenuModel> MenuItems { get; set; } = new();
        public DelegateCommand menuComand {  get; set; }

        private UserControl _mainContent;
        public UserControl MainContent
        {
            get => _mainContent;
            set => SetProperty(ref _mainContent, value);
        }

        #endregion
        #region 构造函数
        public MainWindowViewModel(IContainerProvider container) {
            var menu = new MenuModel
            {
                Name = "文件",
                RegionName = "MainRegion"
            };

            menu.children.Add(new MenuModel { Name = "新建", RegionName = "MainRegion" });
            menu.children.Add(new MenuModel { Name = "打开", RegionName = "MainRegion" });
            menu.children.Add(new MenuModel { Name = "保存", RegionName = "MainRegion" });
            MenuItems.Add(menu);
            var menu1 = new MenuModel
            {
                Name = "文件hm",
                RegionName = "MainRegion"
            };

            menu.children.Add(new MenuModel { Name = "新建", RegionName = "MainRegion" });
            menu.children.Add(new MenuModel { Name = "打开", RegionName = "MainRegion" });
            menu.children.Add(new MenuModel { Name = "保存", RegionName = "MainRegion" });
            MenuItems.Add(menu1);

            // 直接创建 DataCollectionView 实例赋值给 Content
            //MainContent = new MachineLearnDatatcollection.Views.DataCollectionView();
        }
        #endregion

        #region 方法
        private void menu()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
