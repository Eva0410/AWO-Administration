using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpticianMgr.Wpf
{
    public class SortManager
    {
        public List<GridViewColumnHeader> SortHeaders { get; set; }
        public List<GridViewColumnHeader> AllHeaders { get; set; }
        public SortManager()
        {
            SortHeaders = new List<GridViewColumnHeader>();
        }
        public void Init(RoutedEventArgs p)
        {
            AllHeaders = new List<GridViewColumnHeader>();
            ListView lv = p.Source as ListView;
            GridView gv = (GridView)lv.View;
            foreach (var item in gv.Columns)
            {
                AllHeaders.Add(item.Header as GridViewColumnHeader);
            }
        }
        //Click without shift key
        public void SortNormal(RoutedEventArgs e, ref ICollectionView View)
        {
            GridViewColumnHeader columnHeader = e.Source as GridViewColumnHeader;

            if (columnHeader == null)
            {
                return;
            }

            ListSortDirection dir;
            string header;

            //Same column pressed?
            if (SortHeaders.Count == 1 && SortHeaders[0] == columnHeader)
            {
                //Change sort direction
                dir = View.SortDescriptions[0].Direction;
                dir = dir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                header = ChangeArrow(columnHeader, dir, 0);
            }
            else
            {
                //Remove arrow from old column header
                if (SortHeaders.Count > 0)
                {
                    foreach (var item in SortHeaders)
                    {
                        item.Column.HeaderTemplate = null;
                        item.Column.Width = item.ActualWidth - 20;
                    }
                }
                SortHeaders.Clear();
                SortHeaders.Add(columnHeader);
                //default sort direction is ascending
                dir = ListSortDirection.Ascending;
                header = SetNewArrow(columnHeader, dir, 0);
            }
            View.SortDescriptions.Clear();

            //Exception for multiple bindings
            if (columnHeader.Content.ToString() == "Brillenfassung")
                header = "EyeGlassFrame.ModelDescription";
            View.SortDescriptions.Add(new SortDescription(header, dir));
        }
        private string SetNewArrow(GridViewColumnHeader column, ListSortDirection dir, int index)
        {
            //new column header must be wider
            column.Column.Width = column.ActualWidth + 20;

            return ChangeArrow(column, dir, index);
        }
        private string ChangeArrow(GridViewColumnHeader column, ListSortDirection dir, int index)
        {

            //insert arrow
            if (dir == ListSortDirection.Ascending)
            {
                switch(index)
                {
                    case 0:
                        column.Column.HeaderTemplate = Application.Current.FindResource("ArrowUp") as DataTemplate;
                        break;
                    case 1:
                        column.Column.HeaderTemplate = Application.Current.FindResource("ArrowUpOne") as DataTemplate;
                        break;
                    case 2:
                        column.Column.HeaderTemplate = Application.Current.FindResource("ArrowUpTwo") as DataTemplate;
                        break;
                }
                
            }
            else
            {
                switch(index)
                {
                    case 0:
                        column.Column.HeaderTemplate = Application.Current.FindResource("ArrowDown") as DataTemplate;
                        break;
                    case 1:
                        column.Column.HeaderTemplate = Application.Current.FindResource("ArrowDownOne") as DataTemplate;
                        break;
                    case 2:
                        column.Column.HeaderTemplate = Application.Current.FindResource("ArrowDownTwo") as DataTemplate;
                        break;
                }
            }

            string header = string.Empty;

            // get binding name for sort description 
            Binding b = column.Column.DisplayMemberBinding as Binding;
            if (b != null)
            {
                header = b.Path.Path;
            }
            return header;
        }
        //Click with shift
        public void SortShift(object p, ref ICollectionView View)
        {
            string columnName = p as string;
            string germanColumnName = TranslateEnglishToGerman(columnName);
            //exception for multiple binding
            if (germanColumnName == "Modell")
                germanColumnName = "Brillenfassung";
            //Get gridviewcolumnheader
            var columnHeader = AllHeaders.Where(h => String.Equals(h.Content.ToString(), germanColumnName)).ToList().FirstOrDefault();

            if (columnHeader == null)
            {
                return;
            }
            if (View.SortDescriptions.Count >= 1)
            {
                ListSortDirection dir;
                int index = View.SortDescriptions.Count - 1;
                //Change sorting direction
                if (View.SortDescriptions.Count == index + 1 && View.SortDescriptions[index].PropertyName == columnName)
                {
                    dir = View.SortDescriptions[index].Direction;
                    dir = dir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    View.SortDescriptions.RemoveAt(index);
                    View.SortDescriptions.Insert(index, new SortDescription(columnName, dir));
                    ChangeArrow(columnHeader, dir, index);
                    SortHeaders.Add(columnHeader);
                }
                else if (View.SortDescriptions.Count(s => s.PropertyName == columnName) == 0)
                {
                    if (View.SortDescriptions.Count >= 3)
                    {
                        MessageBox.Show("Sie können maximal nach drei Spalten sortieren!", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    dir = ListSortDirection.Ascending;
                    SetNewArrow(columnHeader, dir, index+1);
                    View.SortDescriptions.Add(new SortDescription(columnName, dir));
                    SortHeaders.Add(columnHeader);
                }
            }
        }
        private string TranslateEnglishToGerman(string englishName)
        {
            //Exception
            if(englishName == "Supplier.Name")
            {
                return "Lieferant";
            }
            if (englishName.Contains("."))
            {
                englishName = englishName.Split('.')[1];
            }
            string german = Properties.Resources.ResourceManager.GetString(englishName);
            german = german == "Ortsname" ? "Ort" : german;
            german = german == "Landname" ? "Land" : german;
            
            return german;
        }
    }
}
