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
        /// <summary>
        /// Count of columns
        /// </summary>
        private int colsCount;
        /// <summary>
        /// Count of rows
        /// </summary>
        private int rowsCount;
        /// <summary>
        /// Color of cells
        /// </summary>
        private SolidColorBrush CELL_COLOR = new SolidColorBrush(Color.FromRgb(169, 169, 169));
        /// <summary>
        /// Color of selected cells
        /// </summary>
        private SolidColorBrush CELL_COLOR_SELECTED = new SolidColorBrush(Color.FromRgb(55, 85, 160));

        public MainWindow()
        {
            InitializeComponent();
            colsCount = 1;
            rowsCount = 1;
        }

        /// <summary>
        /// Changes cell color to selected or deselected
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void Select(object sender, MouseButtonEventArgs e)
        {
            if (((StackPanel)sender).Background == CELL_COLOR)
            {
                ((StackPanel)sender).Background = CELL_COLOR_SELECTED;
            }
            else
            {
                ((StackPanel)sender).Background = CELL_COLOR;
            }
        }

        /// <summary>
        /// Refresh entire grid, generate cells anew
        /// </summary>
        private void RefreshCells()
        {
            // Clear all cells definitions
            kioskWindowGrid.Children.Clear();
            kioskWindowGrid.RowDefinitions.Clear();
            kioskWindowGrid.ColumnDefinitions.Clear();
            
            // Add first column to kioskWindowGrid
            ColumnDefinition columnDefinition = new ColumnDefinition();
            kioskWindowGrid.ColumnDefinitions.Add(columnDefinition);
            
            // Form grid cells
            while (kioskWindowGrid.RowDefinitions.Count < rowsCount)
            {
                RowDefinition rowDef = new RowDefinition();
                kioskWindowGrid.RowDefinitions.Add(rowDef);

                // Form new grid that will contain only columns
                Grid rowGrid = new Grid();
                while (rowGrid.ColumnDefinitions.Count < colsCount)
                {
                    ColumnDefinition colDef = new ColumnDefinition();
                    rowGrid.ColumnDefinitions.Add(colDef);

                    // Add StackPanel to grid for further possibility to select cells
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Background = CELL_COLOR;
                    stackPanel.MouseDown += new MouseButtonEventHandler(Select);
                    Grid.SetColumn(stackPanel, rowGrid.ColumnDefinitions.Count - 1);
                    Grid.SetRow(stackPanel, 0);
                    rowGrid.Children.Add(stackPanel);

                    // If first column, there's no need of vertical splitter
                    if (rowGrid.ColumnDefinitions.Count == 1)
                    {
                        continue;
                    }

                    // Add GridSplitter to grid for possibility to change cells dimentions
                    GridSplitter gridSplitter = new GridSplitter();
                    gridSplitter.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
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

                // If first row, there's no need of horizontal splitter
                if (kioskWindowGrid.RowDefinitions.Count == 1)
                {
                    continue;
                }

                GridSplitter horGridSplitter = new GridSplitter();
                horGridSplitter.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
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

        private void mergeButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
