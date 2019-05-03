using System.Text.RegularExpressions;

namespace MadsKristensen.AddAnyFile.Helpers
{
    public static class InputTypeHelper
    {
        public static SelectedInputType DetermineType(string input)
        {
            if (Regex.IsMatch(input, @"^I[A-z]+\.[A-z]{2}$"))
            {
                return SelectedInputType.Interface;
            }

            if (Regex.IsMatch(input, @"^[A-z]+.*Controller\.[A-z]{2}$"))
            {
                return SelectedInputType.Controller | SelectedInputType.Class;
            }

            if (Regex.IsMatch(input, @"^[A-z]+.*Base\.[A-z]{2}$"))
            {
                return SelectedInputType.Base | SelectedInputType.Class;
            }

            if (Regex.IsMatch(input, @"^[A-z]+.*Enum\.[A-z]{2}$") || Regex.IsMatch(input, @"^[A-z]+.*Type\.[A-z]{2}$"))
            {
                return SelectedInputType.Enum;
            }


            if (input[input.Length - 1] == '/')
            {
                return SelectedInputType.Folder;
            }

            if (input.Length > 3 && input.Substring(input.Length - 3, 3) == ".cs")
            {
                return SelectedInputType.Class;
            }

            return SelectedInputType.None;
        }
    }
}
