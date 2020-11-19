namespace MediaPlayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Windows.Media.Imaging;

    [Table("mp.media")]
    public partial class medium
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public medium()
        {
            playlists = new HashSet<playlist>();
            iconKind = MaterialDesignThemes.Wpf.PackIconKind.HeartOutline;
        }

        [Column(TypeName = "uint")]
        public long id { get; set; }

        [Required]
        [StringLength(255)]
        public string path { get; set; }

        [StringLength(255)]
        public string name { get; set; }

        [StringLength(45)]
        public string duration { get; set; }

        public sbyte? favorite { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? heard { get; set; }

        [Required]
        [StringLength(10)]
        public string extension { get; set; }

        [Column(TypeName = "uint")]
        public long album_id { get; set; }

        [Column(TypeName = "uint")]
        public long artist_id { get; set; }

        public virtual album album { get; set; }

        public virtual artist artist { get; set; }

        [NotMapped]
        public MaterialDesignThemes.Wpf.PackIconKind iconKind { get; set; }
        [NotMapped]
        public BitmapImage cover { get; set; }
        [NotMapped]
        public int number { get; set; }
        [NotMapped]
        public MaterialDesignThemes.Wpf.PackIconKind nowPlaying { get; set; } = MaterialDesignThemes.Wpf.PackIconKind.PlayCircleOutline;

        public void SetNowPlaying(MaterialDesignThemes.Wpf.PackIconKind iconKind)
        {
            nowPlaying = iconKind;
        }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ForeignKey("playlist_id")]
        public virtual ICollection<playlist> playlists { get; set; }
    }
}
