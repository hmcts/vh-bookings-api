namespace BookingsApi.DAL.Commands;

public class UpsertLanguageDto(string code, string value, string welshValue, InterpreterType type, bool live)
{
    public string Code { get; } = code;
    public string Value { get; } = value;
    public string WelshValue { get; } = welshValue;
    public InterpreterType Type { get; } = type;
    public bool Live { get; } = live;
}

public class UpsertInterpreterLanguagesCommand(List<UpsertLanguageDto> interpreterLanguages) : ICommand
{
    public List<UpsertLanguageDto> InterpreterLanguages { get; } = interpreterLanguages;

    public class UpsertInterpreterLanguagesCommandHandler(BookingsDbContext context)
        : ICommandHandler<UpsertInterpreterLanguagesCommand>
    {
        public async Task Handle(UpsertInterpreterLanguagesCommand command)
        {
            // get all languages from the database
            var languages = await context.InterpreterLanguages.ToListAsync();

            foreach (var proposedLanguage in command.InterpreterLanguages)
            {
                // if code does not exist, add it
                if (!languages.Exists(x => x.Code == proposedLanguage.Code))
                {
                    await context.InterpreterLanguages.AddAsync(new InterpreterLanguage(proposedLanguage.Code,
                        proposedLanguage.Value, proposedLanguage.WelshValue, proposedLanguage.Type,
                        proposedLanguage.Live));
                }
                else
                {
                    // if the language is not active then set it to inactive
                    var existingLanguage = languages.First(x => x.Code == proposedLanguage.Code);
                    existingLanguage.UpdateLanguageDetails(proposedLanguage.Value, proposedLanguage.WelshValue, proposedLanguage.Live, proposedLanguage.Type);
                }
            }
            // find all codes in the database that are not in the command and deactivate them
            var codesToDeactivate = languages.Select(x => x.Code).Except(command.InterpreterLanguages.Select(x => x.Code));
            foreach (var code in codesToDeactivate)
            {
                var language = languages.First(x => x.Code == code);
                language.Deactivate();
            }
        }
    }
}