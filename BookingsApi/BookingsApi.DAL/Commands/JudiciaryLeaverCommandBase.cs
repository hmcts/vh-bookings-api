using System;
using BookingsApi.DAL.Commands.Core;

namespace BookingsApi.DAL.Commands
{
    public abstract class JudiciaryLeaverCommandBase : ICommand
    {
        protected JudiciaryLeaverCommandBase(string personalCode, bool leaver)
        {
            PersonalCode = personalCode;
            HasLeft = leaver;
        }
        
        public string PersonalCode { get; }
        public bool HasLeft { get; set; }
    }
}