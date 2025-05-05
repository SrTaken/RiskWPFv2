using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RiskWPF.controls
{
    /// <summary>
    /// Lógica de interacción para UserDataControl.xaml
    /// </summary>
    public partial class UserDataControl : UserControl
    {
        public UserDataControl()
        {
            InitializeComponent();
        }



        public User MyUser
        {
            get { return (User)GetValue(MyUserProperty); }
            set { SetValue(MyUserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyUserProperty =
            DependencyProperty.Register("MyUser", typeof(User), typeof(UserDataControl), new PropertyMetadata(null));


    }
}
