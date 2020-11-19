using Microsoft.Win32;
using System;
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
using System.IO;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Diagnostics;

using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for AddPlaylist.xaml
    /// </summary>
    public partial class AddPlaylist : UserControl
    {
        public ObservableCollection<medium> SongsForPlaylist { get; set; }
        public ObservableCollection<album> HasNewAlbum { get; set; }
        public ObservableCollection<artist> HasNewArtist { get; set; }
        public static readonly RoutedEvent SettingConfirmedEvent2 = EventManager.RegisterRoutedEvent("SettingConfirmedEvent2", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddPlaylist));
        System.Windows.Controls.Image image;

        public AddPlaylist()
        {
            InitializeComponent();
            HasNewAlbum = new ObservableCollection<album>();
            HasNewArtist = new ObservableCollection<artist>();
            SongsForPlaylist = new ObservableCollection<medium>();
        }

        public event RoutedEventHandler SettingConfirmed
        {
            add { AddHandler(SettingConfirmedEvent2, value); }
            remove { RemoveHandler(SettingConfirmedEvent2, value); }
        }

        private void SelectSongs_Click(object sender, RoutedEventArgs e)
        {
            errorMessage.Visibility = Visibility.Collapsed;
            lbFiles.Items.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
           
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Audio files (*.mp3)|*.mp3|Video files (*.mp4)|*.mp4|All files|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    foreach (string filename in openFileDialog.FileNames)
                    {
                        lbFiles.Items.Add(System.IO.Path.GetFileName(filename));
                        medium media = MediaAtributte(filename);
                        SongsForPlaylist.Add(media);
                        HasNewArtist.Add(media.artist);
                        HasNewAlbum.Add(media.album);
                    }
                    btnSavePlaylist.IsEnabled = true;
                }
                catch(Exception ex)
                {
                    btnSavePlaylist.IsEnabled = false;
                    errorMessage.Text = " *  Media Player supports only MP3 and MP4 files. File you selected cannot be added to Media Player for that or some other reason.";
                    errorMessage.Visibility = Visibility.Visible;
                }
            }
            lbFiles.Visibility = Visibility.Visible;
        }

        private medium MediaAtributte(string path)
        {
           
            medium media = new medium();
            media.path = path;
            TagLib.File file = TagLib.File.Create(path);
            media.album = AlbumAtributte(path);
            media.artist = ArtistAtributte(path);
            media.name = file.Tag.Title;
            if (string.IsNullOrEmpty(media.name))
            {
                media.name = System.IO.Path.GetFileNameWithoutExtension(media.path);
            }
            media.duration = string.Format("{0}:{1}", file.Properties.Duration.Minutes.ToString("00"), file.Properties.Duration.Seconds.ToString("00"));
            media.extension = System.IO.Path.GetExtension(path);
            if (file.Tag.Pictures.Length >= 1)
            {
                    MemoryStream ms = new MemoryStream(file.Tag.Pictures[0].Data.Data);
                    ms.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms; 
                    bitmap.EndInit();
                    image = new System.Windows.Controls.Image();
                    image.Source = bitmap;
            }
            else
            {
                ShellFile shellFile = ShellFile.FromFilePath(path);
                Bitmap bm = shellFile.Thumbnail.Bitmap;
                MemoryStream ms = new MemoryStream();
                bm.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                image = new System.Windows.Controls.Image();
                image.Source = bitmap;
            }
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
                if (!string.IsNullOrEmpty(media.album.name))
                {
                try
                {
                    string coverPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\album\\" + media.album.name + ".png";
                    if (!File.Exists(coverPath))
                    {
                        using (FileStream stream = new FileStream(coverPath, FileMode.Create))
                            encoder.Save(stream);
                        media.album.cover = coverPath;
                    }
                }
                catch(Exception e)
                {
                    media.album.name= System.IO.Path.GetInvalidFileNameChars().Aggregate(media.album.name, (current, c) => current.Replace(c.ToString(), string.Empty));
                    string coverPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\album\\" + media.album.name + ".png";
                    if (!File.Exists(coverPath))
                    {
                        using (FileStream stream = new FileStream(coverPath, FileMode.Create))
                            encoder.Save(stream);
                        media.album.cover = coverPath;
                    }
                }

                }
                else
                {
                    media.album.name = "Unknown";
                    string coverPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\album\\" + "unknown" + ".png";
                using (FileStream stream = new FileStream(coverPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    encoder.Save(stream);
                }
                    media.album.cover = coverPath;
                }
            
            var encoderSecond = new PngBitmapEncoder();
            encoderSecond.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
            if (!string.IsNullOrEmpty(media.artist.name))
            {
                try
                {
                    string coverPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\artist\\" + media.artist.name + ".png";
                    if (!File.Exists(coverPath))
                    {
                        using (FileStream stream = new FileStream(coverPath, FileMode.Create))
                        {
                            encoderSecond.Save(stream);

                        }
                        media.artist.cover = coverPath;
                    }
                }
                catch(Exception e)
                {
                    media.artist.name = System.IO.Path.GetInvalidFileNameChars().Aggregate(media.artist.name, (current, c) => current.Replace(c.ToString(), string.Empty));
                    string coverPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\artist\\" + media.artist.name + ".png";
                    if (!File.Exists(coverPath))
                    {
                        using (FileStream stream = new FileStream(coverPath, FileMode.Create))
                            encoder.Save(stream);
                        media.artist.cover = coverPath;
                    }
                }
            }
            else
            {
                media.artist.name = "Unknown";
                string coverPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\artist\\" + "unknown" + ".png";
                using (FileStream stream = new FileStream(coverPath, FileMode.Create))
                    encoderSecond.Save(stream);
                media.artist.cover = coverPath;
            }
            return media;
        }

        private album AlbumAtributte(string path)
        {
            album album = new album();
            TagLib.File file = TagLib.File.Create(path);
            album.name = file.Tag.Album;
            return album;
        }
        private artist ArtistAtributte(string path)
        {
            artist artist = new artist();
            TagLib.File file = TagLib.File.Create(path);
            artist.name = file.Tag.FirstArtist;
            return artist;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {         
            RaiseEvent(new RoutedEventArgs(AddPlaylist.SettingConfirmedEvent2));
            lbFiles.Items.Clear();
            errorMessage.Visibility = Visibility.Collapsed;
            playlistName.Text = "";
        }
    }
}
