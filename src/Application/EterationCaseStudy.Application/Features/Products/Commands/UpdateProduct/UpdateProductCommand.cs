using EterationCaseStudy.Domain.Repositories;
using MediatR;

namespace EterationCaseStudy.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(int Id, string Name, decimal Price, int StockQuantity, string? Description) : IRequest<bool>;

    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _products;
        private readonly IUnitOfWork _uow;

        public UpdateProductHandler(IProductRepository products, IUnitOfWork uow)
        {
            _products = products;
            _uow = uow;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _products.GetByIdAsync(request.Id, cancellationToken);

            if (product == null) return false;

            product.UpdateDetails(request.Name, request.Description ?? string.Empty);
            product.UpdatePrice(request.Price);

            if (request.StockQuantity > 0) product.SetStock(request.StockQuantity);

            _products.Update(product);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

