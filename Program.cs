using System;

namespace sudoku_solver
{
	class Program
	{
		static void Main(string[] args)
		{
			//Normal
			//var fieldString = "670002004000700806089050700900103070007080300030205009006090280701008000800600097";
			//Easy
			var fieldString = "001006805600701900072085041210600300000000000004003087780460120006802009309500400";
			//Hard
			//var field = "001093006023610070000000020900000000005361700000000004080000000090086350100450800";
			//var field = "700849030928135006400267089642783951397451628815692300204516093100008060500004010";
			var field = new SudokuField(fieldString);
			var solver = new Solver(field);

			solver.Solve();
			Console.ReadLine();

			// for (int i = 0; i < 81; i++)
			// {
			// 	var pos = new Solver.Position(i);
			// 	pos.ToString();
			// 	Console.ReadLine();
			// }



		}
	}
}
