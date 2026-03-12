# ForskningsdataReparationskit — Udviklerhåndbog

## Hvad er projektet?
ForskningsdataReparationskit er et Windows desktop-program skrevet i C# (.NET 8, Windows Forms). Programmet er målrettet til behandling af afleveringspakker (FD) og arkiveringsversioner (AVID) ift. Bekendtgørelse nr. 128.

Programmet indeholder tre værktøjer:

Værktøj	Beskrivelse:
- XML Table Splitter	Opdeler store XML-tabeller i mindre deltabeller og opdaterer tableIndex.xml automatisk
- XML FK Repair	Reparerer referentiel integritet i XML-datasæt (manglende fremmednøgler)
- CSV FK Repair	Samme som XML FK Repair, men for CSV-filer

Alle tre værktøjer tilgås fra et centralt dashboard (WelcomeForm).

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Teknisk stack
- Sprog: C# med .NET 8
- UI-framework: Windows Forms
- Krav til OS: Windows (net8.0-windows)
- Eksterne NuGet-pakker: Ingen — kun standard .NET-biblioteker
- Build-output: Single-file self-contained .exe (win-x64)

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

## Projektstruktur
<img width="1290" height="1330" alt="image" src="https://github.com/user-attachments/assets/9d032326-c95a-4b8b-b9fc-7a3dd58d0818" />


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
git clone https://github.com/MortErik/ForskningsdataReparationskit
cd ForskningsdataReparationskit

### Byg
dotnet build ForskningsdataReparationskit.sln

### Kør (eller åbn .sln i Visual Studio og tryk F5)
dotnet run --project ForskningsdataReparationskit/ForskningsdataReparationskit.csproj

Publicer som single-file executable
dotnet publish ForskningsdataReparationskit/ForskningsdataReparationskit.csproj \
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
- Ingen automatiserede tests
- Kun Windows (by design — Windows Forms)
- Koden og kommentarerne er primært på dansk
