using System;

namespace BookingsApi.UnitTests.Utilities
{
    /// <summary>
    /// Base class providing a number of useful helper methods
    /// </summary>
    public abstract class TestBase
    {
        protected TestBase()
        {
        }
        
        /// <summary>Help wrapper to build catch clauses</summary>
        protected static Action When(Action action)
        {
            return action;
        }
    }
}