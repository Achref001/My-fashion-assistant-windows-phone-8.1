using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using SQLite;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace My_Fashion_Assistant
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllClothes : Page
    {
        public ObservableCollection<MyClothesImages> clothesImages = new ObservableCollection<MyClothesImages>();
        List<MyClothes> allClothes = new List<MyClothes>();
        List<MyClothesImages> selectedItems = new List<MyClothesImages>();
        SQLiteAsyncConnection conn = new SQLiteAsyncConnection("MYFASHION.db");
        public AllClothes()
        {
            this.InitializeComponent();

            loadURLS();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;


        }
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {

            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                frame.Navigate(typeof(Assistant));
            }

            if (frame.CanGoBack)
            {
                frame.Navigate(typeof(Assistant));
                e.Handled = true;
            }



        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }




        private async void loadURLS()
        {
            try
            {

                var query = conn.Table<MyClothes>();
                var allData = await query.ToListAsync();


                allClothes = allData;

                for (int i = 0; i < allClothes.Count; i++)
                {
                    try
                    {
                        StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(allClothes[i].imgPath);

                        if (file != null)
                        {
                            IRandomAccessStream fileStream = await file.OpenReadAsync();
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(fileStream);
                            bitmapImage.DecodePixelType = DecodePixelType.Physical;
                            bitmapImage.CreateOptions = BitmapCreateOptions.None;
                            bitmapImage.DecodePixelHeight = 50;
                            bitmapImage.DecodePixelWidth = 50;
                            MyClothesImages mci = new MyClothesImages();
                            mci.nameClothes = allClothes[i].nameClothes;
                            mci.season = allClothes[i].season;
                            mci.myImage = bitmapImage;
                            mci.typeClothes = allClothes[i].typeClothes;
                            clothesImages.Add(mci);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Exception accured while trying to conver the image path to a BitmapImage");
                    }
                }



                lst.DataContext = clothesImages;
                mProgressRing.Visibility = Visibility.Collapsed;
                lst.Visibility = Visibility.Visible;


            }
            catch
            {
                Debug.WriteLine("not ok !");
            }

        }






        private void StackPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            try
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);

                flyoutBase.ShowAt(senderElement);
            }
            catch
            {
                Debug.WriteLine("error while holding");
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            bool trouve = false;
            int j = 0;
            MyClothesImages selectedOne = null;
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            if (element.DataContext != null && element.DataContext is MyClothesImages)
            {
                selectedOne = (MyClothesImages)element.DataContext;
                // rest of the code
            }
            //Debug.WriteLine(lst..toString())
            for (int i = 0; i < clothesImages.Count; i++)
            {


                if (clothesImages[i] == selectedOne)
                {
                    trouve = true;
                    j = i;
                }
            }
            if (trouve == true)
            {
                try
                {
                    Debug.WriteLine("tfass5et");

                    //remove from database
                    for (int gass = 0; gass < allClothes.Count; gass++ )
                    {
                        if (selectedOne.nameClothes == allClothes[gass].nameClothes && selectedOne.season == allClothes[gass].season && selectedOne.typeClothes == allClothes[gass].typeClothes)
                            conn.DeleteAsync(allClothes[gass]);
                    }
                       
                    //remove from current list
                    clothesImages.Remove(selectedOne);

                }
                catch
                {
                    Debug.WriteLine("unable to delate item ! 1 ");
                }

            }
        }






    }
}
