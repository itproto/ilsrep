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
        private int colsCount;
        private int rowsCount;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RefreshCells()
        {
            kioskWindowGrid.Children.Clear();
            kioskWindowGrid.RowDefinitions.Clear();
            kioskWindowGrid.ColumnDefinitions.Clear();

            // Add first row and column to kioskWindowGrid
            ColumnDefinition colDefin = new ColumnDefinition();
            RowDefinition rowDefin = new RowDefinition();
            kioskWindowGrid.ColumnDefinitions.Add(colDefin);
            kioskWindowGrid.RowDefinitions.Add(rowDefin);

            Grid firstRowGrid = new Grid();
            ColumnDefinition firstColDefinition = new ColumnDefinition();
            firstRowGrid.ColumnDefinitions.Add(firstColDefinition);
            while (firstRowGrid.ColumnDefinitions.Count < colsCount)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                firstRowGrid.ColumnDefinitions.Add(colDef);
                GridSplitter gridSplitter = new GridSplitter();
                gridSplitter.Background = new SolidColorBrush(Color.FromArgb(255, 214, 229, 248));
                gridSplitter.Width = 5;
                gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                gridSplitter.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(gridSplitter, firstRowGrid.ColumnDefinitions.Count - 1);
                Grid.SetRow(gridSplitter, 0);
                firstRowGrid.Children.Add(gridSplitter);
            }
            kioskWindowGrid.Children.Add(firstRowGrid);
            Grid.SetColumn(firstRowGrid, 0);
            Grid.SetRow(firstRowGrid, kioskWindowGrid.RowDefinitions.Count - 1);

            while (kioskWindowGrid.RowDefinitions.Count < rowsCount)
            {
                RowDefinition rowDef = new RowDefinition();
                kioskWindowGrid.RowDefinitions.Add(rowDef);

                Grid rowGrid = new Grid();
                ColumnDefinition colDefinition = new ColumnDefinition();
                rowGrid.ColumnDefinitions.Add(colDefinition);

                while (rowGrid.ColumnDefinitions.Count < colsCount)
                {
                    ColumnDefinition colDef = new ColumnDefinition();
                    rowGrid.ColumnDefinitions.Add(colDef);
                    GridSplitter gridSplitter = new GridSplitter();
                    gridSplitter.Background = new SolidColorBrush(Color.FromArgb(255, 214, 229, 248));
                    gridSplitter.Width = 5;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Left;
                    Grid.SetColumn(gridSplitter, rowGrid.ColumnDefinitions.Count - 1);
                    Grid.SetRow(gridSplitter, 0);
                    rowGrid.Children.Add(gridSplitter);
                }
                kioskWindowGrid.Children.Add(rowGrid);
                Grid.SetColumn(rowGrid, 0);
                Grid.SetRow(rowGrid, kioskWindowGrid.RowDefinitions.Count - 1);

                GridSplitter horGridSplitter = new GridSplitter();
                horGridSplitter.Background = new SolidColorBrush(Color.FromArgb(255, 214, 229, 248));
                horGridSplitter.Height = 5;
                horGridSplitter.VerticalAlignment = VerticalAlignment.Top;
                horGridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                kioskWindowGrid.Children.Add(horGridSplitter);
                Grid.SetColumn(horGridSplitter, 0);
                Grid.SetRow(horGridSplitter, kioskWindowGrid.RowDefinitions.Count - 1);
            }
        }

        private void ColsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                colsCount = Convert.ToInt32(colsTextBox.Text);
                RefreshCells();
            }
            catch (Exception exception)
            {

            }
        }

        private void RowsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                rowsCount = Convert.ToInt32(rowsTextBox.Text);
                RefreshCells();
            }
            catch (Exception exception)
            {

            }
        }
    }
}
