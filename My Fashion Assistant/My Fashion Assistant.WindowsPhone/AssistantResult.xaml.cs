using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI;
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
    public sealed partial class AssistantResult : Page
    {
        public List<MyClothesImages> selectedpics = new List<MyClothesImages>();
        BitmapImage bb = new BitmapImage();
        private DataTransferManager dataTransferManager;

        public AssistantResult()
        {
            this.InitializeComponent();
          
            Storyboard1.Begin();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                frame.Navigate(typeof(BlankPage1test));
            }

            if (frame.CanGoBack)
            {
                frame.Navigate(typeof(BlankPage1test));
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
            selectedpics = e.Parameter as List<MyClothesImages>;
            if(selectedpics.Count>3)
            {
                result.Visibility = Visibility.Visible;
            }
            Debug.WriteLine("here is assistant result" + selectedpics.Count);
            for (int i = 0; i < selectedpics.Count; i++)
            //{
            //    Debug.WriteLine("i"+selectedpics[i].typeClothes);
            //}
            {
                if (selectedpics[i].typeClothes == "Upper Body")
                {
                    upperBodyPic.Source = selectedpics[i].myImage;
                }
                else if (selectedpics[i].typeClothes == "Lower Body")
                {
                    lowerBodyPic.Source = selectedpics[i].myImage;
                }
                else if (selectedpics[i].typeClothes == "Coat")
                {
                    CoatPic.Source = selectedpics[i].myImage;
                    Debug.WriteLine("ok" + i);
                }
                else if (selectedpics[i].typeClothes == "Dress")
                {
                    dressPic.Source = selectedpics[i].myImage;
                }
                else if (selectedpics[i].typeClothes == "Accessories")
                {
                    accessoriesPic.Source = selectedpics[i].myImage;
                }
                else if (selectedpics[i].typeClothes == "Shoes")
                {
                    ShoesPic.Source = selectedpics[i].myImage;
                }

            }
            mProgressRing.Visibility = Visibility.Collapsed;
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
            //colorDetection();
           

    }

        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unregister the current page as a share source.
            //this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
            this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
        }

             private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            // Call the scenario specific function to populate the datapackage with the data to be shared.
            if (GetShareContent(e.Request))
            {
                // Out of the datapackage properties, the title is required. If the scenario completed successfully, we need
                // to make sure the title is valid since the sample scenario gets the title from the user.
                if (String.IsNullOrEmpty(e.Request.Data.Properties.Title))
                {
                    e.Request.FailWithDisplayText("Enter the text you would like to share and try again.");///MainPage.MissingTitleError);
                }
            }
        }

        //private async void colorDetection()
        //{
           //int[] colorss = null;
           // for (int h = 0; h < selectedpics.Count; h++)
           // {

           //     //Get the color at each pixel
           //     colorss = new int[selectedpics[h].myImage.DecodePixelWidth * selectedpics[h].myImage.DecodePixelHeight];
                
           // }
           // for(int i=0 ; i<5;i++)
           // {
           //     Debug.WriteLine(colorss[i].ToString() + "color is");
           // }
           // for (int h = 0; h < selectedpics.Count; h++)
           //{
            //var pixelData = await .GetPixelDataAsync();
            //var pixels = selectedpics[h].myImage.DecodePixelType;

            //var width = selectedpics[h].myImage.DecodePixelWidth;
            //var height = selectedpics[h].myImage.DecodePixelHeight;

            //for (var i = 0; i < height; i++)
            //{
            //    for (var j = 0; j < width; j++)
            //    {
            //        byte r = pixels[(i * height + j) * 4 + 0]; //red
            //        byte g = pixels[(i * height + j) * 4 + 1]; //green
            //        byte b = pixels[(i * height + j) * 4 + 2]; //blue (rgba)
            //    }
            //}
        //}

        

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));

        }

       
        private void dressPic_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Share_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
        public bool GetShareContent(DataRequest request)
        {
            bool succeeded = false;

            string dataPackageText = "I started using my fashion assistant ! it's very useful and free \n ";
            if (!String.IsNullOrEmpty(dataPackageText))
            {
                DataPackage requestData = request.Data;
                requestData.Properties.Title = "My Fashion assistant";
                requestData.Properties.Description = "My Fashion assistant it's a free application that helps you to get better dressed"; // The description is optional.
                //requestData.Properties.ContentSourceApplicationLink = ApplicationLink;
                requestData.SetText(dataPackageText);
                succeeded = true;
            }
            else
            {
                request.FailWithDisplayText("Enter the text you would like to share and try again.");
            }
            return succeeded;
        }

       

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();

        }
    }
}
