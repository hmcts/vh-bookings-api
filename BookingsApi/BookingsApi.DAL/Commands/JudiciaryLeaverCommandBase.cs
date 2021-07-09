using System;
using BookingsApi.DAL.Commands.Core;

namespace BookingsApi.DAL.Commands
{
    public abstract class JudiciaryLeaverCommandBase : ICommand
    {
        protected JudiciaryLeaverCommandBase(Guid externalRefId, bool leaver)
        {
            ExternalRefId = externalRefId;
            HasLeft = leaver;
        }
        
        public Guid ExternalRefId { get; }
        public bool HasLeft { get; set; }
    }
}