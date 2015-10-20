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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace My_Fashion_Assistant
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1test : Page
    {



        String comingWeather = null;
        
        ;
        public ObservableCollection<MyClothesImages> clothesImagesLower = new ObservableCollection<MyClothesImages>();
        public ObservableCollection<MyClothesImages> clothesImagesCoat = new ObservableCollection<MyClothesImages>();
        public ObservableCollection<MyClothesImages> clothesImagesShoes = new ObservableCollection<MyClothesImages>();
        public ObservableCollection<MyClothesImages> clothesImagesDress = new ObservableCollection<MyClothesImages>();
        public ObservableCollection<MyClothesImages> clothesImagesAccessories = new ObservableCollection<MyClothesImages>();

        //List<MyClothesImages> selectedItemsUpper = new List<MyClothesImages>();
        //List<MyClothesImages> selectedItemsLower = new List<MyClothesImages>();
        //List<MyClothesImages> selectedItemsCoat = new List<MyClothesImages>();
        //List<MyClothesImages> selectedItemsShoes = new List<MyClothesImages>();
        //List<MyClothesImages> selectedItemsDress = new List<MyClothesImages>();
        //List<MyClothesImages> selectedItemsAccessories = new List<MyClothesImages>();

        
        ListView lstUpper;
        ListView lstLower;
        ListView lstCoat;
        ListView lstShoes;
        ListView lstDress;
        ListView lstAccessories;


        public List<MyClothesImages> choosenPictures = new List<MyClothesImages>();
        public BlankPage1test()
        {
            
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            bool dbExist = await CheckDbAsync("MYFASHION.db");
            if (!dbExist)
            {
                await CreateDatabaseAsync();

            }
            comingWeather = e.Parameter as string;
            loadURLS();
       
            

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
     

        private async void loadURLS()
        {
            try
            {
                SQLiteAsyncConnection conn = new SQLiteAsyncConnection("MYFASHION.db");
                var query = conn.Table<MyClothes>();
                var allData = await query.ToListAsync();
                List<MyClothes> allClothes = new List<MyClothes>();
                List<MyClothes> uppers = new List<MyClothes>();
                List<MyClothes> lowers = new List<MyClothes>();
                List<MyClothes> shoess = new List<MyClothes>();
                List<MyClothes> dresses = new List<MyClothes>();
                List<MyClothes> coats = new List<MyClothes>();
                List<MyClothes> accessoriess = new List<MyClothes>();

                allClothes = allData;
                int i ;
                
                for (i = 0; i < allClothes.Count; i++)
                {

                    if (allClothes[i].typeClothes == "Lower Body")
                    {
                        lowers.Add(allClothes[i]);
                    }
                    else if (allClothes[i].typeClothes == "Upper Body")
                    {
                        uppers.Add(allClothes[i]);
                    }
                    else if (allClothes[i].typeClothes == "Shoes")
                    {
                        shoess.Add(allClothes[i]);
                    }
                    else if (allClothes[i].typeClothes == "Dress")
                    {
                        dresses.Add(allClothes[i]);
                    }
                    else if (allClothes[i].typeClothes == "Coat")
                    {
                        coats.Add(allClothes[i]);
                    }
                    else if (allClothes[i].typeClothes == "Accessories")
                    {
                        accessoriess.Add(allClothes[i]);
                    }
                }
                //upper

                for (i = 0; i < uppers.Count; i++)
                {
                    try
                    {
                        StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(uppers[i].imgPath);
                        Debug.WriteLine("image loaded with success");
                        if (file != null)
                        {
                            Debug.WriteLine("file not null");
                            IRandomAccessStream fileStream = await file.OpenReadAsync();
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(fileStream);
                            bitmapImage.DecodePixelType = DecodePixelType.Logical;
                            bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmapImage.DecodePixelHeight = 70;
                            bitmapImage.DecodePixelWidth = 70;
                            MyClothesImages mci = new MyClothesImages();
                            mci.nameClothes = uppers[i].nameClothes;
                            mci.season = uppers[i].season;
                            mci.typeClothes = uppers[i].typeClothes;
                            mci.myImage = bitmapImage;
                            clothesImagesUpper.Add(mci);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Exception accured while trying to conver the image path to a BitmapImage");
                    }
                }

                //lower
                for (i = 0; i < lowers.Count; i++)
                {
                    try
                    {
                        StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(lowers[i].imgPath);
                        Debug.WriteLine("image loaded with success");
                        if (file != null)
                        {
                            Debug.WriteLine("file not null");
                            IRandomAccessStream fileStream = await file.OpenReadAsync();
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(fileStream);
                            bitmapImage.DecodePixelType = DecodePixelType.Logical;
                            bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmapImage.DecodePixelHeight = 70;
                            bitmapImage.DecodePixelWidth = 70;
                            MyClothesImages mci = new MyClothesImages();
                            mci.nameClothes = lowers[i].nameClothes;
                            mci.season = lowers[i].season;
                            mci.typeClothes = lowers[i].typeClothes;
                            mci.myImage = bitmapImage;
                            clothesImagesLower.Add(mci);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Exception accured while trying to conver the image path to a BitmapImage");
                    }
                }
                    
                    //shoesess
                    for (i = 0; i < shoess.Count; i++)
                {
                    try
                    {
                        StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(shoess[i].imgPath);
                        Debug.WriteLine("image loaded with success");
                        if (file != null)
                        {
                            Debug.WriteLine("file not null");
                            IRandomAccessStream fileStream = await file.OpenReadAsync();
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(fileStream);
                            bitmapImage.DecodePixelType = DecodePixelType.Logical;
                            bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmapImage.DecodePixelHeight = 70;
                            bitmapImage.DecodePixelWidth = 70;
                            MyClothesImages mci = new MyClothesImages();
                            mci.nameClothes = shoess[i].nameClothes;
                            mci.season = shoess[i].season;
                            mci.typeClothes = shoess[i].typeClothes;
                            mci.myImage = bitmapImage;
                            clothesImagesShoes.Add(mci);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Exception accured while trying to conver the image path to a BitmapImage");
                    }
                    }
                        //dresses
                  for (i = 0; i < dresses.Count; i++)
                {
                    try
                    {
                        StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(dresses[i].imgPath);
                        Debug.WriteLine("image loaded with success");
                        if (file != null)
                        {
                            Debug.WriteLine("file not null");
                            IRandomAccessStream fileStream = await file.OpenReadAsync();
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(fileStream);
                            bitmapImage.DecodePixelType = DecodePixelType.Logical;
                            bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmapImage.DecodePixelHeight = 70;
                            bitmapImage.DecodePixelWidth = 70;
                            MyClothesImages mci = new MyClothesImages();
                            mci.nameClothes = dresses[i].nameClothes;
                            mci.season = dresses[i].season;
                            mci.typeClothes = dresses[i].typeClothes;
                            mci.myImage = bitmapImage;
                            clothesImagesDress.Add(mci);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Exception accured while trying to conver the image path to a BitmapImage");
                    }
                  }
                      //coats
                  for (i = 0; i < coats.Count; i++)
                  {
                      try
                      {
                          StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(coats[i].imgPath);
                          Debug.WriteLine("image coat loaded with success");
                          if (file != null)
                          {
                              Debug.WriteLine("file coat not null");
                              IRandomAccessStream fileStream = await file.OpenReadAsync();
                              BitmapImage bitmapImage = new BitmapImage();
                              bitmapImage.SetSource(fileStream);
                              bitmapImage.DecodePixelType = DecodePixelType.Logical;
                              bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                              bitmapImage.DecodePixelHeight = 70;
                              bitmapImage.DecodePixelWidth = 70;
                              MyClothesImages mci = new MyClothesImages();
                              mci.nameClothes = coats[i].nameClothes;
                              mci.season = coats[i].season;
                              mci.typeClothes = coats[i].typeClothes;
                              mci.myImage = bitmapImage;
                              clothesImagesCoat.Add(mci);
                          }
                      }
                      catch
                      {
                          Debug.WriteLine("Exception accured while trying to conver the image path to a BitmapImage");
                      }
                  }
                          //accessories
                 for (i = 0; i < accessoriess.Count; i++)
                {
                    try
                    {
                        StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(accessoriess[i].imgPath);
                        Debug.WriteLine("image loaded with success");
                        if (file != null)
                        {
                            Debug.WriteLine("file not null");
                            IRandomAccessStream fileStream = await file.OpenReadAsync();
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(fileStream);
                            bitmapImage.DecodePixelType = DecodePixelType.Logical;
                            bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            bitmapImage.DecodePixelHeight = 70;
                            bitmapImage.DecodePixelWidth = 70;
                            MyClothesImages mci = new MyClothesImages();
                            mci.nameClothes = accessoriess[i].nameClothes;
                            mci.season = accessoriess[i].season;
                            mci.typeClothes = accessoriess[i].typeClothes;
                            mci.myImage = bitmapImage;
                            clothesImagesAccessories.Add(mci);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Exception accured while trying to conver the image path to a BitmapImage");
                    }
                             }



                }
            catch
            {
                Debug.WriteLine("something went wrong");
            }

            myHub.Visibility = Visibility.Visible;
            Debug.WriteLine("clothes Img upper count "+clothesImagesUpper.Count);
            Debug.WriteLine("clothes Img lower count " + clothesImagesLower.Count);
            Debug.WriteLine("clothes Img dress count " + clothesImagesDress.Count);
            Debug.WriteLine("clothes Img coat count " + clothesImagesCoat.Count);
            Debug.WriteLine("clothes Img accessories count " + clothesImagesAccessories.Count);
            Debug.WriteLine("clothes Img shoes count " + clothesImagesShoes.Count);
            mPogressRing.Visibility = Visibility.Collapsed;
            if (clothesImagesCoat.Count > 0)
            {
                try
                { 
                Debug.WriteLine("coat dataCOntext");
                lstCoat.DataContext = clothesImagesCoat;
                    }
                catch
                {
                    Debug.WriteLine("coat is not good");
                }
            }
            if (clothesImagesUpper.Count > 0)
            {
                try
                {
                    Debug.WriteLine("Upper dataCOntext");
                    lstUpper.DataContext = clothesImagesUpper;
                }
                catch
                {
                    Debug.WriteLine("upper is not good");
                }
            }

            if (clothesImagesDress.Count > 0)
            {
                try
                {
                    Debug.WriteLine("dress dataCOntext");
                    lstDress.DataContext = clothesImagesDress;
                }
                catch
                {
                    Debug.WriteLine("dress is not good");
                }
            }
            

            
            
           
            

      }
            


        

        //private async void loadURLSUpper()
        //{
        //    try
        //    {
        //        var query = conn.Table<MyClothes>();
        //        var allData = await query.ToListAsync();
        //        List<MyClothes> allClothes = new List<MyClothes>();
        //        List<MyClothes> uppers = new List<MyClothes>();
        //        allClothes = allData;
        //        for (int i = 0; i < allClothes.Count; i++)
        //        {

        //            if (allClothes[i].typeClothes == "Upper Body")
        //            {
        //                uppers.Add(allClothes[i]);
        //            }
        //        }
        //        for (int i = 0; i < uppers.Count; i++)
        //        {
        //            try
        //            {
        //                StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(uppers[i].imgPath);
        //                Debug.WriteLine("image loaded with success");
        //                if (file != null)
        //                {
        //                    Debug.WriteLine("file not null");
        //                    IRandomAccessStream fileStream = await file.OpenReadAsync();
        //                    BitmapImage bitmapImage = new BitmapImage();
        //                    bitmapImage.SetSource(fileStream);
        //                    bitmapImage.DecodePixelType = DecodePixelType.Logical;
        //                    bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        //                    bitmapImage.DecodePixelHeight = 50;
        //                    bitmapImage.DecodePixelWidth = 50;
        //                    MyClothesImages mci = new MyClothesImages();
        //                    mci.nameClothes = uppers[i].nameClothes;
        //                    mci.season = uppers[i].season;
        //                    mci.myImage = bitmapImage;
        //                    clothesImagesUpper.Add(mci);
        //                }
        //            }
        //            catch
        //            {
        //                Debug.WriteLine("Exception accured while trying to conver the image path to a BitmapImage");
        //            }
        //        }
        //        Debug.WriteLine("clothes Images count is " + clothesImagesUpper.Count);
        //        lstUpper.DataContext = clothesImagesUpper;
        //        Debug.WriteLine("list binded");
        //    }
        //    catch
        //    {
        //        Debug.WriteLine("not ok Upper!");
        //    }

        //}






        //private void lst_ItemClick_1(object sender, ItemClickEventArgs e)
        //{
        //    String s = ((MyClothesImages)e.ClickedItem).nameClothes;
        //    Debug.WriteLine(s);
        //    string h = ((MyClothesImages)e.ClickedItem).season;
        //    Debug.WriteLine("season is" + h);

        //}



        //selection changed event
        private async void lst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //upper body selection changed 
            //Debug.WriteLine("selected item" + ((MyClothesImages)lstUpper.SelectedItem).nameClothes);
            //selectedItemsUpper.Add((MyClothesImages)lstUpper.SelectedItem);
            //Frame.Navigate(typeof(LowerBody), selectedItemsUpper);
             MessageDialog md = new MessageDialog("would you want to add this item to your todays look ?", "My Fashion Assistant");
                md.Commands.Add(new UICommand("Yes", delegate(IUICommand command)
                    {
            choosenPictures.Add((MyClothesImages)lstUpper.SelectedItem);
                    }));
                md.Commands.Add(new UICommand("No"));
                await md.ShowAsync();
        }
        private async void lstCoat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             MessageDialog md = new MessageDialog("would you want to add this item to your todays look ?", "My Fashion Assistant");
             md.Commands.Add(new UICommand("Yes", delegate(IUICommand command)
                 {
                     choosenPictures.Add((MyClothesImages)lstCoat.SelectedItem);
                 }));
             md.Commands.Add(new UICommand("No"));
             await md.ShowAsync();
        }

        private async void lstDress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageDialog md = new MessageDialog("would you want to add this item to your todays look ?", "My Fashion Assistant");
             md.Commands.Add(new UICommand("Yes", delegate(IUICommand command)
                 {
            choosenPictures.Add((MyClothesImages)lstDress.SelectedItem);
                 }));
             md.Commands.Add(new UICommand("No"));
             await md.ShowAsync();
        }
        private async void lstAccessories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageDialog md = new MessageDialog("would you want to add this item to your todays look ?", "My Fashion Assistant");
             md.Commands.Add(new UICommand("Yes", delegate(IUICommand command)
                 {
            choosenPictures.Add((MyClothesImages)lstAccessories.SelectedItem);
                 }));
             md.Commands.Add(new UICommand("No"));
             await md.ShowAsync();
        }

        private async void lstShoes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageDialog md = new MessageDialog("would you want to add this item to your todays look ?", "My Fashion Assistant");
             md.Commands.Add(new UICommand("Yes", delegate(IUICommand command)

                 {
            choosenPictures.Add((MyClothesImages)lstShoes.SelectedItem);
                 }));
             md.Commands.Add(new UICommand("No"));
             await md.ShowAsync();
        }
        private async void lstlower_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageDialog md = new MessageDialog("would you want to add this item to your todays look ?", "My Fashion Assistant");
             md.Commands.Add(new UICommand("Yes", delegate(IUICommand command)

                 {
            choosenPictures.Add((MyClothesImages)lstLower.SelectedItem);
                 }));
             md.Commands.Add(new UICommand("No"));
             await md.ShowAsync();
        }

        //hub specifications :
      
        //handle the load event 
        private void lstLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("upper list loaded with success !");
            lstUpper = (ListView)sender;
           
        }

        private void lstLower_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("lower list loaded with success !");
            lstLower = (ListView)sender;
            if (clothesImagesLower.Count > 0)
            {
                Debug.WriteLine("lower dataCOntext");
                lstLower.DataContext = clothesImagesLower;
            }
        }

        
        private void lstCoatLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("coat list loaded with success !");
            lstCoat = (ListView)sender;
            
        }

        

        private void lstDressLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("dress list loaded with success !");
            lstDress = (ListView)sender;
            
        }

        private void lstAccessoriesLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("accessories list loaded with success !");
            lstAccessories = (ListView)sender;
            if (clothesImagesAccessories.Count > 0)
            {
                Debug.WriteLine("accessories dataCOntext");
                lstAccessories.DataContext = clothesImagesAccessories;
            }
        }

       
        private void lstShoesLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("shoes list loaded with success !");
            lstShoes = (ListView)sender;
            if (clothesImagesShoes.Count > 0)
            {
                Debug.WriteLine("shoes dataCOntext");
                lstShoes.DataContext = clothesImagesShoes;
            }
        }

        private void myHub_Loaded(object sender, RoutedEventArgs e)
        {
            myHub.Visibility = Visibility.Visible;
        }

        private async void AssistantResult_Click(object sender, RoutedEventArgs e)
        {
            float todayWeather= 0;
            try
            {
                todayWeather = Single.Parse(comingWeather);
            }
            catch
            {
                Debug.WriteLine("there is no connection");
            }
            try
                {
            //Debug.WriteLine(comingWeather);
            for (int i = 0; i < choosenPictures.Count; i++)
            {

                for(int j= 0 ; j<choosenPictures.Count ;j++)
                {
                    if(choosenPictures[i].season != choosenPictures[j].season)
                    {
                        Frame.Navigate(typeof(notWellDressed), choosenPictures);
    
                    }
                }
                if(todayWeather>24&&( choosenPictures[i].season.Equals("autumn")|| choosenPictures[i].season.Equals("winter")))
                {
                    Frame.Navigate(typeof(notWellDressed), choosenPictures);
                }
                else if(todayWeather<10&&( choosenPictures[i].season.Equals("summer")|| choosenPictures[i].season.Equals("winter")))
                {
                    Frame.Navigate(typeof(notWellDressed), choosenPictures);
                }
              

                
            }

                }
                catch
                {
                    showMD();
                }
            Frame.Navigate(typeof(AssistantResult),choosenPictures);
        }

        private async void showMD()
        {
            MessageDialog md = new MessageDialog("Please verify your internet connection", "My Fashion Assistant");
            md.Commands.Add(new UICommand("Close", delegate(IUICommand command)
            {
                Frame.Navigate(typeof(MainPage));
            }));
            
            await md.ShowAsync();
        }


        

       

        
    }
}
