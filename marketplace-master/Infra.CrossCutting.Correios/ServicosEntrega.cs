using System;

namespace Infra.Crosscutting.Correios
{
    [Flags]
    public enum ServicosEntrega
    {
        SedexVarejo = 40010,
        SedexACobrarVarejo = 40045,
        Sedex10Varejo = 40215,
        SedexHojeVarejo = 40290,
        PacVarejo = 41106
    }
}
