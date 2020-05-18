using System;

namespace Infra.Crosscutting.Helpers
{
    /// <summary>
    /// Classe que representa funcionalidades para trabalhar com CPF
    /// </summary>
    public static class CPFHelper
    {
        /// <summary>
        /// Verifica se o número CPF é válido
        /// </summary>
        /// <param name="cpf">Número CPF</param>
        /// <returns></returns>
        public static bool IsValid(string cpf)
        {
            var d = new int[14];
            var v = new int[2];
            int j, i, soma;

            var numbers = cpf.ExtractDigits();

            // check numbers are equal
            if (new string(numbers[0], numbers.Length) == numbers)
                return false;

            if (numbers.Length == 11)
            {
                for (i = 0; i <= 10; i++)
                {
                    d[i] = Convert.ToInt32(numbers.Substring(i, 1));
                }

                for (i = 0; i <= 1; i++)
                {
                    soma = 0;

                    for (j = 0; j <= 8 + i; j++)
                    {
                        soma += d[j] * (10 + i - j);
                    }

                    v[i] = (soma * 10) % 11;

                    if (v[i] == 10)
                    {
                        v[i] = 0;
                    }
                }

                return v[0] == d[9] && v[1] == d[10];
            }

            return false;
        }

        /// <summary>
        /// Formata um número CPF
        /// </summary>
        /// <param name="cpf">Número CPF</param>
        /// <returns>Número CPF formatado</returns>
        public static string Format(string cpf)
        {
            return Convert.ToUInt64(cpf.ExtractDigits()).ToString(@"000\.000\.000\-00");
        }
    }
}