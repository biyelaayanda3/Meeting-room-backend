using MeetingRoomBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<User> Users { get; set; }

    //This hook tells EF Core how to build our DB, when the application starts up
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id); //Primary key
            entity.Property(r => r.Name)
                .IsRequired() //Not null in DB
                .HasMaxLength(100);
            entity.Property(r => r.Description)
                .HasMaxLength(500);
            entity.HasMany(r => r.Bookings) // One room can have many bookings
                .WithOne(b => b.Room) // Each Booking has one Room
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict); // Cant delete a Room if it has Bookings
        });

       
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(b => b.OrganizerName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(b => b.OrganizerEmail)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(b => b.Status)
                .HasConversion<string>(); // Store enum as string in DB
        });

        //for JWT
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);
            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        // SEED DATA
        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = 1,
                Name = "Board Room",
                Description = "Main boardroom with projector and video conferencing",
                Capacity = 20,
                IsActive = true
            },
            new Room
            {
                Id = 2,
                Name = "Innovation Hub",
                Description = "Creative space with whiteboards and flexible seating",
                Capacity = 10,
                IsActive = true
            },
            new Room
            {
                Id = 3,
                Name = "Focus Room A",
                Description = "Small quiet room for focused work or 1-on-1 meetings",
                Capacity = 4,
                IsActive = true
            },
            new Room
            {
                Id = 4,
                Name = "Focus Room B",
                Description = "Small quiet room for focused work or 1-on-1 meetings",
                Capacity = 4,
                IsActive = true
            },
            new Room
            {
                Id = 5,
                Name = "Training Room",
                Description = "Large room for training sessions and workshops",
                Capacity = 30,
                IsActive = true
            }
        );
    }
}