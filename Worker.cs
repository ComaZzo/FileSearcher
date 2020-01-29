using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Work_06_04_2018
{
    public static class Worker
    {
        #region Fields



        #endregion

        #region Methods

        public static void Work(string directoryPath, string patternForSearchInFile, bool StrongTypeOfSearch)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(directoryPath);

                var listOfFiles = SearchFilesInDirectory(directoryInfo);

                var res = CheckFilesAsyncByTasks(listOfFiles.ToList(), patternForSearchInFile, StrongTypeOfSearch);

                PrintResultListInOutFile(res);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

        }



        public static IEnumerable<FileInfo> SearchFilesInDirectory(DirectoryInfo directory)
        {
            var resultList = new List<FileInfo>();

            resultList.AddRange(directory.GetFiles("*.txt", SearchOption.AllDirectories));
            resultList.AddRange(directory.GetFiles("*.rtf", SearchOption.AllDirectories));

            return resultList;
        }

        public static void PrintResultListInOutFile(List<List<string>> resultList)
        {
            if (resultList == null)
            {
                throw new ArgumentNullException($"resultList is equals null");
            }
            try
            {
                if (File.Exists("out.txt"))
                {
                    File.Delete("out.txt");
                }
                using (var fileOut = new FileStream("out.txt", FileMode.Create, FileAccess.Write))
                using (var outStream = new StreamWriter(fileOut))
                {
                    if (resultList.Count == 0)
                    {
                        Console.WriteLine("Файлы не обнаружены");
                        outStream.WriteLine("Files are not founded.");
                        return;
                    }
                    foreach (var list in resultList)
                    {
                        if (list.Count == 1)
                        {
                            continue;
                        }
                        foreach (string str in list)
                        {
                            outStream.WriteLine(str);
                            Console.WriteLine(str);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private static List<string> SearchContentInFile(FileInfo fileInfo, string patternForSearch, bool strongTypeOfSearch = false)
        {
            var resList = new List<string>();
            resList.Add($"{fileInfo.Name}");
            int lineCounter = 1;

            try
            {
                using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                using (var streamReader = new StreamReader(fileStream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var tempLine = streamReader.ReadLine();
                        var tempStrings = tempLine?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tempStrings == null)
                        {
                            continue;
                        }
                        foreach (string str in tempStrings)
                        {
                            if (strongTypeOfSearch)
                            {
                                if (Regex.IsMatch(str, patternForSearch))
                                {
                                    resList.Add($"{lineCounter} : {str} : {tempLine}");
                                }
                            }
                            else
                            {
                                if (tempLine.Contains(patternForSearch))
                                {
                                    resList.Add($"{lineCounter} : {str} : {tempLine}");
                                }
                            }
                        }

                        ++lineCounter;
                    }
                }

            }
            catch (FileNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return resList;
        }

        public static List<List<string>> CheckFilesAsyncByTasks(List<FileInfo> listFiles, string patternForSearch, bool strongTypeOfSearch)
        {
            var listTasks = new List<Task<List<string>>>();
            var resultList = new List<List<string>>();
            try
            {
                foreach (var file in listFiles)
                {
                    listTasks.Add(new Task<List<string>>(() => SearchContentInFile(file, patternForSearch, strongTypeOfSearch)));
                }

                foreach (var task in listTasks)
                {
                    task.Start();
                }

                foreach (var task in listTasks)
                {
                    if (!task.IsCompleted)
                    {
                        task.Wait();
                    }

                    var tasklistResult = task.Result;
                    if (tasklistResult.Count == 1)
                    {
                        continue;
                    }

                    //resultList.AddRange(tasklistResult);
                    resultList.Add(tasklistResult);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }


            return resultList;
        }


        #endregion
    }
}