namespace Bookings.DAL.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private readonly ICommandHandlerFactory _queryHandlerFactory;

        public CommandHandler(ICommandHandlerFactory queryHandlerFactory)
        {
            _queryHandlerFactory = queryHandlerFactory;
        }

        public void Handle<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = _queryHandlerFactory.Create(command);
            handler.Handle(command);
        }
    }
}