using System;

namespace Infra.Crosscutting.Helpers
{
    /// <summary>
    /// Classe que representa funcionalidades para trabalhar com CNPJ
    /// </summary>
    public static class CNPJHelper
    {
        /// <summary>
        /// Verifica se o número CNPJ é válido
        /// </summary>
        /// <param name="cnpj">Número CNPJ</param>
        /// <returns></returns>
        public static bool IsValid(string cnpj)
        {
            var d = new int[14];
            var v = new int[2];
            int j, i, soma;

            var numbers = cnpj.ExtractDigits();

            // check numbers are equal
            if (new string(numbers[0], numbers.Length) == numbers)
            {
                return false;
            }

            if (numbers.Length == 14)
            {
                var seq = "6543298765432";

                for (i = 0; i <= 13; i++)
                {
                    d[i] = Convert.ToInt32(numbers.Substring(i, 1));
                }

                for (i = 0; i <= 1; i++)
                {
                    soma = 0;

                    for (j = 0; j <= 11 + i; j++)
                    {
                        soma += d[j] * Convert.ToInt32(seq.Substring(j + 1 - i, 1));
                    }

                    v[i] = (soma * 10) % 11;

                    if (v[i] == 10)
                    {
                        v[i] = 0;
                    }
                }

                return v[0] == d[12] && v[1] == d[13];
            }

            return false;
        }

        /// <summary>
        /// Formata um número CNPJ
        /// </summary>
        /// <param name="cnpj">Número CNPJ</param>
        /// <returns>Número CNPJ formatado</returns>
        public static string Format(string cnpj)
        {
            return Convert.ToUInt64(cnpj.ExtractDigits()).ToString(@"00\.000\.000\/0000\-00");
        }
    }
}