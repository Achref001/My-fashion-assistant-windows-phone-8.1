using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
//using Windows.Phone.UI.Input;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace My_Fashion_Assistant
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class All_Clothes : Page
    {
        public ObservableCollection<MyClothes> clothes = new ObservableCollection<MyClothes>();
        public ObservableCollection<MyClothesImages> clothesImages = new ObservableCollection<MyClothesImages>();
        List<MyClothes> allClothes = new List<MyClothes>();
        public SQLiteConnection conn;

        public All_Clothes()
        {
            this.InitializeComponent();
            loadURLS();
        }
        private async void loadURLS()
        {
            try
            {
                String DBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "My_Fashion_Assistant.db");
                conn = new SQLiteConnection(DBPath);


                var allData = conn.Table<MyClothes>();

                if (allData != null)
                {
                    foreach (MyClothes player in allData)
                    {
                        clothes.Add(player);
                    }
                }

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
        private void Clothes_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Clothes)); 
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            //    if (lst.SelectedItem == null)
            //    {
            //        MessageDialog message = new MessageDialog("Please select an item", "My fashion assistant");
            //        await message.ShowAsync();

            //    }
            //    else
            //    {
            //        MessageDialog md = new MessageDialog("Are you sure you want to delete this item ?", "Delete ?");
            //        md.Commands.Add(new UICommand("Yes", delegate(IUICommand command)
            //        {

            //            for (int i = 0; i < clothesImages.Count; i++)
            //            {
            //                try
            //                {
            //                    if (lst.SelectedItem == null)
            //                    {

            //                    }

            //                    if (clothesImages[i] == (MyClothesImages)lst.SelectedItem)
            //                    {
            //                        try
            //                        {
            //                            Debug.WriteLine("tfass5et");
            //                            conn.DeleteAsync(allClothes[i]);

            //                            clothesImages.Remove(clothesImages[i]);//test with success !
            //                        }
            //                        catch
            //                        {
            //                            Debug.WriteLine("unable to delate item ! 1 ");
            //                        }

            //                    }
            //                }
            //                catch
            //                {
            //                    Debug.WriteLine("unable to delate item ! 2");
            //                }
            //            }
            //        }));
            //        md.Commands.Add(new UICommand("No"));
            //        await md.ShowAsync();

            //    }
            //}
        }
    }
}
