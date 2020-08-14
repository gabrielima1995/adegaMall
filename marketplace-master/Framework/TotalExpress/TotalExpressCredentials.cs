using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.TotalExpress
{
    public class TotalExpressCredentials
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }

        public bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Senha));
        }
    }
}
