using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.IntegrationTests.Models;
using TechTalk.SpecFlow;

namespace BookingsApi.IntegrationTests.Transformations
{
    [Binding]
    public class Transforms
    {
        [StepArgumentTransformation]
        public IDictionary<string, IEnumerable<string>> TableCaseTypeHearingTypes(Table table)
        {
            return table.Rows.Select(row => new CaseHearingType
            {
                CaseTypeName = row["CaseTypeName"],
                HearingTypeName = row["HearingTypeName"].Split(",", StringSplitOptions.RemoveEmptyEntries)
            }).ToDictionary(x => x.CaseTypeName, x => x.HearingTypeName);
        }
    }
}