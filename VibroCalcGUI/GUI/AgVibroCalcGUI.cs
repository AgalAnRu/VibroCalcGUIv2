using System;

namespace VibroCalcGUI
{
    internal class AgVibroCalcGUI
    {
        internal delegate double Method();
        internal delegate void Method2(double value);
        private static int StartPositionX = 5;
        private static int StartPositionY = 1;
        private static string[,] MenuItemArray;
        private static int ColomnTotal = 1;
        private static int RowTotal = 1;
        private static int CellWidth = 10;
        private static readonly int СellHeight = 3;
        private const int SpaceBetweenWordsX = 3;
        private static string[,] ResultValueString;
        private static string NewValueFirstSimbol = string.Empty;

        internal static void SetStartPositionXY(int x, int y)
        {
            StartPositionX = x;
            StartPositionY = y;
        }
        internal static void Call2dGUI(string[,] itemsName, double[,] itemsValues, Method2[,] methodsSetValue, Method[,] methodsGetValue)
        {
            if (!IsInputParamsCorrect(itemsName, itemsValues, methodsSetValue, methodsGetValue))
                return;
            string newValueString = string.Empty;
            int selectedRow = 0;
            int selectedColomn = 0;
            do
            {
                ResultValueString = ConvertToStringArray(itemsValues);
                Call2dGUI(itemsName, ResultValueString, out selectedRow, out selectedColomn, out newValueString);
                if (double.TryParse(newValueString, out double newValue))
                {
                    itemsValues[selectedRow, selectedColomn] = newValue;
                    methodsSetValue[selectedRow, selectedColomn](newValue);
                    for (int row = 0; row < methodsGetValue.GetLength(0); row++)
                    {
                        for (int colomn = 0; colomn < methodsGetValue.GetLength(1); colomn++)
                            itemsValues[row, colomn] = methodsGetValue[row, colomn]();
                    }
                }
            }
            while (selectedRow != -1 && selectedColomn != -1);
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
                if (newValue == string.Empty)
                    newValue = itemsValues[selectedRow, selectedColomn];
            }
            return;
        }
        private static bool IsInputParamsCorrect(string[,] itemsName, double[,] itemsValues, Method2[,] methodsSetValue, Method[,] methodsGetValue)
        {
            int[] length1 = { itemsName.GetLength(0), itemsValues.GetLength(0), methodsSetValue.GetLength(0), methodsGetValue.GetLength(0) };
            int[] length2 = { itemsName.GetLength(1), itemsValues.GetLength(1), methodsSetValue.GetLength(1), methodsGetValue.GetLength(1) };
            for (int i = 0; i < length1.Length - 1; i++)
            {
                for (int j = i + 1; j < length1.Length; j++)
                {
                    if (length1[i] != length1[j] || length2[i] != length2[j])
                    {
                        Console.WriteLine("Входные параметры некорретны");
                        return false;
                    }
                }
            }
            return true;
        }
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
            {
                newValue = GetNewValue(selectedItem, colomn);
                if (newValue == string.Empty)
                    newValue = itemsValues[selectedItem];
            }
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
            {
                newValue = GetNewValue(row, selectedItem);
                if (newValue == string.Empty)
                    newValue = itemsValues[selectedItem];
            }
            return;
        }
        private static bool GetSelectedItem(out int row, out int colomn)
        {
            row = 0;
            colomn = 0;
            PrintItemValueToCell(row, colomn);
            bool isCursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;
            do
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                ConsoleKey key = cki.Key;
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
                    NewValueFirstSimbol = string.Empty;
                    Console.CursorVisible = isCursorVisible;
                    return true;
                }
                if (Char.IsDigit(cki.KeyChar))
                {
                    NewValueFirstSimbol = cki.KeyChar.ToString();
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
            string resultLine = NewValueFirstSimbol.PadLeft(valueStringWidth - 1, ' ') + " ";
            Console.Write(resultLine);
            MoveCursorToPositionXY(x + valueStringWidth - 1, y);
            return GetFixedLengthString(valueStringWidth);
        }
        private static string GetFixedLengthString(int length)
        {
            string str = NewValueFirstSimbol;
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
                Console.Write(str + " ");
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
        internal static int[] GetCursorPozitionXY()
        {
            int[] xy = new int[2];
            xy[0] = Console.CursorTop;
            xy[1] = Console.CursorLeft;
            return xy;
        }
        private static void InvertColor()
        {
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
    }
}
