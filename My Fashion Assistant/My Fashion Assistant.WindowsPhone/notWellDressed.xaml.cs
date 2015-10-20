using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
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
    public sealed partial class notWellDressed : Page
    {
        public List<MyClothesImages> selectedpics = new List<MyClothesImages>();
        BitmapImage bb = new BitmapImage();
        public notWellDressed()
        {
            this.InitializeComponent(); HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Storyboard1.Begin();

        }
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {

            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.Navigate(typeof(BlankPage1test));
            }


        }
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            selectedpics = e.Parameter as List<MyClothesImages>;
          
             result.Visibility = Visibility.Visible;


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
            //colorDetection();
        
        }
        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));

        }

        
    }
}
