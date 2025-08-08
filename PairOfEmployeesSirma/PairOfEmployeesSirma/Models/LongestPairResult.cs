namespace PairOfEmployeesSirma.Models
{
    public class LongestPairResult
    {
        public List<EmployeePairResult> AllPairs { get; set; } = new List<EmployeePairResult>();
        public EmployeePairResult? LongestPair { get; set; }
    }
}
