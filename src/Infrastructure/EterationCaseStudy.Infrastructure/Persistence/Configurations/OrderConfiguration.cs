using EterationCaseStudy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EterationCaseStudy.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderNumber).IsRequired().HasMaxLength(32);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.UserId).IsRequired();

            builder.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
