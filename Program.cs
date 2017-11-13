using System;
using System.IO;

namespace balancete
{

    class Program
    {

        static void Main(string[] args) {
            int menuOption = Menu();
            while(menuOption != OPTION_EXIT) {
                switch(menuOption) {
                    case OPTION_NEW_ENTRIES:
                        NewEntriesOption();
                    break;
                    case OPTION_CONSOLIDATED_ENTRIES:
                        ConsolidatedEntriesOption();
                    break;
                    case OPTION_CLEANUP_ENTRIES:
                        CleanupEntriesOption();
                    break;
                }
                Console.WriteLine("Aperte qualquer tecla para retornar ao menu principal.");

                Console.ReadKey();
                Console.Clear();

                menuOption = Menu();
            }
        }

        public static string debitFile = @"data/debit";
        public static string creditFile = @"data/credit";

        const int OPTION_NEW_ENTRIES = 1;
        const int OPTION_CONSOLIDATED_ENTRIES = 2;
        const int OPTION_CLEANUP_ENTRIES = 3;
        const int OPTION_EXIT = 4;

        public static int Menu()
        {
            bool isValidOption = false;
            int output = -1;

            while (!isValidOption)
            {
                Console.WriteLine("\n-------------------------------");
                Console.WriteLine("Selecione a opção:");
                Console.WriteLine("-------------------------------");
                Console.WriteLine("1 - Inserir novos lançamentos");
                Console.WriteLine("2 - Ver total consolidado");
                Console.WriteLine("3 - Apagar todos registros");
                Console.WriteLine("4 - Sair");
                Console.WriteLine("-------------------------------");
                Console.Write("Digite a opção desejada: ");
                isValidOption = int.TryParse(Console.ReadLine(), out output);
                int[] validOptions = { OPTION_NEW_ENTRIES, OPTION_CONSOLIDATED_ENTRIES, OPTION_CLEANUP_ENTRIES, OPTION_EXIT };
                if (!isValidOption || Array.IndexOf(validOptions, output) == -1)
                {
                    Console.WriteLine("Opção inválida!");
                    Console.WriteLine("Aperte qualquer tecla para continuar...");
                    Console.ReadKey();
                    isValidOption = false;
                }
                Console.Clear();
            }

            return output;
        }

        public static void WriteEntry(StreamWriter handler, double value) {
            value = Math.Abs(Math.Round(value, 3, MidpointRounding.AwayFromZero));
            handler.WriteLine(value);
        }

        public static void NewEntriesOption() {
            Console.WriteLine("Insira os valores, positivos são creditos e negativos débitos e 0 para finalizar.");
            double currentAmount = double.NaN;

            StreamWriter debitHandler = new StreamWriter(debitFile, true);
            StreamWriter creditHandler = new StreamWriter(creditFile, true);

            while(currentAmount != 0) {
                Console.Write("Digite o valor: ");
                bool isValidCurrentAmount = double.TryParse(Console.ReadLine(), out currentAmount);
                if (isValidCurrentAmount) {
                    if (currentAmount > 0) {
                        WriteEntry(creditHandler, currentAmount);
                    }
                    else if (currentAmount < 0) {
                        WriteEntry(debitHandler, currentAmount);
                    }
                }
            }

            debitHandler.Close();
            creditHandler.Close();
        }

        public static void ConsolidatedEntriesOption() {
            if (File.Exists(debitFile) && File.Exists(creditFile)) {
                StreamReader debitHandler = new StreamReader(debitFile);
                StreamReader creditHandler = new StreamReader(creditFile);

                double totalDebit = totalAmount(debitHandler);
                double totalCredit = totalAmount(creditHandler);

                double total = Math.Round(totalCredit - totalDebit, 2, MidpointRounding.AwayFromZero);

                // Gambiarra pq to sem saco pra pesquisar uma biblioteca para formatação de moedas em C#
                string signal = "";
                if (total < 0) {
                    signal = "-";
                }

                Console.WriteLine("O seu total consolidado é de {0}R$ {1}", signal, Math.Abs(total));

                debitHandler.Close();
                creditHandler.Close();
            }
            else {
                Console.WriteLine("Você não possui entradas para serem exibidas.");
            }
        }

        public static void CleanupEntriesOption() {
            File.Delete(debitFile);
            File.Delete(creditFile);
            Console.WriteLine("Registros removidos com sucesso!");
        }

        public static double totalAmount(StreamReader file) {
            double totalAmount = 0;
            while(!file.EndOfStream) {
                totalAmount += double.Parse(file.ReadLine());
            }
            return totalAmount;
        }


    }
}
