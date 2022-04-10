using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Drawing.Bitmap;
using System.Drawing;
using Microsoft.Win32;

namespace cheloSecret
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //public static byte[,,] BitmapToByteRgbNaive(Bitmap bmp) // простой алгоритм для преобразования 
        //{                                                       // рисунка
        //    int width = bmp.Width,
        //        height = bmp.Height;
        //    byte[,,] res = new byte[3, height, width];
        //    for (int y = 0; y < height; ++y)
        //    {
        //        for (int x = 0; x < width; ++x)
        //        {
        //            System.Drawing.Color color = bmp.GetPixel(x, y);
        //            res[0, y, x] = color.R;
        //            res[1, y, x] = color.G;
        //            res[2, y, x] = color.B;
        //        }
        //    }

        //    return res;
        //}

        public static Bitmap LoadBitmap(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return new Bitmap(fs);
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e) 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string FilePath = openFileDialog.FileName;
                textBoxPath.Text = FilePath;

                Bitmap sourceBitmap = new Bitmap(LoadBitmap(textBoxPath.Text));
                imaga1.Source = BitmapToImageSource(sourceBitmap);
            }
        }

        private void btnSteGO_Click(object sender, RoutedEventArgs e)
        {
            if(textBox1.Text != "") 
            {
                Bitmap sourceBitmap = new Bitmap(LoadBitmap(textBoxPath.Text));
                //Bitmap targetBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);
                textBoxLenght.Text = textBox1.Text.Length.ToString();
                imaga2.Source = null;
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    for (int y = 0; y < sourceBitmap.Height; y++)
                    {
                        System.Drawing.Color pixel = sourceBitmap.GetPixel(x, y);
                        if (x < 1 && y < textBox1.Text.Length)
                        {
                            //MessageBox.Show("fir \n" + x + "\n" + y + "\n");
                            char letter = Convert.ToChar(textBox1.Text.Substring(y, 1));
                            int value = Convert.ToInt32(letter);

                            sourceBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel.R, pixel.G, value));
                        }
                        if (x == sourceBitmap.Width - 1 && y == sourceBitmap.Height - 1)
                        {
                            //char lenght = Convert.ToChar(textBoxLenght.Text.Substring(y, 1));
                            //MessageBox.Show("sec \n" + x + "\n" + y + "\n");
                            sourceBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel.R, pixel.G, textBox1.Text.Length));
                        }
                    }
                }
                imaga2.Source = BitmapToImageSource(sourceBitmap);
            }
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxPath.Text == "")
            {
                MessageBox.Show("Path is missed");
            }
            else
            {
                Bitmap ImgToDecode = new Bitmap(LoadBitmap(textBoxPath.Text));
                imaga1.Source = BitmapToImageSource(ImgToDecode);
                imaga2.Source = null;
                //Bitmap img = new Bitmap(textBoxPath.Text);
                string msg = "";
                System.Drawing.Color lastpixel = ImgToDecode.GetPixel(ImgToDecode.Width - 1, ImgToDecode.Height - 1);
                int msgLength = lastpixel.B;

                for (int x = 0; x < ImgToDecode.Width; x++)
                {

                    for (int y = 0; y < ImgToDecode.Height; y++)
                    {
                        System.Drawing.Color pixel = ImgToDecode.GetPixel(x, y);

                        if (x < 1 && y < msgLength)
                        {
                            int value = pixel.B;
                            char c = Convert.ToChar(value);
                            string letter = Encoding.ASCII.GetString(new byte[] { Convert.ToByte(c) });
                            Console.WriteLine("letter : " + letter + " value : " + value);
                            msg = msg + letter;
                        }
                    }
                }
                textBox2.Text = msg;
                if (textBox2.Text == "")
                {
                    MessageBox.Show("No data");
                }
                else
                {
                    MessageBox.Show("Decoded text:"+msg);
                }
            }
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Image Files (*.png) | *.png";

            if (saveFile.ShowDialog() != DialogResult)
            {
                textBoxPath.Text = saveFile.FileName.ToString();

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imaga2.Source));
                using (FileStream stream = new FileStream(textBoxPath.Text, FileMode.Create))
                    encoder.Save(stream);
                MessageBox.Show("Image Saved");
            }
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {//snipped to convert the Bitmap to a ImageSource, нужен потому что в WPF отсутствует пичкурбокс
            //https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text != "") 
            { 
                textBox2.Text = textDissamble(textBox1.Text);
            }
        }//не используются в получившейся реализации, но оставлены на будущее
        private void btnConvert2_Click(object sender, RoutedEventArgs e)
        {
            if (textBox2.Text != "")
            {
                textBox1.Text = textAssemble(textBox2.Text);
            }
        }


        
        public string textDissamble(string inp)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in System.Text.Encoding.Default.GetBytes(textBox1.Text))
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }//не используются в получившейся реализации, но оставлены на будущее
        public string textAssemble(string data)
        {
            List<byte> byteList = new List<Byte>();
            for (int i=0; i<data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i,8), 2));
            }
            return Encoding.Default.GetString(byteList.ToArray());

        }


    }
}
