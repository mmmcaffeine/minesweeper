using System;

namespace Dgt.Minesweeper.Engine
{
    public static class ColumnConverter
    {
        private const int AsciiCodeForA = 65;
        private const int BaseOneOffset = 1;
        
        // TODO 1 or greater
        public static string ToColumnName(this int columnIndex)
        {
            var c = (char)(columnIndex + AsciiCodeForA - BaseOneOffset);

            return new string(new[] { c });
        }
        
        // TODO No nulls or empty strings
        // TODO No strings that contain non-chars
        // TODO Case insensitive
        public static int ToColumnIndex(this string columnName)
        {
            return columnName[0] - AsciiCodeForA + BaseOneOffset;
        }
    }
}