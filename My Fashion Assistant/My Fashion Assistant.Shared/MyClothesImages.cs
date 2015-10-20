using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace My_Fashion_Assistant
{
    //we will use this class to have a list of images that we will Bind to our listView 
    public class MyClothesImages
    {
        public string nameClothes { get; set; }


        public string typeClothes { get; set; }
       
        public string season { get; set; }
        public BitmapImage myImage { get; set; }
    }

}
