namespace AudioStudy.Bot.SharedUtils.Localization.LocalizationSource
{
    public interface ILocalizationSource
    {
        string GetKey(string locale, string key);
    }
}