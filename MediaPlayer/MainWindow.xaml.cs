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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TagLib;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows.Shell;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack;
using System.Globalization;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool MenuClosed = false;
        private DispatcherTimer timer;
        private bool ArtistClicked = false;
        private bool AlbumClicked = false;
        private bool PlaylistClicked = false;
        private string currentMediaPath;
        private string currentMediaListTitle;
        private bool userIsDraggingSlider = false;
        playlist SelectedPlaylist { get; set; }
        private bool playlistPlusWasClicked = false;
        private bool fullScreenButtonIsClicked = false;

        private ObservableCollection<medium> Songs { get; set; }
        private ObservableCollection<album> Albums { get; set; }
        private ObservableCollection<medium> Artist { get; set; }

        private medium Medium { get; set; }


        public MainWindow()
        {
            InitializeComponent(); 
            Recent_Click(new object(), new RoutedEventArgs());
            MediaPlayerIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/icon2.png"));
            DataContext = this; 

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();

            AddHandler(SpecificCollection.SettingConfirmedEvent,
                new RoutedEventHandler(Window_SpecificCollection_SettingConfirmedEventHandlerMethod));
            AddHandler(AddPlaylist.SettingConfirmedEvent2,
                new RoutedEventHandler(Window_AddPlaylist_SettingConfirmedEventHandlerMethod));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((mediaPlayer.Source != null) && (mediaPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                TrackSlider.Minimum = 0;
                TrackSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                TrackSlider.Value = mediaPlayer.Position.TotalSeconds;
                InfoLabel2.Content = mediaPlayer.NaturalDuration.TimeSpan.TotalHours.ToString("00") + ":" + mediaPlayer.NaturalDuration.TimeSpan.Minutes.ToString("00") + ":" + mediaPlayer.NaturalDuration.TimeSpan.Seconds.ToString("00");
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MenuClosed)
            {
                Storyboard openMenu = (Storyboard)button.FindResource("OpenMenu");
                openMenu.Begin();
            }
            else
            {
                Storyboard closeMenu = (Storyboard)button.FindResource("CloseMenu");
                closeMenu.Begin();
            }

            MenuClosed = !MenuClosed;
        }

        private void DropdownMenuOpen(object sender, RoutedEventArgs e)
        {
            if (DropdownMenu.Visibility == Visibility.Visible)
                DropdownMenu.Visibility = Visibility.Collapsed;
            else
                DropdownMenu.Visibility = Visibility.Visible;
        }

        private void WindowHide(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void WindowResize(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
            if(fullScreenButtonIsClicked)
            {
                mediaElementAndImage.Width = WholeWindow.ActualWidth;
                mediaElementAndImage.Height = WholeWindow.ActualHeight;
            }
        }

        private void WindowClosed(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MusicPlay_Pause(object sender, RoutedEventArgs e)
        {
            if (Play.Kind == MaterialDesignThemes.Wpf.PackIconKind.Play)
            {
                mediaPlayer.Play();
                Play.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                
                MediaSlider.Visibility = Visibility.Visible;
                InfoLabel1.Visibility = Visibility.Visible;
                InfoLabel2.Visibility = Visibility.Visible;
                TrackSlider.Visibility = Visibility.Visible;

            }
            else
            {
                mediaPlayer.Pause();
                Play.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                MediaSlider.Visibility = Visibility.Collapsed;
            }
        }

        private void MusicVolume(object sender, RoutedEventArgs e)
        {
            if (VolumeSlider.IsVisible)
            {
                VolumeSlider.Visibility = Visibility.Collapsed;
            }
            else
            {
                VolumeSlider.Visibility = Visibility.Visible;
            }
        }

        private void VolumeChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(mediaPlayer!=null)
            mediaPlayer.Volume = VolumeSlider.Value;
            if (VolumeSlider.Value == VolumeSlider.Minimum)
            {
                Volume.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeMute;
            }
            else if (Volume != null)
                Volume.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh;

        }

        private void Artist_Click(object sender, RoutedEventArgs e)
        {
            coverImage.Source = null;
            smallImage.Source = null;
            NewPlaylist.Visibility = Visibility.Collapsed;
            PlaylistAdd.Visibility = Visibility.Collapsed;
            if (Folder.Visibility == Visibility.Visible && !AlbumClicked && !PlaylistClicked)
            {
                Folder.Visibility = Visibility.Hidden;
                ArtistClicked = false;
            }
            else
            {
                Folder.Visibility = Visibility.Visible;
                Folder.GetArtistCollection();
                ArtistClicked = true;
                AlbumClicked = false;
                PlaylistClicked = false;
            }
        }

        private void Album_Click(object sender, RoutedEventArgs e)
        {
            coverImage.Source = null;
            smallImage.Source = null;
            NewPlaylist.Visibility = Visibility.Collapsed;
            PlaylistAdd.Visibility = Visibility.Collapsed;
            if (Folder.Visibility == Visibility.Visible && !ArtistClicked && !PlaylistClicked)
            {
                Folder.Visibility = Visibility.Hidden;
                AlbumClicked = false;
            }
            else
            {
                Folder.Visibility = Visibility.Visible;
                Folder.GetAlbumCollection();
                AlbumClicked = true;
                ArtistClicked = false;
                PlaylistClicked = false;
            }
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            DropdownMenu.Visibility = Visibility.Collapsed;
            NewPlaylist.Visibility = Visibility.Collapsed;
            columnNames.Visibility = Visibility.Visible;
            coverImage.Source = null;
            smallImage.Source = null;
            if (Folder.Visibility == Visibility.Visible && !ArtistClicked && !AlbumClicked)
            {
                Folder.Visibility = Visibility.Hidden;
                PlaylistClicked = false;
                PlaylistAdd.Visibility = Visibility.Collapsed;
            }
            else
            {
                Folder.Visibility = Visibility.Visible;
                Folder.GetPlaylistCollection();
                PlaylistClicked = true;
                AlbumClicked = false;
                ArtistClicked = false;
                PlaylistAdd.Visibility = Visibility.Visible;
            }
        }

        private void NewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            DropdownMenu.Visibility = Visibility.Collapsed;
            columnNames.Visibility = Visibility.Visible;
            NewPlaylist.lbFiles.Items.Clear();
            NewPlaylist.errorMessage.Visibility = Visibility.Collapsed;
            NewPlaylist.playlistName.Text = "";
            coverImage.Source = null;
            smallImage.Source = null;
            if (NewPlaylist.Visibility == Visibility.Visible)
            {
                NewPlaylist.Visibility = Visibility.Hidden;
            }
            else
            {
                NewPlaylist.Visibility = Visibility.Visible;
            }
        }
        private void Window_AddPlaylist_SettingConfirmedEventHandlerMethod(object sender, RoutedEventArgs e) ///napraviti try-catch
        {
            playlist newPlaylist;
            medium newMedia;
            string newPlaylistCover = String.Empty;
            using (Model1 model = new Model1())
            {
                newPlaylist = new playlist()
                {
                    name = NewPlaylist.playlistName.Text,
                    cover = "pack://application:,,,/Resources/default.png",
                };
                model.playlists.Add(newPlaylist);
                model.SaveChanges();

                foreach (var a in NewPlaylist.SongsForPlaylist)
                {
                   if (!ArtistExist(a) && a.album.name!= "Unknown")
                     {
                             var newArtist = new artist()
                             {
                                 name = a.artist.name,
                                 cover = a.artist.cover
                             };
                             model.artists.Add(newArtist);
                             model.SaveChanges();
                             var newAlbum = new album()
                             {
                                 name = a.album.name,
                                 cover = a.album.cover
                             };
                             model.albums.Add(newAlbum);
                             model.SaveChanges();
                             var specificAlbum = (from c in model.albums where c.name == a.album.name select c).FirstOrDefault();
                             var specificArtist = (from c in model.artists where c.name == a.artist.name select c).FirstOrDefault();
                        newMedia = new medium()
                        {
                            name = a.name,
                            path = a.path,
                            duration = a.duration,
                            favorite = 0,
                                 extension = a.extension,
                                 album_id = specificAlbum.id,
                                 artist_id = specificArtist.id,
                             };
                             model.media.Add(newMedia);
                             model.SaveChanges();
                         
                     }
                   else if(!ArtistExist(a) && a.album.name == "Unknown")
                   {
                        var newArtist = new artist()
                        {
                            name = a.artist.name,
                            cover = a.artist.cover
                        };
                        model.artists.Add(newArtist);
                        model.SaveChanges();
                        var specificAlbum = (from c in model.albums where c.name == a.album.name select c).FirstOrDefault();
                        if(specificAlbum == null)
                        {
                            var newAlbum = new album()
                            {
                                name = a.album.name,
                                cover = a.album.cover
                            };
                            model.albums.Add(newAlbum);
                            model.SaveChanges();
                            specificAlbum = newAlbum;
                        }
                        var specificArtist = (from c in model.artists where c.name == a.artist.name select c).FirstOrDefault();
                        newMedia = new medium()
                        {
                            name = a.name,
                            path = a.path,
                            duration = a.duration,
                            favorite = 0,
                            extension = a.extension,
                            album_id = specificAlbum.id,
                            artist_id = specificArtist.id,
                        };
                        model.media.Add(newMedia);
                        model.SaveChanges();
                    }
                     else if (ArtistExist(a) && !AlbumExist(a))
                     {
                             var newAlbum = new album()
                             {
                                 name = a.album.name,
                                 cover = a.album.cover
                             };
                             model.albums.Add(newAlbum);
                             model.SaveChanges();
                             var specificAlbum = (from c in model.albums where c.name == a.album.name select c).FirstOrDefault();
                             var specificArtist = (from c in model.artists where c.name == a.artist.name select c).FirstOrDefault();
                             newMedia = new medium()
                             {
                                 name = a.name,
                                 path = a.path,
                                 duration = a.duration,
                                 favorite=0,
                                 extension = a.extension,
                                 album_id = specificAlbum.id,
                                 artist_id = specificArtist.id,
                             };
                             model.media.Add(newMedia);
                             model.SaveChanges();
                         
                     }
                     else if (ArtistExist(a) && AlbumExist(a) && !MediaExist(a))
                     {
                             var specificAlbum = (from c in model.albums where c.name == a.album.name select c).FirstOrDefault();
                             var specificArtist = (from c in model.artists where c.name == a.artist.name select c).FirstOrDefault();
                        newMedia = new medium()
                        {
                            name = a.name,
                            path = a.path,
                            duration = a.duration,
                            favorite = 0,
                                 extension = a.extension,
                                 album_id = specificAlbum.id,
                                 artist_id = specificArtist.id,
                             };
                         
                             model.media.Add(newMedia);
                             model.SaveChanges();
                     }

                    medium media = model.media.FirstOrDefault(m => m.name == a.name);
                    newPlaylist.media.Add(media);
                    media.playlists.Add(newPlaylist);
                    model.SaveChanges(); 
                }
            }
            Folder.currentPlaylist = newPlaylist;
            Folder.playlistActive = true;
            NewPlaylist.Visibility = Visibility.Collapsed;
            Window_SpecificCollection_SettingConfirmedEventHandlerMethod(sender, e);
        }

        private void Window_SpecificCollection_SettingConfirmedEventHandlerMethod(object sender, RoutedEventArgs e)
        {
            Folder.Visibility = Visibility.Hidden;
            ListOfSongs.Visibility = Visibility.Visible;
            columnNames.Visibility = Visibility.Visible;
            ListViewMediaFiles.ItemsSource = null;
            Songs = new ObservableCollection<medium>();
          
            using (Model1 model = new Model1())
            {
                if (playlistPlusWasClicked == true)
                {

                    var specificPlaylist = (from c in model.playlists where c.name == Folder.currentPlaylist.name select c).FirstOrDefault();
                    if (specificPlaylist != null)
                    {
                        medium medium= model.media.FirstOrDefault(m => m.name == Medium.name);
                        specificPlaylist.media.Add(medium);
                        medium.playlists.Add(specificPlaylist);
                        model.SaveChanges();
                        playlistPlusWasClicked = false;
                    }
                }
                if (Folder.artistActive)
                {
                    var all = model.media.Select(m => new { m.path, m.name, m.album, m.artist, m.duration, m.extension, m.favorite }).Where(m => m.artist.id == Folder.currentArtist.id).ToList();
                    int counter = 0;
                    TimeSpan duration = new TimeSpan();
                    foreach (var citem in all)
                    {
                        counter += 1;
                        TimeSpan time = TimeSpan.ParseExact(citem.duration, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                        duration += time;
                        currentMediaListTitle = citem.artist.name;
                        TagLib.File file = TagLib.File.Create(citem.path);
                        BitmapImage bitmap = new BitmapImage();
                        if (file.Tag.Pictures.Length >= 1)
                        {
                            MemoryStream ms = new MemoryStream(file.Tag.Pictures[0].Data.Data);
                            ms.Seek(0, SeekOrigin.Begin);

                            bitmap.BeginInit();
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                            coverImage.Source = bitmap;
                            smallImage.Source = bitmap;
                        }
                        else
                        {
                            GetImageFrameFromVideo(citem.path, bitmap);
                        }
                        Songs.Add(new medium() { path = citem.path,number=counter, name = citem.name, album = citem.album, artist = citem.artist, duration = citem.duration, iconKind = (citem.favorite==0)?PackIconKind.HeartOutline:PackIconKind.Heart,cover=bitmap, extension = citem.extension });
                    }
                    numOfSongs.Text = Songs.Count() + "  songs";
                    durationAll.Text = duration.ToString(@"hh\:mm\:ss");
                }
                else if (Folder.albumActive)
                {
                    var all = model.media.Select(m => new { m.path, m.name, m.album, m.artist, m.duration, m.extension,m.favorite }).Where(m => m.album.id == Folder.currentAlbum.id).ToList();
                    int counter = 0;
                    TimeSpan duration = new TimeSpan();
                    foreach (var citem in all)
                    {
                        counter += 1;
                        TimeSpan time = TimeSpan.ParseExact(citem.duration, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                        duration += time;
                        TagLib.File file = TagLib.File.Create(citem.path);
                        BitmapImage bitmap = new BitmapImage();
                        if (file.Tag.Pictures.Length >= 1)
                        {
                            MemoryStream ms = new MemoryStream(file.Tag.Pictures[0].Data.Data);
                                ms.Seek(0, SeekOrigin.Begin);

                                bitmap.BeginInit();
                                bitmap.StreamSource = ms;
                                bitmap.EndInit();
                                coverImage.Source = bitmap;
                                smallImage.Source = bitmap;
                             
                        }
                        else
                        {
                            GetImageFrameFromVideo(citem.path, bitmap);
                        }
                        
                        currentMediaListTitle = citem.album.name;
                        Songs.Add(new medium() { path = citem.path,number=counter, name = citem.name, album = citem.album, artist = citem.artist, duration = citem.duration, iconKind = (citem.favorite == 0) ? PackIconKind.HeartOutline : PackIconKind.Heart, cover=bitmap, extension = citem.extension });
                    }
                    numOfSongs.Text = Songs.Count() + "  songs";
                    durationAll.Text = duration.ToString(@"hh\:mm\:ss");
                }
                else if(Folder.playlistActive)
                {
                    SelectedPlaylist = new playlist();
                    SelectedPlaylist=Folder.currentPlaylist;
                    var all = model.media.Include("playlists").Where(b => b.playlists.Any(n => n.id == Folder.currentPlaylist.id)).ToList();
                    int counter = 0;
                    TimeSpan duration = new TimeSpan();
                    foreach (var citem in all)
                    {
                        counter += 1;
                        TimeSpan time = TimeSpan.ParseExact(citem.duration, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                        duration += time;
                        currentMediaListTitle = Folder.currentPlaylist.name;
                        TagLib.File file = TagLib.File.Create(citem.path);
                        BitmapImage bitmap = new BitmapImage();
                        if (file.Tag.Pictures.Length >= 1)
                        {
                            MemoryStream ms = new MemoryStream(file.Tag.Pictures[0].Data.Data);
                            ms.Seek(0, SeekOrigin.Begin);

                            bitmap.BeginInit();
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                            coverImage.Source = bitmap;
                            smallImage.Source = bitmap;
                        }
                        else
                        {
                            GetImageFrameFromVideo(citem.path, bitmap);
                        }
                        Songs.Add(new medium() { path = citem.path,number=counter, name = citem.name, album = citem.album, artist = citem.artist, duration = citem.duration, iconKind = (citem.favorite == 0) ? PackIconKind.HeartOutline : PackIconKind.Heart,cover=bitmap, extension = citem.extension });
                    }
                    durationAll.Text = duration.ToString(@"hh\:mm\:ss");
                    numOfSongs.Text = Songs.Count + "  songs";
                }
                ListViewMediaFiles.ItemsSource = Songs;
                generalTitle.Text = currentMediaListTitle;
            }
        }

        private void NowPlaying() 
        {
            PlayedSong.Visibility = Visibility.Visible;
            SelectSongToPlay(Medium);
            if (Medium.nowPlaying == MaterialDesignThemes.Wpf.PackIconKind.PlayCircleOutline)
                Medium.nowPlaying = MaterialDesignThemes.Wpf.PackIconKind.Equalizer;         
            ListViewMediaFiles.ItemsSource = null;
            Folder.Visibility = Visibility.Hidden;
            int elementCounter = 0;
            foreach (var song in Songs)
            {
                if (song.path != Medium.path)
                {
                    Songs.ElementAt(elementCounter).nowPlaying = PackIconKind.PlayCircleOutline;
                }
                elementCounter += 1;
            }
            ListViewMediaFiles.ItemsSource = Songs;
        }
        private void PlayMusic_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var dataObject = btn.DataContext as medium;
            
            if (dataObject != null)
            {
                currentMediaPath = dataObject.path;
                Medium = new medium();
                Medium = dataObject;
            }
            NowPlaying();
            Play.Kind = PackIconKind.Play;
            MusicPlay_Pause(sender, e);
        }

        private void Songs_Click(object sender, RoutedEventArgs e)
        {
            ListViewMediaFiles.ItemsSource = null;
            columnNames.Visibility = Visibility.Visible;
            ListOfSongs.Visibility = Visibility.Visible;
            NewPlaylist.Visibility = Visibility.Collapsed;
            PlaylistAdd.Visibility = Visibility.Collapsed;
            Folder.Visibility = Visibility.Hidden;
         
            Songs = new ObservableCollection<medium>();
            TimeSpan duration = new TimeSpan();
            using (Model1 model = new Model1())
            {
                var all = model.media.Select(m => new { m.path, m.name, m.album, m.artist, m.duration, m.extension, m.favorite }).Where(n => n.extension == ".mp3").ToList();
                int counter = 0;
                foreach (var citem in all)
                {
                    counter += 1;
                    TimeSpan time = TimeSpan.ParseExact(citem.duration, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    duration += time;
                    TagLib.File file = TagLib.File.Create(citem.path);
                    BitmapImage bitmap = new BitmapImage();
                    if (file.Tag.Pictures.Length >= 1)
                    {
                        MemoryStream ms = new MemoryStream(file.Tag.Pictures[0].Data.Data);
                        ms.Seek(0, SeekOrigin.Begin);
                       
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        coverImage.Source = bitmap;
                        smallImage.Source = bitmap;
                    }
                    else
                    {
                        GetImageFrameFromVideo(citem.path, bitmap);
                    }
                    Songs.Add(new medium() { path = citem.path,number=counter, name = citem.name, album = citem.album, artist = citem.artist, duration = citem.duration, iconKind = (citem.favorite == 0) ? PackIconKind.HeartOutline : PackIconKind.Heart, cover=bitmap, extension = citem.extension });
                }
                
            }
            durationAll.Text = duration.ToString(@"hh\:mm\:ss");
            numOfSongs.Text = Songs.Count + "  songs";
            currentMediaListTitle = "All songs";
            generalTitle.Text = currentMediaListTitle;
            ListViewMediaFiles.ItemsSource = Songs;
        }

        private void Video_Click(object sender, RoutedEventArgs e)
        {
            DropdownMenu.Visibility = Visibility.Collapsed;
            NewPlaylist.Visibility = Visibility.Collapsed;
            PlaylistAdd.Visibility = Visibility.Collapsed;
            ListOfSongs.Visibility = Visibility.Visible;
            columnNames.Visibility = Visibility.Visible;
            ListViewMediaFiles.ItemsSource = null;
            Folder.Visibility = Visibility.Hidden;
            Songs = new ObservableCollection<medium>();
            TimeSpan duration = new TimeSpan();
            using (Model1 model = new Model1())
            {
                var all = model.media.Select(m => new { m.path, m.name, m.album, m.artist, m.duration,  m.extension, m.favorite }).Where(n => n.extension == ".mp4").ToList();
                int counter = 0;
                foreach (var citem in all)
                {
                    TimeSpan time = TimeSpan.ParseExact(citem.duration, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    duration += time;
                    BitmapImage bitmap = new BitmapImage();
                    GetImageFrameFromVideo(citem.path, bitmap);
                    
                    counter += 1;
                    Songs.Add(new medium() { path = citem.path, number = counter, name = citem.name, album = citem.album, artist = citem.artist, cover=bitmap, duration = citem.duration, iconKind = (citem.favorite == 0) ? PackIconKind.HeartOutline : PackIconKind.Heart, extension = citem.extension }); ;
                }
                
            }
            durationAll.Text = duration.ToString(@"hh\:mm\:ss");
            numOfSongs.Text = Songs.Count + "  songs";
            currentMediaListTitle = "All videos";
            generalTitle.Text = currentMediaListTitle;
            ListViewMediaFiles.ItemsSource = Songs;
        }

        private void LastTimeHeard()
        {
            using (Model1 model = new Model1())
            {
                var all = (from c in model.media where c.path == currentMediaPath select c).FirstOrDefault();
                if (all != null)
                {
                    all.heard = DateTime.Now;
                    model.SaveChanges();
                }
            }
        }

        private void Recent_Click(object sender, RoutedEventArgs e)
        {
            DropdownMenu.Visibility = Visibility.Collapsed;
            NewPlaylist.Visibility = Visibility.Collapsed;
            PlaylistAdd.Visibility = Visibility.Collapsed;
            ListOfSongs.Visibility = Visibility.Visible;
            columnNames.Visibility = Visibility.Visible;
            ListViewMediaFiles.ItemsSource = null;
            Folder.Visibility = Visibility.Hidden;
            Songs = new ObservableCollection<medium>();
            TimeSpan duration = new TimeSpan();
            using (Model1 model = new Model1())
            {
                var all = (from c in model.media orderby c.heard descending select c).Take(3).ToList(); 
                int counter = 0;
                foreach (var citem in all)
                {
                    counter += 1;
                    TimeSpan time = TimeSpan.ParseExact(citem.duration, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    duration += time;
                    TagLib.File file = TagLib.File.Create(citem.path);
                    BitmapImage bitmap = new BitmapImage();
                    if (file.Tag.Pictures.Length >= 1)
                    {
                        MemoryStream ms = new MemoryStream(file.Tag.Pictures[0].Data.Data);
                        ms.Seek(0, SeekOrigin.Begin);

                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        coverImage.Source = bitmap;
                        smallImage.Source = bitmap;
                    }
                    else
                    {
                        GetImageFrameFromVideo(citem.path, bitmap);
                    }
                    Songs.Add(new medium() { path = citem.path,number=counter, name = citem.name, album = citem.album, artist = citem.artist, duration = citem.duration, iconKind = (citem.favorite == 0) ? PackIconKind.HeartOutline : PackIconKind.Heart,cover=bitmap, extension = citem.extension });
                    
                }
                durationAll.Text = duration.ToString(@"hh\:mm\:ss");
                numOfSongs.Text = Songs.Count + "  songs";
                currentMediaListTitle = "Recently played";
                generalTitle.Text = currentMediaListTitle;
                ListViewMediaFiles.ItemsSource = Songs;
            }
        }
        private void RecentlyAdded_Click(object sender, RoutedEventArgs e)
        {
            DropdownMenu.Visibility = Visibility.Collapsed;
            NewPlaylist.Visibility = Visibility.Collapsed;
            PlaylistAdd.Visibility = Visibility.Collapsed;
            ListOfSongs.Visibility = Visibility.Visible;
            columnNames.Visibility = Visibility.Visible;
            ListViewMediaFiles.ItemsSource = null;
            Folder.Visibility = Visibility.Hidden;
            Songs = new ObservableCollection<medium>();
            using (Model1 model = new Model1())
            {
                var all = (from c in model.media orderby c.id descending select c).Take(5).ToList(); 
                int counter = 0;
                TimeSpan duration = new TimeSpan();
                foreach (var citem in all)
                {
                    counter += 1;
                    TimeSpan time = TimeSpan.ParseExact(citem.duration, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    duration += time;
                    TagLib.File file = TagLib.File.Create(citem.path);
                    BitmapImage bitmap = new BitmapImage();
                    if (file.Tag.Pictures.Length >= 1)
                    {
                        MemoryStream ms = new MemoryStream(file.Tag.Pictures[0].Data.Data);
                        ms.Seek(0, SeekOrigin.Begin);

                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        coverImage.Source = bitmap;
                        smallImage.Source = bitmap;
                    }
                    else
                    {
                        GetImageFrameFromVideo(citem.path, bitmap);
                    }
                    Songs.Add(new medium() { path = citem.path, number = counter, name = citem.name, album = citem.album, artist = citem.artist, duration = citem.duration, iconKind = (citem.favorite == 0) ? PackIconKind.HeartOutline : PackIconKind.Heart, cover = bitmap, extension = citem.extension });
                    
                }
                durationAll.Text = duration.ToString(@"hh\:mm\:ss");
                numOfSongs.Text = Songs.Count + "  songs";
                currentMediaListTitle = "Recently added";
                generalTitle.Text = currentMediaListTitle;
                ListViewMediaFiles.ItemsSource = Songs;
            }
        }

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            DropdownMenu.Visibility = Visibility.Collapsed;
            NewPlaylist.Visibility = Visibility.Collapsed;
            PlaylistAdd.Visibility = Visibility.Collapsed;
            ListOfSongs.Visibility = Visibility.Visible;
            columnNames.Visibility = Visibility.Visible;
            ListViewMediaFiles.ItemsSource = null;
            Folder.Visibility = Visibility.Hidden;
            Songs = new ObservableCollection<medium>();
            TimeSpan duration = new TimeSpan();
            using (Model1 model = new Model1())
            {
                var all = (from c in model.media where c.favorite == 1 select c).ToList();
                int counter = 0;
                foreach (var citem in all)
                {
                    counter += 1;
                    TimeSpan time = TimeSpan.ParseExact(citem.duration, @"mm\:ss", CultureInfo.InvariantCulture, TimeSpanStyles.None);
                    duration = duration.Add(time);
                    TagLib.File file = TagLib.File.Create(citem.path);
                    BitmapImage bitmap = new BitmapImage();
                    if (file.Tag.Pictures.Length >= 1)
                    {
                        MemoryStream ms = new MemoryStream(file.Tag.Pictures[0].Data.Data);
                        ms.Seek(0, SeekOrigin.Begin);

                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        coverImage.Source = bitmap;
                        smallImage.Source = bitmap;
                    }
                    else
                    {
                        GetImageFrameFromVideo(citem.path, bitmap);
                    }
                    Songs.Add(new medium() { path = citem.path,number=counter, name = citem.name, album = citem.album, artist = citem.artist, duration = citem.duration, iconKind = PackIconKind.Heart,cover=bitmap, extension = citem.extension });
                    
                }
                durationAll.Text = duration.ToString(@"hh\:mm\:ss");
                numOfSongs.Text = Songs.Count + "  songs";
                currentMediaListTitle = "Favorite songs";
                generalTitle.Text = currentMediaListTitle;
                ListViewMediaFiles.ItemsSource = Songs;
            }
        }

        private void Heart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var dataObject = btn.DataContext as medium;
            currentMediaPath = dataObject.path;
            if (dataObject.iconKind == MaterialDesignThemes.Wpf.PackIconKind.HeartOutline)
                dataObject.iconKind = MaterialDesignThemes.Wpf.PackIconKind.Heart;
            else
                dataObject.iconKind = MaterialDesignThemes.Wpf.PackIconKind.HeartOutline;
            ListViewMediaFiles.ItemsSource = null;
            Folder.Visibility = Visibility.Hidden;
            foreach (var song in Songs)
                {
                    if (song.id != dataObject.id)
                    {
                        Songs.Remove(new medium() { path = song.path, name = song.name, album = song.album, artist = song.artist, duration = song.duration, iconKind = song.iconKind, extension = song.extension });
                        Songs.Add(new medium() { path = song.path, name = song.name, album = song.album, artist = song.artist, duration = song.duration, iconKind = dataObject.iconKind, extension = song.extension });
                    }
                }
            ListViewMediaFiles.ItemsSource = Songs;
            using (Model1 model = new Model1())
            {
                var all = (from c in model.media where c.path == currentMediaPath select c).FirstOrDefault(); 
                if (all != null)
                {
                    all.favorite = (dataObject.iconKind == MaterialDesignThemes.Wpf.PackIconKind.Heart)? Convert.ToSByte(1): Convert.ToSByte(0);
                    model.SaveChanges();
                }
            }
        }
    
        private void SkipNext_Click(object sender, RoutedEventArgs e)
        {
            if (Shuffe.Kind == PackIconKind.Shuffle)
            {
                int currentSongIndex = 0;
                foreach (var song in Songs)
                {
                    currentSongIndex += 1;
                    if (song.path == currentMediaPath || currentSongIndex == Songs.Count)
                        break;
                }
                if (currentSongIndex == Songs.Count)
                {
                    medium next = Songs.ElementAt(currentSongIndex - 1);
                    currentMediaPath = next.path;
                    SelectSongToPlay(next);
                    mediaPlayer.Stop();
                    Play.Kind = PackIconKind.Play;
                    ListViewMediaFiles.ItemsSource = null;
                    int elementCounter = 0;
                    foreach (var song in Songs)
                    {
                        Songs.ElementAt(elementCounter).nowPlaying = PackIconKind.PlayCircleOutline;
                        elementCounter += 1;
                    }
                    ListViewMediaFiles.ItemsSource = Songs;
                }
                else
                {
                    medium next = Songs.ElementAt(currentSongIndex);
                    currentMediaPath = next.path;
                    Medium = new medium();
                    Medium = next;
                    NowPlaying();
                    Play.Kind = PackIconKind.Play;
                    MusicPlay_Pause(sender, e);
                }
            }
            else if (Shuffe.Kind == PackIconKind.ShuffleDisabled)
            {
                Random rnd = new Random();
                int randomIndex = rnd.Next(0, Songs.Count);
                medium now = Songs.ElementAt(randomIndex);
                currentMediaPath = now.path;
                Medium = new medium();
                Medium = now;
                NowPlaying();
                Play.Kind = PackIconKind.Play;
                MusicPlay_Pause(sender, e);
            }
        }

        private void SelectSongToPlay(medium media)
        {
            coverImage.Source = null;
            smallImage.Source = null;
            mediaPlayer.ScrubbingEnabled = false;
            PlayedSongTitle.Text = media.name;
            PlayedSongArtist.Text = media.artist.name;
            mediaPlayer.Source = new Uri(media.path);
            if (media.extension.Equals(".mp4"))
            {
                mediaPlayer.ScrubbingEnabled = true;
                mediaPlayer.Position = TimeSpan.FromSeconds(1);
                BitmapImage bitmap = new BitmapImage();
                GetImageFrameFromVideo(media.path, bitmap);
                PlayedSongCover.Source = bitmap;
            }
            
            else
            {
                smallImage.Source = media.cover;
                coverImage.Source = media.cover;
            }
            PlayedSongCover.Source = media.cover;
            LastTimeHeard();
        }

        private void SkipPrevious_Click(object sender, RoutedEventArgs e)
        {
            if(Shuffe.Kind== PackIconKind.Shuffle)
            { 
            int currentSongIndex = -1;
            foreach (var song in Songs)
            {
                if (song.path == currentMediaPath)
                    break;
                else
                    currentSongIndex += 1;
            }
            if (currentSongIndex == -1)
            {
                medium next = Songs.ElementAt(0);
                currentMediaPath = next.path;
                SelectSongToPlay(next);
                next.nowPlaying = PackIconKind.PlayCircleOutline;
                mediaPlayer.Stop();
                Play.Kind = PackIconKind.Play;
                ListViewMediaFiles.ItemsSource = null;
                int elementCounter = 0;
                foreach (var song in Songs)
                {
                    Songs.ElementAt(elementCounter).nowPlaying = PackIconKind.PlayCircleOutline;
                    elementCounter += 1;
                }
                ListViewMediaFiles.ItemsSource = Songs;
            }
            else
            {
                medium prev = Songs.ElementAt(currentSongIndex);
                currentMediaPath = prev.path;
                Medium = new medium();
                Medium = prev;
                NowPlaying();
                Play.Kind = PackIconKind.Play;
                MusicPlay_Pause(sender, e);
            }
        }
            else if (Shuffe.Kind == PackIconKind.ShuffleDisabled)
            {
                Random rnd = new Random();
                int randomIndex = rnd.Next(0, Songs.Count);
                medium now = Songs.ElementAt(randomIndex);
                currentMediaPath = now.path;
                Medium = new medium();
                Medium = now;
                NowPlaying();
                Play.Kind = PackIconKind.Play;
                MusicPlay_Pause(sender, e);
    }
}

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            int currentSongIndex = 0;
            foreach (var song in Songs)
            {
                if (song.path == currentMediaPath)
                    break;
                else
                    currentSongIndex += 1;
            }
            medium now = Songs.ElementAt(currentSongIndex);
            currentMediaPath = now.path;
            Medium = new medium();
            Medium = now;
            NowPlaying();
            Play.Kind = PackIconKind.Play;
            MusicPlay_Pause(sender, e);

        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (Shuffe.Kind == PackIconKind.Shuffle)
            {
                Shuffe.Kind = PackIconKind.ShuffleDisabled;
            }
            else
                Shuffe.Kind = PackIconKind.Shuffle;

        }

        private void TrackSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void TrackSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(TrackSlider.Value);
        }

        private void TrackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            InfoLabel1.Content = TimeSpan.FromSeconds(TrackSlider.Value).ToString(@"hh\:mm\:ss");
        }

        private void PlayNext_Click(object sender, RoutedEventArgs e)
        {
            if (Shuffe.Kind == PackIconKind.Shuffle)
            {
                int currentSongIndex = 0;
                foreach (var song in Songs)
                {
                    currentSongIndex += 1;
                    if (song.path == currentMediaPath || currentSongIndex == Songs.Count)
                        break;
                }
                if (currentSongIndex == Songs.Count)
                {
                    medium next = Songs.ElementAt(0);
                    currentMediaPath = next.path;
                    SelectSongToPlay(next);
                    mediaPlayer.Stop();
                    Play.Kind = PackIconKind.Play;
                }
                else
                {
                    Songs.ElementAt(currentSongIndex);
                    medium next = Songs.ElementAt(currentSongIndex);
                    currentMediaPath = next.path;
                    Medium = new medium();
                    Medium = next;
                    NowPlaying();

                    Play.Kind = PackIconKind.Play;
                    MusicPlay_Pause(sender, e);
                }
            }
            else if(Shuffe.Kind== PackIconKind.ShuffleDisabled)
            {
                Random rnd = new Random();
                int randomIndex = rnd.Next(0, Songs.Count);
                medium now = Songs.ElementAt(randomIndex);
                currentMediaPath = now.path;
                Medium = new medium();
                Medium = now;
                NowPlaying();
                Play.Kind = PackIconKind.Play;
                MusicPlay_Pause(sender, e);
            }
        }

        private bool ArtistExist(medium media)
        {
            int counter = 0;
            using (Model1 model = new Model1())
            {
               
                var all = model.media.Select(m => new {m.artist}).ToList();
                foreach (var citem in all)
                {
                    if (citem.artist.name.Equals(media.artist.name, StringComparison.InvariantCultureIgnoreCase))
                        counter += 1;
                }
            }
            return counter > 0 ? true : false;
        }

        private bool AlbumExist(medium media)
        {
            int counter = 0;
            using (Model1 model = new Model1())
            {

                var all = model.media.Select(m => new { m.album }).ToList();
                foreach (var citem in all)
                {
                    if (citem.album.name.Equals(media.album.name, StringComparison.InvariantCultureIgnoreCase))
                        counter += 1;
                }
            }
            return counter > 0 ? true : false;
        }

        private bool MediaExist(medium media)
        {
            int counter = 0;
            using (Model1 model = new Model1())
            {

                var all = model.media.Select(m => new { m.name }).ToList();
                foreach (var citem in all)
                {
                     if (citem.name.Equals(media.name, StringComparison.InvariantCultureIgnoreCase))
                        counter += 1;
                }
            }
            return counter > 0 ? true : false;
        }

        private void GetImageFrameFromVideo( string path, BitmapImage bitmap)
        {
                ShellFile shellFile = ShellFile.FromFilePath(path);
                Bitmap bm = shellFile.Thumbnail.Bitmap;
                MemoryStream ms = new MemoryStream();
                bm.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                bitmap.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                bitmap.StreamSource = ms;
                bitmap.EndInit();
            }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            playlistPlusWasClicked = true;
            Playlist_Click(sender,e);
        }
        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            fullScreenButtonIsClicked = true;
            mediaElementAndImage.Width = WholeWindow.ActualWidth;
            mediaElementAndImage.Height = WholeWindow.ActualHeight;
            ListOfSongs.Visibility = Visibility.Collapsed;
            grid.Visibility = Visibility.Collapsed;
            Thickness margin = mediaElementAndImage.Margin;
            margin.Top=0;
            margin.Left = 0;
            margin.Right = 0;
            margin.Bottom = 0;
            mediaElementAndImage.Margin = margin;
        }

    }

      
    
}
