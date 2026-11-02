using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Theatre> Theatres { get; set; }

        public DbSet<Screen> Screens { get; set; }

        public DbSet<Seat> Seats { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowSeat> ShowSeats { get; set; }
        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Movie
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(m => m.MovieId);

                entity.Property(m => m.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(m => m.Description)
                    .HasMaxLength(1000);

                entity.Property(m => m.Language)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(m => m.Genre)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Theatre
            modelBuilder.Entity<Theatre>(entity =>
            {
                entity.HasKey(t => t.TheatreId);

                entity.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(t => t.Location)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            // Screen
            modelBuilder.Entity<Screen>(entity =>
            {
                entity.HasKey(s => s.ScreenId);

                entity.Property(s => s.ScreenName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(s => s.Theatre)
                    .WithMany()
                    .HasForeignKey(s => s.TheatreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s =>
                    new
                    {
                        s.TheatreId,
                        s.ScreenName
                    })
                    .IsUnique();
            });

            // Seat
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(s => s.SeatId);

                entity.Property(s => s.RowName)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(s => s.SeatNumber)
                    .IsRequired();

                entity.Property(s => s.SeatType)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(s => s.Screen)
                    .WithMany(s => s.Seats)
                    .HasForeignKey(s => s.ScreenId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s =>
                    new
                    {
                        s.ScreenId,
                        s.RowName,
                        s.SeatNumber
                    })
                    .IsUnique();
            });

            modelBuilder.Entity<Show>(entity =>
            {
                entity.HasKey(s => s.ShowId);

                entity.Property(s => s.StartTime)
                    .IsRequired();

                entity.Property(s => s.EndTime)
                    .IsRequired();

                entity.Property(s => s.TicketPrice)
                    .IsRequired()
                    .HasPrecision(10, 2);

                entity.HasOne(s => s.Movie)
                    .WithMany()
                    .HasForeignKey(s => s.MovieId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Screen)
                    .WithMany()
                    .HasForeignKey(s => s.ScreenId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ShowSeat>(entity =>
            {
                entity.HasKey(ss => ss.ShowSeatId);

                entity.Property(ss => ss.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(ss => ss.Show)
                    .WithMany(s => s.ShowSeats)
                    .HasForeignKey(ss => ss.ShowId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ss => ss.Seat)
                    .WithMany()
                    .HasForeignKey(ss => ss.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ss => new { ss.ShowId, ss.SeatId })
                    .IsUnique();
            });
        }
    }
}