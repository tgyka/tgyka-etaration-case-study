using EterationCaseStudy.Application.Common.Pagination;
using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using MediatR;

namespace EterationCaseStudy.Application.Features.Products.Queries
{
    public record GetProductsQuery(
        int Page = 1,
        int PageSize = 10,
        string? NameContains = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        string? SortBy = null,
        string? SortDir = null
    ) : IRequest<PagedResult<Product>>;

    public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<Product>>
    {
        private readonly IProductRepository _products;

        public GetProductsHandler(IProductRepository products)
        {
            _products = products;
        }

        public Task<PagedResult<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _products.Query().Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(request.NameContains))
            {
                var term = request.NameContains.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            var sortBy = (request.SortBy ?? "").ToLowerInvariant();
            var sortDir = (request.SortDir ?? "asc").ToLowerInvariant();
            bool desc = sortDir == "desc";

            query = sortBy switch
            {
                "price" => desc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "name" => desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                _ => desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id)
            };

            int total = query.Count();
            int skip = Math.Max(0, (request.Page - 1) * request.PageSize);
            var items = query.Skip(skip).Take(request.PageSize).ToList();

            var result = new PagedResult<Product>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = total,
                Items = items
            };
            return Task.FromResult(result);
        }
    }
}
