namespace MediaPlayer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=ModelMP")
        {
        }

        public virtual DbSet<album> albums { get; set; }
        public virtual DbSet<artist> artists { get; set; }
        public virtual DbSet<medium> media { get; set; }
        public virtual DbSet<playlist> playlists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<album>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<album>()
                .Property(e => e.cover)
                .IsUnicode(false);

            modelBuilder.Entity<album>()
                .HasMany(e => e.media)
                .WithRequired(e => e.album)
                .HasForeignKey(e => e.album_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<artist>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<artist>()
                .Property(e => e.cover)
                .IsUnicode(false);

            modelBuilder.Entity<artist>()
                .HasMany(e => e.media)
                .WithRequired(e => e.artist)
                .HasForeignKey(e => e.artist_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<medium>()
                .Property(e => e.path)
                .IsUnicode(false);

            modelBuilder.Entity<medium>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<medium>()
                .Property(e => e.duration)
                .IsUnicode(false);

            modelBuilder.Entity<medium>()
                .Property(e => e.extension)
                .IsUnicode(false);

            modelBuilder.Entity<medium>()
                .HasMany(e => e.playlists)
                .WithMany(e => e.media)
                .Map(m => m.ToTable("mp", "mp").MapLeftKey("media_id"));

            modelBuilder.Entity<playlist>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<playlist>()
                .Property(e => e.cover)
                .IsUnicode(false);
        }
    }
}
