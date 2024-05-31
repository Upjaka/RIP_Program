using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using iText.Layout;

namespace AvaloniaApplication2.Models
{
    public class PDFCreator
    {
        public static readonly string TEMP_FILE_NAME = "natur_list.pdf";
        private static readonly float[] COLUMN_WIDTHS = { 1, 3, 2, 3, 3, 3, 1, 5 };
        private static readonly string[] COLUMN_NAMES = { "№п\\п", "№ ВЦ", "Рег №", "Завод", "ВОиГИ", "Депо", "Прод", "Брак" };

        public PDFCreator() 
        {

        }

        public void Create(Station station, Track track)
        {
            using (PdfWriter writer = new PdfWriter(TEMP_FILE_NAME))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {              
                    Document document = new Document(pdf);

                    Paragraph title = new Paragraph()
                        .SetFont(GetFont())
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(18)
                        .Add(new Text("НАТУРНЫЙ ЛИСТ №3 на ").SetBold())
                        .Add(new Text($"{track.TrackNumber}-й путь района {station.StationName}").SetUnderline());

                    document.Add(title);

                    DateTime now = DateTime.Now;
                    string formattedDateTime = DateTime.Now.ToString("Время HH час mm мин dd MMMM yyyy года");

                    Paragraph paragraph = new Paragraph()
                        .SetFont(GetFont())
                        .SetFontSize(14)
                        .Add(new Text(formattedDateTime));

                    document.Add(paragraph);
                    
                    int cellFontSize = 12;
                    Table table = new Table(UnitValue.CreatePercentArray(COLUMN_WIDTHS)).UseAllAvailableWidth()
                        .SetBorder(Border.NO_BORDER);

                    var row = GetRow(true);

                    for (int i = 0; i < 8; i++)
                    {
                        row[i].Add(new Paragraph(COLUMN_NAMES[i]));
                        table.AddCell(row[i]);
                    }

                    foreach (Car car in track.Cars)
                    {
                        row = GetRow();

                        row[0].Add(new Paragraph(car.SerialNumber.ToString()));
                        row[1].Add(new Paragraph(car.CarNumber));
                        row[3].Add(new Paragraph("//"));
                        row[4].Add(new Paragraph("//"));
                        row[5].Add(new Paragraph("//"));
                        row[6].Add(new Paragraph("0"));
                        row[7].Add(new Paragraph(car.DefectCodes));

                        foreach (var cell in row)
                        {
                            table.AddCell(cell);
                        }
                    }

                    // Добавление таблицы в документ
                    document.Add(table);
                }
            }

            Debug.WriteLine("Printing report");
        }

        public void Delete()
        {
            if (File.Exists(TEMP_FILE_NAME))
            {
                try
                {
                    File.Delete(TEMP_FILE_NAME);
                    Console.WriteLine("File deleted successfully.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine("An error occurred while trying to delete the file: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
        }

        private Cell[] GetRow(bool IsFirst = false)
        {
            var row = new Cell[8];

            for (int i = 0; i < 8; i++)
            {
                Cell cell = new Cell()
                   .SetFont(GetFont())
                   .SetFontSize(12)
                   .SetTextAlignment(TextAlignment.CENTER);

                if (!IsFirst) SetBorders(cell, i);

                row[i] = cell;
            }

            return row;
        }

        private void SetBorders(Cell cell, int serialNumber)
        {
            cell.SetBorderTop(Border.NO_BORDER);
            cell.SetBorderBottom(new SolidBorder(0.5f));
            cell.SetBorderRight((serialNumber == 7) ? new SolidBorder(0.5f) : Border.NO_BORDER);
            cell.SetBorderLeft((serialNumber == 0) ? new SolidBorder(0.5f) : Border.NO_BORDER);
        }

        private PdfFont GetFont()
        {
            string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //projectDir = Directory.GetParent(currentDir).FullName;
            //projectDir = Directory.GetParent(projectDir).FullName;
            //projectDir = Directory.GetParent(projectDir).FullName;

            //string fontPath = Path.Combine("Fonts", "arial.ttf");
            string fontPath = "arial.ttf";
            return PdfFontFactory.CreateFont(Path.Combine(currentDir, fontPath));
        }
    }
}
