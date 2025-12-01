using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TestvaerkstedetToolkit.Models;

namespace TestvaerkstedetToolkit.Services
{
    /// <summary>
    /// Service for TableIndex.xml generation and transformation
    /// Håndterer opdatering af tableIndex med split tabeller, FK referencer og reunion views
    /// </summary>
    public class TableIndexService
    {
        /// <summary>
        /// Generer opdateret tableIndex.xml med split tabeller og opdaterede FK referencer
        /// ERSTATTER original tabel med første split på samme position
        /// </summary>
        public string GenerateUpdatedTableIndex(List<SplitTable> splitTables, string outputDirectory, UIDataContainer uiData, int originalTableNumber, int startingTableNumber)
        {
            try
            {
                string sourceTableIndexPath = uiData.TableIndexPath;
                string outputTableIndexPath = Path.Combine(outputDirectory, "tableIndex_updated.xml");
                var doc = XDocument.Load(sourceTableIndexPath);
                var ns = doc.Root.GetDefaultNamespace();

                var logger = new StringBuilder();
                logger.AppendLine("═══════════════════════════════════════════════════════════");
                logger.AppendLine("         TABLEINDEX TRANSFORMATION LOG");
                logger.AppendLine("═══════════════════════════════════════════════════════════");
                logger.AppendLine($"Original tabel: {uiData.OriginalTableEntry.Name}");
                logger.AppendLine($"Split strategi: {splitTables.Count} tabeller");
                logger.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                logger.AppendLine();

                // STEP 1: Find og erstat original tabel med første split
                logger.AppendLine("STEP 1: ERSTAT ORIGINAL TABEL");
                logger.AppendLine("───────────────────────────────");

                var tablesElement = doc.Descendants(ns + "tables").First();
                var originalTableElement = tablesElement.Elements(ns + "table")
                    .FirstOrDefault(t => t.Element(ns + "name")?.Value == uiData.OriginalTableEntry.Name);

                if (originalTableElement == null)
                {
                    throw new Exception($"Original tabel '{uiData.OriginalTableEntry.Name}' ikke fundet i tableIndex!");
                }

                int originalPosition = tablesElement.Elements(ns + "table").ToList().IndexOf(originalTableElement) + 1;
                logger.AppendLine($"  Position i tableIndex: #{originalPosition}");
                logger.AppendLine($"  Original table nummer: {originalTableNumber}");

                // Opret første split element (erstatter original)
                var firstSplitElement = CreateCompleteTableElement(
                    splitTables[0],
                    splitTables,
                    ns,
                    uiData,
                    originalTableNumber  // BEVAR original table nummer
                );

                // ERSTAT original tabel med første split
                originalTableElement.ReplaceWith(firstSplitElement);
                logger.AppendLine($"✓ Erstattede '{uiData.OriginalTableEntry.Name}' med '{splitTables[0].TableName}'");
                logger.AppendLine($"  → Beholder table{originalTableNumber} folder");
                logger.AppendLine();

                // STEP 2: Tilføj resterende splits til bunden
                if (splitTables.Count > 1)
                {
                    logger.AppendLine("STEP 2: TILFØJ RESTERENDE SPLITS");
                    logger.AppendLine("─────────────────────────────────");

                    for (int i = 1; i < splitTables.Count; i++)
                    {
                        int currentTableNumber = startingTableNumber + (i - 1);
                        var splitElement = CreateCompleteTableElement(
                            splitTables[i],
                            splitTables,
                            ns,
                            uiData,
                            currentTableNumber
                        );

                        tablesElement.Add(splitElement);
                        logger.AppendLine($"✓ Tilføjede '{splitTables[i].TableName}' som table{currentTableNumber}");
                    }
                    logger.AppendLine();
                }

                // STEP 3: Opdater alle foreign key referencer
                logger.AppendLine("STEP 3: OPDATER FOREIGN KEY REFERENCER");
                logger.AppendLine("───────────────────────────────────────");
                var fkUpdateLog = UpdateForeignKeyReferences(doc, ns, splitTables, uiData);
                logger.Append(fkUpdateLog);
                logger.AppendLine();

                // STEP 4: Opret eller opdater views sektion
                logger.AppendLine("STEP 4: GENERER REUNION VIEW");
                logger.AppendLine("────────────────────────────");
                CreateOrUpdateViewsSection(doc, ns, splitTables, uiData);
                logger.AppendLine($"✓ Oprettet view: AV_Opsamling_af_{uiData.OriginalTableName}");
                logger.AppendLine();

                // STEP 5: Validering
                logger.AppendLine("STEP 5: VALIDERING");
                logger.AppendLine("──────────────────");
                ValidateTableIndexTransformation(doc, ns, uiData, splitTables, logger);
                logger.AppendLine();

                // Gem opdateret tableIndex
                doc.Save(outputTableIndexPath);
                logger.AppendLine("═══════════════════════════════════════════════════════════");
                logger.AppendLine($"tableIndex_updated.xml gemt: {outputTableIndexPath}");
                logger.AppendLine("═══════════════════════════════════════════════════════════");

                System.Diagnostics.Debug.WriteLine($"TableIndex transformation fuldført: {splitTables.Count} splits");
                return logger.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fejl ved tableIndex transformation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Opdater alle foreign key referencer til split tabeller
        /// Håndterer sammensatte FKs der spænder på tværs af splits
        /// </summary>
        private string UpdateForeignKeyReferences(XDocument doc, XNamespace ns, List<SplitTable> splitTables, UIDataContainer uiData)
        {
            var log = new StringBuilder();
            string originalTableName = uiData.OriginalTableName;
            var pkColumns = uiData.PrimaryKey.GetAllPrimaryKeyColumns();

            // Find ALLE foreign keys der refererer til original tabel
            var allForeignKeys = doc.Descendants(ns + "foreignKey")
                .Where(fk => fk.Element(ns + "referencedTable")?.Value == originalTableName)
                .ToList();

            if (allForeignKeys.Count == 0)
            {
                log.AppendLine("  → Ingen eksterne foreign key referencer fundet");
                return log.ToString();
            }

            log.AppendLine($"  Fundet {allForeignKeys.Count} foreign key(s) der refererer til {originalTableName}");
            log.AppendLine();

            int updatedCount = 0;
            int splitCount = 0;
            int skippedCount = 0;

            foreach (var fkElement in allForeignKeys)
            {
                var parentTable = fkElement.Ancestors(ns + "table").First();
                var parentTableName = parentTable.Element(ns + "name")?.Value;
                var fkName = fkElement.Element(ns + "name")?.Value;

                log.AppendLine($"  FK: {fkName} (fra tabel: {parentTableName})");

                // Hent alle reference elementer
                var references = fkElement.Elements(ns + "reference").ToList();

                if (references.Count == 1)
                {
                    // SIMPLE FK - opdater til composite hvis nødvendigt
                    var referencedColumn = references[0].Element(ns + "referenced")?.Value;
                    var targetSplit = FindSplitContainingColumn(splitTables, referencedColumn, pkColumns, uiData);

                    if (targetSplit == null)
                    {
                        log.AppendLine($"    ⚠ ADVARSEL: Kolonne '{referencedColumn}' ikke fundet i nogen split!");
                        log.AppendLine($"    → FK kunne ikke opdateres automatisk");
                        log.AppendLine($"    → MANUEL REVIEW PÅKRÆVET i output");
                        log.AppendLine();
                        skippedCount++;
                        continue;
                    }

                    var referencedTableElement = fkElement.Element(ns + "referencedTable");
                    string oldValue = referencedTableElement.Value;
                    referencedTableElement.Value = targetSplit.TableName;

                    // Hvis target har sammensat PK, udvid FK med manglende kolonner
                    if (pkColumns.Count > 1)
                    {
                        // Find source tabelens kolonner for at matche sammensat PK
                        var sourceTableElement = fkElement.Ancestors(ns + "table").First();
                        var sourceColumns = sourceTableElement.Descendants(ns + "column")
                            .Select(c => c.Element(ns + "name")?.Value)
                            .Where(n => n != null)
                            .ToList();

                        var addedColumns = new List<string>();

                        // Tilføj manglende PK-kolonner til FK hvis de findes i source tabel
                        foreach (var pkCol in pkColumns)
                        {
                            if (pkCol == referencedColumn) continue; // Allerede der

                            // Tjek om source tabel har denne kolonne
                            if (sourceColumns.Contains(pkCol))
                            {
                                var newReference = new XElement(ns + "reference");
                                newReference.Add(new XElement(ns + "column", pkCol));
                                newReference.Add(new XElement(ns + "referenced", pkCol));
                                fkElement.Add(newReference);
                                addedColumns.Add(pkCol);
                            }
                        }

                        if (addedColumns.Count > 0)
                        {
                            log.AppendLine($"    ✓ Opdateret til sammensat FK: {oldValue} → {targetSplit.TableName}");
                            log.AppendLine($"      Tilføjet {addedColumns.Count} PK kolonne(r): {string.Join(", ", addedColumns)}");
                        }
                        else
                        {
                            log.AppendLine($"    ✓ Opdateret: {oldValue} → {targetSplit.TableName}");
                            log.AppendLine($"      ⚠ ADVARSEL: Source tabel mangler sammensat PK kolonner!");
                        }
                    }
                    else
                    {
                        log.AppendLine($"    ✓ Opdateret: {oldValue} → {targetSplit.TableName}");
                    }

                    log.AppendLine($"      Kolonne '{referencedColumn}' findes i split {targetSplit.SplitIndex}");
                    updatedCount++;
                }
                else
                {
                    // Sammensatte FK - multiple kolonner
                    log.AppendLine($"    Sammensatte FK med {references.Count} kolonner:");

                    // Group references by target split
                    var referencesByTarget = new Dictionary<string, List<XElement>>();
                    var unresolvedReferences = new List<string>();

                    foreach (var refElement in references)
                    {
                        var referencedColumn = refElement.Element(ns + "referenced")?.Value;
                        var targetSplit = FindSplitContainingColumn(splitTables, referencedColumn, pkColumns, uiData);

                        if (targetSplit == null)
                        {
                            unresolvedReferences.Add(referencedColumn);
                            continue;
                        }

                        if (!referencesByTarget.ContainsKey(targetSplit.TableName))
                        {
                            referencesByTarget[targetSplit.TableName] = new List<XElement>();
                        }
                        referencesByTarget[targetSplit.TableName].Add(refElement);

                        log.AppendLine($"      - '{referencedColumn}' → {targetSplit.TableName}");
                    }

                    // Tjek om der er unresolved kolonner
                    if (unresolvedReferences.Count > 0)
                    {
                        log.AppendLine($"    ⚠ ADVARSEL: {unresolvedReferences.Count} kolonne(r) ikke fundet:");
                        foreach (var col in unresolvedReferences)
                        {
                            log.AppendLine($"      - '{col}' kunne ikke resolves");
                        }
                        log.AppendLine($"    → FK kunne ikke opdateres fuldstændigt");
                        log.AppendLine($"    → MANUEL REVIEW PÅKRÆVET");
                        log.AppendLine();
                        skippedCount++;
                        continue;
                    }

                    if (referencesByTarget.Count == 1)
                    {
                        // Alle referencer går til SAMME split - simple opdatering
                        var targetTableName = referencesByTarget.Keys.First();
                        fkElement.Element(ns + "referencedTable").Value = targetTableName;

                        log.AppendLine($"    ✓ Alle kolonner i samme split → {targetTableName}");
                        updatedCount++;
                    }
                    else if (referencesByTarget.Count > 1)
                    {
                        // Referenser spænder FLERE splits - split FK op!
                        log.AppendLine($"    ⚠ SPLITTER FK i {referencesByTarget.Count} separate FKs:");

                        // Fjern original FK element
                        var parentForeignKeys = fkElement.Parent;
                        fkElement.Remove();

                        int subFKIndex = 1;
                        foreach (var kvp in referencesByTarget)
                        {
                            string targetTable = kvp.Key;
                            var refs = kvp.Value;

                            // FK navn kollision detection
                            string newFKName = $"{fkName}_Split{subFKIndex}";
                            int collisionCounter = 1;

                            while (parentForeignKeys.Elements(ns + "foreignKey")
                                .Any(fk => fk.Element(ns + "name")?.Value == newFKName))
                            {
                                newFKName = $"{fkName}_Split{subFKIndex}_{collisionCounter}";
                                collisionCounter++;
                            }

                            // Opret ny split FK
                            var newFK = new XElement(ns + "foreignKey");
                            newFK.Add(new XElement(ns + "name", newFKName));
                            newFK.Add(new XElement(ns + "referencedTable", targetTable));

                            foreach (var refElement in refs)
                            {
                                newFK.Add(new XElement(refElement));
                            }

                            parentForeignKeys.Add(newFK);

                            string collisionNote = collisionCounter > 1 ? $" (kollision løst: _{collisionCounter - 1})" : "";
                            log.AppendLine($"      ✓ {newFKName} → {targetTable} ({refs.Count} kolonner){collisionNote}");
                            subFKIndex++;
                        }

                        splitCount++;
                        updatedCount += referencesByTarget.Count;
                    }
                }

                log.AppendLine();
            }

            log.AppendLine($"  RESULTAT:");
            log.AppendLine($"    • {updatedCount} FK opdateret");
            log.AppendLine($"    • {splitCount} FK split op");
            if (skippedCount > 0)
            {
                log.AppendLine($"    • {skippedCount} FK skipped (kræver manuel review)");
            }

            return log.ToString();
        }

        /// <summary>
        /// Validér at tableIndex transformation lykkedes korrekt
        /// </summary>
        private void ValidateTableIndexTransformation(XDocument doc, XNamespace ns, UIDataContainer uiData, List<SplitTable> splitTables, StringBuilder logger)
        {
            bool isValid = true;

            logger.AppendLine("VALIDERING:");
            logger.AppendLine("───────────");

            // 1. Tjek at original tabel IKKE findes mere
            var remainingOriginal = doc.Descendants(ns + "table")
                .FirstOrDefault(t => t.Element(ns + "name")?.Value == uiData.OriginalTableName);

            if (remainingOriginal != null)
            {
                logger.AppendLine($"  ✗ FEJL: Original tabel '{uiData.OriginalTableName}' findes stadig!");
                isValid = false;
            }
            else
            {
                logger.AppendLine($"  ✓ Original tabel '{uiData.OriginalTableName}' korrekt erstattet");
            }

            // 2. Tjek at alle split tabeller findes
            foreach (var split in splitTables)
            {
                var splitElement = doc.Descendants(ns + "table")
                    .FirstOrDefault(t => t.Element(ns + "name")?.Value == split.TableName);

                if (splitElement == null)
                {
                    logger.AppendLine($"  ✗ FEJL: Split tabel '{split.TableName}' ikke fundet!");
                    isValid = false;
                }
                else
                {
                    logger.AppendLine($"  ✓ Split tabel '{split.TableName}' OK");
                }
            }

            // 3. Tjek at ingen FK refererer til original tabel længere (med warning for unresolved)
            var orphanedFKs = doc.Descendants(ns + "foreignKey")
                .Where(fk => fk.Element(ns + "referencedTable")?.Value == uiData.OriginalTableName)
                .ToList();

            if (orphanedFKs.Count > 0)
            {
                logger.AppendLine($"  ⚠ ADVARSEL: {orphanedFKs.Count} FK refererer stadig til '{uiData.OriginalTableName}'");
                logger.AppendLine($"    Dette kan være FKs der ikke kunne opdateres automatisk");
                logger.AppendLine($"    → Tjek foreign_key_transformation_analysis.log for detaljer");
                // IKKE fail validation - det er forventet for unresolved kolonner
            }
            else
            {
                logger.AppendLine($"  ✓ Ingen orphaned FK referencer");
            }

            logger.AppendLine();
            logger.AppendLine(isValid ? "✓ VALIDERING BESTÅET" : "✗ VALIDERING FEJLEDE - SE FEJL OVENFOR");
        }

        /// <summary>
        /// Opret eller opdater views sektion med split reunion view
        /// </summary>
        private void CreateOrUpdateViewsSection(XDocument doc, XNamespace ns, List<SplitTable> splitTables, UIDataContainer uiData)
        {
            // Find eller opret views element
            var viewsElement = doc.Descendants(ns + "views").FirstOrDefault();
            if (viewsElement == null)
            {
                viewsElement = new XElement(ns + "views");
                doc.Root.Add(viewsElement);
            }

            // Generer reunion view
            var reunionView = GenerateSplitReunionView(splitTables, uiData, ns);
            viewsElement.Add(reunionView);
        }

        /// <summary>
        /// Generer SQL view til at samle split tabeller tilbage til original format
        /// </summary>
        private XElement GenerateSplitReunionView(List<SplitTable> splitTables, UIDataContainer uiData, XNamespace ns)
        {
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            var originalTableName = uiData.OriginalTableName;

            // View navn: AV_Opsamling_af_R_testfil
            string viewName = $"RA_Samling_af_{originalTableName}";

            // Byg SQL forespørgsel
            string sqlQuery = GenerateReunionSQL(splitTables, pkColumns, uiData);

            // Opret view element
            var viewElement = new XElement(ns + "view");
            viewElement.Add(new XElement(ns + "name", viewName));
            viewElement.Add(new XElement(ns + "queryOriginal", sqlQuery));

            // Arkivfaglig beskrivelse med reference til kontekstdokumentation
            string tableNames = string.Join(", ", splitTables.Select(t => t.TableName));
            string pkDescription = string.Join(", ", pkColumns);

            string description = $"Dette SQL-view er oprettet af Rigsarkivet i forbindelse med behandling af arkiveringsversionen. " +
                                $"Viewet rekonstruerer den originale tabel {originalTableName} ved at samle de opdelte tabeller ({tableNames}) via primærnøglekobling på kolonne(r): {pkDescription}. " +
                                $"Tabellen blev opdelt for at overholde tekniske begrænsninger i relationelle databasesystemer (maksimalt 1000 kolonner per tabel). " +
                                $"Se kontekstdokumentation for mere information om opdelingen, datasammenhæng og anvendelse af denne forespørgsel.";

            viewElement.Add(new XElement(ns + "description", description));

            return viewElement;
        }

        /// <summary>
        /// Generer SQL forespørgsel til at samle split tabeller - FORENKLET med SELECT *
        /// </summary>
        private string GenerateReunionSQL(List<SplitTable> splitTables, List<string> pkColumns, UIDataContainer uiData)
        {
            var sql = new StringBuilder();

            // SELECT clause - simpel * approach
            var selectParts = new List<string>();
            for (int i = 0; i < splitTables.Count; i++)
            {
                selectParts.Add($"t{i + 1}.*");
            }

            sql.AppendLine($"SELECT {string.Join(", ", selectParts)}");

            // FROM clause med første tabel
            var firstTable = splitTables.First();
            sql.AppendLine($"FROM {firstTable.TableName} t1");

            // INNER JOINs med resterende tabeller
            for (int i = 1; i < splitTables.Count; i++)
            {
                var table = splitTables[i];
                int tableNum = i + 1;

                sql.AppendLine($"INNER JOIN {table.TableName} t{tableNum}");

                // JOIN conditions på alle sammensat PK kolonner
                var joinConditions = new List<string>();
                foreach (var pkColumn in pkColumns)
                {
                    joinConditions.Add($"t1.{pkColumn} = t{tableNum}.{pkColumn}");
                }

                sql.AppendLine($"\tON {string.Join(" AND ", joinConditions)}");
            }

            // ORDER BY første PK kolonne
            sql.AppendLine($"ORDER BY t1.{pkColumns.First()}");

            return sql.ToString().TrimEnd();
        }

        /// <summary>
        /// Opret komplet table element med sammensat PK support
        /// </summary>
        private XElement CreateCompleteTableElement(SplitTable splitTable, List<SplitTable> allSplits, XNamespace ns, UIDataContainer uiData, int tableNumber)
        {
            var tableElement = new XElement(ns + "table");
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            // Naming logic - ALLE splits får suffix
            string tableName = $"{uiData.OriginalTableEntry.Name}_{splitTable.SplitIndex}";
            string folderName = $"{uiData.OriginalTableEntry.Folder}_{splitTable.SplitIndex}";

            tableElement.Add(new XElement(ns + "name", splitTable.TableName));
            tableElement.Add(new XElement(ns + "folder", $"table{tableNumber}"));

            // Description med sammensat PK information
            string description = CreateDescription(splitTable, allSplits, uiData);
            tableElement.Add(new XElement(ns + "description", description));

            // Columns med sammensat PK logic
            var columnsElement = CreateColumnsElement(splitTable, uiData, ns);
            tableElement.Add(columnsElement);

            // Primary key sektion med composite support
            var primaryKeySection = CreatePrimaryKeySection(splitTable, uiData, ns);
            tableElement.Add(primaryKeySection);

            // Foreign keys med cross-references
            var foreignKeysElement = CreateForeignKeysElement(splitTable, allSplits, ns, uiData);
            if (foreignKeysElement.HasElements)
            {
                tableElement.Add(foreignKeysElement);
            }

            // Row count
            tableElement.Add(new XElement(ns + "rows", uiData.TotalRows));

            return tableElement;
        }

        /// <summary>
        /// Opret columns element med sammensat PK placering logik
        /// </summary>
        private XElement CreateColumnsElement(SplitTable splitTable, UIDataContainer uiData, XNamespace ns)
        {
            var columnsElement = new XElement(ns + "columns");
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            int xmlColumnCounter = 1;

            if (splitTable.SplitIndex == 1)
            {
                // TABLE 1: Behold original kolonne rækkefølge
                foreach (var column in splitTable.Columns.OrderBy(c => c.Position))
                {
                    var columnElement = CreateColumnElement(ns, column, $"c{xmlColumnCounter}", uiData);
                    columnsElement.Add(columnElement);
                    xmlColumnCounter++;
                }
            }
            else
            {
                // TABLE 2+: Data kolonner først, PK kolonner i slutningen
                var dataColumns = splitTable.Columns.Where(c => !pkColumns.Contains(c.Name)).OrderBy(c => c.Position);
                var pkColumnsInTable = splitTable.Columns.Where(c => pkColumns.Contains(c.Name) && c.Name != pkInfo.AutoGeneratedColumnName).OrderBy(c => c.Position);

                // Data kolonner
                foreach (var column in dataColumns)
                {
                    var columnElement = CreateColumnElement(ns, column, $"c{xmlColumnCounter}", uiData);
                    columnsElement.Add(columnElement);
                    xmlColumnCounter++;
                }

                // Eksisterende PK kolonner
                foreach (var column in pkColumnsInTable)
                {
                    var enhancedColumn = new XMLColumn
                    {
                        Name = column.Name,
                        ColumnID = column.ColumnID,
                        DataType = column.DataType,
                        TypeOriginal = column.TypeOriginal,
                        IsNullable = column.IsNullable,
                        Description = (column.Description ?? "") + " (Primary Key - duplikeret til split)",
                        Position = column.Position
                    };
                    var columnElement = CreateColumnElement(ns, enhancedColumn, $"c{xmlColumnCounter}", uiData);
                    columnsElement.Add(columnElement);
                    xmlColumnCounter++;
                }

                // Auto-generated PK kolonne
                if (pkInfo.IncludeAutoGenerated)
                {
                    var autoColumnElement = CreateAutoGeneratedColumnElement(ns, $"c{xmlColumnCounter}", pkInfo.AutoGeneratedColumnName);
                    columnsElement.Add(autoColumnElement);
                    xmlColumnCounter++;
                }
            }

            return columnsElement;
        }

        /// <summary>
        /// Opret standard column element
        /// </summary>
        private XElement CreateColumnElement(XNamespace ns, XMLColumn column, string newColumnID, UIDataContainer uiData)
        {
            var columnElement = new XElement(ns + "column");
            columnElement.Add(new XElement(ns + "name", column.Name));
            columnElement.Add(new XElement(ns + "columnID", newColumnID));  // Ny sekventiel ID
            columnElement.Add(new XElement(ns + "type", column.DataType));
            columnElement.Add(new XElement(ns + "typeOriginal", column.TypeOriginal ?? ""));
            columnElement.Add(new XElement(ns + "nullable", column.IsNullable.ToString().ToLower()));
            columnElement.Add(new XElement(ns + "description", column.Description ?? ""));
            return columnElement;
        }

        /// <summary>
        /// Opret auto-generated column element
        /// </summary>
        private XElement CreateAutoGeneratedColumnElement(XNamespace ns, string columnID, string columnName)
        {
            var columnElement = new XElement(ns + "column");
            columnElement.Add(new XElement(ns + "name", columnName));
            columnElement.Add(new XElement(ns + "columnID", columnID));
            columnElement.Add(new XElement(ns + "type", "INTEGER"));
            columnElement.Add(new XElement(ns + "typeOriginal", ""));
            columnElement.Add(new XElement(ns + "nullable", "false"));
            columnElement.Add(new XElement(ns + "description", "Auto-genereret primærnøgle til split tabel kobling"));
            return columnElement;
        }

        /// <summary>
        /// Opret composite primary key sektion
        /// </summary>
        private XElement CreatePrimaryKeySection(SplitTable splitTable, UIDataContainer uiData, XNamespace ns)
        {
            var primaryKeySection = new XElement(ns + "primaryKey");
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            // Konsistent naming - alle får suffix
            string tableName = $"{uiData.OriginalTableEntry.Name}_{splitTable.SplitIndex}";

            if (pkInfo.IsComposite)
            {
                // sammensat PK navn
                primaryKeySection.Add(new XElement(ns + "name", $"PK_{tableName}_Composite"));

                // Tilføj alle PK kolonner
                foreach (var pkColumn in pkColumns)
                {
                    primaryKeySection.Add(new XElement(ns + "column", pkColumn));
                }
            }
            else
            {
                // Single PK
                primaryKeySection.Add(new XElement(ns + "name", $"PK_{tableName}"));
                primaryKeySection.Add(new XElement(ns + "column", pkColumns.First()));
            }

            return primaryKeySection;
        }

        /// <summary>
        /// Opret foreign keys med sammensat PK cross-references
        /// </summary>
        private XElement CreateForeignKeysElement(SplitTable splitTable, List<SplitTable> allSplits, XNamespace ns, UIDataContainer uiData)
        {
            var foreignKeysElement = new XElement(ns + "foreignKeys");
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            // 1. INHERITED FOREIGN KEYS (kun relevante)
            foreach (var originalFK in uiData.OriginalTableEntry.ForeignKeys)
            {
                var columnInThisSplit = splitTable.Columns.FirstOrDefault(c => c.Name == originalFK.Column);
                if (columnInThisSplit != null && !pkColumns.Contains(originalFK.Column))
                {
                    var foreignKeyElement = new XElement(ns + "foreignKey");

                    string tableName = splitTable.SplitIndex == 1 ?
                        uiData.OriginalTableEntry.Name :
                        $"{uiData.OriginalTableEntry.Name}_{splitTable.SplitIndex}";

                    string fkName = $"{originalFK.Name}_Split{splitTable.SplitIndex}";
                    foreignKeyElement.Add(new XElement(ns + "name", fkName));
                    foreignKeyElement.Add(new XElement(ns + "referencedTable", originalFK.ReferencedTable));

                    var referenceElement = new XElement(ns + "reference");
                    referenceElement.Add(new XElement(ns + "column", originalFK.Column));
                    referenceElement.Add(new XElement(ns + "referenced", originalFK.Referenced));

                    foreignKeyElement.Add(referenceElement);
                    foreignKeysElement.Add(foreignKeyElement);
                }
            }

            // 2. CROSS-REFERENCE FOREIGN KEYS mellem split tabeller
            if (allSplits.Count > 1 && splitTable.SplitIndex < allSplits.Count)
            {
                // Find næste split i kæden
                var nextSplit = allSplits.FirstOrDefault(s => s.SplitIndex == splitTable.SplitIndex + 1);

                if (nextSplit != null)
                {
                    // Opret ÉN sammensat FK med alle PK kolonner til næste split
                    var crossRefFK = new XElement(ns + "foreignKey");

                    string currentTableName = splitTable.SplitIndex == 1 ?
                        uiData.OriginalTableEntry.Name :
                        $"{uiData.OriginalTableEntry.Name}_{splitTable.SplitIndex}";
                    string nextTableName = $"{uiData.OriginalTableEntry.Name}_{nextSplit.SplitIndex}";
                    string fkName = $"FK_{currentTableName}_to_next";

                    crossRefFK.Add(new XElement(ns + "name", fkName));
                    crossRefFK.Add(new XElement(ns + "referencedTable", nextTableName));

                    // Tilføj ALLE PK kolonner som references i SAMME FK
                    foreach (var pkColumn in pkColumns)
                    {
                        var referenceElement = new XElement(ns + "reference");
                        referenceElement.Add(new XElement(ns + "column", pkColumn));
                        referenceElement.Add(new XElement(ns + "referenced", pkColumn));
                        crossRefFK.Add(referenceElement);
                    }

                    foreignKeysElement.Add(crossRefFK);
                }
            }

            return foreignKeysElement;
        }

        /// <summary>
        /// Opret beskrivelse med sammensat PK information
        /// </summary>
        private string CreateDescription(SplitTable splitTable, List<SplitTable> allSplits, UIDataContainer uiData)
        {
            string originalDescription = uiData.OriginalTableEntry.Description ?? "";
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            string pkDescription = string.Join(", ", pkColumns);

            // Beregn kolonne intervals for denne split
            int startColumn = splitTable.StartColumn;
            int endColumn = splitTable.EndColumn;

            // Find næste split i kæden (chain topology)
            string nextSplitName = splitTable.SplitIndex < allSplits.Count
                ? $"{uiData.OriginalTableEntry.Name}_{splitTable.SplitIndex + 1}"
                : null;

            string splitInfo;
            string archiveContext = "Denne tabel er opdelt af Rigsarkivet i forbindelse med behandling af arkiveringsversionen for at overholde tekniske begrænsninger i relationelle databasesystemer (maksimalt 1000 kolonner per tabel). Se kontekstdokumentation for mere information om opdelingen og datasammenhæng.";

            if (splitTable.SplitIndex == 1)
            {
                splitInfo = $"Del 1 af {allSplits.Count}: Indeholder kolonnerne {startColumn}-{endColumn} fra tabellen {uiData.OriginalTableEntry.Name} samt primærnøgle ({pkDescription}) til datakobling. " +
                           $"Denne split er koblet til næste split ({nextSplitName}) via fremmednøgle. " +
                           $"For at rekonstruere den komplette tabel skal alle {allSplits.Count} splits kobles i kæde via primærnøglekolonne(r): {pkDescription}. " +
                           $"XML kolonneinterval: c{startColumn}-c{endColumn}.";
            }
            else if (splitTable.SplitIndex == allSplits.Count)
            {
                splitInfo = $"Del {splitTable.SplitIndex} af {allSplits.Count} (afsluttende): Indeholder kolonnerne {startColumn}-{endColumn} fra tabellen {uiData.OriginalTableEntry.Name} samt primærnøgle ({pkDescription}) til datakobling. " +
                           $"Dette er den sidste split i kæden (ingen udgående fremmednøgle). " +
                           $"For at rekonstruere den komplette tabel skal alle {allSplits.Count} splits kobles i kæde via primærnøglekolonne(r): {pkDescription}. " +
                           $"XML kolonneinterval: c{startColumn}-c{endColumn}.";
            }
            else
            {
                splitInfo = $"Del {splitTable.SplitIndex} af {allSplits.Count}: Indeholder kolonnerne {startColumn}-{endColumn} fra tabellen {uiData.OriginalTableEntry.Name} samt primærnøgle ({pkDescription}) til datakobling. " +
                           $"Denne split er koblet til næste split ({nextSplitName}) via fremmednøgle. " +
                           $"For at rekonstruere den komplette tabel skal alle {allSplits.Count} splits kobles i kæde via primærnøglekolonne(r): {pkDescription}. " +
                           $"XML kolonneinterval: c{startColumn}-c{endColumn}.";
            }

            // Byg samlet beskrivelse
            if (!string.IsNullOrWhiteSpace(originalDescription))
            {
                return $"{originalDescription} | {archiveContext} | {splitInfo}";
            }
            else
            {
                return $"{archiveContext} | {splitInfo}";
            }
        }

        /// <summary>
        /// Find split der indeholder kolonne (til UpdateForeignKeyReferences)
        /// Prioriterer data-kolonner over PK kolonner (da PK er duplikeret)
        /// </summary>
        private SplitTable FindSplitContainingColumn(List<SplitTable> splitTables, string columnName, List<string> pkColumns, UIDataContainer uiData)
        {
            if (string.IsNullOrEmpty(columnName))
                return null;

            // Hvis kolonnen er en PK kolonne → returner første split (canonical reference)
            if (pkColumns.Contains(columnName))
            {
                return splitTables.FirstOrDefault();
            }

            // Find split der indeholder denne data-kolonne
            foreach (var split in splitTables)
            {
                var column = split.Columns.FirstOrDefault(c => c.Name == columnName && !pkColumns.Contains(c.Name));
                if (column != null)
                {
                    return split;
                }
            }

            return null;
        }
    }
}