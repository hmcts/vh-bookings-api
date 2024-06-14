namespace BookingsApi.DAL.Mappings;

public class InterpreterLanguageMap : IEntityTypeConfiguration<InterpreterLanguage>
{
    public void Configure(EntityTypeBuilder<InterpreterLanguage> builder)
    {
        builder.ToTable(nameof(InterpreterLanguage));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired();
        builder.Property(x => x.Value).IsRequired();
        builder.Property(x => x.WelshValue);
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Live).IsRequired().HasDefaultValue(true);
    }
}