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
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
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
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace My_Fashion_Assistant
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Clothes : Page
    {
        public SQLiteConnection conn;
        BitmapImage bitmapCamera;
        string path = "";
        public Clothes()
        {
            String DBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "My_Fashion_Assistant.db");
            conn = new SQLiteConnection(DBPath);
            conn.CreateTable<MyClothes>();
            this.InitializeComponent();
        }

        private  void Camera_Click(object sender, RoutedEventArgs e)
        {
            openMyCamera();
        }

        private async void openMyCamera()
        {
            CameraCaptureUI cameraUI = new CameraCaptureUI();

            cameraUI.PhotoSettings.AllowCropping = false;
            cameraUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.MediumXga;

            Windows.Storage.StorageFile capturedMedia =
                await cameraUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (capturedMedia != null)
            {
                using (var streamCamera = await capturedMedia.OpenAsync(FileAccessMode.Read))
                {

                    bitmapCamera = new BitmapImage();
                    bitmapCamera.SetSource(streamCamera);
                    //To display the image in a XAML image object, do this:
                    imagePreivew.Source = bitmapCamera;

                    // Convert the camera bitap to a WriteableBitmap object, 
                    // which is often a more useful format.

                    int width = bitmapCamera.PixelWidth;
                    int height = bitmapCamera.PixelHeight;

                    WriteableBitmap wBitmap = new WriteableBitmap(width, height);

                    using (var stream = await capturedMedia.OpenAsync(FileAccessMode.Read))
                    {
                        wBitmap.SetSource(stream);
                    }
                    SaveImageAsJpeg(wBitmap);
                }

            }
        }

        private async void SaveImageAsJpeg(WriteableBitmap wBitmap)
        {
            // using Windows.Graphics.Imaging;
            // using Windows.Storage.Pickers;
            // using Windows.Storage.Streams;
            // using System.Runtime.InteropServices.WindowsRuntime; // If you leave this out, AsStream() will not be available

            // Create the File Picker control
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPG File", new List<string>() { ".jpg" });


            StorageFile file = await picker.PickSaveFileAsync();
            path = file.Path.ToString();
            Debug.WriteLine("ok gass" + path);
            if (file != null)
            {
                // If the file path and name is entered properly, and user has not tapped 'cancel'..

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {

                    // Encode the image into JPG format,reading for saving
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    Stream pixelStream = wBitmap.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)wBitmap.PixelWidth, (uint)wBitmap.PixelHeight, 96.0, 96.0, pixels);
                    //conn.Insert(new MyClothes() { image = pixels, typeClothes = ((ListViewItem)comboType.SelectedItem).Content.ToString() });

                    await encoder.FlushAsync();
                }
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            string warning;

            try
            {
                conn.Insert(new MyClothes()
                {
                    nameClothes = txtClothesName.Text,

                    typeClothes = ((ComboBoxItem)cmbCathegory.SelectedItem).Content.ToString(),
                    season = ((ComboBoxItem)cmbCathegory.SelectedItem).Content.ToString(),
                    imgPath = path,

                });
                warning = "Content saved";





            }
            catch
            {

                warning = "Invalid entry \n please verify that you have entred the correct items";
            }
            MessageDialog dialog = new MessageDialog(warning, "My Fashion Assistance");
            dialog.Commands.Add(new UICommand("OK", delegate(IUICommand command)
            {
                Frame.Navigate(typeof(Clothes));
            }));

            await dialog.ShowAsync();
            
        }

        private void allClothes_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All_Clothes));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void txtClothesName_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            txtClothesName.Text = "";
        }

        private void imagePreivew_Tapped(object sender, TappedRoutedEventArgs e)
        {
            openMyCamera();
        }
    }
}
