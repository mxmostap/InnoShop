using MediatR;
using ProductManagement.Application.Common.Interfaces;

namespace ProductManagement.Application.Commands;

public class SoftDeleteProductsCommand : IRequest<Unit>, IIdCommand
{
    public int Id { get; set; }
}