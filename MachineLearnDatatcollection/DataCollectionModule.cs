using Prism.Ioc;
using Prism.Modularity;
using MachineLearnDatatcollection.Views;

namespace MachineLearnDatatcollection
{
    public class DataCollectionModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var region = containerProvider.Resolve<IRegionManager>();
            region.RegisterViewWithRegion("nain", typeof(DataCollectionView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册视图到导航，键名默认是 "DataCollectionView"
            containerRegistry.RegisterForNavigation<DataCollectionView>();
        }
    }
}
