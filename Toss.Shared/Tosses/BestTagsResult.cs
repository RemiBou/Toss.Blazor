namespace Toss.Shared.Tosses
{
    public class BestTagsResult
    {
        public BestTagsResult()
        {
        }

        public BestTagsResult(string tags, int countLastMonth)
        {
            Tag = tags;
            CountLastMonth = countLastMonth;
        }

        public string Tag { get; set; }

        public int CountLastMonth { get; set; }
    }
}