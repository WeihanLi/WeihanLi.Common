using System.Collections.Generic;

namespace WeihanLi.Common.Models
{
    public class ValidateResultModel
    {
        /// <summary>
        /// Valid
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// ErrorMessages
        /// Key: memberName
        /// Value: errorMessage
        /// </summary>
        public Dictionary<string, List<string>> ErrorMessages { get; set; }
    }
}
