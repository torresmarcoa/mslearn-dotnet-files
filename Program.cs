using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");



IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

CreateSalesSummaryReport(salesFiles);

void CreateSalesSummaryReport(IEnumerable<string> salesFiles)
{
    var reportPath = Path.Combine(salesTotalDir, "salesSummaryReport.txt");

    var total = CalculateSalesTotal(salesFiles);

    var report = new StringBuilder();
    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine($" Total Sales: {total:C}");
    report.AppendLine();
    report.AppendLine(" Details:");

    foreach (var file in salesFiles)
    {
        string parentFolder = Path.GetFileName(Path.GetDirectoryName(file));
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData>(salesJson);
        var fileTotal = data?.Total ?? 0;

        report.AppendLine($"  {parentFolder}{Path.DirectorySeparatorChar}{Path.GetFileName(file)}: {fileTotal:C}");
    }

    File.WriteAllText(reportPath, report.ToString());
}


double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data = JsonConvert.DeserializeObject<SalesData>(salesJson);

        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}
record SalesData(double Total);


