using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Crosscutting.PayPal.Buttons
{
    public enum UnitOfDuration
    {
        /// <summary>
        /// For days.
        /// </summary>
        D,

        /// <summary>
        /// For weeks.
        /// </summary>
        W,

        /// <summary>
        /// For months.
        /// </summary>
        M,

        /// <summary>
        /// For years.
        /// </summary>
        Y
    }
}