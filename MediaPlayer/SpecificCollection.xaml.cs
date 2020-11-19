using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SpecificCollection : UserControl
    {
        public ObservableCollection<artist> MyArtistCollection { get; set; }
        public ObservableCollection<album> MyAlbumCollection { get; set; }
        public ObservableCollection<playlist> MyPlaylistCollection { get; set; }
        public album currentAlbum { get; set; }
        public artist currentArtist { get; set; }
        public playlist currentPlaylist { get; set; }

        public bool albumActive { get; set; }
        public bool artistActive { get; set; }
        public bool playlistActive { get; set; }

        public static readonly RoutedEvent SettingConfirmedEvent =
            EventManager.RegisterRoutedEvent("SettingConfirmedEvent", RoutingStrategy.Bubble,typeof(RoutedEventHandler), typeof(SpecificCollection));
   

        public SpecificCollection()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler SettingConfirmed
        {
            add { AddHandler(SettingConfirmedEvent, value); }
            remove { RemoveHandler(SettingConfirmedEvent, value); }
        }
        public void GetArtistCollection()
        {
            artistActive = true;
            albumActive = false;
            playlistActive = false;
            using (Model1 model = new Model1())
            {
                var all = model.artists.Select(m => new {m.id, m.name, m.cover }).Distinct().ToList();
                MyArtistCollection = new ObservableCollection<artist>();
                foreach (var citem in all)
                {
                    MyArtistCollection.Add(new artist() {id=citem.id, name = citem.name, cover = citem.cover });
                }

            }
            FolderUserControl.ItemsSource = MyArtistCollection;
        }
        public void GetAlbumCollection()
        {
            artistActive = false;
            albumActive = true;
            playlistActive = false;
            using (Model1 model = new Model1())
            {
                var all = model.albums.Select(m => new {m.id, m.name, m.cover }).Distinct().ToList();
                MyAlbumCollection = new ObservableCollection<album>();
                foreach (var citem in all)
                {
                    MyAlbumCollection.Add(new album() {id=citem.id, name = citem.name, cover = citem.cover });
                }
            }
           FolderUserControl.ItemsSource = MyAlbumCollection;

        }
        public void GetPlaylistCollection()
        {
            artistActive = false;
            albumActive = false;
            playlistActive = true;
            using (Model1 model = new Model1())
            {
                var all = model.playlists.Select(m => new {m.id, m.name, m.cover }).Distinct().ToList();
                MyPlaylistCollection = new ObservableCollection<playlist>();
                foreach (var citem in all)
                {
                    MyPlaylistCollection.Add(new playlist() {id=citem.id, name = citem.name, cover = citem.cover });
                }

            }
            FolderUserControl.ItemsSource = MyPlaylistCollection;
        }

        public void GetSongs_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (artistActive)
            {
                var dataObject = btn.DataContext as artist;
                currentArtist = new artist() { name = dataObject.name, id = dataObject.id };
            }
            else if (albumActive)
            {
                var dataObject = btn.DataContext as album;
                currentAlbum = new album() { name = dataObject.name, id = dataObject.id };
            }
            else
            {
                var dataObject = btn.DataContext as playlist;
                currentPlaylist = new playlist() { name = dataObject.name, id = dataObject.id };
            }
            RaiseEvent(new RoutedEventArgs(SpecificCollection.SettingConfirmedEvent));
        }
        
    }
}
