using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Data.Contexts;

public class CollectionManagerContext(DbContextOptions<CollectionManagerContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
    public DbSet<PenCollectionEntry> Collections { get; init; }
    public DbSet<PenCatalogueEntry> PenCatalog { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PenCollectionEntry>()
            .HasOne(pce => pce.Pen)
            .WithMany()
            .HasForeignKey(pce => pce.PenId);
        
        base.OnModelCreating(modelBuilder);
    }
}