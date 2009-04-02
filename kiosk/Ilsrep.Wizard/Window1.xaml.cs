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

namespace Ilsrep.Wizard
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void RefreshChildren()
		{
			int size = grid.RowDefinitions.Count * grid.ColumnDefinitions.Count;
			double width = grid.Width / grid.ColumnDefinitions.Count;
			double height = grid.Height / grid.RowDefinitions.Count;

			if (size == 0)
				return;

			grid.Children.Clear();

			for (int i = 0; i < grid.ColumnDefinitions.Count-1; ++i)
			{
				GridSplitter gs = new GridSplitter();
				gs.VerticalAlignment = VerticalAlignment.Stretch;
				gs.HorizontalAlignment = HorizontalAlignment.Right;
				gs.Width = 3;
				BrushConverter bc = new BrushConverter();
				gs.Background = (Brush)bc.ConvertFromString("#486896");

				grid.Children.Add(gs);
				Grid.SetColumn(gs, i);
				Grid.SetRowSpan(gs, grid.RowDefinitions.Count);
			}
			
			for (int i = 0; i < grid.RowDefinitions.Count-1; ++i)
			{
				GridSplitter gs = new GridSplitter();
				gs.VerticalAlignment = VerticalAlignment.Bottom;
				gs.HorizontalAlignment = HorizontalAlignment.Stretch;
				gs.Height = 3;
				BrushConverter bc = new BrushConverter();
				gs.Background = (Brush)bc.ConvertFromString("#486896");

				grid.Children.Add(gs);
				Grid.SetRow(gs, i);
				Grid.SetColumnSpan(gs, grid.ColumnDefinitions.Count);
			}
		}

		private void rows_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (txtRows.Text == "")
				return;

			int rows = Convert.ToInt32( txtRows.Text );

			while (grid.RowDefinitions.Count < rows)
			{
				RowDefinition rd = new RowDefinition();
				grid.RowDefinitions.Add(rd);
			}

			while (grid.RowDefinitions.Count > rows)
				grid.RowDefinitions.RemoveAt(0);

			RefreshChildren();
		}

		private void columns_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (txtColumns.Text == "")
				return;

			int columns = Convert.ToInt32(txtColumns.Text);

			while (grid.ColumnDefinitions.Count < columns)
			{
				ColumnDefinition cd = new ColumnDefinition();
				grid.ColumnDefinitions.Add(cd);
			}

			while (grid.ColumnDefinitions.Count > columns)
				grid.ColumnDefinitions.RemoveAt(0);

			RefreshChildren();
		}
	}
}
