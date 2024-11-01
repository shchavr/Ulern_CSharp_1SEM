using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews
{
    public class StatisticsTask
    {
        public static double GetMedianTimePerSlide(List<VisitRecord> visitRecords,
            SlideType targetSlideType)
        {
            var userSlideTimes = GetUserSlideTimes(visitRecords, targetSlideType);
            return userSlideTimes.DefaultIfEmpty().Median();
        }

        /// <summary>
        /// Получает времена пребывания пользователя на слайдах заданного типа.
        /// </summary>
        /// <param name="visitRecords">Список записей о посещении.</param>
        /// <param name="targetSlideType">Целевой тип слайда.</param>
        /// <returns>
        /// Перечисление значений, представляющих времена пребывания пользователя на слайдах заданного типа.
        /// </returns>
        private static IEnumerable<double> GetUserSlideTimes(List<VisitRecord> visitRecords,
            SlideType targetSlideType)
        {
            return visitRecords
                .GroupBy(record => record.UserId)
                .Select(group => group.OrderBy(record => record.DateTime)
                .Bigrams())
                .SelectMany(collection => collection)
                .Where(pair => FilterUserSlideTimes(pair, targetSlideType))
                .Select(pair => (pair.Second.DateTime - pair.First.DateTime))
                .Where(timeSpan => FilterTimeSpan(timeSpan))
                .Select(timeSpan => timeSpan.TotalMinutes);
        }

        /// <summary>
        /// Фильтрует пары записей о посещении по заданному типу слайда.
        /// </summary>
        /// <param name="pair">Пара записей о посещении.</param>
        /// <param name="targetSlideType">Целевой тип слайда.</param>
        /// <returns>True, если пара удовлетворяет условию фильтрации, иначе False.</returns>
        private static bool FilterUserSlideTimes((VisitRecord First, VisitRecord Second) pair, 
            SlideType targetSlideType)
        {
            return pair.First.SlideType == targetSlideType && pair.First.SlideId != pair.Second.SlideId;
        }

        /// <summary>
        /// Фильтрует временной интервал по заданным условиям.
        /// </summary>
        /// <param name="timeSpan">Временной интервал.</param>
        /// <returns>True, если временной интервал удовлетворяет заданным условиям, иначе False.</returns>
        private static bool FilterTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan >= TimeSpan.FromMinutes(1) && timeSpan <= TimeSpan.FromMinutes(120);
        }
    }
}


