using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class FiniteAutomata
{
    private int statesQuantity;
    private int initialState;
    private List<int> finalStates;
    private Dictionary<(int, string), int> transitions;

    public FiniteAutomata(int statesQuantity, int initialState, List<int> finalStates)
    {
        this.statesQuantity = statesQuantity;
        this.initialState = initialState;
        this.finalStates = finalStates;
        this.transitions = new Dictionary<(int, string), int>();
    }
    public void AddTransition(int initialState, string stringRead, int finalState)
    {
        var key = (initialState, stringRead);
        transitions[key] = finalState;
    }

}



    class DocumentReader
    {
        private FiniteAutomata automata;


    public void PrintTransitionsFromFile(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length < 3)
            {
                Console.WriteLine("Formato de archivo incorrecto. Líneas insuficientes para configurar el autómata.");
                return;
            }

            Console.WriteLine("Transiciones del autómata:");

            for (int i = 3; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');

                if (values.Length == 3)
                {
                    int transitionInitialState = int.Parse(values[0]);
                    string stringRead = values[1].Trim();
                    int transitionFinalState = int.Parse(values[2].Trim());

                    // Imprimir la transición
                    Console.WriteLine($"{transitionInitialState} --({stringRead})--> {transitionFinalState}");
                }
                else
                {
                    Console.WriteLine($"Formato de línea inválido en la línea {i + 1}: {lines[i]}");
                }
            }

            Console.WriteLine($"El autómata tiene un total de {lines.Length - 3} transiciones.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }
    }

        public FiniteAutomata ReadAutomatonFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length < 3)
                {
                    Console.WriteLine("Formato de archivo incorrecto. Líneas insuficientes para configurar el autómata.");
                    return null;
                }

                int totalStates = int.Parse(lines[0]);
                int initialState = int.Parse(lines[1]);
                List<int> finalStates = new List<int>(Array.ConvertAll(lines[2].Split(','), int.Parse));

                automata = new FiniteAutomata(totalStates, initialState, finalStates);

                for (int i = 3; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(',');

                    if (values.Length == 3)
                    {
                        int transitionInitialState = int.Parse(values[0]);
                        string stringRead = values[1].Trim();
                        int transitionFinalState = int.Parse(values[2].Trim()); // Corrección aplicada aquí

                        automata.AddTransition(transitionInitialState, stringRead, transitionFinalState);
                    }
                    else
                    {
                        Console.WriteLine($"Formato de línea inválido en la línea {i + 1}: {lines[i]}");
                    }
                }

                Console.WriteLine($"El autómata tiene {finalStates.Count} estados finales");
                Console.WriteLine($"El autómata tiene un total de {lines.Length - 3} transiciones.");

                return automata;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer el archivo: {ex}");
                return null;
            }
        }
    }


    class Program
{
    static void Main(string[] args)
    {
        DocumentReader documentReader = new DocumentReader();

        while (true)
        {
            Console.WriteLine("Ingrese la ruta del archivo del AFD (o escriba 'salir' para salir): ");
            string filePath = Console.ReadLine();

            if (filePath.ToLower() == "salir")
                break;

            // Llamar al método PrintTransitionsFromFile para imprimir las transiciones del archivo
            documentReader.PrintTransitionsFromFile(filePath);

            Console.WriteLine("\nPresiona Enter para continuar...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
