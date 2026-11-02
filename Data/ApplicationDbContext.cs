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
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

                entity.HasIndex(s => new { s.TheatreId, s.ScreenName })
                    .IsUnique();
            });

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

                entity.HasIndex(ss =>
                    new
                    {
                        ss.ShowId,
                        ss.SeatId
                    })
                    .IsUnique();
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingId);

                entity.Property(b => b.BookingReference)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(b => b.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(b => b.CustomerEmail)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(b => b.CustomerPhone)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(b => b.BookingDate)
                    .IsRequired();

                entity.Property(b => b.TotalAmount)
                    .IsRequired()
                    .HasPrecision(10, 2);

                entity.Property(b => b.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(b => b.BookingReference)
                    .IsUnique();

                entity.HasOne(b => b.Show)
                    .WithMany()
                    .HasForeignKey(b => b.ShowId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BookingSeat>(entity =>
            {
                entity.HasKey(bs => bs.BookingSeatId);

                entity.Property(bs => bs.Price)
                    .IsRequired()
                    .HasPrecision(10, 2);

                entity.HasOne(bs => bs.Booking)
                    .WithMany(b => b.BookingSeats)
                    .HasForeignKey(bs => bs.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bs => bs.Seat)
                    .WithMany()
                    .HasForeignKey(bs => bs.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasIndex(u => u.Username)
                    .IsUnique();

                entity.HasIndex(u => u.Email)
                    .IsUnique();
            });
        }
    }
}