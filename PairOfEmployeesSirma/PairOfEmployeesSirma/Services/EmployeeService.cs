using PairOfEmployeesSirma.Models;

namespace PairOfEmployeesSirma.Services
{
    public class EmployeeService
    {
        public List<EmployeeProject> LoadFromCsv(Stream fileStream)
        {
            string[] formats = new[]
            {
                  "yyyy-MM-dd",  
                  "dd/MM/yyyy",  
                  "MM-dd-yyyy",  
                  "dd-MM-yyyy",  
                  "yyyy/MM/dd",  
                  "dd.MM.yyyy",  
                  "MM/dd/yyyy"   
            };
            var result = new List<EmployeeProject>();
            using (var reader = new StreamReader(fileStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.Contains("EmpID,ProjectID,DateFrom,DateTo"))
                    {

                        var parts = line.Split(',');
                        if (parts.Length == 4)
                        {


                            int employeeId = int.Parse(parts[0]);
                            int projectId = int.Parse(parts[1]);
                            DateTime startDate;
                            if (!DateTime.TryParseExact(parts[2], formats,
     System.Globalization.CultureInfo.InvariantCulture,
     System.Globalization.DateTimeStyles.None, out startDate))
                            {
                                throw new FormatException($"Unknown date format for: {parts[2]}");
                            }
                            DateTime endDate;
                            if (parts[3] != "NULL"&& !string.IsNullOrEmpty(parts[3]))
                            {

                                if (!DateTime.TryParseExact(parts[3], formats,
   System.Globalization.CultureInfo.InvariantCulture,
   System.Globalization.DateTimeStyles.None, out endDate))
                                {
                                    throw new FormatException($"Unknown date format for: {parts[3]}");

                                }
                            }
                            else
                            {
                                endDate = DateTime.Now;
                            }

                            result.Add(new EmployeeProject
                            {
                                EmployeeID = employeeId,
                                ProjectID = projectId,
                                StartDate = startDate,
                                EndDate = endDate
                            });
                        }
                    }

                }
            }
            return result;
        }
        public LongestPairResult FindLongestPair(List<EmployeeProject> employeeProjects)
        {
            LongestPairResult longestPairResult = new LongestPairResult();
            List<EmployeePairResult> results = new List<EmployeePairResult>();
            var groupedProjects = employeeProjects
                .GroupBy(ep => ep.ProjectID);
            Dictionary<(int, int), int> pairDays = new Dictionary<(int, int), int>();
            foreach (var projectGroup in groupedProjects)
            {
                var employees = projectGroup.ToList();
                HashSet<(int, int)> seenPairs = new HashSet<(int, int)>();
                foreach (var employee1 in employees)
                {
                    foreach (var employee2 in employees)
                    {
                        if (employee1.EmployeeID == employee2.EmployeeID)
                        {
                            continue;
                        }
                        var key = (Math.Min(employee1.EmployeeID, employee2.EmployeeID), Math.Max(employee1.EmployeeID, employee2.EmployeeID));
                        if (!seenPairs.Contains(key))
                        {
                            DateTime overlapStart = default;
                            DateTime overlapEnd = default;
                            if (employee1.StartDate > employee2.StartDate)
                            {
                                overlapStart = employee1.StartDate;
                            }
                            else
                            {
                                overlapStart = employee2.StartDate;
                            }
                            if (employee1.EndDate > employee2.EndDate)
                            {
                                overlapEnd = employee2.EndDate;
                            }
                            else
                            {
                                overlapEnd = employee1.EndDate;
                            }
                            if (overlapStart <= overlapEnd)
                            {
                                int days = (overlapEnd - overlapStart).Days + 1;
                                results.Add(new EmployeePairResult
                                {
                                    Employee1 = employee1.EmployeeID,
                                    Employee2 = employee2.EmployeeID,
                                    ProjectID = projectGroup.Key,
                                    DaysTogether = days
                                });

                                if (pairDays.ContainsKey(key))
                                {
                                    pairDays[key] += days;
                                }
                                else
                                {
                                    pairDays[key] = days;
                                }
                                seenPairs.Add(key);
                            }

                        }
                    }
                }
            }
            var topPair = pairDays
                .OrderByDescending(kvp => kvp.Value)
                .FirstOrDefault();
            EmployeePairResult longestPair = new EmployeePairResult
            {
                Employee1 = topPair.Key.Item1,
                Employee2 = topPair.Key.Item2,
                DaysTogether = topPair.Value
            };
            longestPairResult.AllPairs = results;
            longestPairResult.LongestPair = longestPair;
            return longestPairResult;
        }
    }
}
