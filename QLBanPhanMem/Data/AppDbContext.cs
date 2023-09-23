using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QLBanPhanMem.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<PhanMemModel> PhanMems { get; set; }
    public DbSet<NhaPhatHanhModel> NhaPhatHanhs { get; set; }
    public DbSet<LoaiPM> LoaiPMs { get; set; }
    public DbSet<ThuocLoaiPM> ThuocLoaiPMs { get; set; }
    public DbSet<AccountModel> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PhanMemModel>()
            
            .HasOne(p => p.NhaPhatHanh)
            .WithMany()
            .HasForeignKey(p => p.MANPH);

        modelBuilder.Entity<ThuocLoaiPM>()
            
            .HasKey(tlp => new { tlp.MAPM, tlp.MALOAI });

        modelBuilder.Entity<ThuocLoaiPM>()
            .HasOne(tlp => tlp.PhanMem)
            .WithMany()
            .HasForeignKey(tlp => tlp.MAPM);

        modelBuilder.Entity<ThuocLoaiPM>()
            .HasOne(tlp => tlp.LoaiPM)
            .WithMany()
            .HasForeignKey(tlp => tlp.MALOAI);
    }
}
