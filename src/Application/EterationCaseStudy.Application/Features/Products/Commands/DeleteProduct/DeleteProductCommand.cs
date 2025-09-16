using EterationCaseStudy.Domain.Repositories;
using MediatR;

namespace EterationCaseStudy.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(int Id) : IRequest<bool>;

    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _products;
        private readonly IUnitOfWork _uow;

        public DeleteProductHandler(IProductRepository products, IUnitOfWork uow)
        {
            _products = products;
            _uow = uow;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _products.GetByIdAsync(request.Id, cancellationToken);
            if (product == null) return false;
            _products.Remove(product);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

