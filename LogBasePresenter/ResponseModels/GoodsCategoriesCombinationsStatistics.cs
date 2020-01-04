using System;
using System.Collections.Generic;
using System.Text;

namespace LogBasePresenter.ResponseModels
{
    public class GoodsCategoriesCombinationsStatistics
    {
        public Dictionary<string, Dictionary<string, double>> Statistics { get; set; }

        public GoodsCategoriesCombinationsStatistics()
        {
            Statistics = new Dictionary<string, Dictionary<string, double>>();
        }
        public double GetValue(string firstCategoryName, string secondCategoryName)
        {
            return Statistics[firstCategoryName][secondCategoryName];
        }
        public bool TryGetValue(string firstCategoryName, string secondCategoryName, out double percentage)
        {
            if (Statistics.TryGetValue(firstCategoryName, out var tempDictionary))
            {
                if (tempDictionary.TryGetValue(secondCategoryName, out var tempDouble))
                {
                    percentage = tempDouble;
                    return true;
                }

            }
            else if (Statistics.TryGetValue(secondCategoryName, out tempDictionary))
            {
                if (tempDictionary.TryGetValue(firstCategoryName, out var tempDouble))
                {
                    percentage = tempDouble;
                    return true;
                }
            }
            percentage = new double();
            return false;
        }

        public void SetCombination(string firstCategoryName, string secondCategoryName, double percentage)
        {
            setCombination(firstCategoryName, secondCategoryName, percentage);
            setCombination(secondCategoryName, firstCategoryName, percentage);
        }
        private void setCombination(string firstCategoryName, string secondCategoryName, double percentage)
        {
            if (!Statistics.ContainsKey(firstCategoryName))
            {
                Statistics.Add(firstCategoryName, new Dictionary<string, double>() { { secondCategoryName, percentage } });
            }
            else
            {
                if (!Statistics[firstCategoryName].ContainsKey(secondCategoryName))
                {
                    Statistics[firstCategoryName].Add(secondCategoryName, percentage);
                }
                else
                {
                    Statistics[firstCategoryName][secondCategoryName] = percentage;
                }
            }
        }
    }
}
