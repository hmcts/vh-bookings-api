using System.Threading.Tasks;

namespace BookingsApi.DAL.Commands.Core
{
    public interface ICommandHandler
    {
        Task Handle<TCommand>(TCommand command) where TCommand : ICommand;
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}