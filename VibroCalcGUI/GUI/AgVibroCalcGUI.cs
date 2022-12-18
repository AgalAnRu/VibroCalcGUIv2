using System;
using System.Collections.Generic;

namespace VibroCalcGUI
{
    internal class AgVibroCalcGUI
    {

        private static int StartPositionX = 5;
        private static int StartPositionY = 1;
        private static string[,] MenuItemArray;
        private static int ColomnTotal = 1;
        private static int RowTotal = 1;
        private static int CellWidth = 10;
        private static readonly int СellHeight = 3;
        private const int SpaceBetweenWordsX = 3;
        private static string[,] ResultValueString;

        internal static void CallGUIVertical(string[] itemsName, string[] itemsValues, out int selectedItem, out string newValue)
        {
            int colomn = 0;
            newValue = string.Empty;
            if (itemsName.Length != itemsValues.Length)
            {
                Console.WriteLine("Массив пунктов меню не соответствует массиву результатов");
                selectedItem = -1;
                return;
            }
            RowTotal = itemsName.Length;
            ColomnTotal = 1;
            MenuItemArray = new string[RowTotal, ColomnTotal];
            ResultValueString = new string[RowTotal, ColomnTotal];
            for (int row = 0; row < RowTotal; row++)
            {
                ResultValueString[row, colomn] = itemsValues[row].ToString();
                MenuItemArray[row, colomn] = itemsName[row];
            }
            PrintTemplateGUI();
            if (GetSelectedItem(out selectedItem, out colomn))
                newValue = GetNewValue(selectedItem, colomn);
            return;
        }
        internal static void CallGUIHorizontal(string[] itemsName, string[] itemsValues, out int selectedItem, out string newValue)
        {
            int row = 0;
            newValue = string.Empty;
            if (itemsName.Length != itemsValues.Length)
            {
                Console.WriteLine("Массив пунктов меню не соответствует массиву результатов");
                selectedItem = -1;
                return;
            }
            RowTotal = 1;
            ColomnTotal = itemsName.Length;
            MenuItemArray = new string[RowTotal, ColomnTotal];
            ResultValueString = new string[RowTotal, ColomnTotal];
            for (int colomn = 0; colomn < ColomnTotal; colomn++)
            {
                ResultValueString[row, colomn] = itemsValues[colomn].ToString();
                MenuItemArray[row, colomn] = itemsName[colomn];
            }
            PrintTemplateGUI();
            if (GetSelectedItem(out row, out selectedItem))
                newValue = GetNewValue(row, selectedItem);
            return;
        }
        internal static void Call2dGUI(string[,] itemsName, string[,] itemsValues, out int selectedRow, out int selectedColomn, out string newValue)
        {
            newValue = string.Empty;
            if (itemsName.GetLength(0) != itemsValues.GetLength(0) || itemsName.GetLength(1) != itemsValues.GetLength(1))
            {
                Console.WriteLine("Массив пунктов меню не соответствует массиву результатов");
                selectedRow = -1;
                selectedColomn = -1;
                return;
            }
            ColomnTotal = itemsName.GetLength(1);
            RowTotal = itemsName.GetLength(0);
            MenuItemArray = itemsName;
            ResultValueString = itemsValues;
            PrintTemplateGUI();
            if (GetSelectedItem(out selectedRow, out selectedColomn))
            {
                newValue = GetNewValue(selectedRow, selectedColomn);
            }
            return;
        }
        private static bool GetSelectedItem(out int row, out int colomn)
        {
            row = 0;
            colomn = 0;
            ConsoleKey key = new ConsoleKey();
            PrintItemValueToCell(row, colomn);
            bool isCursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;
            do
            {
                key = Console.ReadKey(true).Key;
                PrintItemValueToCell(row, colomn, cursorHighlighting: false);
                if (key == ConsoleKey.UpArrow)
                {
                    MoveToNextUpCell(row, colomn, out row);
                }
                if (key == ConsoleKey.DownArrow)
                {
                    MoveToNextDownCell(row, colomn, out row);
                }
                if (key == ConsoleKey.LeftArrow)
                {
                    MoveToNextLeftCell(row, colomn, out colomn);
                }
                if (key == ConsoleKey.RightArrow)
                {
                    MoveToNextRightCell(row, colomn, out colomn);
                }
                PrintItemValueToCell(row, colomn);
                if (key == ConsoleKey.Escape)
                {
                    colomn = -1;
                    row = -1;
                    Console.CursorVisible = isCursorVisible;
                    return false;
                }
                if (key == ConsoleKey.Enter)
                {
                    Console.CursorVisible = isCursorVisible;
                    return true;
                }
            }
            while (true);
        }
        private static string GetNewValue(int row, int colomn)
        {
            int x = StartPositionX + colomn * CellWidth + 2;
            int y = StartPositionY + row * СellHeight + 2;
            MoveCursorToPositionXY(x, y);
            int valueStringWidth = CellWidth - SpaceBetweenWordsX;
            string resultLine = String.Empty.PadLeft(valueStringWidth, ' ');
            Console.Write(resultLine);
            MoveCursorToPositionXY(x + valueStringWidth - 1, y);
            return GetFixedLengthString(valueStringWidth);
        }
        private static string GetFixedLengthString(int length)
        {
            string str = String.Empty;
            int[] xy = GetCursorPozitionXY();
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                    return str;
                if (cki.Key == ConsoleKey.Escape)
                    return string.Empty;
                str += cki.KeyChar;
                MoveCursorToPositionXY(xy[1] - str.Length, xy[0]);
                Console.Write(str);
                MoveCursorToPositionXY(xy[1], xy[0]);
            }
            while (str.Length != length);
            return str;
        }
        private static void PrintTemplateGUI()
        {
            SetCellWidth();
            for (int row = 0; row < RowTotal; row++)
            {
                for (int colomn = 0; colomn < ColomnTotal; colomn++)
                {
                    PrintFormatedCell(row, colomn);
                }
            }
        }
        private static void PrintFormatedCell(int row, int colomn)
        {
            if (MenuItemArray[row, colomn] == String.Empty)
                return;
            MoveCursorToCellTopLeft(row, colomn);
            int[] cellTopLeftXY = GetCursorPozitionXY();
            string horizontalLine = "+".PadRight(CellWidth, '-') + "+";
            string menuItemLine = "| " + MenuItemArray[row, colomn].PadLeft(CellWidth - SpaceBetweenWordsX, ' ') + " |";
            string resultLine = "| " + ResultValueString[row, colomn].PadLeft(CellWidth - SpaceBetweenWordsX, ' ') + " |";
            Console.Write(horizontalLine);
            MoveCursorToPositionXY(cellTopLeftXY[1], cellTopLeftXY[0] + 1);
            Console.Write(menuItemLine);
            MoveCursorToPositionXY(cellTopLeftXY[1], cellTopLeftXY[0] + 2);
            Console.Write(resultLine);
            MoveCursorToPositionXY(cellTopLeftXY[1], cellTopLeftXY[0] + 3);
            Console.WriteLine(horizontalLine);
        }
        private static void SetCellWidth()
        {
            int itemWidth = 0;
            foreach (string item in MenuItemArray)
            {
                if (itemWidth < item.Length)
                    itemWidth = item.Length;
                if (CellWidth < itemWidth + SpaceBetweenWordsX)
                    CellWidth = itemWidth + SpaceBetweenWordsX;
            }
        }
        private static void MoveToNextLeftCell(int row, int colomn, out int colomnNext)
        {
            colomnNext = colomn;
            do
            {
                colomnNext--;
                if (colomnNext < 0)
                {
                    colomnNext = colomn;
                    return;
                }
            }
            while (MenuItemArray[row, colomnNext] == String.Empty);
            return;
        }
        private static void MoveToNextRightCell(int row, int colomn, out int colomnNext)
        {
            colomnNext = colomn;
            do
            {
                colomnNext++;
                if (colomnNext == ColomnTotal)
                {
                    colomnNext = colomn;
                    return;
                }
            }
            while (MenuItemArray[row, colomnNext] == String.Empty);
            return;
        }
        private static void MoveToNextUpCell(int row, int colomn, out int rowNext)
        {
            rowNext = row;
            do
            {
                rowNext--;
                if (rowNext < 0)
                {
                    rowNext = row;
                    return;
                }

            }
            while (MenuItemArray[rowNext, colomn] == String.Empty);
            return;
        }
        private static void MoveToNextDownCell(int row, int colomn, out int rowNext)
        {
            rowNext = row;
            do
            {
                rowNext++;
                if (rowNext == RowTotal)
                {
                    rowNext = row;
                    return;
                }
            }
            while (MenuItemArray[rowNext, colomn] == String.Empty);
            return;
        }
        private static void MoveCursorToPositionXY(int x, int y)
        {
            Console.CursorLeft = x;
            Console.CursorTop = y;
        }
        private static void MoveCursorToCellTopLeft(int row, int colomn)
        {
            int x = StartPositionX + colomn * CellWidth;
            int y = StartPositionY + row * СellHeight;
            MoveCursorToPositionXY(x, y);
        }
        private static void PrintItemValueToCell(int row, int colomn, bool cursorHighlighting = true)
        {
            int x = StartPositionX + colomn * CellWidth + 2;
            int y = StartPositionY + row * СellHeight + 2;
            MoveCursorToPositionXY(x, y);
            if (cursorHighlighting)
                InvertColor();
            string resultLine = ResultValueString[row, colomn].PadLeft(CellWidth - SpaceBetweenWordsX, ' ');
            Console.Write(resultLine);
            if (cursorHighlighting)
                InvertColor();
        }
        private static int[] GetCursorPozitionXY()
        {
            int[] xy = new int[2];
            xy[0] = Console.CursorTop;
            xy[1] = Console.CursorLeft;
            return xy;
        }
        private static void InvertColor()
        {
            /*
            ConsoleColor color = Console.BackgroundColor;
            Console.BackgroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            */
            (Console.BackgroundColor, Console.ForegroundColor) = (Console.ForegroundColor, Console.BackgroundColor);

        }
        internal static string[,] ConvertToStringArray(double[,] values)
        {
            string[,] strValues = new string[values.GetLength(0), values.GetLength(1)];
            for (int row = 0; row < values.GetLength(0); row++)
                for (int colomn = 0; colomn < values.GetLength(1); colomn++)
                    strValues[row, colomn] = values[row, colomn].ToString();
            return strValues;
        }
        internal static string[] ConvertToStringArray(double[] values)
        {
            string[] strValues = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
                strValues[i] = values[i].ToString();
            return strValues;
        }
        //===ToDel
        /* //ToDel
        
        private static int MoveToNextUpCell(int row, int colomn)
        {
            int rowNext = row;
            do
            {
                rowNext--;
                if (rowNext < 0)
                    return row;
            }
            while (MenuItemArray[rowNext, colomn] == String.Empty);
            return rowNext;
        }
        internal static MenuResult Menu(string[,] itemsName, string[,] itemsValues)
        {
            MenuResult menuResult = new MenuResult();
            if (itemsName.GetLength(0) != itemsValues.GetLength(0) || itemsName.GetLength(1) != itemsValues.GetLength(1))
            {
                Console.WriteLine("Массив пунктов меню не соответствует массиву результатов");
                menuResult.selectedItem[0] = -1;
                menuResult.selectedItem[1] = -1;
                return menuResult;
            }
            ColomnTotal = itemsName.GetLength(1);
            RowTotal = itemsName.GetLength(0);
            MenuItemArray = itemsName;
            ResultValueString = itemsValues;
            PrintTemplateGUI();
            //  Test
            if (!GetSelectedItem2(out menuResult.rowSelected, out menuResult.colomnSelected))
                return menuResult;
            menuResult.selectedItem = GetSelectedItem();
            if (menuResult.selectedItem[0] < 0 || menuResult.selectedItem[1] < 0)
                return menuResult;
            menuResult.newValue = GetNewValue(menuResult.selectedItem);
            return menuResult;
        }
        private static string GetNewValue(int[] rowColomn)
        {
            int row = rowColomn[1];
            int colomn = rowColomn[0];
            int x = StartPositionX + colomn * CellWidth + 2;
            int y = StartPositionY + row * СellHeight + 2;
            MoveCursorToPositionXY(x, y);
            int valueStringWidth = CellWidth - SpaceBetweenWordsX;
            string resultLine = String.Empty.PadLeft(valueStringWidth, ' ');
            Console.Write(resultLine);
            MoveCursorToPositionXY(x + valueStringWidth - 1, y);
            return GetFixedLengthString(valueStringWidth);
        }
        private static void MoveCursorToPositionXY(int[] xy)
        {
            Console.CursorLeft = xy[1];
            Console.CursorTop = xy[0];
        }
        private static int[] GetSelectedItem1()
        {
            int[] selected = new int[] { 0, 0 };
            int row = 0;
            int colomn = 0;
            ConsoleKey key = new ConsoleKey();
            MoveCursorToResult(row, colomn);
            bool isCursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;
            do
            {
                key = Console.ReadKey(true).Key;
                MoveCursorToResult(row, colomn, cursorHighlighting: false);
                if (key == ConsoleKey.UpArrow)
                {
                    row = MoveToNextUpCell(row, colomn);
                }
                if (key == ConsoleKey.DownArrow)
                {
                    row = MoveToNextDownCell(row, colomn);
                }
                if (key == ConsoleKey.LeftArrow)
                {
                    colomn = MoveToNextLeftCell(row, colomn);
                }
                if (key == ConsoleKey.RightArrow)
                {
                    colomn = MoveToNextRightCell(row, colomn);
                }
                MoveCursorToResult(row, colomn);
                if (key == ConsoleKey.Escape)
                {
                    colomn = -1;
                    row = -1;
                }
            }
            while (key != ConsoleKey.Escape && key != ConsoleKey.Enter);
            selected[0] = colomn;
            selected[1] = row;
            Console.CursorVisible = isCursorVisible;
            return selected;
        }
        
        */
    }
}
