using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MachineLearn
{
    public class Control1:UserControl
    {
        public Control1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control1), new FrameworkPropertyMetadata(typeof(Control1)));
        }


        public object tempory
        {
            get { return (object)GetValue(temporyProperty); }
            set { SetValue(temporyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for tempory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty temporyProperty =
            DependencyProperty.Register(nameof(tempory), typeof(object), typeof(Control1));


    }
}
