using System;
using System.Threading.Tasks;

namespace sudoku_solver
{
	class Program
	{
		static void Main(string[] args)
		{
			var task = new Task(async () =>
			{
				var solver = new Solver();
				var server = new Server();

				server.Start += async (sender, e) =>
				 {
					 solver.Setup(e.Field);
					 await server.UpdateField(solver._field, solver._candidates);
				 };

				server.Solve += async (sender, e) =>
				{
					solver.Solve();
					await server.UpdateField(solver._field, solver._candidates);
				};

				server.Step += async (sender, e) =>
				 {
					 solver.Step();
					 await server.UpdateField(solver._field, solver._candidates);
				 };
				await server.ReceiveConnection();
			});
			task.Start();
			Task.WaitAll(task);

			// var server = new Server();
			// server.Start();
			//Normal
			//var fieldString = "670002004000700806089050700900103070007080300030205009006090280701008000800600097";
			//Easy
			//var fieldString = "001006805600701900072085041210600300000000000004003087780460120006802009309500400";
			//Hard
			//var fieldString = "001093006023610070000000020900000000005361700000000004080000000090086350100450800";
			//var fieldString = "700000000008010004032460000040002500060731080007900010000054670100090800000000009";
			//var fieldString = "700849030928135006400267089642783951397451628815692300204516093100008060500004010";
			//var solver = new Solver();
			//solver.Solve(fieldString);
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
