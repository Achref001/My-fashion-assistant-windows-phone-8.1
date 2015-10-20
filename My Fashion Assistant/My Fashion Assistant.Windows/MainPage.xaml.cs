using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
//using Windows.Phone.UI.Input;
using Windows.UI.Popups;
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
    public sealed partial class MainPage : Page
    {
        string mlatitude;
        string mlongitude;
        public static MainPage Current;
        City city = new City();
        [DllImport("wininet.dll")]
        extern static bool InternetGetConnectedState(int connectionDescription, int reservedValue);
        public MainPage()
        {
            this.InitializeComponent();
            localiseMe();
            Current = this;

            this.NavigationCacheMode = NavigationCacheMode.Required;
            //HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }
        //void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        //{
        //    if (Frame.CanGoBack)
        //    {
        //        e.Handled = true;
        //        Frame.GoBack();
        //    }
        //}



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (IsConnected() == false)
            {
                MessageDialog dialog = new MessageDialog("Sorry no internet connection !", "Warning");
                await dialog.ShowAsync();
                NoDataFound.Visibility = Visibility.Visible;

            }

        }

        static bool IsConnected()
        {

            int connectionDescription = 0;
            return InternetGetConnectedState(connectionDescription, 0);
        }

        private async void localiseMe()
        {

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                mlatitude = geoposition.Coordinate.Latitude.ToString("0.00");
                mlongitude = geoposition.Coordinate.Longitude.ToString("0.00");
                Debug.WriteLine("this is the latitude" + mlatitude);
                Debug.WriteLine("this is the longitude" + mlongitude);


            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    textCity.Text = "location  is disabled in phone settings.";
                }
                //else
                {
                    // something else happened acquring the location
                }
            }
            loadWeather();


        }
        private async Task loadWeather()
        {


            List<Weather> weathers;
            String s1 = "http://api.openweathermap.org/data/2.5/forecast/daily?lat=" + mlatitude + "&lon=" + mlongitude + "&mode=xml&units=metric&cnt=9";
            Debug.WriteLine("the link that i work with for ArianaTn is" + s1);
            var uri = new Uri(s1);
            var httpClient = new HttpClient();
            try
            {

                var result = await httpClient.GetAsync(uri);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    String xmlContent = await result.Content.ReadAsStringAsync();
                    XDocument loadedData = XDocument.Parse(xmlContent);

                    var data = (from query in loadedData.Descendants("weatherdata")
                                select new City
                                {
                                    cityName = (String)query.Element("location").Element("name"),
                                    country = (String)query.Element("location").Element("country"),
                                    description = (String)query.Element("forecast").Element("time").Element("symbol").Attribute("name"),
                                    humidity = (String)query.Element("forecast").Element("time").Element("humidity").Attribute("value"),
                                    pressure = (String)query.Element("forecast").Element("time").Element("pressure").Attribute("value"),
                                }


                                    ).Single();



                    city = data;

                    textCity.Text = city.cityName + "," + city.country;
                    //textDescription.Text = city.description;
                    textHumidity.Text = city.humidity + "%";
                    textPressure.Text = city.pressure + " hPa";
                    //Source="Assets/snow.png"
                    //Debug.WriteLine(city.description+"city description");




                }
            }
            catch { }
            try
            {

                var result = await httpClient.GetAsync(uri);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    String xmlContent = await result.Content.ReadAsStringAsync();
                    XDocument loadedData = XDocument.Parse(xmlContent);

                    var data1 = (from query in loadedData.Descendants("time")
                                 select new Weather
                                 {

                                     Date = (DateTime)query.Attribute("day"),
                                     temperature = (String)query.Element("temperature").Attribute("day"),

                                     // temperatureValue = (String)query.Element("temperature").Attribute("unit")
                                     //cloudValue = (Int32)query.Element("clouds").Attribute("")
                                     //humidity=(String)query.Element("humidity").Attribute("value"),
                                     // wind=(String)query.Element("wind").Element("speed").Attribute("value"),
                                     // windDirection =(String)query.Element("wind").Element("direction").Attribute("name"),
                                     // cloudName = (String)query.Element("clouds").Attribute("name"),
                                     // lastUpdate = (String)query.Element("lastupdate").Attribute("value")
                                 }


                                    ).ToList();


                    weathers = new List<Weather>();

                    foreach (Weather s in data1)
                    {
                        weathers.Add(s);
                    }
                    changeWeatherIcon();
                    textWeather.Text = weathers[0].temperature;

                    day1.Text = weathers[1].temperature;
                    day1Name.Text = weathers[1].Date.ToString("dddd");

                    day2.Text = weathers[2].temperature;
                    day2Name.Text = weathers[2].Date.ToString("dddd");

                    day3.Text = weathers[3].temperature;
                    day3Name.Text = weathers[3].Date.ToString("dddd");

                    day4.Text = weathers[4].temperature;
                    day4Name.Text = weathers[4].Date.ToString("dddd");

                    day5.Text = weathers[5].temperature;
                    day5Name.Text = weathers[5].Date.ToString("dddd");

                    day6.Text = weathers[6].temperature;
                    day6Name.Text = weathers[6].Date.ToString("dddd");

                    day7.Text = weathers[7].temperature;
                    day7Name.Text = weathers[7].Date.ToString("dddd");

                    day8.Text = weathers[8].temperature;
                    day8Name.Text = weathers[8].Date.ToString("dddd");
                    mProgressRing.Visibility = Visibility.Collapsed;
                    NoDataFound.Visibility = Visibility.Collapsed;
                    imageHumidity.Visibility = Visibility.Visible;
                    imageWeather.Visibility = Visibility.Visible;
                    imagePressure.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        private void changeWeatherIcon()
        {
            try
            {
                Debug.WriteLine("here is change weather icon :D" + city.description.ToString());
                if (city.description.ToString() == "sky is clear")
                {
                    Debug.WriteLine("ffffffffffine!!");
                    //imageWeather.Source = "Assets/snow.png";
                    imageWeather.Source = new BitmapImage(new Uri(this.BaseUri, @"Assets/clearSky.png"));



                }
                else if (city.description.ToString() == "light rain")
                {
                    Debug.WriteLine("ffffffffffine!!");
                    //imageWeather.Source = "Assets/snow.png";
                    imageWeather.Source = new BitmapImage(new Uri(this.BaseUri, @"Assets/rain.png"));



                }
                else if (city.description.ToString() == "moderate rain")
                {
                    Debug.WriteLine("ffffffffffine!!");
                    //imageWeather.Source = "Assets/snow.png";
                    imageWeather.Source = new BitmapImage(new Uri(this.BaseUri, @"Assets/showerRain.png"));



                }
                else if (city.description.ToString() == "Broken Clouds")
                {
                    Debug.WriteLine("ffffffffffine!!");
                    //imageWeather.Source = "Assets/snow.png";
                    imageWeather.Source = new BitmapImage(new Uri(this.BaseUri, @"Assets/brokenClouds.png"));



                }
                else if (city.description.ToString() == "Snow")
                {
                    Debug.WriteLine("ffffffffffine!!");
                    //imageWeather.Source = "Assets/snow.png";
                    imageWeather.Source = new BitmapImage(new Uri(this.BaseUri, @"Assets/snow.png"));



                }
            }
            catch
            {
                Debug.WriteLine("icon");
            }


        }

        private void Assistant_Click(object sender, RoutedEventArgs e)
        {

            //Frame.Navigate(typeof(BlankPage1test), textWeather.Text.ToString());
        }

        private void closet_ClickBack(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Clothes));
        }

        //private void upperbody(object sender, RoutedEventArgs e)
        //{
        //    Frame.Navigate(typeof(Dress));
        //}
    }
}
