using JwtMusic.WebApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtMusic.WebApi.Context
{
    public class JwtContext : IdentityDbContext<AppUser>
    {
        public JwtContext(DbContextOptions<JwtContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Genre>().HasIndex(x => x.Name).IsUnique();
            builder.Entity<Playlist>().HasMany(x => x.Songs).WithMany(x => x.Playlists);
            builder.Entity<ListeningHistory>().HasIndex(x => new { x.AppUserId, x.SongId });
            builder.Entity<Song>().HasOne(x => x.Album).WithMany(x => x.Songs)
                .HasForeignKey(x => x.AlbumId).OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<ListeningHistory> ListeningHistory { get; set; }
    }
}
