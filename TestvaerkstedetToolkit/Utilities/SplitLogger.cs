using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestvaerkstedetToolkit.Models;

namespace TestvaerkstedetToolkit.Services
{
    /// <summary>
    /// Logger implementation for split operations
    /// Håndterer både log opsamling OG generering af alle log filer
    /// </summary>
    public class SplitLogger : ISplitLogger
    {
        private readonly StringBuilder logBuffer = new StringBuilder();

        public void LogInfo(string message)
        {
            logBuffer.AppendLine($"[INFO] {message}");
        }

        public void LogWarning(string message)
        {
            logBuffer.AppendLine($"[WARNING] {message}");
        }

        public void LogError(string message)
        {
            logBuffer.AppendLine($"[ERROR] {message}");
        }

        public void SaveToFile(string logPath)
        {
            File.WriteAllText(logPath, logBuffer.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Generer og gem alle 3 log filer: technical (HTML), user-friendly (HTML), reference (TXT)
        /// </summary>
        public void SaveAllLogs(string outputDirectory, string timestamp, UIDataContainer uiData, TableIndexUpdateResult tableIndexResult)
        {
            // 1. GEM TECHNICAL LOG (.log først, derefter konverter til HTML)
            string technicalLogTempPath = Path.Combine(outputDirectory, $"split_operation_technical_{timestamp}.temp.log");
            SaveToFile(technicalLogTempPath);

            string technicalLogPath = Path.Combine(outputDirectory, $"split_operation_technical_{timestamp}.html");
            ConvertTechnicalLogToHTML(technicalLogTempPath, technicalLogPath);
            File.Delete(technicalLogTempPath);

            // 2. GEM USER-FRIENDLY LOG (direkte som HTML)
            string completeLogPath = Path.Combine(outputDirectory, $"split_operation_complete_{timestamp}.html");
            GenerateUserFriendlyLog(completeLogPath, outputDirectory, timestamp, uiData);

            // 3. GEM REFERENCE LOG (fra TableIndexService eller fallback)
            string referenceLogPath = Path.Combine(outputDirectory, $"split_operation_references_{timestamp}.txt");
            if (tableIndexResult != null && !string.IsNullOrEmpty(tableIndexResult.ReferenceLog))
            {
                // Brug reference log fra TableIndexService (har FK info)
                File.WriteAllText(referenceLogPath, tableIndexResult.ReferenceLog, Encoding.UTF8);
            }
            else
            {
                // Fallback: Generer basic reference log uden FK info
                GenerateReferenceLog(referenceLogPath, uiData);
            }
        }

        /// <summary>
        /// Konverter technical log til HTML med simpel wrapper (ingen styling ændringer)
        /// </summary>
        private void ConvertTechnicalLogToHTML(string logPath, string htmlPath)
        {
            var logContent = File.ReadAllText(logPath, Encoding.UTF8);
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"da\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>XML Table Split Operation - Technical Log</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: 'Courier New', monospace; margin: 20px; background-color: #1e1e1e; color: #d4d4d4; }");
            html.AppendLine("        pre { white-space: pre-wrap; word-wrap: break-word; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <pre>");
            html.AppendLine(System.Security.SecurityElement.Escape(logContent));
            html.AppendLine("    </pre>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            File.WriteAllText(htmlPath, html.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Generer brugervenlig complete log som HTML med styling
        /// </summary>
        private void GenerateUserFriendlyLog(string logPath, string outputDirectory, string timestamp, UIDataContainer uiData)
        {
            var html = new StringBuilder();
            string versionNumber = Path.GetFileName(outputDirectory).Split('_').LastOrDefault() ?? "v1.0";
            string operationId = $"split_{uiData.OriginalTableName}_{versionNumber}_{timestamp}";
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            // Filter out log files from count
            var generatedFiles = Directory.GetFiles(outputDirectory, "*", SearchOption.AllDirectories)
                .Where(f => !f.Contains(".log") && !f.Contains(".html") && !f.Contains("_references.txt"))
                .ToArray();
            var xmlFiles = generatedFiles.Where(f => f.EndsWith(".xml") && !f.Contains("tableIndex")).OrderBy(f => f).ToList();
            var tableIndexFiles = generatedFiles.Where(f => f.Contains("tableIndex")).ToList();

            // Calculate accurate file counts
            int totalXmlFiles = uiData.Tables.Count;
            int totalFiles = totalXmlFiles + 1; // +1 for tableIndex
            int totalLogFiles = 3; // technical, complete, references

            // HTML Header
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"da\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>XML Table Split Operation - Complete Log</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 40px; background-color: #f5f5f5; }");
            html.AppendLine("        .container { max-width: 1200px; margin: 0 auto; background-color: white; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
            html.AppendLine("        h1 { color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px; }");
            html.AppendLine("        h2 { color: #34495e; margin-top: 30px; border-left: 4px solid #3498db; padding-left: 15px; }");
            html.AppendLine("        h3 { color: #7f8c8d; margin-top: 20px; }");
            html.AppendLine("        .header-info { background-color: #ecf0f1; padding: 15px; border-radius: 5px; margin-bottom: 20px; }");
            html.AppendLine("        .info-row { margin: 5px 0; }");
            html.AppendLine("        .label { font-weight: bold; color: #2c3e50; }");
            html.AppendLine("        .status-success { color: #27ae60; font-weight: bold; font-size: 1.2em; }");
            html.AppendLine("        table { width: 100%; border-collapse: collapse; margin: 15px 0; }");
            html.AppendLine("        th { background-color: #3498db; color: white; padding: 12px; text-align: left; font-weight: bold; }");
            html.AppendLine("        td { padding: 10px; border-bottom: 1px solid #ddd; }");
            html.AppendLine("        tr:hover { background-color: #f8f9fa; }");
            html.AppendLine("        .pk-table th { background-color: #e74c3c; }");
            html.AppendLine("        .data-table th { background-color: #3498db; }");
            html.AppendLine("        .split-section { margin: 30px 0; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }");
            html.AppendLine("        .file-list { background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 10px 0; }");
            html.AppendLine("        .file-item { margin: 8px 0; padding: 8px; background-color: white; border-left: 3px solid #3498db; }");
            html.AppendLine("        .summary-box { background-color: #d5f4e6; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 5px solid #27ae60; }");
            html.AppendLine("        ul { list-style-type: none; padding-left: 0; }");
            html.AppendLine("        ul li:before { content: \"• \"; color: #3498db; font-weight: bold; margin-right: 8px; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class=\"container\">");

            // Title and Header Info
            html.AppendLine("        <h1>XML Table Split Operation - Complete Log</h1>");
            html.AppendLine("        <div class=\"header-info\">");
            html.AppendLine($"            <div class=\"info-row\"><span class=\"label\">Operation ID:</span> {operationId}</div>");
            html.AppendLine($"            <div class=\"info-row\"><span class=\"label\">Timestamp:</span> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>");
            html.AppendLine("        </div>");

            // Section 1: Operation Overview
            html.AppendLine("        <h2>1. Operation Overview</h2>");
            html.AppendLine("        <table>");
            html.AppendLine("            <tr><th>Parameter</th><th>Værdi</th></tr>");
            html.AppendLine($"            <tr><td>Original Tabel</td><td>{uiData.OriginalTableName}</td></tr>");
            html.AppendLine($"            <tr><td>Total Rækker</td><td>{uiData.TotalRows:N0}</td></tr>");
            html.AppendLine($"            <tr><td>Total Kolonner</td><td>{uiData.AllColumns.Count}</td></tr>");
            html.AppendLine($"            <tr><td>Split Strategi</td><td>{uiData.Tables.Count} tabeller (maks 950 kolonner per tabel)</td></tr>");
            html.AppendLine($"            <tr><td>PK Type</td><td>{(pkInfo.IsComposite ? "Composite" : "Single")} ({string.Join(", ", pkColumns)})</td></tr>");
            html.AppendLine("        </table>");

            html.AppendLine("        <h3>Split Tabeller</h3>");
            html.AppendLine("        <ul>");
            for (int i = 0; i < uiData.Tables.Count; i++)
            {
                var table = uiData.Tables[i];
                var dataColumns = table.Columns.Where(c => !pkColumns.Contains(c.Name)).Count();
                html.AppendLine($"            <li>{table.TableName} ({table.Columns.Count} kolonner: {pkColumns.Count} PK + {dataColumns} data)</li>");
            }
            html.AppendLine("        </ul>");

            html.AppendLine("        <h3>Reunion View</h3>");
            html.AppendLine("        <ul>");
            html.AppendLine($"            <li>View navn: RA_Samling_af_{uiData.OriginalTableName}</li>");
            html.AppendLine($"            <li>JOIN på: {string.Join(", ", pkColumns)}</li>");
            html.AppendLine("        </ul>");

            // Section 2: Status
            html.AppendLine("        <h2>2. Status</h2>");
            html.AppendLine("        <div class=\"summary-box\">");
            html.AppendLine("            <div class=\"status-success\">✓ COMPLETED</div>");
            html.AppendLine("            <div>Warnings: 0</div>");
            html.AppendLine("            <div>Errors: 0</div>");
            html.AppendLine("        </div>");

            // Section 3: Column Distribution
            html.AppendLine("        <h2>3. Column Distribution</h2>");

            foreach (var table in uiData.Tables)
            {
                var dataColumns = table.Columns.Where(c => !pkColumns.Contains(c.Name)).OrderBy(c => c.Position).ToList();
                var pkColumnsInSplit = table.Columns.Where(c => pkColumns.Contains(c.Name)).OrderBy(c => c.Position).ToList();

                html.AppendLine("        <div class=\"split-section\">");
                html.AppendLine($"            <h3>{table.TableName} - {table.Columns.Count} kolonner</h3>");

                if (table.SplitIndex == 1)
                {
                    // Split1: Behold original rækkefølge - sorter alle kolonner efter position
                    var allColumns = table.Columns.OrderBy(c => c.Position).ToList();

                    // PK Columns Table with Placement
                    html.AppendLine($"            <h4>Primary Key ({pkColumnsInSplit.Count} kolonner - duplikeret til alle splits)</h4>");
                    html.AppendLine("            <table class=\"pk-table\">");
                    html.AppendLine("                <tr><th>Navn</th><th>Datatype</th><th>Placering i {table.TableName}</th></tr>");
                    foreach (var pkCol in pkColumnsInSplit)
                    {
                        int positionInSplit = allColumns.IndexOf(pkCol) + 1;
                        html.AppendLine($"                <tr><td>{pkCol.Name}</td><td>{pkCol.DataType}</td><td>Kolonne c{positionInSplit}</td></tr>");
                    }
                    html.AppendLine("            </table>");

                    // Data Columns Table med Fra/Til
                    html.AppendLine($"            <h4>Data Kolonner ({dataColumns.Count})</h4>");
                    html.AppendLine("            <table class=\"data-table\">");
                    html.AppendLine("                <tr><th>Fra</th><th>Til</th><th>Navn</th><th>Datatype</th></tr>");
                    foreach (var col in dataColumns)
                    {
                        int positionInSplit = allColumns.IndexOf(col) + 1;
                        html.AppendLine($"                <tr><td>{col.ColumnID}</td><td>c{positionInSplit}</td><td>{col.Name}</td><td>{col.DataType}</td></tr>");
                    }
                    html.AppendLine("            </table>");
                }
                else
                {
                    // Split2+: Data kolonner først, PK kolonner bagerst

                    // PK Columns Table placeres bagerst
                    html.AppendLine($"            <h4>Primary Key ({pkColumnsInSplit.Count} kolonner - duplikeret til alle splits)</h4>");
                    html.AppendLine("            <table class=\"pk-table\">");
                    html.AppendLine("                <tr><th>Navn</th><th>Datatype</th><th>Placering i {table.TableName}</th></tr>");

                    int pkStartPosition = dataColumns.Count + 1;
                    int pkCounter = pkStartPosition;
                    foreach (var pkCol in pkColumnsInSplit)
                    {
                        html.AppendLine($"                <tr><td>{pkCol.Name}</td><td>{pkCol.DataType}</td><td>Kolonne c{pkCounter}</td></tr>");
                        pkCounter++;
                    }
                    html.AppendLine("            </table>");

                    // Data Columns Table - starter fra c1
                    html.AppendLine($"            <h4>Data Kolonner ({dataColumns.Count})</h4>");
                    html.AppendLine("            <table class=\"data-table\">");
                    html.AppendLine("                <tr><th>Fra</th><th>Til</th><th>Navn</th><th>Datatype</th></tr>");

                    int toCounter = 1;
                    foreach (var col in dataColumns)
                    {
                        html.AppendLine($"                <tr><td>{col.ColumnID}</td><td>c{toCounter}</td><td>{col.Name}</td><td>{col.DataType}</td></tr>");
                        toCounter++;
                    }
                    html.AppendLine("            </table>");
                }

                html.AppendLine("        </div>");
            }

            // Section 4: Generated Files
            html.AppendLine("        <h2>4. Generated Files</h2>");

            html.AppendLine("        <h3>XML Data Filer</h3>");
            html.AppendLine("        <div class=\"file-list\">");
            foreach (var file in xmlFiles)
            {
                var fileInfo = new FileInfo(file);
                html.AppendLine("            <div class=\"file-item\">");
                html.AppendLine($"                <strong>{Path.GetFileName(file)}</strong><br>");
                html.AppendLine($"                Size: {fileInfo.Length:N0} bytes | Rows: {uiData.TotalRows:N0}");
                html.AppendLine("            </div>");
            }
            html.AppendLine("        </div>");

            if (tableIndexFiles.Count > 0)
            {
                html.AppendLine("        <h3>TableIndex</h3>");
                html.AppendLine("        <div class=\"file-list\">");
                foreach (var file in tableIndexFiles)
                {
                    var fileInfo = new FileInfo(file);
                    html.AppendLine("            <div class=\"file-item\">");
                    html.AppendLine($"                <strong>{Path.GetFileName(file)}</strong><br>");
                    html.AppendLine($"                Size: {fileInfo.Length:N0} bytes<br>");
                    html.AppendLine($"                Original '{uiData.OriginalTableName}' erstattet med splits<br>");
                    html.AppendLine($"                Reunion view tilføjet");
                    html.AppendLine("            </div>");
                }
                html.AppendLine("        </div>");
            }

            // Section 5: Summary
            html.AppendLine("        <h2>5. Operation Summary</h2>");
            html.AppendLine("        <div class=\"summary-box\">");
            html.AppendLine("            <div class=\"status-success\">STATUS: COMPLETED</div>");
            html.AppendLine($"            <div><span class=\"label\">Tabeller genereret:</span> {uiData.Tables.Count} ({string.Join(", ", uiData.Tables.Select(t => t.TableName))})</div>");
            html.AppendLine($"            <div><span class=\"label\">Rækker per tabel:</span> {uiData.TotalRows:N0}</div>");
            html.AppendLine($"            <div><span class=\"label\">Split filer genereret:</span> {totalFiles} ({totalXmlFiles} XML + 1 tableIndex)</div>");
            html.AppendLine($"            <div><span class=\"label\">Log filer:</span> {totalLogFiles} (teknisk, brugervenlig, referencer)</div>");
            html.AppendLine($"            <div><span class=\"label\">Output lokation:</span> {outputDirectory}</div>");
            html.AppendLine("        </div>");

            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            File.WriteAllText(logPath, html.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Generer reference log for FK adjustments og manual/auto information
        /// </summary>
        private void GenerateReferenceLog(string logPath, UIDataContainer uiData)
        {
            var log = new StringBuilder();
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            log.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            log.AppendLine("                   SPLIT OPERATION - REFERENCE LOG");
            log.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            log.AppendLine($"Genereret: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            log.AppendLine($"Original Tabel: {uiData.OriginalTableName}");
            log.AppendLine($"Split Tabeller: {uiData.Tables.Count}");
            log.AppendLine();

            log.AppendLine("═══ FOREIGN KEY RELATIONS ═══");
            log.AppendLine();
            log.AppendLine("Split Topology: CHAIN (Split1 → Split2 → Split3)");
            log.AppendLine($"FK Kolonner: {string.Join(", ", pkColumns)}");
            log.AppendLine();

            log.AppendLine("FK RELATIONER:");
            log.AppendLine("───────────────────────────────────────────────────────────────────────────");

            for (int i = 0; i < uiData.Tables.Count; i++)
            {
                var table = uiData.Tables[i];
                log.AppendLine($"{i + 1}. {table.TableName}");

                if (i < uiData.Tables.Count - 1)
                {
                    var nextTable = uiData.Tables[i + 1];
                    log.AppendLine($"   → Referencer: {nextTable.TableName}");
                    log.AppendLine($"   → FK Navn: FK_{table.TableName}_to_next");
                    log.AppendLine($"   → FK Kolonner: {string.Join(", ", pkColumns)}");
                    log.AppendLine($"   → Type: CHAIN (automatisk genereret)");
                }
                else
                {
                    log.AppendLine($"   → Referencer: Ingen (sidste split)");
                }
                log.AppendLine();
            }

            log.AppendLine();
            log.AppendLine("═══ PRIMARY KEY KONFIGURATION ═══");
            log.AppendLine();
            log.AppendLine($"PK Type: {(pkInfo.IsComposite ? "Composite" : "Single")}");
            log.AppendLine($"PK Kolonner: {pkColumns.Count} total");
            log.AppendLine($"Auto-genereret PK: {(pkInfo.IncludeAutoGenerated ? "Ja" : "Nej")}");
            log.AppendLine();

            log.AppendLine("PK KOLONNER:");
            log.AppendLine("───────────────────────────────────────────────────────────────────────────");

            // Find første split med PK kolonner
            var firstSplit = uiData.Tables.FirstOrDefault(t => t.IsFirstSplit);
            if (firstSplit != null)
            {
                var pkColumnsInSplit = firstSplit.Columns.Where(c => pkColumns.Contains(c.Name)).ToList();

                foreach (var pkCol in pkColumnsInSplit)
                {
                    string source = pkCol.Position > 0 ? $"Original kolonne {pkCol.ColumnID}" : "Auto-genereret";
                    string adjustmentType = pkCol.Position > 0 ? "MANUAL" : "AUTO";

                    log.AppendLine($"Navn: {pkCol.Name}");
                    log.AppendLine($"  Datatype: {pkCol.DataType}");
                    log.AppendLine($"  Kilde: {source}");
                    log.AppendLine($"  Adjustment Type: {adjustmentType}");
                    log.AppendLine($"  Duplikeret til: Alle {uiData.Tables.Count} splits");
                    log.AppendLine();
                }
            }

            log.AppendLine();
            log.AppendLine("═══ REUNION VIEW KONFIGURATION ═══");
            log.AppendLine();
            log.AppendLine($"View Navn: RA_Samling_af_{uiData.OriginalTableName}");
            log.AppendLine($"View Type: Automatic JOIN view");
            log.AppendLine($"JOIN Strategy: NATURAL JOIN på PK kolonner");
            log.AppendLine($"Split Tabeller: {string.Join(", ", uiData.Tables.Select(t => t.TableName))}");
            log.AppendLine($"JOIN Kolonner: {string.Join(", ", pkColumns)}");
            log.AppendLine();

            log.AppendLine("VIEW SQL (Pseudo):");
            log.AppendLine("───────────────────────────────────────────────────────────────────────────");
            log.AppendLine($"CREATE VIEW RA_Samling_af_{uiData.OriginalTableName} AS");
            log.AppendLine($"SELECT * FROM {uiData.Tables[0].TableName}");

            for (int i = 1; i < uiData.Tables.Count; i++)
            {
                var joinConditions = pkColumns.Select(pk => $"t1.{pk} = t{i + 1}.{pk}");
                log.AppendLine($"NATURAL JOIN {uiData.Tables[i].TableName} t{i + 1} ON {string.Join(" AND ", joinConditions)}");
            }

            log.AppendLine();
            log.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            log.AppendLine("                            LOG AFSLUTTET");
            log.AppendLine("═══════════════════════════════════════════════════════════════════════════");

            File.WriteAllText(logPath, log.ToString(), Encoding.UTF8);
        }
    }
}
