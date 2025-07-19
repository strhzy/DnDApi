using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DnDAPI.Models;

public class CharacterSheetDocument : IDocument
{
    private readonly PlayerCharacter _character;
    private readonly List<Attack> _attacks;

    public CharacterSheetDocument(PlayerCharacter character, List<Attack> attacks)
    {
        _character = character;
        _attacks = attacks;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(0);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Content().Stack(stack =>
            {
                var templatePath = Path.Combine(AppContext.BaseDirectory, "Assets", "CharacterSheetTemplate.png");
                // Шаблонный фон
                stack.Item().Image(templatePath, ImageScaling.FitArea);

                // Текст поверх шаблона
                stack.Item().Element(ComposeOverlayContent);
            });
        });
    }

    void ComposeOverlayContent(IContainer container)
    {
        container.PaddingLeft(80).PaddingTop(40).Column(column =>
        {
            column.Item().Text(_character.Name).FontSize(14).SemiBold();
            column.Item().Text($"Level {_character.Level} {_character.Race} {_character.ClassType}");
            column.Item().Text($"Player: {_character.PlayerName}");

            column.Item().PaddingTop(15).Text($"HP: {_character.CurrentHitPoints}/{_character.MaxHitPoints}  |  AC: {_character.ArmorClass}  |  Initiative: {_character.Initiative}");

            column.Item().PaddingTop(10).Text("Abilities:");
            column.Item().Text($"STR: {_character.Strength}, DEX: {_character.Dexterity}, CON: {_character.Constitution}, INT: {_character.Intelligence}, WIS: {_character.Wisdom}, CHA: {_character.Charisma}");

            column.Item().PaddingTop(10).Text("Attacks:");
            foreach (var attack in _attacks)
            {
                column.Item().Text($"{attack.Name} ({attack.DamageDice}) +{attack.AttackBonus}");
            }

            column.Item().PaddingTop(10).Text("Traits:");
            column.Item().Text(_character.FeaturesAndTraits).FontSize(9);
        });
    }

    public byte[] GeneratePdf()
    {
        using var stream = new MemoryStream();
        this.GeneratePdf(stream);
        return stream.ToArray();
    }
}
