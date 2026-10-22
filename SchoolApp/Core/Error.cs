namespace SchoolApp.Core
{
    public class Error
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public string? field { get; set; }

        public Error() { }

        public Error(string? code, string? message, string? field)
        {
            Code = code;
            Message = message;
            this.field = field;
        }
    }
}