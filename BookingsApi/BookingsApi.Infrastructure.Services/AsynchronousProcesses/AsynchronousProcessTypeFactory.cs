//using System.Collections.Generic;
//using System.Linq;

//namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
//{
//    public interface IAsynchronousProcessTypeFactory
//    {
//        IBookingAsynchronousProcess Get(AsynchronousProcessType type);
//    }

//    public class AsynchronousProcessTypeFactory: IAsynchronousProcessTypeFactory
//    {
//        private readonly IEnumerable<IBookingAsynchronousProcess> _bookingAsynchronousProcesses;
//        public AsynchronousProcessTypeFactory(IEnumerable<IBookingAsynchronousProcess> bookingAsynchronousProcesses)
//        {
//            _bookingAsynchronousProcesses = bookingAsynchronousProcesses;
//        }

//        public IBookingAsynchronousProcess Get(AsynchronousProcessType type)
//        {
//            return _bookingAsynchronousProcesses.Single(x => x.Type == type);
//        }
//    }
//}
