using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ResponseModels
{
    public class StatusRM
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string Url { get; set; }

        public static StatusRM CreateSuccess(string url = null, string successMessage = null)
        {
            var responseModel = new StatusRM()
            {
                Success = true,
                Url = url,
                SuccessMessage = successMessage
            };

            return responseModel;
        }

        public static StatusRM CreateError(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                error = "An unexpected error occurred.";

            var responseModel = new StatusRM()
            {
                ErrorMessage = error
            };

            return responseModel;
        }

        public static StatusRM CreateError(Exception ex)
        {
            if (ex == null)
                return CreateError("Error: An unexpected error occurred.");

            var errorMessage = ex.Message;

            if (string.IsNullOrWhiteSpace(errorMessage) && ex.InnerException != null)
                errorMessage = ex.InnerException.Message;
            if (string.IsNullOrWhiteSpace(errorMessage))
                errorMessage = "An unexpected error occurred.";

            var responseModel = new StatusRM
            {
                ErrorMessage = "Error: " + errorMessage
            };

            return responseModel;
        }
    }
}
