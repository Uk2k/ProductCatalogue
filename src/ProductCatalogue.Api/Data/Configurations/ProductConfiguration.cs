using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalogue.Api.Features.Products;

namespace ProductCatalogue.Api.Data.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(
            "Products",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Products_Price_Positive",
                    "[Price] > 0");
                tableBuilder.HasCheckConstraint(
                    "CK_Products_Stock_NonNegative",
                    "[Stock] >= 0");
            });

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id)
            .ValueGeneratedOnAdd();

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(product => product.Price)
            .HasPrecision(18, 2);

        builder.Property(product => product.Stock)
            .IsRequired();
    }
}
