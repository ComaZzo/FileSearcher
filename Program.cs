using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Консольное приложение которое выполняет поиск по текстовым файлам в директории.
//На вход подается путь директории
//И шаблон поиска по файлам
//Многопоточный поиск по файлу
//Указание типа поиска - Строгий и слабый поиск. 
//Строгий - слово целиком соответствует
//Слабый - подстрока
//формат вывода: (имя txt файла) : (номер строки) (соответствие)
//Вывод как на консоль, так и в выходной файл

namespace Work_06_04_2018
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Введите путь директории:");
            var pathDirectory = Console.ReadLine();
            Console.WriteLine("Введите шаблон поиска:");
            var pattern = Console.ReadLine();

            Worker.Work(pathDirectory, pattern, true);

            Console.ReadLine();
        }
    }
}
