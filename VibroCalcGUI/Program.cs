using System;

namespace VibroCalcGUI
{
    internal class Program
    {
        internal static double ValueMain = 0;
        static void Main(string[] args)
        {
            int rowSelected;
            int colomnSelected;
            string newValueString = string.Empty;
            // Примеры для использования GUI
            // Элементы меню задаются одно- или двух-мерным массивом строк (любое количество строк и столбцов отличное от нуля)
            // (размер окна консоли в данной версии не учитывается!)
            // Неиспользуемые элементы заполняются пустым значением ("" или string.Empty)
            // Ширина ячеек меню выравнивается по наиболее широкому элементу
            // количество разрядов значений расчёта = ширине элементов меню (для увеличения разрядов добавить в элемент меню пробелы)

            
            // == Пример 1 == (использование делегатов с методами сет-гет из другой библиотеки. Блок логики GUI прописан внутри класса.
            //                 Блок расчётов передан в GUI по делегату)
            string[,] menuItem2 = { { "item 1", "item 2", "item 3" },
                                    { "item 4", "item 5", "" },
                                    { "item 6", "item 7", "item 8" } };
            double[,] paramValues2 = new double[menuItem2.GetLength(0), menuItem2.GetLength(1)];
            AgVibroCalcGUI.Method2[,] methodsSetValue2 = { { SetValue1, SetValue2, SetValue3 },
                                                         { SetValue4, SetValue5, EmptyMethod },
                                                         { SetValue6, SetValue7, SetValue8} };

            AgVibroCalcGUI.Method[,] methodsGetValue2 = { { GetValue1, GetValue2, GetValue3 },
                                                         { GetValue4, GetValue5, EmptyMethod },
                                                         { GetValue6, GetValue7, GetValue8} };
            AgVibroCalcGUI.Call2dGUI(menuItem2, paramValues2, methodsSetValue2, methodsGetValue2);
            // == Конец Примера 1 ==

            // == Пример 2  == (использование без делегатов. Требуется блок логики GUI и блок расчётов. GUI используется только для ввода вывода string)
            string[,] menuItem = { { "item 0,0", "item 0,1", "",  "" },
                                   { "item 1,0", "item 1,1", "",  "" },
                                   { "", "", "", ""},
                                   { "item 3,0", "item 3,1", "item 3,2","item 3,3"},
                                   { "item 4,0", "item 4,1", "","item 4,3" },
                                   { "item 5,0", "item 5,1", "item 5,2","item 5,3" },
                                   { "","","item 6,2", "item 6,3" } };
            double[,] paramValues = new double[menuItem.GetLength(0), menuItem.GetLength(1)];
            string[,] paramValuesString;
            paramValues[3, 0] = 9.807;
            Console.Clear();
            AgVibroCalcGUI.SetStartPositionXY(50, 5);
            do
            {

                paramValuesString = AgVibroCalcGUI.ConvertToStringArray(paramValues);
                AgVibroCalcGUI.Call2dGUI(menuItem, paramValuesString, out rowSelected, out colomnSelected, out newValueString);
                if (rowSelected < 0 || colomnSelected < 0)
                    continue;
                CalculateNewValue(paramValues, rowSelected, colomnSelected, newValueString);
            }
            while (rowSelected >= 0 && colomnSelected >= 0);
            //== Конец Примера 2 ==

            // == Пример 3 и Пример 4 == (Упрощённый пример 2. Вывод меню либо в одну строку, либо в один столбец)
            Console.Clear();
            AgVibroCalcGUI.SetStartPositionXY(30, 2);
            string[] menuItem3 = { "item 0,0", "item 0,1", "", "item 1,0", "item 1,1" };
            double[] paramValues3 = new double[menuItem3.Length];
            string[] paramValuesString3;
            // ==пример вертикального линейного меню 
            do
            {
                paramValuesString3 = AgVibroCalcGUI.ConvertToStringArray(paramValues3);
                AgVibroCalcGUI.CallGUIVertical(menuItem3, paramValuesString3, out rowSelected, out newValueString);
                CalculateNewValue(paramValues3, rowSelected, newValueString);
            }
            while (rowSelected != -1);

            // ==пример горизонтального линейного меню
            Console.Clear();
            AgVibroCalcGUI.SetStartPositionXY(5, 10);
            menuItem3[2] = "New";
            menuItem3[3] = string.Empty;
            do
            {
                paramValuesString3 = AgVibroCalcGUI.ConvertToStringArray(paramValues3);
                AgVibroCalcGUI.CallGUIHorizontal(menuItem3, paramValuesString3, out rowSelected, out newValueString);
                CalculateNewValue(paramValues3, rowSelected, newValueString);
            }
            while (rowSelected != -1);
            // == Конец Примеров 3 и 4
        }
        // === Методы Блока расчётов для Примера 2
        static void CalculateNewValue(double[,] paramValues, int row, int colomn, string newValue)
        {
            if (double.TryParse(newValue, out double value))
                paramValues[row, colomn] = value;
            // Код ниже заменить методом расчёта остальных значений:
            if (row != 0 || colomn != 0)
                paramValues[0, 0] = value + 10;
            for (int r = 0; r < paramValues.GetLength(0); r++)
            {
                for (int c = 0; c < paramValues.GetLength(1); c++)
                {
                    if ((r == 0 && c == 0) || (r == row && c == colomn))
                        continue;
                    paramValues[r, c] = paramValues[0, 0] + 10 * r + c;
                }
            }
        }
        // === Методы Блока расчётов для Примеров 3 и 4
        static void CalculateNewValue(double[] paramValues, int index, string newValue)
        {
            if (double.TryParse(newValue, out double value))
                paramValues[index] = value;
            // Код ниже заменить методом расчёта остальных значений
            if (index != 0)
                paramValues[0] = value + 10;
            for (int i = 0; i < paramValues.Length; i++)
            {
                if (i == 0 || i == index)
                    continue;
                paramValues[i] = paramValues[0] + 10 * i;
            }
        }
        // == Методы для Примера 1 (будут написаны в отдельной библиотеке VibroCalcMath)
        static void SetValue1(double value) { ValueMain = value; }
        static void SetValue2(double value) { ValueMain = 10 * value; }
        static void SetValue3(double value) { ValueMain = 100 * value; }
        static void SetValue4(double value) { ValueMain = 2*value; }
        static void SetValue5(double value) { ValueMain = 20*value; }
        static void SetValue6(double value) { ValueMain = 5*value; }
        static void SetValue7(double value) { ValueMain = 50 * value; }
        static void SetValue8(double value) { ValueMain = 500 * value; }
        static double GetValue1() { double value = ValueMain; return value; }
        static double GetValue2() { double value = ValueMain/10; return value; }
        static double GetValue3() { double value = ValueMain/100; return value; }
        static double GetValue4() { double value = ValueMain/2; return value; }
        static double GetValue5() { double value = ValueMain/20; return value; }
        static double GetValue6() { double value = ValueMain/5; return value; }
        static double GetValue7() { double value = ValueMain/50; return value; }
        static double GetValue8() { double value = ValueMain/500; return value; }
        static void EmptyMethod(double value = 0) { }
        static double EmptyMethod() { double value = 0; return value; }
    }
}
