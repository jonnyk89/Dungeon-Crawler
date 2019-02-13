using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Easy = 10x10
             * Medium = 15x15
             * Hard = 20x20
            */

            List<int[]> TakenPositions = new List<int[]>();

            Console.Write("Choose Difficulty (Easy/Medium/Hard): ");
            string difficulty = Console.ReadLine();
            Console.Clear();
        beginning:
            int tiles = 0;

            // Base number of tiles on difficulty
            switch (difficulty)
            {
                case "Easy": tiles = 10; break;
                case "easy": tiles = 10; break;
                case "Medium": tiles = 15; break;
                case "medium": tiles = 15; break;
                case "Hard": tiles = 20; break;
                case "hard": tiles = 20; break;
                default: break;
            }

            // Get total treasures and monsters
            Random rnd = new Random();
            int treasures = rnd.Next(1, tiles / 3);
            int monsters = rnd.Next(1, tiles / 2);

            // Generate Player position
            Player player = new Player();
            int[] PlayerPosition = GenerateRandomPosition(tiles, rnd);
            player.symbol = '☻';
            player.position = PlayerPosition;
            TakenPositions.Add(PlayerPosition);

            // Generate positions of treasures and monsters on the grid
            List<Treasure> Treasures = new List<Treasure>();
            for (int i = 0; i < treasures; i++)
            {
                Treasure treasure = new Treasure();
                // If position is already taken, generate a new random one;
            redoTreasure:
                int[] Position = GenerateRandomPosition(tiles, rnd);
                if (TakenPositions.Contains(Position))
                {
                    goto redoTreasure;
                }
                treasure.symbol = '■';
                treasure.position = Position;

                Treasures.Add(treasure);
                TakenPositions.Add(Position);
            }

            List<Monster> Monsters = new List<Monster>();
            for (int i = 0; i < monsters; i++)
            {
                Monster monster = new Monster();
                // If position is already taken, generate a new random one;
            redoMonster:
                int[] Position = GenerateRandomPosition(tiles, rnd);
                if (TakenPositions.Contains(Position))
                {
                    goto redoMonster;
                }
                monster.symbol = '§';
                monster.position = Position;

                Monsters.Add(monster);
                TakenPositions.Add(Position);
            }

            // Generate available position for exit
            Exit exit = new Exit();
            redoExit:
            int[] ExitPosition = GenerateRandomPosition(tiles, rnd);
            if (TakenPositions.Contains(ExitPosition))
            {
                goto redoExit;
            }
            exit.symbol = '█';
            exit.position = ExitPosition;

            // Grid developed
        printGrid:
            char[,] Grid = DevelopGrid(tiles, player, Treasures, Monsters, exit);

            // Grid printed
            for (int rows = 0; rows < Grid.GetLength(0); rows++)
            {
                for (int cols = 0; cols < Grid.GetLength(1); cols++)
                {
                    Console.Write(string.Format("{0} ", Grid[rows, cols]));
                }
                Console.WriteLine();
            }

            Console.WriteLine("\nLegend: ");
            Console.WriteLine("Player ({0}): 1", player.symbol );
            Console.WriteLine("Exit ({0}): 1", exit.symbol);
            Console.WriteLine("Treasures ({0}): {1}", Treasures[0].symbol, treasures);
            Console.WriteLine("Monsters ({0}): {1}", Monsters[0].symbol, monsters);

            Console.WriteLine();

            // Key inputs
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.R)
                {
                    Console.Clear();
                    goto beginning;
                }
                else if (cki.Key == ConsoleKey.UpArrow)
                {
                    if (player.position[0] > 0)
                    {
                        player.position[0]--;
                        Console.Clear();
                        goto printGrid;
                    }
                }
                else if (cki.Key == ConsoleKey.LeftArrow)
                {
                    if (player.position[1] > 0)
                    {
                        player.position[1]--;
                        Console.Clear();
                        goto printGrid;
                    }
                }
                else if (cki.Key == ConsoleKey.DownArrow)
                {
                    if (player.position[0] < tiles - 1)
                    {
                        player.position[0]++;
                        Console.Clear();
                        goto printGrid;
                    }
                }
                else if (cki.Key == ConsoleKey.RightArrow)
                {
                    if (player.position[1] < tiles - 1)
                    {
                        player.position[1]++;
                        Console.Clear();
                        goto printGrid;
                    }
                }

                // Check if the player position is the same as that of one of the other objects

                // Treasures:
                foreach (var treasure in Treasures)
                {
                    if(treasure.position[0] == player.position[0] && treasure.position[1] == player.position[1])
                    {
                        Console.WriteLine("Treasure Opened!");
                    }
                }

                // Monsters:
                foreach (var monster in Monsters)
                {
                    if (monster.position[0] == player.position[0] && monster.position[1] == player.position[1])
                    {
                        Console.WriteLine("Encountered Monster");
                    }

                    // TODO: take player into another gameplay where he and the monster can battle (like the Duel Arena game)
                }

                // Exit:
                // TODO: make exit take player to a new randomly generated grid and player keep all buffs.

            }
        }

        // Generate random position
        static int[] GenerateRandomPosition(int tiles, Random rnd)
        {
            int[] Coordinates = new int[2];

            int x = rnd.Next(1, tiles - 1);
            int y = rnd.Next(1, tiles - 1);

            Coordinates[0] = x;
            Coordinates[1] = y;

            return Coordinates;
        }

        static char[,] DevelopGrid(int tiles, Player player,
                                              List<Treasure> Treasures,
                                              List<Monster> Monsters,
                                              Exit exit)                         
        {
            char[,] Grid = new char[tiles, tiles];

            for (int rows = 0; rows < Grid.GetLength(0); rows++)
            {

                for (int cols = 0; cols < Grid.GetLength(1); cols++)
                {
                    if (rows == player.position[0] && cols == player.position[1])
                    {
                        Grid[rows, cols] = player.symbol;
                    }

                    foreach (Treasure treasure in Treasures)
                    {
                        if (rows == treasure.position[0] && cols == treasure.position[1])
                        {
                            Grid[rows, cols] = treasure.symbol;
                        }
                    }

                    foreach (Monster monster in Monsters)
                    {
                        if (rows == monster.position[0] && cols == monster.position[1])
                        {
                            Grid[rows, cols] = monster.symbol;
                        }
                    }
                    
                    if (rows == exit.position[0] && cols == exit.position[1])
                    {
                        Grid[rows, cols] = exit.symbol;
                    }

                    if (Grid[rows, cols] != '☻' && Grid[rows, cols] != '■' && Grid[rows, cols] != '§' && Grid[rows, cols] != '█')
                    {
                        Grid[rows, cols] = '-';
                    }
                }
            }
            return Grid;
        }
    }

    struct Player
    {
        public char symbol;
        public int[] position;

        public int health;
        public int attack;
        public int defense;
    }

    struct Treasure
    {
        public char symbol;
        public int[] position;

        public string buff;
        public int buffAmount;
    }

    struct Monster
    {
        public char symbol;
        public int[] position;

        public string type;
    }

    struct Exit
    {
        public char symbol;
        public int[] position;
    }
}
