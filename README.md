# TestvaerkstedetToolkit — Udviklerhåndbog

## Hvad er projektet?
TestvaerkstedetToolkit er et Windows desktop-program skrevet i C# (.NET 8, Windows Forms), der hjælper med at transformere og kvalitetssikre arkivdata i XML- og CSV-format. 
Programmet er målrettet til behandling af afleveringspakker (FD) og arkiveringsversioner (AVID) ift. Bekendtgørelse nr. 128.

Programmet indeholder fire specialiserede værktøjer:

Værktøj	Beskrivelse
- XML Table Splitter	Opdeler store XML-tabeller i mindre deltabeller og opdaterer tableIndex.xml automatisk
- XML FK Repair	Reparerer referentiel integritet i XML-datasæt (manglende fremmednøgler)
- CSV FK Repair	Samme som XML FK Repair, men for CSV-filer
- XML Conversion	Konverterer CSV/tekst-data til XML-format

Alle fire værktøjer tilgås fra et centralt dashboard (WelcomeForm).

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Teknisk stack
- Sprog: C# med .NET 8
- UI-framework: Windows Forms
- Krav til OS: Windows (net8.0-windows)
- Eksterne NuGet-pakker: Ingen — kun standard .NET-biblioteker
- Build-output: Single-file self-contained .exe (win-x64)

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Projektstruktur
TestvaerkstedetToolkit/
│
├── Program.cs
│
├── WelcomeForm.cs
├── XMLTableSplitterForm.cs
├── XMLFKRepairForm.cs
│   └── ScalableXmlFKRepair.cs
├── XMLConversionForm.cs
├── CSVFKRepairForm.cs
│ 
├── Controls/
│   └── CompositePKSelector.cs
│
├── Services/
│   ├── XMLSplitService.cs
│   ├── XMLFileGenerationService.cs
│   ├── TableIndexService.cs
│   ├── SplitConfigurationService.cs
│   ├── PrimaryKeyAnalysisService.cs
│   ├── CompositePKSplitAlgorithm.cs
│   └── AutoIDManager.cs
│
├── Models/
│   ├── UIDataContainer.cs
│   ├── TableIndexEntry.cs
│   ├── TableIndexColumn.cs
│   ├── PrimaryKeyInfo.cs
│   ├── SplitTable.cs
│   ├── SplitConfiguration.cs
│   ├── SplitResult.cs
│   ├── AnalysisResult.cs
│   ├── ForeignKeyInfo.cs
│   └── XMLColumn.cs
│
├── Utilities/
│   ├── TableIndexParser.cs
│   ├── XMLHelper.cs
│   ├── XMLNamespaceHelper.cs
│   ├── DataTypeMapper.cs
│   ├── SplitGenerator.cs
│   ├── SplitLogger.cs
│   └── FileSystemHelper.cs
│
└── Properties/
    ├── Settings.Designer.cs
    └── Resources.Designer.cs

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Arkitektur
Projektet følger en service-orienteret arkitektur med klar lagdeling:

- Forms (View-lag): Windows Forms-klasser håndterer UI og brugerinteraktion. Indeholder minimal logik — delegerer til services.
- Services (Forretningslag): Alle tunge operationer foregår her. XMLSplitService fungerer som overordnet orkestrator.
- Models: Rene datastrukturer uden logik.
- Utilities: Generiske hjælpefunktioner til XML-parsing, filsystem, logging m.m.

Vigtige designbeslutninger:
- Atomiske filoperationer: Filer genereres i en midlertidig mappe, valideres, og flyttes derefter atomisk til den endelige destination. Dette forhindrer korrupte/ufuldstændige output.
- Sammensatte PK-understøttelse: Programmet håndterer sammensatte primærnøgler via CompositePKSplitAlgorithm og CompositePKSelector-kontrollen.
- Interfaces til testbarhed: IProgressReporter og ISplitLogger gør det muligt at teste services isoleret (og rapportere fremskridt til UI).
- Streaming XML-læsning: XmlReader bruges til store filer (memory-effektivt). XDocument bruges, når DOM-manipulation er nødvendig.

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Opsætning og kørsel

Krav
- Windows 10/11
- .NET 8 SDK (til udvikling)
- Visual Studio 2022 eller 2026

Byg og kør lokalt

### Klon projektet
git clone https://github.com/MortErik/TestvaerkstedetToolkit
cd TestvaerkstedetToolkit

### Byg
dotnet build TestvaerkstedetToolkit.sln

### Kør (eller åbn .sln i Visual Studio og tryk F5)
dotnet run --project TestvaerkstedetToolkit/TestvaerkstedetToolkit.csproj

Publicer som single-file executable
dotnet publish TestvaerkstedetToolkit/TestvaerkstedetToolkit.csproj \
  -c Release \
  -r win-x64 \
  /p:PublishSingleFile=true \
  --self-contained true

Den færdige .exe findes i bin/Release/net8.0-windows/win-x64/publish/

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Konfiguration
Brugerpræferencer gemmes i App.config (via .NET User Settings):

Setting	Beskrivelse	Default
TableSplitterOutputPath	Output-sti for XML-splits	Skrivebordet
XMLFKRepairOutputPath	Output-sti for FK-reparationer	Skrivebordet

Sti-præferencerne sættes i UI og gemmes automatisk. Standard output-mapper oprettes på brugerens skrivebord:

- %DESKTOP%\XML_Table_Splits
- %DESKTOP%\XML_FK_Repairs

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Test
Der er pt. ingen automatiserede tests i projektet. Al validering sker manuelt og via intern logik:

- SplitConfiguration validerer split-punkter
- PrimaryKeyAnalysisService tjekker PK-unikhed
- Operationer logges til en .log-fil i output-mappen

Anbefaling til fremtiden: Tilføj et xUnit- eller NUnit-testprojekt til solution og start med at dække services (de er allerede interface-baserede og let testbare).

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Logging
Programmet logger via SplitLogger-klassen:
- Log-filer gemmes i output-mappen for hvert kørt job
- To log-niveauer: brugervenlige beskeder + teknisk detaljelogning
- Progress rapporteres til UI via IProgressReporter-interfacet

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Dataformat og domæneviden
Programmet arbejder med arkiveringsversioner og afleveringspakker i overensstemmelse med Bekendtgørelse nr. 128. Centrale begreber:
- tableIndex.xml: Metadata-fil der beskriver alle tabeller i en arkiveringsversion. Programmet opdaterer denne automatisk ved splitting.
- Sammensatte PK: En primærnøgle bestående af flere kolonner — understøttes fuldt ud.
- Ved XML Table Splitter med Auto-ID: Programmet kan generere nye ID-kolonner, hvis originale PK'er ikke er unikke efter split.
- FK Repair: Genererer "dummy records" for manglende fremmednøgle-referencer, så referentiel integritet genoprettes.

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Kendte begrænsninger og fremtidige forbedringer
- Ingen automatiserede tests (høj prioritet at tilføje)
- Ingen CI/CD-pipeline
- Kun Windows (by design — Windows Forms)
- Koden og kommentarerne er primært på dansk