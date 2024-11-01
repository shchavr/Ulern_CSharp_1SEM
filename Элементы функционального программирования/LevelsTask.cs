using System;
using System.Collections.Generic;
using System.Drawing;

namespace func_rocket;

public class LevelsTask
{
    private static readonly Physics standardGravity = new();

    private static readonly Rocket initialRocketPosition =
        new(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI);

    private static readonly Vector targetPosition = new Vector(600, 200);

    public static IEnumerable<Level> CreateLevels()
    {
        yield return CreateLevel("Zero", (size, location) => Vector.Zero);
        yield return CreateLevel("Heavy", (size, location) => new Vector(0, 0.9));
        yield return CreateLevel("Up", target: new Vector(700, 500),
            gravity: (size, location) => new Vector(0, -300 / (size.Y - location.Y + 300)));
        yield return CreateLevel("WhiteHole", gravity: (size, location) => CalculateWhiteHole(location));
        yield return CreateLevel("BlackHole", gravity: (size, location) => CalculateBlackHole(location));
        yield return CreateLevel("BlackAndWhite",
            gravity: (size, location) => CalculateBlackAndWhite(location));
    }
    /// <summary>
    /// Создает уровень с заданными параметрами.
    /// </summary>
    /// <param name="levelName">Название уровня.</param>
    /// <param name="gravity">Гравитация уровня.</param>
    /// <param name="rocket">Ракета на уровне.</param>
    /// <param name="target">Целевая точка на уровне.</param>
    /// <param name="physics">Физика уровня.</param>
    /// <returns>Созданный уровень.</returns>
    private static Level CreateLevel
        (string levelName, Gravity gravity, Rocket rocket = null!, Vector target = null!, Physics physics = null!)
    {
        rocket ??= initialRocketPosition;
        target ??= targetPosition;
        physics ??= standardGravity;
        return new Level(levelName, rocket, target, gravity, physics);
    }
    /// <summary>
    /// Вычисляет вектор силы притяжения от белой дыры до указанной точки.
    /// </summary>
    /// <param name="inputVector">Входной вектор указанной точки.</param>
    /// <returns>Вектор силы притяжения.</returns>
    private static Vector CalculateWhiteHole(Vector inputVector)
    {
        var distanceTarget = new Vector(inputVector.X - 600, inputVector.Y - 200);
        return distanceTarget.Normalize() * 140 * distanceTarget.Length 
            / (distanceTarget.Length * distanceTarget.Length + 1);
    }
    /// <summary>
    /// Вычисляет вектор силы притяжения от черной дыры до указанной точки.
    /// </summary>
    /// <param name="inputVector">Входной вектор указанной точки.</param>
    /// <returns>Вектор силы притяжения.</returns>
    private static Vector CalculateBlackHole(Vector inputVector)
    {
        var anomalyVector = new Vector(200 + 600, 500 + 200) / 2;
        var distanceTarget = new Vector(anomalyVector.X - inputVector.X, anomalyVector.Y - inputVector.Y);
        return distanceTarget.Normalize() * 300 * distanceTarget.Length 
            / (distanceTarget.Length * distanceTarget.Length + 1);
    }
    /// <summary>
    /// Вычисляет средний вектор силы притяжения от черной и белой дыр до указанной точки.
    /// </summary>
    /// <param name="inputVector">Входной вектор указанной точки.</param>
    /// <returns>Вектор силы притяжения.</returns>
    private static Vector CalculateBlackAndWhite(Vector inputVector)
    {
        var distanceTargetOne = new Vector(inputVector.X - 600, inputVector.Y - 200);
        var forceOne = distanceTargetOne.Normalize() * 140 * distanceTargetOne.Length /
                 (distanceTargetOne.Length * distanceTargetOne.Length + 1);
        var anomalyVector = new Vector(200 + 600, 500 + 200) / 2;
        var distanceTargetTwo = new Vector(anomalyVector.X - inputVector.X, anomalyVector.Y - inputVector.Y);
        var forceTwo = distanceTargetTwo.Normalize() * 300 * distanceTargetTwo.Length /
                 (distanceTargetTwo.Length * distanceTargetTwo.Length + 1);
        return (forceOne + forceTwo) / 2;
    }
}