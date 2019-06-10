using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sqlite_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            listViewItems.ItemsSource = DataAccess.GetData(); // Add data to listview on startup
            txtBoxInput.FontFamily = new FontFamily("Helvetica");
        }

        private async void appBarBtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (txtBoxInput.Text == "")
            {

                var messageDialog = new MessageDialog("ERROR: You must enter in a value to add to the database.", "NULL ERROR");
                await messageDialog.ShowAsync();
            }
            else
            {
                // Add item to database by button click
                DataAccess.AddData(txtBoxInput.Text);
                listViewItems.ItemsSource = DataAccess.GetData();
                txtBoxInput.Text = "";
            }
        }

        private async void appBarBtnDeleteItems_Click(object sender, RoutedEventArgs e)
        {
            if (listViewItems.Items.Count == 0) // Empty database
            {
                var messageDialog = new MessageDialog("No items to delete. The list is empty.", "ERROR: Empty list");
                await messageDialog.ShowAsync();
            }
            else
            {
                // Delete selected items
                var copyOfSelectedItems = listViewItems.SelectedItems.ToArray();
                DataAccess.DeleteItem(copyOfSelectedItems);
                listViewItems.ItemsSource = DataAccess.GetData();

                // Covers when the user presses select all to delete all items
                if (btnToggleSelectAll.IsChecked == true)
                    btnToggleSelectAll.IsChecked = false;
            }
        }

        private void txtBoxInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Add item to database by Enter key
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // Avoid empty items
                if (txtBoxInput.Text == "")
                    return;
                else
                {
                    DataAccess.AddData(txtBoxInput.Text);
                    listViewItems.ItemsSource = DataAccess.GetData();
                    txtBoxInput.Text = "";
                }
            }
        }

        private void listViewItems_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Press Ctrl+A while listview is in focus to select all
            if (e.Key == Windows.System.VirtualKey.Control && e.Key == Windows.System.VirtualKey.A)
                listViewItems.SelectAll();
        }

        private async void appBarBtnUpdateItem_Click(object sender, RoutedEventArgs e)
        {
            if (listViewItems.SelectedItems.Count > 1)
            {
                listViewItems.SelectedItem = null; // Deslect Items
                var messageDialog = new MessageDialog("NOTE: Only one item can be updated at a time.", "ERROR");
                await messageDialog.ShowAsync();
            }
            else if (listViewItems.SelectedItems.Count == 0)
                return;
            else
            {
                // Database Item ID (listview starts at index 0)
                var id = listViewItems.SelectedIndex + 1;

                // Textbox to add to content dialog
                var txtBoxUpdateItem = new TextBox
                {
                    Header = "Update database item:",
                    Text = listViewItems.SelectedItem.ToString()
                };
                txtBoxUpdateItem.Select(txtBoxUpdateItem.Text.Length, 0); // Set cursor to end of txtBox text

                // Dialog to update database item
                ContentDialog contentDialogUpdateItem = new ContentDialog()
                {
                    Title = "Update Database Item",
                    Content = txtBoxUpdateItem,
                    PrimaryButtonText = "Update",
                    SecondaryButtonText = "Cancel"
                };

                var result = await contentDialogUpdateItem.ShowAsync();
                if (result == ContentDialogResult.Primary) // Update Item
                {
                    DataAccess.UpdateItem(id, txtBoxUpdateItem.Text);
                    listViewItems.ItemsSource = DataAccess.GetData();
                }
                else // Cancel
                    return;
            }
        }

        private void btnToggleSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (listViewItems.Items.Count == 0)
                btnToggleSelectAll.IsChecked = false;
            else
                listViewItems.SelectAll();
        }

        private void btnToggleSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            listViewItems.SelectedItem = null; // Deselect
        }
    }
}
