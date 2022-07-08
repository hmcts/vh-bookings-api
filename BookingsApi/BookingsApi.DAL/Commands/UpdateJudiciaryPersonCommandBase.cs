using System;
using BookingsApi.DAL.Commands.Core;

namespace BookingsApi.DAL.Commands
{

    public abstract class UpdateJudiciaryPersonCommandBase : ICommand
    {
        protected UpdateJudiciaryPersonCommandBase(string externalRefId, bool hasLeft)
        {
            ExternalRefId = externalRefId;
            HasLeft = hasLeft;
        }

        public string ExternalRefId { get; }
        public bool HasLeft { get; set; }
    }
}