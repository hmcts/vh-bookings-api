using System.Threading.Tasks;

namespace Bookings.DAL.Commands
{
    public interface ICommandHandler
    {
        void Handle<TCommand>(TCommand command) where TCommand : ICommand;
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}