using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using MediatR;

namespace EterationCaseStudy.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(string Name, decimal Price, int StockQuantity, string? Description) : IRequest<int>;

    public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IProductRepository _products;
        private readonly IUnitOfWork _uow;

        public CreateProductHandler(IProductRepository products, IUnitOfWork uow)
        {
            _products = products;
            _uow = uow;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Name, request.Price, request.StockQuantity, request.Description);
            await _products.AddAsync(product, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}

