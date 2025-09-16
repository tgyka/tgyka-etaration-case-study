using EterationCaseStudy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EterationCaseStudy.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(x => x.Id);

            builder.Property<int>("OrderId").IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Quantity).IsRequired();

            builder.Ignore(x => x.Subtotal);
        }
    }
}