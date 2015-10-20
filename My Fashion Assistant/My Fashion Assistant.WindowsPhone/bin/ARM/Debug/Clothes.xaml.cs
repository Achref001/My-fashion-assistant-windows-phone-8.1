using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
//using Windows.Media.Capture;
//using Windows.Storage;
//using Windows.UI.Xaml.Media.Imaging;
using SQLite;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Phone.UI.Input;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace My_Fashion_Assistant
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    // public sealed partial class Assistant : Page, IFileOpenPickerContinuable
    public sealed partial class Assistant : Page, IFileOpenPickerContinuable
    {

        // public SQLiteConnection conn;
        // public List<MyClothes> allTrophys;
        BitmapImage bitmapImage;
        String myPath;
         SQLiteAsyncConnection conn = new SQLiteAsyncConnection("MYFASHION.db");
         List<MyClothes> allClothe = new List<MyClothes>();
        public Assistant()
        {
            //String DBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "MYFASHION");
            //conn = new SQLiteConnection(DBPath);
            //conn.CreateTable<MyClothes>();



            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                frame.Navigate(typeof(MainPage));
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Create Db if not exist
            
            bool dbExist = await CheckDbAsync("MYFASHION.db");
            if (!dbExist)
            {
                await CreateDatabaseAsync();

            }

            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>



        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".png");
                // Launch file open picker and caller app is suspended 
                // and may be terminated if required
                openPicker.PickSingleFileAndContinue();
            }
            catch
            {
                Debug.WriteLine("enable to open file picker");
                Frame.Navigate(typeof(Assistant));
            }

        }


        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            try
            {


                StorageFile file = args.Files[0]; // get picked filename
                myPath = args.Files[0].Path.ToString(); // show filename
                //Debug.WriteLine(args.Files[0].Path);
                if (file != null)
                {
                    // Open a stream for the selected file.
                    IRandomAccessStream fileStream = await file.OpenReadAsync();

                    // Set the image source to the selected bitmap.
                    bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(fileStream);
                    imagePreivew.Source = bitmapImage;
                }
            }
            catch
            {
                Debug.WriteLine("enable to get content from the file picker");
               
                
            }

            //second method :

            //Windows.Storage.FileProperties.StorageItemThumbnail thumb = await args.Files[0].GetScaledImageAsThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.ListView, 49);
            //BitmapImage tmpbmp = new BitmapImage();
            //tmpbmp.SetSource(thumb);
            //imagePreivew.Source = tmpbmp;



        }

        //private void Back_Click(object sender, RoutedEventArgs e)
        //{
        //    Frame.Navigate(typeof(MainPage));

        //}

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (myPath == null)
            {
                MessageDialog dialog = new MessageDialog("please take a picture", "Warning");
                await dialog.ShowAsync();
            }
            else
            {
                
                MyClothes mClothes = new MyClothes()
                {
                    // the Id will be set by SQlite
                    nameClothes = txtClothesName.Text,
                    typeClothes = ((ComboBoxItem)cmbCathegory.SelectedItem).Content.ToString(),
                    imgPath = myPath,
                    season = ((ComboBoxItem)cmbSeason.SelectedItem).Content.ToString(),
                };
                Debug.WriteLine("pfff" + mClothes.typeClothes.ToString());
                // Add row to the User Table
                //verify if the name exist
                bool trouve = false;
                var query = conn.Table<MyClothes>();
                var allData = await query.ToListAsync();
                allClothe = allData;
                for (int i = 0; i < allClothe.Count; i++)
                {
                    if (txtClothesName.Text == allClothe[i].nameClothes && ((ComboBoxItem)cmbSeason.SelectedItem).Content.ToString() == allClothe[i].season && ((ComboBoxItem)cmbCathegory.SelectedItem).Content.ToString()== allClothe[i].typeClothes || myPath == allClothe[i].imgPath)
                    {
                        trouve = true;
                    }
                }
              
                    
              if(trouve == false)
              { 
                await conn.InsertAsync(mClothes);
                
                MessageDialog dialog = new MessageDialog("Content saved", "My Fashion assistant");
                await dialog.ShowAsync();
                    }
              else
              {
                  MessageDialog dialog = new MessageDialog("Item already exist \nYou have entred "+txtClothesName.Text+" before\nPlease choose another item", "My Fashion assistant");
                await dialog.ShowAsync();
              }
                
            }
        }
       


        #region SQLite utils
        private async Task<bool> CheckDbAsync(string dbName)
        {
            bool dbExist = true;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(dbName);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }

        private async Task CreateDatabaseAsync()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("MYFASHION.db");
            await conn.CreateTableAsync<MyClothes>();
        }






        #endregion SQLite utils

        private void txtClothesName_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            txtClothesName.Text = "";
        }

        private void imagePreivew_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            // Launch file open picker and caller app is suspended 
            // and may be terminated if required
            openPicker.PickSingleFileAndContinue();
        }

        private void allClothes_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AllClothes));
        }

        

       

       
    }
}
