using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace linq_slideviews
{
    public class ParsingTask
    {
        public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
        {
            return lines
                .Select(ParseAndCreateSlideRecord)
                .Where(record => record != null)
                .ToDictionary(record => record.SlideId, record => record);
        }
        /// <summary>
        /// Анализирует строку и создает объект записи слайда.
        /// </summary>
        /// <param name="inputString"> Входная строка для синтаксического анализа.</param>
        /// <returns> Объект записи слайда, если строка успешно проанализирована, 
        /// в противном случае возвращает null.</returns>
        private static SlideRecord ParseAndCreateSlideRecord(string line)
        {
            var parts = line.Split(';');
            if (parts.Length != 3 || !int.TryParse(parts[0], out int slideId) ||
                !Enum.TryParse(parts[1], true, out SlideType slideType))
            {
                return null;
            }

            return new SlideRecord(slideId, slideType, parts[2]);
        }


        public static IEnumerable<VisitRecord> ParseVisitRecords(
            IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
        {
            return lines
                .Skip(1)
                .Select(line => TryParseLine(slides, line));
        }
        /// <summary>
        /// Разбирает строку данных записи посещения и создать объект записи посещения.
        /// </summary>
        /// <param name="dataDictionary"> Словарь, содержащий записи слайдов.</param>
        /// <param name="dataString"> Строка данных для синтаксического анализа.</param>
        /// <returns> Объект записи посещения, созданный на основе проанализированных данных.</returns>
        /// <exception cref="FormatException"> Возникает при ошибке синтаксического анализа данных строки.</exception>
        private static VisitRecord TryParseLine(IDictionary<int, SlideRecord> dataDictionary, string dataString)
        {
            try
            {
                var lineData = dataString.Split(';');
                return new VisitRecord(
                    int.Parse(lineData[0]),
                    int.Parse(lineData[1]),
                    DateTime.ParseExact(
                        $"{lineData[2]} {lineData[3]}",
                        "yyyy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None),
                    dataDictionary[int.Parse(lineData[1])].SlideType);
            }
            catch (Exception processingError)
            {
                throw new FormatException($"Wrong line [{dataString}]", processingError);
            }
        }
    }
}





