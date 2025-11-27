using MediatR;
using ProductManagement.Application.Common.Interfaces;

namespace ProductManagement.Application.Commands;

public class DeleteProductCommand : IRequest<Unit>, IIdCommand
{
    public int Id { get; set; }
}