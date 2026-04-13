```mermaid
graph TD
    A["SendForSignatureAction.RunAsync"] --> B["IAutentiApiService"]
    B --> C["AutentiApiService"]
    C --> D["IDocumentProcessRequestBuilder"]
    D --> E["DocumentProcessRequestBuilder"]
    E --> F["IParticipantsProvider"]
    E --> G["IAttachmentsProvider"]
    E --> R["OrganizationSenderProvider"]
    C --> H["IAutentiHttpClient"]
    H --> I["AutentiHttpClient"]
    C --> Q["AutentiClientProvider"]
    F --> J["ParticipantsProvider"]
    J --> K["IPartyEnricher[ ]"]

    K --> L["CoreDataEnricher
        (name, email, role, contacts)"]
    K --> M["SignatureEnricher
        (signature type constraints)"]
    K --> N["SmsEnricher
        (SMS auth & document unlock)"]
    K --> O["SigningOrderEnricher
        (participation priority)"]
    K --> P["OrganizationRepresentativeEnricher
        (org name, VAT, representative)"]
```