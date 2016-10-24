using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoComplete.ACChange
{
    class SecurityTypeChange
    {


        public string Symbol
        {
            get { return Symbol; }
            set { Symbol = value; }
        }


        public string Exchange
        {
            get { return Exchange; }
            set { Exchange = value; }
        }

        public string OldSecurityType
        {
            get { return OldSecurityType; }
            set { OldSecurityType = value; }
        }

        public string NewSecurityType
        {
            get { return NewSecurityType; }
            set { NewSecurityType = value; }
        }


    }
}
