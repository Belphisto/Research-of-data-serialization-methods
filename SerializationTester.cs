using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClosedXML.Excel; //dotnet add package ClosedXML

// Класс для выполнения тестов сериализации
public class SerializationTester
{
    private readonly ISerializationStrategy _strategy;
    private readonly string _formatName;

    public SerializationTester(ISerializationStrategy strategy, string formatName)
    {
        _strategy = strategy;
        _formatName = formatName;
    }

    public SerializationResult TestSerialization(ISerializableObject obj)
    {
        var stopWatch = Stopwatch.StartNew();
        var serializedData = _strategy.Serialize(obj);
        stopWatch.Stop();
        serializedData.SerializationTime = stopWatch.Elapsed;

        stopWatch.Restart();
        _strategy.Deserialize(serializedData.SerializedData, out var result);
        stopWatch.Stop();
        result.DeserializationTime = stopWatch.Elapsed;

        result.SerializationFormat = _formatName; // Установка формата сериализации

        return result;
    }

    public void SaveResultsToExcel(List<SerializationResult> results, string filePath)
    {
        XLWorkbook workbook;
        IXLWorksheet worksheet;

        if (System.IO.File.Exists(filePath))
        {
            // Открыть существующий файл
            workbook = new XLWorkbook(filePath);
            worksheet = workbook.Worksheet("Serialization Results");
        }
        else
        {
            // Создать новый файл
            workbook = new XLWorkbook();
            worksheet = workbook.Worksheets.Add("Serialization Results");

            worksheet.Cell(1, 1).Value = "SerializationFormat";
            worksheet.Cell(1, 2).Value = "SizeInBytes";
            worksheet.Cell(1, 3).Value = "SerializationTime (ms)";
            worksheet.Cell(1, 4).Value = "DeserializationTime (ms)";
        }

        int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int i = 0; i < results.Count; i++)
        {
            worksheet.Cell(lastRow + 1, 1).Value = results[i].SerializationFormat;
            worksheet.Cell(lastRow + 1, 2).Value = results[i].SizeInBytes;
            worksheet.Cell(lastRow + 1, 3).Value = results[i].SerializationTime.TotalMilliseconds;
            worksheet.Cell(lastRow + 1, 4).Value = results[i].DeserializationTime.TotalMilliseconds;
            lastRow++;
        }

        workbook.SaveAs(filePath);
    }

}
