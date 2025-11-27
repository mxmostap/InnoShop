using MediatR;
using ProductManagement.Application.Common.Interfaces;

namespace ProductManagement.Application.Commands;

public class DeleteProductsByUserIdCommand : IRequest<Unit>, IIdCommand
{
    public int Id { get; set; }
}