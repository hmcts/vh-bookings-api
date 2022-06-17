using System;
using BookingsApi.DAL.Commands.Core;

namespace BookingsApi.DAL.Commands
{
    public abstract class JudiciaryLeaverCommandBase : ICommand
    {
        protected JudiciaryLeaverCommandBase(string externalRefId, bool leaver)
        {
            ExternalRefId = externalRefId;
            HasLeft = leaver;
        }
        
        public string ExternalRefId { get; }
        public bool HasLeft { get; set; }
    }
}