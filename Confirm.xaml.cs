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
using System.Windows.Shapes;

namespace AES
{
    /// <summary>
    /// Interaction logic for Confirm.xaml
    /// </summary>
    public partial class Confirm : Window
    {
        public string SelectedOption { get; private set; }

        public Confirm()
        {
            InitializeComponent();
        }

        public Confirm(string title, string item1, string item2)
        {
            InitializeComponent();
            lbTitle.Content = title;
            cbo1.Content = item1;
            cbo2.Content = item2;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedOption = selectedItem.Content.ToString();
                DialogResult = true;
            }

        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
