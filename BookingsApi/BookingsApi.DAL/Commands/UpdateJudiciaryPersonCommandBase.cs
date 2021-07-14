using System;
using BookingsApi.DAL.Commands.Core;

namespace BookingsApi.DAL.Commands
{

    public abstract class UpdateJudiciaryPersonCommandBase : ICommand
    {
        protected UpdateJudiciaryPersonCommandBase(Guid externalRefId, bool hasLeft)
        {
            ExternalRefId = externalRefId;
            HasLeft = hasLeft;
        }

        public Guid ExternalRefId { get; }
        public bool HasLeft { get; set; }
    }
}