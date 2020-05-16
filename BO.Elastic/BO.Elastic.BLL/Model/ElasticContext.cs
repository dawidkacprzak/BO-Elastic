using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BO.Elastic.BLL.Model
{
    public partial class ElasticContext : DbContext
    {
        public ElasticContext()
        {
        }

        public ElasticContext(DbContextOptions<ElasticContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ClusterNode> ClusterNode { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<ServiceType> ServiceType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=10.10.1.214;Database=Elastic; User Id=Panel; Password=qwerty;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClusterNode>(entity =>
            {
                entity.HasIndex(e => e.NodeId)
                    .HasName("XI_NodeId")
                    .IsUnique();

                entity.HasOne(d => d.Cluster)
                    .WithMany(p => p.ClusterNodeCluster)
                    .HasForeignKey(d => d.ClusterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceIdCluster");

                entity.HasOne(d => d.Node)
                    .WithOne(p => p.ClusterNodeNode)
                    .HasForeignKey<ClusterNode>(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceIdNode");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasIndex(e => e.ServiceType)
                    .HasName("IX_ServiceType");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Port)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.HasOne(d => d.ServiceTypeNavigation)
                    .WithMany(p => p.Service)
                    .HasForeignKey(d => d.ServiceType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceType");
            });

            modelBuilder.Entity<ServiceType>(entity =>
            {
                entity.HasIndex(e => e.Type)
                    .HasName("UQ__ServiceT__F9B8A48BE8DFA8E9")
                    .IsUnique();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
