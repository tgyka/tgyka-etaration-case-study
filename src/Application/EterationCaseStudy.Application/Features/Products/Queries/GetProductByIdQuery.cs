using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using MediatR;

namespace EterationCaseStudy.Application.Features.Products.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<Product?>;

    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
    {
        private readonly IProductRepository _products;

        public GetProductByIdHandler(IProductRepository products) => _products = products;

        public Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return _products.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}