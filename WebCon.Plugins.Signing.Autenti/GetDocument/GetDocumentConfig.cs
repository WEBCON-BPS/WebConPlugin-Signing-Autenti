using WebCon.Plugins.Signing.Autenti.Common;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.Plugins.Signing.Autenti.GetDocument;

public class GetDocumentConfig : PluginConfiguration
{
    [ConfigStudioTranslation("Uwierzytelnienie", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Authorization", Order = 0)]
    public AuthorizationConfig Authorization { get; set; }

    [ConfigStudioTranslation("Dane wejściowe", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Input", Order = 1)]
    public DocumentStatusInput Input { get; set; }

    [ConfigStudioTranslation("Dane wyjściowe", TranslationCulture.plPL)]
    [ConfigGroupBox(DisplayName = "Response", Order = 2)]
    public AttachmentResponse Response { get; set; }
}

public class AttachmentResponse
{
    [ConfigStudioTranslation("Nazwa pliku", TranslationCulture.plPL)]
    [ConfigEditableText("File name")]
    public string FileName { get; set; }

    [ConfigStudioTranslation("Opis pliku", TranslationCulture.plPL)]
    [ConfigEditableText("File description", Lines = 5)]
    public string FileDescription { get; set; }

    [ConfigStudioTranslation("Kategoria", TranslationCulture.plPL)]
    [ConfigEditableEnum("Category")]
    public Category CategoryMode { get; set; }

    [ConfigStudioTranslation("Dynamiczna", TranslationCulture.plPL)]
    [ConfigEditableText("Dynamic")]
    public string DynamicCategory { get; set; }

    [ConfigStudioTranslation("Nadpisz załącznik o tej samej nazwie", TranslationCulture.plPL)]
    [ConfigEditableBool("Overwrite attachment with the same name")]
    public bool OverwriteAttachment { get; set; }

    [ConfigStudioTranslation("Pobierz kartę podpisów", TranslationCulture.plPL, Description = "Karta podpisów jest zapisywana jako osobny załącznik. Autenti udostępnia ten plik wyłącznie dla procesów z wizualizacją \"Podpis w dowolnym miejscu wybranym przez odbiorcę\".")]
    [ConfigEditableBool("Download signature card", DefaultValue = false, Description = "The signature card is saved as a separate attachment. Autenti provides this file only for processes with the \"Signature in any place chosen by the recipient\" visualisation.")]
    public bool DownloadSignatureCard { get; set; }

    [ConfigStudioTranslation("Nazwa pliku z kartą podpisów", TranslationCulture.plPL)]
    [ConfigEditableText("Signature card file name")]
    public string FileNameSignCard { get; set; }
}

public enum Category
{
    None = 0,
    Dynamic = 1,
}