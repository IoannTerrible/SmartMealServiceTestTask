using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmsClientLibrary.Database.Entities;

namespace SmsClientLibrary.Database.EntityConfigurations;

public class DishEntityConfiguration
    : IEntityTypeConfiguration<DishEntity>
{
    public void Configure(EntityTypeBuilder<DishEntity> builder)
    {
        builder.ToTable("dishes");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
               .HasColumnName("id");

        builder.Property(d => d.Article)
               .HasColumnName("article")
               .IsRequired();

        builder.Property(d => d.Name)
               .HasColumnName("name")
               .IsRequired();

        builder.Property(d => d.Price)
               .HasColumnName("price");

        builder.Property(d => d.IsWeighted)
               .HasColumnName("is_weighted");

        builder.Property(d => d.FullPath)
               .HasColumnName("full_path")
               .IsRequired();

        //Для демонстрации сделан json
        builder.Property(d => d.Barcodes)
               .HasColumnName("barcodes")
               .HasColumnType("jsonb");

        builder.HasIndex(d => d.Article)
               .HasDatabaseName("IX_Dishes_Article");

    }
}