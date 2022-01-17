namespace WebApiSchoolManagement.Utilities
{
    public static class UsernameGenerator
    {
        public static string Genarate(string name) 
        {
            string studentCode = name.Substring(0, 3);
            int randomNumber = new Random().Next(1000, 9999);
            studentCode = $"{studentCode.ToUpper()}{randomNumber.ToString()}";

            return studentCode;
        }
    }
}
