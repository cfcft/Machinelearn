using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MachineLearn.Controls
{
    // 原生 WPF 分页控件：用 DependencyProperty + 原生 ICommand + 路由事件
    public partial class NativePager : UserControl
    {
        // ① 依赖属性：总页数
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(NativePager),
                new PropertyMetadata(1, OnPageChanged));

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(TotalPagesProperty, value);
        }

        // ② 依赖属性：当前页
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(NativePager),
                new PropertyMetadata(1, OnPageChanged));

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        // ③ 原生 WPF ICommand：上一页
        public ICommand PrevCommand { get; }
        // ④ 原生 WPF ICommand：下一页
        public ICommand NextCommand { get; }

        // ⑤ 路由事件：页码变化
        public static readonly RoutedEvent PageChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(PageChanged), RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<int>), typeof(NativePager));

        public event RoutedPropertyChangedEventHandler<int> PageChanged
        {
            add => AddHandler(PageChangedEvent, value);
            remove => RemoveHandler(PageChangedEvent, value);
        }

        public NativePager()
        {
            InitializeComponent();
            // ⚠️ 关键：让 UserControl 的 DataContext 指向自己
            // 这样内部按钮的 {Binding PrevCommand} 才能找到正确的对象
            this.DataContext = this;

            PrevCommand = new RelayCommand(_ =>
            {
                if (CurrentPage > 1)
                {
                    int old = CurrentPage;
                    CurrentPage--;
                    RaiseEvent(new RoutedPropertyChangedEventArgs<int>(old, CurrentPage, PageChangedEvent));
                }
            }, _ => CurrentPage > 1);

            NextCommand = new RelayCommand(_ =>
            {
                if (CurrentPage < TotalPages)
                {
                    int old = CurrentPage;
                    CurrentPage++;
                    RaiseEvent(new RoutedPropertyChangedEventArgs<int>(old, CurrentPage, PageChangedEvent));
                }
            }, _ => CurrentPage < TotalPages);

            UpdateDisplay();
            // 依赖属性变化时刷新 Command 的 CanExecute
            DependencyPropertyDescriptor.FromProperty(CurrentPageProperty, typeof(NativePager))
                .AddValueChanged(this, (_, _) => CommandManager.InvalidateRequerySuggested());
            DependencyPropertyDescriptor.FromProperty(TotalPagesProperty, typeof(NativePager))
                .AddValueChanged(this, (_, _) => CommandManager.InvalidateRequerySuggested());
        }

        private static void OnPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NativePager pager) pager.UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            PageText.Text = $"{CurrentPage} / {TotalPages}";
        }
    }

    // 原生 WPF ICommand 实现（不用 Prism）
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);
    }
}
