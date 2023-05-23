using Microsoft.EntityFrameworkCore;

namespace OrderServer.Model
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
            
        }

        // These should perhaps be in separate contexts, but for now they are in the same one. 
        public DbSet<Order> Orders { get; set; }

        public DbSet<Item> Items { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            // Primary keys - For now, we will put them as autoincremented Int. 
            // Had we assume that this data would ship to customers, I would have considered using Guids instead. 
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.ItemId, oi.OrderId});

            modelBuilder.Entity<Item>()
                .HasKey(i => i.ItemId);

            modelBuilder.Entity<Item>()
                .Property(oi => oi.ItemId)
                .ValueGeneratedOnAdd();

            // Apparently this isn't enforced when working with in-memory solutions :(
            modelBuilder.Entity<Item>()
                .HasIndex(i => i.Name)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderId);

            modelBuilder.Entity<Order>()
                .Property(oi => oi.OrderId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Item)
                .WithMany()
                .HasForeignKey(oi => oi.ItemId);
        }


    }
}
