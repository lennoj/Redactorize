using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redactorize.Redact
{
    public class RedactionParameter
    {
       
        /// <summary>
        /// The Fixed Phrase value to search or Regular Expression pattern to search.
        /// </summary>
        public string TextValue { get; set; } = String.Empty;

        /// <summary>
        /// Define the type of TextValue
        /// </summary>
        public RedactorEnums.RedactionMatchingStrategy Strategy { get; set; } = RedactorEnums.RedactionMatchingStrategy.FixedPhrase;

        public RedactionParameter(RedactorEnums.RedactionMatchingStrategy strategy, string textValue)
        {
            this.Strategy = strategy;
            this.TextValue = textValue; 
        }
    }
}
