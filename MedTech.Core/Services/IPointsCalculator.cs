namespace MedTech.Core.Services
{
    public interface IPointsCalculator
    {
        int CalculatePoints(string procedureCode, decimal cost);
    }

    public class StandardPointsCalculator : IPointsCalculator
    {
        private readonly Dictionary<string, int> _procedurePoints = new()
        {
            { "BOTOX", 50 },
            { "FILLER", 75 },
            { "LASER", 30 },
            { "FACIAL", 25 },
            { "COOLSCULPT", 100 }
        };

        public int CalculatePoints(string procedureCode, decimal cost)
        {
            // Base points from procedure type
            var basePoints = _procedurePoints.GetValueOrDefault(procedureCode.ToUpper(), 10);

            // Additional points based on cost (1 point per $10)
            var costPoints = (int)(cost / 10);

            return basePoints + costPoints;
        }
    }
}
