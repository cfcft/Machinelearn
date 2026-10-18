using System.ComponentModel;
using System.Runtime.CompilerServices;
using Prism.Commands;

namespace MachineLearn.Controls
{
    // Prism 分页控件的 ViewModel：用 BindableBase + DelegateCommand
    public class PrismPagerViewModel : INotifyPropertyChanged
    {
        private int _totalPages = 10;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (_totalPages != value)
                {
                    _totalPages = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanPrev));
                    OnPropertyChanged(nameof(CanNext));
                }
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanPrev));
                    OnPropertyChanged(nameof(CanNext));
                }
            }
        }

        public bool CanPrev => CurrentPage > 1;
        public bool CanNext => CurrentPage < TotalPages;

        public DelegateCommand PrevCommand { get; }
        public DelegateCommand NextCommand { get; }

        public PrismPagerViewModel()
        {
            PrevCommand = new DelegateCommand(() =>
            {
                if (CurrentPage > 1) CurrentPage--;
            }, () => CanPrev).ObservesProperty(() => CurrentPage).ObservesProperty(() => TotalPages);

            NextCommand = new DelegateCommand(() =>
            {
                if (CurrentPage < TotalPages) CurrentPage++;
            }, () => CanNext).ObservesProperty(() => CurrentPage).ObservesProperty(() => TotalPages);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
