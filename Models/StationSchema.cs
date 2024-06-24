using DynamicData;
using System;
using System.Collections.Generic;

namespace AvaloniaApplication2.Models
{
    public class StationSchema
    {
        private static readonly Dictionary<string, List<List<int>>> schemas = new Dictionary<string, List<List<int>>>
        {
            { "РИП пути 1-8", new List<List<int>>
                {
                    new List<int> { 8 },
                    new List<int> { 7 },
                    new List<int> { 6 },
                    new List<int> { 5 },
                    new List<int> { 4 },
                    new List<int> { 3 },
                    new List<int> { 2 },
                    new List<int> { 1 },
                    new List<int> { 22 }
                }
            },
            { "РИП пути 9-20", new List<List<int>>
                {
                    new List<int> { 18, 26 },
                    new List<int> { 17, 25 },
                    new List<int> { 24, 20 },
                    new List<int> { 16, 61, 19 },
                    new List<int> { 15, 51 },
                    new List<int> { 14, 42 },
                    new List<int> { 13, 32 },
                    new List<int> { 12 },
                    new List<int> { 11 },
                    new List<int> { 10 },
                    new List<int> { 9 }
                }
            },
            { "Денисовка", new List<List<int>>
                {
                    new List<int> { 1 }
                }
            },
            { "Тобольск", new List<List<int>>
                {
                    new List<int> { 1 }
                }
            },
        };

        private int _rows;
        public int Rows { get { return _rows; } }

        public List<List<int>> schema;

        public StationSchema(string stationName)
        {
            schema = schemas[stationName];
            _rows = schema.Count;
        }
    }

    /**
    public class Table
    {
        public List<Row> Rows { get; private set; }

        public Table(List<List<int>> table) 
        {
            
        }
    }

    public class Row
    {
        public List<int> Columns { get; private set; }

        public Row(List<int> columns) 
        {
            Columns = columns;
        }
    }
    */
}
