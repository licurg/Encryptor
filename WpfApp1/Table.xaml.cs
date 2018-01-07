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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Table.xaml
    /// </summary>
    public partial class Table : Window
    {

        public Table()
        {
            InitializeComponent();
        }
        public void DrawTable(List<int> ways, byte[] key)
        {
            for(int i = 0; i < ways.Count; i++)
            {
                StackPanel col = new StackPanel();
                Label way = new Label();
                Label value = new Label();

                way.Content = Encrypt.alphaWays[i];
                way.BorderBrush = Brushes.Gray;
                way.BorderThickness = new Thickness(1);
                way.Padding = new Thickness(10);

                value.Content = Encrypt.alphaK[i];
                value.BorderBrush = Brushes.Gray;
                value.BorderThickness = new Thickness(1);
                value.Padding = new Thickness(10);

                col.Children.Add(way);
                col.Children.Add(value);
                alphaCell.Children.Add(col);

                col = new StackPanel();
                way = new Label();
                value = new Label();

                way.Content = Encrypt.redWays[i];
                way.BorderBrush = Brushes.Red;
                way.BorderThickness = new Thickness(1);
                way.Padding = new Thickness(10);

                value.Content = Encrypt.redK[i];
                value.BorderBrush = Brushes.Red;
                value.BorderThickness = new Thickness(1);
                value.Padding = new Thickness(10);

                col.Children.Add(way);
                col.Children.Add(value);
                redCell.Children.Add(col);

                col = new StackPanel();
                way = new Label();
                value = new Label();

                way.Content = Encrypt.greenWays[i];
                way.BorderBrush = Brushes.Green;
                way.BorderThickness = new Thickness(1);
                way.Padding = new Thickness(10);

                value.Content = Encrypt.greenK[i];
                value.BorderBrush = Brushes.Green;
                value.BorderThickness = new Thickness(1);
                value.Padding = new Thickness(10);

                col.Children.Add(way);
                col.Children.Add(value);
                greenCell.Children.Add(col);

                col = new StackPanel();
                way = new Label();
                value = new Label();

                way.Content = Encrypt.blueWays[i];
                way.BorderBrush = Brushes.Blue;
                way.BorderThickness = new Thickness(1);
                way.Padding = new Thickness(10);

                value.Content = Encrypt.blueK[i];
                value.BorderBrush = Brushes.Blue;
                value.BorderThickness = new Thickness(1);
                value.Padding = new Thickness(10);

                col.Children.Add(way);
                col.Children.Add(value);
                blueCell.Children.Add(col);
            }   
        }
    }
}
