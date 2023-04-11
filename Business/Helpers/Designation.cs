namespace ChatApp.Business.Helpers
{
    public static class Designation
    {
        public static string getDesignationType(int id)
        {
            
            switch (id)
            {
                case 2:
                    return "Programmer Analyst";
                case 3:
                    return "Solution Analyst";
                case 4:
                    return "Lead Solution Analyst";
                case 5:
                    return "Intern";
                case 6:
                    return "Probationer";
                case 7:
                    return "Quality Analyst";
                default:
                    return "Imposter";
            }
            
        }
    }
}
