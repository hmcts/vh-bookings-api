using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;

namespace Testing.Common.Builders.Domain
{
    public class JudgeBuilder
    {
        private readonly Judge _judge;
        
        public JudgeBuilder()
        {
            var judgeCaseRole = new CaseRole(5, "Judge") { Group = CaseRoleGroup.Judge };
            var judgeHearingRole = new HearingRole((int)HearingRoleIds.Judge, "Judge") { UserRole = new UserRole(1, "Judge") };
            var judgePerson = new PersonBuilder(true).Build();
            _judge = new Judge(judgePerson, judgeHearingRole, judgeCaseRole)
            {
                DisplayName = $"{judgePerson.FirstName} {judgePerson.LastName}"
            };
            _judge.SetProtected(nameof(_judge.CaseRole), judgeCaseRole);
            _judge.SetProtected(nameof(_judge.HearingRole), judgeHearingRole);
        }
        
        public Judge Build()
        {
            return _judge;
        }
    }
}
