using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ilsrep.Kiosk
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ColumnDefinition colDef;
        private RowDefinition rowDef;
        private GridSplitter gridSplitter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            widthTextBox.Text = kioskWindowGrid.Width.ToString();
            heightTextBox.Text = kioskWindowGrid.Height.ToString();
            widthSlider.Value = kioskWindowGrid.Width;
            heightSlider.Value = kioskWindowGrid.Height;
        }

        private void widthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kioskWindowGrid.Width = Math.Round(widthSlider.Value);
            widthTextBox.Text = kioskWindowGrid.Width.ToString();
        }

        private void heightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kioskWindowGrid.Height = Math.Round(heightSlider.Value);
            heightTextBox.Text = kioskWindowGrid.Height.ToString();
        }

        private void kioskWindowGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainWindow.Height = kioskWindowGrid.Height + 150;
            mainWindow.Width = kioskWindowGrid.Width + 260;
        }

        private void widthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                widthSlider.Value = Convert.ToDouble(widthTextBox.Text);
                kioskWindowGrid.Width = Math.Round(widthSlider.Value);                
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void heightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                heightSlider.Value = Convert.ToDouble(heightTextBox.Text);
                kioskWindowGrid.Height = Math.Round(heightSlider.Value);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void addColBtn_Click(object sender, RoutedEventArgs e)
        {
            colDef = new ColumnDefinition();
            kioskWindowGrid.ColumnDefinitions.Add(colDef);
        }

        private void addRowBtn_Click(object sender, RoutedEventArgs e)
        {
            rowDef = new RowDefinition();
            kioskWindowGrid.RowDefinitions.Add(rowDef);
        }
    }
}
